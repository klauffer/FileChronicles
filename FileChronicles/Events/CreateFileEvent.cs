using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.InMemoryFileSystem;

namespace FileChronicles.Events
{
    internal sealed class CreateFileEvent : IChronicleEvent
    {
        private readonly string _fileName;
        private readonly byte[] _bytes;
        private readonly FileManager _fileManager;
        private readonly CancellationToken _cancellationToken;

        public CreateFileEvent(string fileName, byte[] bytes, FileManager fileManager, CancellationToken cancellationToken)
        {
            _fileName = fileName;
            _bytes = bytes;
            _fileManager = fileManager;
            _cancellationToken = cancellationToken;
        }

        public Task<EventResult<EventInfo, ErrorCode>> Stage()
        {
            EventResult<EventInfo, ErrorCode> result = new EventResult<EventInfo, ErrorCode>.Success(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            if (_fileManager.Exists(_fileName) || File.Exists(_fileName))
            {
                result = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            }
            else
            {
                _fileManager.Create(_fileName, _bytes);
            }
            return Task.FromResult(result);
        }

        public async Task<EventResult<EventInfo, ErrorCode>> Action()
        {
            if (!File.Exists(_fileName))
            {
                await File.WriteAllBytesAsync(_fileName, _bytes, _cancellationToken);
                return new EventResult<EventInfo, ErrorCode>.Success(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            }
            return new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
        }

        public Task<EventResult<EventInfo, ErrorCode>> RollBack()
        {
            File.Delete(_fileName);
            EventResult<EventInfo, ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Success(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            return Task.FromResult(eventResult);
        }
    }
}
