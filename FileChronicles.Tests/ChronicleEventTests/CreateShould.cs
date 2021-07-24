using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests.ChronicleEventTests
{
    public class CreateShould : TestFixture
    {

        [Fact]
        public async Task CreateAFile()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Create(path, _fileContentsBytes, token);
            var eventResult = await chronicler.Commit();
            var text = eventResult.Match(() => File.ReadAllText(path), errorCode => errorCode.ToString());

            Assert.Equal(_fileContents, text);
        }

        [Fact]
        public async Task GiveMeInfo()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var eventResult = await chronicler.Create(path, _fileContentsBytes, token);
            var filePath = eventResult.Match(eventInfo => eventInfo.FileName, errorCode => errorCode.ToString());

            Assert.Equal(path, filePath);
        }

        [Fact]
        public async Task NotCreateFileUntilCommit()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Create(path, _fileContentsBytes, token);
            Assert.False(File.Exists(path));
        }

        [Fact]
        public async Task FailWhenFileAlreadyExists()
        {
            var path = GetFileFullPath();
            using var safeFile = SafeFile.Create(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var eventResult = await chronicler.Create(path, _fileContentsBytes, token);
            
            var errorCode = eventResult.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), errorCode);
        }

        [Fact]
        public async Task HonorCancellationToken()
        {
            var path = GetFileFullPath();
            using var safeFile = SafeFile.Clear(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await chronicler.Create(path, _fileContentsBytes, token);
            tokenSource.Cancel();

            var eventResult = await chronicler.Commit();

            var errorCode = eventResult.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.EventCancelled.ToString(), errorCode);
        }
    }
}
