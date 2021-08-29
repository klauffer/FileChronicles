using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.InMemoryFileSystem;
using FunkyBasics.Either;

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

        public Task<EitherResult<EventInfo, ErrorCode>> Action()
        {
            EitherResult<EventInfo,ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileAlreadyExists);
            if (!File.Exists(_fileNameDestination))
            {
                File.Move(_fileNameSource, _fileNameDestination);
                var eventInfo = new EventInfo(_fileNameDestination, EventInfo.EventTypes.Move);
                EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(eventInfo);
            }

            return Task.FromResult(EitherResult);
        }

        public Task<EitherResult<EventInfo, ErrorCode>> RollBack()
        {
            File.Move(_fileNameDestination, _fileNameSource);
            var eventInfo = new EventInfo(_fileNameSource, EventInfo.EventTypes.Move);
            EitherResult<EventInfo, ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(eventInfo);
            return Task.FromResult(EitherResult);
        }

        public async Task<EitherResult<EventInfo, ErrorCode>> Stage()
        {
            EitherResult<EventInfo, ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.None);
            if (_fileManager.Exists(_fileNameDestination) || File.Exists(_fileNameDestination))
            {
                EitherResult = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileAlreadyExists);
                return EitherResult;
            }
            // if the file has already been moved OR the file doesnt exist in the disk or memory file system
            if (_fileManager.HasAlreadyBeenMoved(_fileNameSource) || (!_fileManager.Exists(_fileNameSource) && !File.Exists(_fileNameSource)))
            {
                EitherResult = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileDoesNotExist);
                return EitherResult;
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
                    EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(eventInfo);
                    return true;
                },
                errorCode => {
                    EitherResult = new EitherResult<EventInfo, ErrorCode>.Right(errorCode);
                    return false;
                });
                return EitherResult;
            
            
        }
    }
}
