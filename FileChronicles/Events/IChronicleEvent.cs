using System.Threading.Tasks;
using FunkyBasics.Either;

namespace FileChronicles.Events
{
    internal interface IChronicleEvent
    {
        Task<EitherResult<EventInfo, ErrorCode>> Stage();

        Task<EitherResult<EventInfo, ErrorCode>> Action();

        Task<EitherResult<EventInfo, ErrorCode>> RollBack();
    }
}
