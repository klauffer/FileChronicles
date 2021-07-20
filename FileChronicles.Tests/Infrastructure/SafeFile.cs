using System;
using System.IO;

namespace FileChronicles.Tests.Infrastructure
{
    internal class SafeFile : IDisposable
    {
        private readonly string _fileName;

        private SafeFile(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Creates the given file
        /// </summary>
        /// <param name="fileName">fully qualified file name</param>
        public static SafeFile Create(string fileName)
        {
            var safeFile = new SafeFile(fileName);
            safeFile.CreateFile();
            return safeFile;
        }

        private void CreateFile()
        {
            using var stream = File.Create(_fileName);
        }

        public void Dispose()
        {
            File.Delete(_fileName);
        }
    }
}
