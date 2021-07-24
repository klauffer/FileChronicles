﻿using System;
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
        private byte[] fileContents;

        public DeleteFileEvent(string fileName, CancellationToken cancellationToken)
        {
            _fileName = fileName;
            _cancellationToken = cancellationToken;
            fileContents = Array.Empty<byte>();
        }

        public async Task<EventResult<EventInfo, ErrorCode>> Action()
        {
            fileContents = await File.ReadAllBytesAsync(_fileName, _cancellationToken);
            File.Delete(_fileName);
            var eventInfo = new EventInfo(_fileName, EventInfo.EventTypes.Delete);
            EventResult<EventInfo, ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Success(eventInfo);
            return eventResult;
        }

        public async Task<EventResult<EventInfo, ErrorCode>> RollBack()
        {
            await File.WriteAllBytesAsync(_fileName, fileContents, _cancellationToken);
            var eventInfo = new EventInfo(_fileName, EventInfo.EventTypes.Delete);
            EventResult<EventInfo, ErrorCode> eventResult = new EventResult<EventInfo, ErrorCode>.Success(eventInfo);
            return eventResult;
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
