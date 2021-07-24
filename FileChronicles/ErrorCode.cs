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
        /// When the file already exists at the destination location
        /// </summary>
        FileAlreadyExists,
        /// <summary>
        /// When the file does no exist at the destination location
        /// </summary>
        FileDoesNotExist,
        /// <summary>
        /// when a single event is cancelled in a chroncile
        /// </summary>
        EventCancelled,
    }
}
