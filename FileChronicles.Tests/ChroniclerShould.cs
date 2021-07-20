using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FileChronicles.Tests
{
    public class ChroniclerShould
    {

        private readonly string _testContents = "This is a test";

        private byte[] GetTestContentsBytes() => Encoding.ASCII.GetBytes(_testContents);

        [Fact]
        public async Task CreateAFile()
        {
            var path = "TestFile.txt";
            var chronicler = new Chronicler();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var eventResult = await chronicler.Create(path, GetTestContentsBytes(), token);
            var text = eventResult.Match(path => File.ReadAllText(path), errorCode => errorCode.ToString());
            Assert.Equal(_testContents, text);
            File.Delete(path);
        }
    }
}
