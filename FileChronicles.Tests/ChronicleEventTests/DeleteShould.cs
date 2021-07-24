using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests.ChronicleEventTests
{
    public class DeleteShould
    {
        private static byte[] FileContents => Array.Empty<byte>();

        [Fact]
        public async Task GiveMeInfo()
        {
            var path = "TestFile.txt";
            using SafeFile safeFile1 = SafeFile.Create(path);
            var chronicler = Chronicler.Begin();
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            var eventResult = await chronicler.Delete(path, token);
            var filePath = eventResult.Match(eventInfo => eventInfo.FileName, errorCode => errorCode.ToString());

            Assert.Equal(path, filePath);
        }

    }
}
