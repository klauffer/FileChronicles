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
            await chronicler.Create(path, GetTestContentsBytes(), token);
            var eventResult = await chronicler.Commit();
            var text = eventResult.Match(() => File.ReadAllText(path), errorCode => errorCode.ToString());

            Assert.Equal(_testContents, text);
        }

        [Fact]
        public async Task CreateAFileGivesMeInfo()
        {
            var path = "TestFile.txt";
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var eventResult = await chronicler.Create(path, GetTestContentsBytes(), token);
            var filePath = eventResult.Match(filePath => filePath, errorCode => errorCode.ToString());

            Assert.Equal(path, filePath);
        }

        [Fact]
        public async Task NotCreateFileUntilCommit()
        {
            var path = "TestFile.txt";
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Create(path, GetTestContentsBytes(), token);
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

            var eventResult = await chronicler.Create(path, GetTestContentsBytes(), token);
            
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

            await chronicler.Create(path, GetTestContentsBytes(), token);
            tokenSource.Cancel();

            var eventResult = await chronicler.Commit();

            var errorCode = eventResult.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.EventCancelled.ToString(), errorCode);
        }
    }
}
