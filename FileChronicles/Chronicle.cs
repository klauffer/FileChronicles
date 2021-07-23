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

        public async Task<EventResult<string, ErrorCode>> AddEvent(IChronicleEvent chronicleEvent)
        {
            var eventResult = await chronicleEvent.Validate();
            eventResult.IfSuccess(() => _livingChronicleEvents.Add(chronicleEvent));
            return eventResult;
        }

        public async Task<EventResult<string, ErrorCode>> Commit()
        {
            foreach (var chroncileEvent in _livingChronicleEvents)
            {
                try
                {
                    var eventResult = await chroncileEvent.Action();
                    if (eventResult is EventResult<string, ErrorCode>.Error)
                    {
                        await Rollback();
                        return eventResult;
                    }
                    else
                    {
                        _deadChronicleEvents.Add(chroncileEvent);
                    }
                }
                catch (TaskCanceledException)
                {
                    return new EventResult<string,ErrorCode>.Error(ErrorCode.EventCancelled);
                }
                
            }
            return new EventResult<string, ErrorCode>.Success(string.Empty);//TODO: This doesnt feel good. maybe this shouldnt be returning a string
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
