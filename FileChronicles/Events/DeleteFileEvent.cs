using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.InMemoryFileSystem;
using FunkyBasics.Either;

namespace FileChronicles.Events
{
    internal class DeleteFileEvent : IChronicleEvent
    {
        private readonly string _fileName;
        private readonly CancellationToken _cancellationToken;
        private readonly FileManager _fileManager;
        private byte[] fileContents;

        public DeleteFileEvent(string fileName, FileManager fileManager, CancellationToken cancellationToken)
        {
            _fileName = fileName;
            _cancellationToken = cancellationToken;
            fileContents = Array.Empty<byte>();
            _fileManager = fileManager;
        }

        public async Task<EitherResult<EventInfo, ErrorCode>> Action()
        {
            fileContents = await File.ReadAllBytesAsync(_fileName, _cancellationToken);
            File.Delete(_fileName);
            var eventInfo = new EventInfo(_fileName, EventInfo.EventTypes.Delete);
            EitherResult<EventInfo, ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(eventInfo);
            return EitherResult;
        }

        public async Task<EitherResult<EventInfo, ErrorCode>> RollBack()
        {
            await File.WriteAllBytesAsync(_fileName, fileContents, _cancellationToken);
            var eventInfo = new EventInfo(_fileName, EventInfo.EventTypes.Delete);
            EitherResult<EventInfo, ErrorCode> EitherResult = new EitherResult<EventInfo, ErrorCode>.Left(eventInfo);
            return EitherResult;
        }

        public Task<EitherResult<EventInfo, ErrorCode>> Stage()
        {
            if (_fileManager.Exists(_fileName) || File.Exists(_fileName))
            {
                _fileManager.Delete(_fileName);
                EitherResult<EventInfo, ErrorCode> successResult = new EitherResult<EventInfo, ErrorCode>.Left(new EventInfo(_fileName, EventInfo.EventTypes.Delete));
                return Task.FromResult(successResult);
            }
            EitherResult<EventInfo, ErrorCode> errorResult = new EitherResult<EventInfo, ErrorCode>.Right(ErrorCode.FileDoesNotExist);
            return Task.FromResult(errorResult);


        }
    }
}
