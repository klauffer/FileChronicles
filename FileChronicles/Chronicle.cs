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

        public void AddEvent(IChronicleEvent chronicleEvent) => _livingChronicleEvents.Add(chronicleEvent);

        public async Task<EventResult<ErrorCode>> Commit()
        {
            foreach (var chroncileEvent in _livingChronicleEvents)
            {
                try
                {
                    var eventResult = await chroncileEvent.Action();
                    if (eventResult is EventResult<ErrorCode>.Error)
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
                    return new EventResult<ErrorCode>.Error(ErrorCode.EventCancelled);
                }
                
            }
            return new EventResult<ErrorCode>.Success();
        }

        public async Task<EventResult<ErrorCode>> Rollback()
        {
            foreach (var chronicleEvent in _deadChronicleEvents)
            {
                await chronicleEvent.RollBack();
            }
            _livingChronicleEvents = new List<IChronicleEvent>();
            _deadChronicleEvents = new List<IChronicleEvent>();
            return new EventResult<ErrorCode>.Success();
        }
    }
}
