using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace FileChronicles.Tests
{
    public class ChroniclerShould
    {

        private string _testContents = "This is a test";

        private byte[] GetTestContentsBytes() => Encoding.ASCII.GetBytes(_testContents);

        [Fact]
        public void CreateAFile()
        {
            var path = "TestFile.txt";
            var chronicler = new Chronicler();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            chronicler.Create(path, GetTestContentsBytes(), token);
            var text = File.ReadAllText(path);
            Assert.Equal(_testContents, text);
            File.Delete(path);
        }
    }
}
