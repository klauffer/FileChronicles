using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests
{
    public class ChroniclerShould
    {
        [Fact]
        public async Task CommitMoreThenOneAction()
        {
            var fileName1 = "TestFile1.txt";
            var fileName2 = "TestFile2.txt";
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile2 = SafeFile.Clear(fileName2);

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            chronicler.Create(fileName1, new byte[0], token);
            chronicler.Create(fileName2, new byte[0], token);
            var eventResult = await chronicler.Commit();

            Assert.True(eventResult.Match(() => true, errorCode => false));
            Assert.True(File.Exists(fileName1));
            Assert.True(File.Exists(fileName2));
        }

        [Fact]
        public async Task StopOnFirstError()
        {
            var fileName1 = "TestFile1.txt";
            var fileName1Duplicate = "TestFile1.txt";
            var fileName2 = "TestFile2.txt";
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile1Duplicate = SafeFile.Create(fileName1Duplicate),//create the first file to cause error
                           safeFile2 = SafeFile.Clear(fileName2);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var chronicler = Chronicler.Begin();
            
            chronicler.Create(fileName1, new byte[0], token);
            chronicler.Create(fileName2, new byte[0], token);
            var eventResult = await chronicler.Commit();

            Assert.Equal(ErrorCode.FileAlreadyExists.ToString() ,eventResult.Match(() => "", errorCode => errorCode.ToString()));
            Assert.False(File.Exists(fileName2));//The second file, although valid, was never made
        }

        [Fact]
        public async Task RollbackChangesOnError()
        {

            var fileName1 = "TestFile1.txt";
            var fileName2 = "TestFile2.txt";
            var fileName2Duplicate = "TestFile2.txt";
            using SafeFile safeFile1 = SafeFile.Clear(fileName1),
                           safeFile2 = SafeFile.Clear(fileName2),
                           safeFile2Duplicate = SafeFile.Create(fileName2Duplicate);//create the second file to cause error
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var chronicler = Chronicler.Begin();
            chronicler.Create(fileName1, new byte[0], token);
            chronicler.Create(fileName2, new byte[0], token);

            var eventResult = await chronicler.Commit();
            //The first file should've been created then the second file failed and rolled back the first action.
            //so this file should no longer exist
            Assert.False(File.Exists(fileName1));
        }

        [Fact]
        public async Task RollbackRemovesUncommittedChanges()
        {
            var fileName1 = "TestFile1.txt";
            using SafeFile safeFile1 = SafeFile.Clear(fileName1);
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var chronicler = Chronicler.Begin();
            chronicler.Create(fileName1, new byte[0], token);
            await chronicler.Rollback();
            await chronicler.Commit();

            Assert.False(File.Exists(fileName1));
        }
    }
}
