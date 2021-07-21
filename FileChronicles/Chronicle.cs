using System.Collections.Generic;
using System.Threading.Tasks;
using FileChronicles.Events;

namespace FileChronicles
{
    internal sealed class Chronicle
    {
        private readonly List<IChronicleEvent> _chronicleEvents;

        public Chronicle()
        {
            _chronicleEvents = new List<IChronicleEvent>();
        }

        public void AddEvent(IChronicleEvent chronicleEvent) => _chronicleEvents.Add(chronicleEvent);

        public async Task<EventResult<ErrorCode>> Commit()
        {
            foreach (var chroncileEvent in _chronicleEvents)
            {
                try
                {
                    var eventResult = await chroncileEvent.Action();
                    if (eventResult is EventResult<ErrorCode>.Error)
                    {
                        return eventResult;
                    }
                }
                catch (TaskCanceledException)
                {
                    return new EventResult<ErrorCode>.Error(ErrorCode.EventCancelled);
                }
                
            }
            return new EventResult<ErrorCode>.Success();
        }
    }
}
