using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests
{
    public class ChroniclerShould : TestFixture
    {
        [Fact]
        public async Task CommitMoreThenOneAction()
        {
            var fileName1 = GetNewFileFullPath();
            var fileName2 = GetNewFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile2 = SafeFile.Clear(fileName2);

            await using var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            await chronicler.Create(fileName1, _emptyFileContents, token);
            await chronicler.Create(fileName2, _emptyFileContents, token);
            var eventResult = await chronicler.Commit();

            Assert.True(eventResult.Match(() => true, errorCode => false));
            Assert.True(File.Exists(fileName1));
            Assert.True(File.Exists(fileName2));
        }

        [Fact]
        public async Task StopOnFirstError()
        {
            var fileName1 = GetNewFileFullPath();
            var fileName1Duplicate = fileName1;
            var fileName2 = GetNewFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile2 = SafeFile.Clear(fileName2);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await using var chronicler = Chronicler.Begin();

            await chronicler.Create(fileName1, _emptyFileContents, token);
            await chronicler.Create(fileName2, _emptyFileContents, token);

            //create the first file to cause error
            using SafeFile safeFile1Duplicate = SafeFile.Create(fileName1Duplicate);

            var eventResult = await chronicler.Commit();

            Assert.Equal(ErrorCode.FileAlreadyExists.ToString() ,eventResult.Match(() => "", errorCode => errorCode.ToString()));
            Assert.False(File.Exists(fileName2));//The second file, although valid, was never made
        }

        [Fact]
        public async Task EventShouldFailEarly()
        {

            var fileName2 = GetNewFileFullPath();
            var fileName2Duplicate = fileName2;
            using SafeFile safeFile2 = SafeFile.Clear(fileName2),
                           safeFile2Duplicate = SafeFile.Create(fileName2Duplicate);//create the second file to cause error
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await using var chronicler = Chronicler.Begin();
            var event2 = await chronicler.Create(fileName2, _emptyFileContents, token);
            var errorMessage = event2.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), errorMessage);
        }

        [Fact]
        public async Task RollbackChangesOnError()
        {

            var fileName1 = GetNewFileFullPath();
            var fileName2 = GetNewFileFullPath();
            var fileName2Duplicate = fileName2;
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile2 = SafeFile.Clear(fileName2);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await using var chronicler = Chronicler.Begin();
            var event1 = await chronicler.Create(fileName1, _emptyFileContents, token);
            var event2 = await chronicler.Create(fileName2, _emptyFileContents, token);

            //create the second file to cause error after the initial create
            using SafeFile safeFile2Duplicate = SafeFile.Create(fileName2Duplicate);

            var eventResult = await chronicler.Commit();
            //The first file should've been created then the second file failed and rolled back the first action.
            //so this file should no longer exist
            Assert.False(File.Exists(fileName1));
        }

        [Fact]
        public async Task RollbackRemovesUncommittedChanges()
        {
            var fileName1 = GetNewFileFullPath();
            using SafeFile safeFile1 = SafeFile.Clear(fileName1);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            await using var chronicler = Chronicler.Begin();
            await chronicler.Create(fileName1, _emptyFileContents, token);
            await chronicler.Rollback();
            await chronicler.Commit();

            Assert.False(File.Exists(fileName1));
        }
    }
}
