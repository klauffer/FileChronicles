using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<EventResult<InMemoryFile, ErrorCode>> Move(string sourceFileName, string destinationFileName, CancellationToken cancellationToken = default)
        {
            EventResult<InMemoryFile, ErrorCode> eventResult = new EventResult<InMemoryFile, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            if (!Exists(destinationFileName) && !File.Exists(destinationFileName))
            {
                var contents = await File.ReadAllBytesAsync(sourceFileName, cancellationToken);
                var inMemoryFile = new InMemoryFile(sourceFileName, contents);
                _inMemoryFiles.Add(destinationFileName, inMemoryFile);
                eventResult = new EventResult<InMemoryFile, ErrorCode>.Success(inMemoryFile);
            }
            return eventResult;
        }
    }
}
