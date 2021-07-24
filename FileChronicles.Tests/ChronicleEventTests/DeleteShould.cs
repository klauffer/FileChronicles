using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests.ChronicleEventTests
{
    public class DeleteShould : TestFixture
    {
        

        [Fact]
        public async Task GiveMeInfo()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Create(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var eventResult = await chronicler.Delete(path, token);
            var filePath = eventResult.Match(eventInfo => eventInfo.FileName, errorCode => errorCode.ToString());

            Assert.Equal(path, filePath);
        }

        [Fact]
        public async Task DeleteFileOnCommit()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Create(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Delete(path, token);
            var eventResult = await chronicler.Commit();
            var doesFileExist = eventResult.Match(count => File.Exists(path) , errorCode => true);

            Assert.False(doesFileExist);
        }

        [Fact]
        public async Task RollbackDeletedFile()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Create(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Delete(path, token);
            await chronicler.Commit();

            await chronicler.Rollback();

            Assert.True(File.Exists(path));
        }

        [Fact]
        public async Task RollbackDeletedFileContainsSameContents()
        {
            var path = GetFileFullPath();
            using SafeFile safeFile1 = SafeFile.Create(path, _fileContentsBytes);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Delete(path, token);
            await chronicler.Commit();

            await chronicler.Rollback();

            Assert.Equal(_fileContents, File.ReadAllText(path));
        }

    }
}
