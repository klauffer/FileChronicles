namespace FileChronicles
{
    /// <summary>
    /// Information of the outcome of an event
    /// </summary>
    public sealed class EventInfo
    {
        /// <summary>
        /// The full path to a file
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// The type of event that this information is representing
        /// </summary>
        public EventTypes EventType { get; }

        /// <summary>
        /// Creates a immutable EventInfo Object
        /// </summary>
        /// <param name="fileName">The full path to a file</param>
        /// <param name="eventType">The type of event that this information is representing</param>
        public EventInfo(string fileName, EventTypes eventType)
        {
            FileName = fileName;
            EventType = eventType;
        }

        


        /// <summary>
        /// Types of events that this info could represent
        /// </summary>
        public enum EventTypes
        {
            /// <summary>
            /// A file was created
            /// </summary>
            Create,
            /// <summary>
            /// A file was deleted
            /// </summary>
            Delete
        }
    }
}
