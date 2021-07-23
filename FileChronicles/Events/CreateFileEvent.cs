using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileChronicles.Events
{
    internal sealed class CreateFileEvent : IChronicleEvent
    {
        private readonly string _fileName;
        private readonly byte[] _bytes;
        private readonly CancellationToken _cancellationToken;

        public CreateFileEvent(string fileName, byte[] bytes, CancellationToken cancellationToken)
        {
            _fileName = fileName;
            _bytes = bytes;
            _cancellationToken = cancellationToken;
        }

        public Task<EventResult<string, ErrorCode>> Validate()
        {
            EventResult<string, ErrorCode> result = new EventResult<string, ErrorCode>.Success(_fileName);
            if (File.Exists(_fileName))
            {
                result = new EventResult<string, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            }
            return Task.FromResult(result);
        }

        public async Task<EventResult<string, ErrorCode>> Action()
        {
            if (!File.Exists(_fileName))
            {
                await File.WriteAllBytesAsync(_fileName, _bytes, _cancellationToken);
                return new EventResult<string, ErrorCode>.Success(_fileName);
            }
            return new EventResult<string, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
        }

        public Task<EventResult<string, ErrorCode>> RollBack()
        {
            File.Delete(_fileName);
            EventResult<string, ErrorCode> eventResult = new EventResult<string, ErrorCode>.Success(_fileName);
            return Task.FromResult(eventResult);
        }
    }
}
