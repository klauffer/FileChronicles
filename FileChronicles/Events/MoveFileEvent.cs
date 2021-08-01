using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.InMemoryFileSystem;

namespace FileChronicles.Events
{
    internal class MoveFileEvent : IChronicleEvent
    {
        private readonly string _fileNameSource;
        private readonly string _fileNameDestination;
        private readonly FileManager _fileManager;
        private readonly CancellationToken _cancellationToken;

        public MoveFileEvent(string fileNameSource, string fileNameDestination, FileManager fileManager, CancellationToken cancellationToken)
        {
            _fileNameSource = fileNameSource;
            _fileNameDestination = fileNameDestination;
            _fileManager = fileManager;
            _cancellationToken = cancellationToken;
        }

        public Task<EventResult<EventInfo, ErrorCode>> Action()
        {
            EventResult<EventInfo,ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            if (!File.Exists(_fileNameDestination))
            {
                File.Move(_fileNameSource, _fileNameDestination);
                var eventInfo = new EventInfo(_fileNameDestination, EventInfo.EventTypes.Move);
                eventResult = new EventResult<EventInfo, ErrorCode>.Success(eventInfo);
            }

            return Task.FromResult(eventResult);
        }

        public Task<EventResult<EventInfo, ErrorCode>> RollBack()
        {
            File.Move(_fileNameDestination, _fileNameSource);
            var eventInfo = new EventInfo(_fileNameSource, EventInfo.EventTypes.Move);
            EventResult<EventInfo, ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Success(eventInfo);
            return Task.FromResult(eventResult);
        }

        public async Task<EventResult<EventInfo, ErrorCode>> Stage()
        {
            EventResult<EventInfo, ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.None);
            if (_fileManager.Exists(_fileNameDestination) || File.Exists(_fileNameDestination))
            {
                eventResult = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
                return eventResult;
            }
            // if the file has already been moved OR the file doesnt exist in the disk or memory file system
            if (_fileManager.HasAlreadyBeenMoved(_fileNameSource) || (!_fileManager.Exists(_fileNameSource) && !File.Exists(_fileNameSource)))
            {
                eventResult = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileDoesNotExist);
                return eventResult;
            }

            if (!_fileManager.Exists(_fileNameSource))
            {
                var fileContents = await File.ReadAllBytesAsync(_fileNameSource, _cancellationToken);
                _fileManager.Create(_fileNameSource, fileContents);
            }
            var fileManagerResult = await _fileManager.Move(_fileNameSource, _fileNameDestination, _cancellationToken);
                fileManagerResult.Match(
                inMemoryFile => {
                    var eventInfo = new EventInfo(_fileNameDestination, EventInfo.EventTypes.Move);
                    eventResult = new EventResult<EventInfo, ErrorCode>.Success(eventInfo);
                    return true;
                },
                errorCode => {
                    eventResult = new EventResult<EventInfo, ErrorCode>.Error(errorCode);
                    return false;
                });
                return eventResult;
            
            
        }
    }
}
