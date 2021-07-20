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

        public async Task<EventResult<ErrorCode>> Action()
        {
            if (!File.Exists(_fileName))
            {
                await File.WriteAllBytesAsync(_fileName, _bytes, _cancellationToken);
                return new EventResult<ErrorCode>.Success();
            }
            return new EventResult<ErrorCode>.Error(ErrorCode.FileAlreadyExists);
        }
    }
}
