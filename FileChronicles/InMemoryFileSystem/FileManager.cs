using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileChronicles.InMemoryFileSystem
{
    internal class FileManager
    {
        private readonly Dictionary<string, InMemoryFile> _inMemoryFiles;
        private readonly List<HistoryRecord> _history;

        public FileManager()
        {
            _inMemoryFiles = new Dictionary<string, InMemoryFile>();
            _history = new List<HistoryRecord>();
        }

        public InMemoryFile Create(string fileName, byte[] contents)
        {
            var inMemoryFile = new InMemoryFile(fileName, contents);
            _inMemoryFiles.Add(fileName, inMemoryFile);
            return inMemoryFile;
        }

        public bool Delete(string fileName) =>
            _inMemoryFiles.Remove(fileName);

        public bool Exists(string fileName) =>
            _inMemoryFiles.ContainsKey(fileName);

        public bool HasAlreadyBeenMoved(string fileName) =>
            _history.Any(x => x.InMemoryFile.FileName == fileName
                              && x.Action == HistoryRecord.Actions.moved);

        public Task<EventResult<InMemoryFile, ErrorCode>> Move(string sourceFileName, string destinationFileName, CancellationToken cancellationToken = default)
        {
            EventResult<InMemoryFile, ErrorCode> eventResult = new EventResult<InMemoryFile, ErrorCode>.Error(ErrorCode.FileDoesNotExist);

            if (Exists(destinationFileName))
            {
                eventResult = new EventResult<InMemoryFile, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
                return Task.FromResult(eventResult);
            }
            if (Exists(sourceFileName))
            {
                GetFile(sourceFileName).Match(
                inMemoryFile =>
                {
                    _inMemoryFiles.Add(destinationFileName, inMemoryFile);
                    RecordHistoryOfMove(inMemoryFile);
                    _inMemoryFiles.Remove(sourceFileName);
                    eventResult = new EventResult<InMemoryFile, ErrorCode>.Success(inMemoryFile);
                    return true;
                },
                errorCode =>
                {
                    eventResult = new EventResult<InMemoryFile, ErrorCode>.Error(errorCode);
                    return false;
                });
            }
            return Task.FromResult(eventResult);
        }

        private void RecordHistoryOfMove(InMemoryFile sourceFile) =>
            _history.Add(HistoryRecord.CreateMovedRecord(sourceFile));

        private EventResult<InMemoryFile, ErrorCode> GetFile(string fileName)
        {
            if (_inMemoryFiles.TryGetValue(fileName, out var inMemoryFile))
            {
                return new EventResult<InMemoryFile, ErrorCode>.Success(inMemoryFile);
            }
            return new EventResult<InMemoryFile, ErrorCode>.Error(ErrorCode.FileDoesNotExist);
        }
    }
}
