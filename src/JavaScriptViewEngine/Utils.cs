using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace JavaScriptViewEngine
{
    /// <summary>
    /// Some internal utility methods
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Gets an embedded resource as a string
        /// </summary>
        public static string GetResourceAsString(string resourceName, Type type)
        {
            var assembly = type.Assembly;
            return GetResourceAsString(resourceName, assembly);
        }

        /// <summary>
        /// GGets an embedded resource as a string
        /// </summary>
        public static string GetResourceAsString(string resourceName, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new NullReferenceException(string.Format("No resource found for " + resourceName));

                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Load a file 
        /// </summary>
        public static string GetFileTextContent(string path, Encoding encoding = null)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);
            
            using (var file = new StreamReader(path, encoding ?? Encoding.UTF8))
                return file.ReadToEnd();
        }
    }
}
