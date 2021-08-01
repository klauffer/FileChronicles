namespace FileChronicles.InMemoryFileSystem
{
    internal class HistoryRecord
    {
        public InMemoryFile InMemoryFile { get; }

        public Actions Action { get; }

        private HistoryRecord(InMemoryFile inMemoryFile, Actions action)
        {
            InMemoryFile = inMemoryFile;
            Action = action;
        }

        public static HistoryRecord CreateMovedRecord(InMemoryFile inMemoryFile) => new HistoryRecord(inMemoryFile, Actions.moved);


        public enum Actions
        {
            moved
        }
    }
}