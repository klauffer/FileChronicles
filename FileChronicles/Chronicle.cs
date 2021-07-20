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
                var eventResult = await chroncileEvent.Action();
                if (eventResult is EventResult<ErrorCode>.Error)
                {
                    return eventResult;
                }
            }
            return new EventResult<ErrorCode>.Success();
        }
    }
}
