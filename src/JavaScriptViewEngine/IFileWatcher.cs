using System;
using System.Collections.Generic;

namespace JavaScriptViewEngine
{
    /// <summary>
	/// Handles watching for changes to files.
	/// </summary>
	public interface IFileWatcher : IDisposable
    {
        /// <summary>
        /// Occurs when any watched files have changed (including renames and deletions).
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Gets or sets the path to watch.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the files to watch in the path. If no files are provided, every file in the 
        /// path is watched.
        /// </summary>
        IEnumerable<string> Files { get; set; }

        /// <summary>
        /// The debounce timeout before a file changed event is fired.
        /// Give this a large value when you are watching a large directory that has many changes during a build process.
        /// </summary>
        int DebounceTimeout { get; set; }

        /// <summary>
        /// Starts watching for changes in the specified path.
        /// </summary>
        /// <returns>Whether creation of the watcher was successful</returns>
        bool Start();

        /// <summary>
        /// Stops watching for changes in the specified path.
        /// </summary>
        void Stop();
    }
}
