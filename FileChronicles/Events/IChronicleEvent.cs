﻿using System.Threading.Tasks;

namespace FileChronicles.Events
{
    internal interface IChronicleEvent
    {
        Task<EventResult<EventInfo, ErrorCode>> Validate();

        Task<EventResult<EventInfo, ErrorCode>> Action();

        Task<EventResult<EventInfo, ErrorCode>> RollBack();
    }
}
