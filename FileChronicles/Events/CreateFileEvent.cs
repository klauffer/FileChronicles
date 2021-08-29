using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.InMemoryFileSystem;
using FunkyBasics.Either;

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

        public Task<EitherResult<EventInfo, ErrorCode>> Stage()
        {
            EitherResult<EventInfo, ErrorCode> result = new EitherResult<EventInfo, ErrorCode>.Left(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            if (_fileManager.Exists(_fileName) || File.Exists(_fileName))
            {
                result = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileAlreadyExists);
            }
            else
            {
                _fileManager.Create(_fileName, _bytes);
            }
            return Task.FromResult(result);
        }

        public async Task<EitherResult<EventInfo, ErrorCode>> Action()
        {
            if (!File.Exists(_fileName))
            {
                await File.WriteAllBytesAsync(_fileName, _bytes, _cancellationToken);
                return new EitherResult<EventInfo, ErrorCode>.Left(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            }
            return new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileAlreadyExists);
        }

        public Task<EitherResult<EventInfo, ErrorCode>> RollBack()
        {
            File.Delete(_fileName);
            EitherResult<EventInfo, ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(new EventInfo(_fileName, EventInfo.EventTypes.Create));
            return Task.FromResult(EitherResult);
        }
    }
}
