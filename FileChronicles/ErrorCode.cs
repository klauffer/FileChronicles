namespace FileChronicles
{
    /// <summary>
    /// Error codes of failed chronicle events
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Represents No Error
        /// </summary>
        None,
        /// <summary>
        /// When the a file already exists in the destination location
        /// </summary>
        FileAlreadyExists,
        /// <summary>
        /// when a single event is cancelled in a chroncile
        /// </summary>
        EventCancelled,
    }
}
