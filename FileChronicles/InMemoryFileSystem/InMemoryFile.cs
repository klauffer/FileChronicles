using System.Collections.Generic;
using FileChronicles.Infrastucture;

namespace FileChronicles.InMemoryFileSystem
{
    internal class InMemoryFile : ValueObject
    {
        public byte[] Contents { get; }

        public string FileName { get; }

        public InMemoryFile(string fileName, byte[] contents)
        {
            Contents = contents;
            FileName = fileName;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FileName;
        }
    }
}