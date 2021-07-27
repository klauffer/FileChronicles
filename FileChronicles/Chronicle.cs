using System.Collections.Generic;
using System.Threading.Tasks;
using FileChronicles.Events;

namespace FileChronicles
{
    internal sealed class Chronicle
    {
        private List<IChronicleEvent> _livingChronicleEvents;
        private List<IChronicleEvent> _deadChronicleEvents;

        public Chronicle()
        {
            _livingChronicleEvents = new List<IChronicleEvent>();
            _deadChronicleEvents = new List<IChronicleEvent>();
        }

        public async Task<EventResult<EventInfo, ErrorCode>> AddEvent(IChronicleEvent chronicleEvent)
        {
            var eventResult = await chronicleEvent.Stage();
            eventResult.IfSuccess(() => _livingChronicleEvents.Add(chronicleEvent));
            return eventResult;
        }

        public async Task<EventResult<int, ErrorCode>> Commit()
        {
            ErrorCode shortCircuitErrorCode = ErrorCode.None;
            foreach (var chroncileEvent in _livingChronicleEvents)
            {
                try
                {
                    var eventResult = await chroncileEvent.Action();
                    var successfulAction = await eventResult.Match(() =>
                                            {
                                                _deadChronicleEvents.Add(chroncileEvent);
                                                return Task.FromResult(true);
                                            }, 
                                            async errorCode =>
                                            {
                                                await Rollback();
                                                shortCircuitErrorCode = errorCode;
                                                return false;
                                            });
                    if (!successfulAction)
                    {
                        return new EventResult<int, ErrorCode>.Error(shortCircuitErrorCode);
                    }
                }
                catch (TaskCanceledException)
                {
                    return new EventResult<int, ErrorCode>.Error(ErrorCode.EventCancelled);
                }
                
            }
            //maybe clear living events here?
            return new EventResult<int, ErrorCode>.Success(_deadChronicleEvents.Count);//TODO: This doesnt feel good. maybe this shouldnt be returning a string
        }

        public async Task<EventResult<int, ErrorCode>> Rollback()
        {
            foreach (var chronicleEvent in _deadChronicleEvents)
            {
                await chronicleEvent.RollBack();
            }
            var count = _deadChronicleEvents.Count;
            _livingChronicleEvents = new List<IChronicleEvent>();
            _deadChronicleEvents = new List<IChronicleEvent>();
            return new EventResult<int, ErrorCode>.Success(count);
        }
    }
}
