using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace JavaScriptViewEngine.Edge
{
    public class Edge
    {
        static object syncRoot = new object();
        static bool initialized;
        static Func<object, Task<object>> compileFunc;
        static ManualResetEvent waitHandle = new ManualResetEvent(false);
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Task<object> InitializeInternal(object input)
        {
            compileFunc = (Func<object, Task<object>>)input;
            initialized = true;
            waitHandle.Set();

            return Task.FromResult((object)null);
        }

        // Find the entry point with `dumpbin /exports node.exe`, look for Start@node
        [DllImport("node.dll", EntryPoint = "#928", CallingConvention = CallingConvention.Cdecl)]
        static extern int NodeStart(int argc, string[] argv);

        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

        public static Func<object, Task<object>> Func(string code)
        {
            if (!initialized)
            {
                lock (syncRoot)
                {
                    if (!initialized)
                    {
                        if (IntPtr.Size == 4)
                            LoadDll("node", "x86");
                        else if (IntPtr.Size == 8)
                            LoadDll("node", "x64");

                        var type = typeof(Edge).Assembly.Location;

                        var dirName = Path.Combine(Path.GetTempPath(), "JavaScriptViewEngine." + typeof(Edge).Assembly.GetName().Version.ToString());

                        if (!Directory.Exists(dirName))
                            Directory.CreateDirectory(dirName);

                        var doubleEdgeJsLocation = Path.Combine(dirName, "double_edge.js");

                        if(!File.Exists(doubleEdgeJsLocation))
                            File.WriteAllText(doubleEdgeJsLocation, Utils.GetResourceAsString("JavaScriptViewEngine.Edge.double_edge.js", typeof(Edge)));

                        Thread v8Thread = new Thread(() =>
                        {
                            List<string> argv = new List<string>();
                            argv.Add("node");
                            string node_params = Environment.GetEnvironmentVariable("EDGE_NODE_PARAMS");
                            if (!string.IsNullOrEmpty(node_params))
                            {
                                foreach (string p in node_params.Split(' '))
                                {
                                    argv.Add(p);
                                }
                            }
                            argv.Add(doubleEdgeJsLocation);
                            NodeStart(argv.Count, argv.ToArray());
                            waitHandle.Set();
                        });

                        v8Thread.IsBackground = true;
                        v8Thread.Start();
                        waitHandle.WaitOne();

                        if (!initialized)
                        {
                            throw new InvalidOperationException("Unable to initialize Node.js runtime.");
                        }
                    }
                }
            }

            if (compileFunc == null)
            {
                throw new InvalidOperationException("Edge.Func cannot be used after Edge.Close had been called.");
            }

            var task = compileFunc(code);
            task.Wait();
            return (Func<object, Task<object>>)task.Result;
        }

        private static void LoadDll(string dllName, string architecture)
        {
            var dirName = Path.Combine(Path.GetTempPath(), "JavaScriptViewEngine." + typeof(Edge).Assembly.GetName().Version.ToString());

            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            dirName = Path.Combine(dirName, architecture);

            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            var dllPath = Path.Combine(dirName, dllName + ".dll");

            if (!File.Exists(dllPath))
                using (Stream stm = typeof(Edge).Assembly.GetManifestResourceStream("JavaScriptViewEngine.Edge." + dllName + "-" + architecture + ".dll"))
                {
                    try
                    {
                        using (Stream outFile = File.Create(dllPath))
                        {
                            const int sz = 4096;
                            byte[] buf = new byte[sz];
                            while (true)
                            {
                                int nRead = stm.Read(buf, 0, sz);
                                if (nRead < 1)
                                    break;
                                outFile.Write(buf, 0, nRead);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        // This may happen if another process has already created and loaded the file.
                        // Since the directory includes the version number of this assembly we can
                        // assume that it's the same bits, so we just ignore the excecption here and
                        // load the DLL.
                    }
                }

            var loadLibraryResult = LoadLibrary(dllPath);
            if (loadLibraryResult == 0)
                throw new Exception("Couldn't load native assembly at " + dllPath);
        }
    }
}
