using System.Threading.Tasks;

namespace FileChronicles.Events
{
    internal interface IChronicleEvent
    {
        Task<EventResult<string, ErrorCode>> Validate();

        Task<EventResult<string, ErrorCode>> Action();

        Task<EventResult<string, ErrorCode>> RollBack();
    }
}
