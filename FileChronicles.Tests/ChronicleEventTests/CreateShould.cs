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
            var path = GetNewFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Create(path, _fileContentsBytes, token);
            var EitherResult = await chronicler.Commit();
            var text = EitherResult.Match(x => File.ReadAllText(path), errorCode => errorCode.ToString());

            Assert.Equal(_fileContents, text);
        }

        [Fact]
        public async Task GiveMeInfo()
        {
            var path = GetNewFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var EitherResult = await chronicler.Create(path, _fileContentsBytes, token);
            var filePath = EitherResult.Match(eventInfo => eventInfo.FileName, errorCode => errorCode.ToString());

            Assert.Equal(path, filePath);
        }

        [Fact]
        public async Task NotCreateFileUntilCommit()
        {
            var path = GetNewFileFullPath();
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
            var path = GetNewFileFullPath();
            using var safeFile = SafeFile.Create(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var EitherResult = await chronicler.Create(path, _fileContentsBytes, token);
            
            var errorCode = EitherResult.Match(x => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), errorCode);
        }

        [Fact]
        public async Task HonorCancellationToken()
        {
            var path = GetNewFileFullPath();
            using var safeFile = SafeFile.Clear(path);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await chronicler.Create(path, _fileContentsBytes, token);
            tokenSource.Cancel();

            var EitherResult = await chronicler.Commit();

            var errorCode = EitherResult.Match(x => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.EventCancelled.ToString(), errorCode);
        }
    }
}
