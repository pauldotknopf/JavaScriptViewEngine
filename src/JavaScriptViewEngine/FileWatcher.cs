using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace JavaScriptViewEngine
{
    /// <summary>
	/// Handles watching for changes to files.
	/// </summary>
	public class FileWatcher : IFileWatcher
    {
        #region Fields

        FileSystemWatcher _watcher;
        ISet<string> _watchedFiles;
        Timer _timer;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcher"/> class.
        /// </summary>
        public FileWatcher()
        {
            DebounceTimeout = 25;
        }

        #endregion

        #region IFileWatcher

        /// <summary>
        /// Occurs when any watched files have changed (including renames and deletions).
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Gets or sets the path to watch.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the files to watch in the path. If no files are provided, every file in the 
        /// path is watched.
        /// </summary>
        public IEnumerable<string> Files
        {
            get { return _watchedFiles; }
            set
            {
                if (value == null || !value.Any())
                {
                    _watchedFiles = null;
                }
                else
                {
                    _watchedFiles = new HashSet<string>(value.Select(name => name.ToLowerInvariant()));
                }
            }
        }

        /// <summary>
        /// The debounce timeout before a file changed event is fired.
        /// Give this a large value when you are watching a large directory that has many changes during a build process.
        /// </summary>
        public int DebounceTimeout { get; set; }

        /// <summary>
        /// Starts watching for changes in the specified path.
        /// </summary>
        /// <returns>Whether creation of the watcher was successful</returns>
        public virtual bool Start()
        {
            if (Path == null)
            {
                throw new InvalidOperationException("Path must be set first");
            }

            _timer = new Timer(OnTimer);
            try
            {
                // Attempt to initialise a FileSystemWatcher so we can recycle the JavaScript
                // engine pool when files are changed.
                _watcher = new FileSystemWatcher
                {
                    Path = Path,
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true,
                };
                _watcher.Changed += OnFileChanged;
                _watcher.Created += OnFileChanged;
                _watcher.Deleted += OnFileChanged;
                _watcher.Renamed += OnFileChanged;
                return true;
            }
            catch (Exception ex)
            {
                // Can't use FileSystemWatcher (eg. not running in Full Trust)
                Trace.WriteLine("Unable to initialise FileSystemWatcher: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Stops watching for changes in the specified path.
        /// </summary>
        public virtual void Stop()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles events fired when any files are changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The <see cref="FileSystemEventArgs"/> instance containing the event data</param>
        protected virtual void OnFileChanged(object sender, FileSystemEventArgs args)
        {
            // If we're watching specific files, we need to check if the file that changed is one that we
            // care about.
            if (_watchedFiles != null && !_watchedFiles.Contains(args.FullPath.ToLowerInvariant()))
            {
                return;
            }

            Trace.WriteLine($"[JsPool] Watched file '{args.FullPath}' changed");
            // Use a timer so multiple changes only result in a single reset.
            _timer.Change(DebounceTimeout, Timeout.Infinite);
        }

        /// <summary>
        /// Handles events fired when any files are changed, and the changes have been debounced.
        /// </summary>
        /// <param name="state">The state.</param>
        protected virtual void OnTimer(object state)
        {
            var changed = Changed;
            changed?.Invoke(this, null);
        }

        #endregion
    } 
}
