using System.Threading.Tasks;

namespace FileChronicles.Events
{
    internal interface IChronicleEvent
    {
        Task<EventResult<ErrorCode>> Action();

        Task<EventResult<ErrorCode>> RollBack();
    }
}
