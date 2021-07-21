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

            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            chronicler.Create(fileName1, new byte[0], token);
            chronicler.Create(fileName2, new byte[0], token);
            var eventResult = await chronicler.Commit();

            Assert.Equal(ErrorCode.FileAlreadyExists.ToString() ,eventResult.Match(() => "", errorCode => errorCode.ToString()));
            Assert.False(File.Exists(fileName2));//The second file, although valid, was never made
        }
    }
}
