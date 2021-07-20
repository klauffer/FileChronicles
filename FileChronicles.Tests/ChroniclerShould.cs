using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
            File.Delete(fileName1);
            File.Delete(fileName2);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            chronicler.Create(fileName1, new byte[0], token);
            chronicler.Create(fileName2, new byte[0], token);
            var eventResult = await chronicler.Commit();

            Assert.True(eventResult.Match(() => true, errorCode => false));
            Assert.True(File.Exists(fileName1));
            Assert.True(File.Exists(fileName2));

            File.Delete(fileName1);
            File.Delete(fileName2);
        }
    }
}
