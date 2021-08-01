using System;
using System.IO;

namespace FileChronicles.Tests.Infrastructure
{
    internal class SafeFile : IDisposable
    {
        public string FileName { get; }
        private readonly byte[] _fileContents;

        private SafeFile(string fileName, byte[] fileContents)
        {
            FileName = fileName;
            _fileContents = fileContents;
        }

        /// <summary>
        /// Creates the given file
        /// </summary>
        /// <param name="fileName">fully qualified file name</param>
        /// <param name="fileContents">contents of what will be in the file</param>
        public static SafeFile Create(string fileName, byte[] fileContents)
        {
            var safeFile = new SafeFile(fileName, fileContents);
            safeFile.CreateFile();
            return safeFile;
        }

        /// <summary>
        /// Creates the given file
        /// </summary>
        /// <param name="fileName">fully qualified file name</param>
        public static SafeFile Create(string fileName) => Create(fileName, Array.Empty<byte>());

        /// <summary>
        /// Creates the given file
        /// </summary>
        /// <param name="fileName">fully qualified file name</param>
        public static SafeFile Clear(string fileName)
        {
            var safeFile = new SafeFile(fileName, Array.Empty<byte>());
            safeFile.Dispose();
            return safeFile;
        }

        private void CreateFile()
        {
            File.WriteAllBytes(FileName, _fileContents);
        }

        public void Dispose()
        {
            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }
        }
    }
}
