using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileChronicles.Events
{
    internal class DeleteFileEvent : IChronicleEvent
    {
        private readonly string _fileName;
        private readonly CancellationToken _cancellationToken;

        public DeleteFileEvent(string fileName, CancellationToken cancellationToken)
        {
            _fileName = fileName;
            _cancellationToken = cancellationToken;
        }

        public Task<EventResult<EventInfo, ErrorCode>> Action()
        {
            throw new NotImplementedException();
        }

        public Task<EventResult<EventInfo, ErrorCode>> RollBack()
        {
            throw new NotImplementedException();
        }

        public Task<EventResult<EventInfo, ErrorCode>> Validate()
        {
            if (File.Exists(_fileName))
            {
                EventResult<EventInfo, ErrorCode> successResult = new EventResult<EventInfo, ErrorCode>.Success(new EventInfo(_fileName, EventInfo.EventTypes.Create));
                return Task.FromResult(successResult);
            }
            EventResult<EventInfo, ErrorCode> errorResult = new EventResult<EventInfo, ErrorCode>.Error(ErrorCode.FileDoesNotExist);
            return Task.FromResult(errorResult);


        }
    }
}
