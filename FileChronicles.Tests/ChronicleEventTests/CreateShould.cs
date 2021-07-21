using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests.ChronicleEventTests
{
    public class CreateShould
    {

        private readonly string _testContents = "This is a test";

        private byte[] GetTestContentsBytes() => Encoding.ASCII.GetBytes(_testContents);

        [Fact]
        public async Task CreateAFile()
        {
            var path = "TestFile.txt";
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            chronicler.Create(path, GetTestContentsBytes(), token);
            var eventResult = await chronicler.Commit();
            var text = eventResult.Match(() => File.ReadAllText(path), errorCode => errorCode.ToString());

            Assert.Equal(_testContents, text);
        }

        [Fact]
        public void NotCreateFileUntilCommit()
        {
            var path = "TestFile.txt";
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            chronicler.Create(path, GetTestContentsBytes(), token);
            Assert.False(File.Exists(path));
        }

        [Fact]
        public async Task FailWhenFileAlreadyExists()
        {
            var path = "FailWhenFileAlreadyExists.txt";
            using var safeFile = SafeFile.Create(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            chronicler.Create(path, GetTestContentsBytes(), token);
            var eventResult = await chronicler.Commit();

            var errorCode = eventResult.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), errorCode);
        }

        [Fact]
        public async Task HonorCancellationToken()
        {
            var path = "test.txt";
            using var safeFile = SafeFile.Clear(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            chronicler.Create(path, GetTestContentsBytes(), token);
            tokenSource.Cancel();

            var eventResult = await chronicler.Commit();

            var errorCode = eventResult.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.EventCancelled.ToString(), errorCode);
        }
    }
}
