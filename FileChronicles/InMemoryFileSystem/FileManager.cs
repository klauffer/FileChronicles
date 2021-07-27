using System.Collections.Generic;

namespace FileChronicles.InMemoryFileSystem
{
    internal class FileManager
    {
        private readonly Dictionary<string, InMemoryFile> _inMemoryFiles;

        public FileManager()
        {
            _inMemoryFiles = new Dictionary<string, InMemoryFile>();
        }

        public InMemoryFile Create(string fileName, byte[] contents)
        {
            var inMemoryFile = new InMemoryFile(fileName, contents);
            _inMemoryFiles.Add(fileName, inMemoryFile);
            return inMemoryFile;
        }

        public bool Delete(string fileName) => _inMemoryFiles.Remove(fileName);

        public bool Exists(string fileName) => _inMemoryFiles.ContainsKey(fileName);
    }
}
