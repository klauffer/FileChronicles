using System;
using System.Text;

namespace FileChronicles.Tests
{
    public abstract class TestFixture
    {
        protected readonly static byte[] _emptyFileContents = Array.Empty<byte>();

        protected readonly static string _fileContents = "This is a test";

        protected readonly static byte[] _fileContentsBytes = Encoding.ASCII.GetBytes(_fileContents);

        private static string GetNewFileName() => Guid.NewGuid().ToString();

        protected static string GetNewFileFullPath(int id = 0) => $"{GetNewFileName()}-{id}.txt";
    }
}
