using System.Collections.Generic;
using System.Threading.Tasks;
using FileChronicles.Events;
using FunkyBasics.Either;

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

        public async Task<EitherResult<EventInfo, ErrorCode>> AddEvent(IChronicleEvent chronicleEvent)
        {
            var EitherResult = await chronicleEvent.Stage();
            if(EitherResult.IsLeft().Match(() => true, () => false))
            {
                _livingChronicleEvents.Add(chronicleEvent); 
            }
            return EitherResult;
        }

        public async Task<EitherResult<int, ErrorCode>> Commit()
        {
            ErrorCode shortCircuitErrorCode = ErrorCode.None;
            foreach (var chroncileEvent in _livingChronicleEvents)
            {
                try
                {
                    var EitherResult = await chroncileEvent.Action();
                    var successfulAction = await EitherResult.Match(eventInfo =>
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
                        return new EitherResult<int, ErrorCode>.Right(shortCircuitErrorCode);
                    }
                }
                catch (TaskCanceledException)
                {
                    return new EitherResult<int, ErrorCode>.Right(ErrorCode.EventCancelled);
                }
                
            }
            var eventsActedOn = _deadChronicleEvents.Count;
            ClearChronicleEvents();
            return new EitherResult<int, ErrorCode>.Left(eventsActedOn);//TODO: This doesnt feel good. maybe this shouldnt be returning a integer
        }

        public async Task<EitherResult<int, ErrorCode>> Rollback()
        {
            foreach (var chronicleEvent in _deadChronicleEvents)
            {
                await chronicleEvent.RollBack();
            }
            var count = _deadChronicleEvents.Count;
            _livingChronicleEvents = new List<IChronicleEvent>();
            _deadChronicleEvents = new List<IChronicleEvent>();
            return new EitherResult<int, ErrorCode>.Left(count);
        }

        private void ClearChronicleEvents()
        {
            _livingChronicleEvents = new List<IChronicleEvent>();
            _deadChronicleEvents = new List<IChronicleEvent>();
        }
    }
}
