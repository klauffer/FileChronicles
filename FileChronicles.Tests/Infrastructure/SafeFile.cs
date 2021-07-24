using System;
using System.IO;
using System.Threading.Tasks;

namespace FileChronicles.Tests.Infrastructure
{
    internal class SafeFile : IDisposable
    {
        private readonly string _fileName;
        private readonly byte[] _fileContents;

        private SafeFile(string fileName, byte[] fileContents)
        {
            _fileName = fileName;
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
            File.WriteAllBytes(_fileName, _fileContents);
        }

        public void Dispose()
        {
            File.Delete(_fileName);
        }
    }
}
