using System.IO;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests.ChronicleEventTests
{
    public class MoveShould : TestFixture
    {
        [Fact]
        public async Task MoveFileToRequestedLocation()
        {
            //var sourceFile
            using var sourceFile = SafeFile.Create(GetNewFileFullPath());
            using var destinationFile = SafeFile.Clear(GetNewFileFullPath());
            await using var chronicler = Chronicler.Begin();
            var stagingResponse = await chronicler.Move(sourceFile.FileName, destinationFile.FileName);
            var response = await chronicler.Commit();
            var doesFileExist = response.Match(() => File.Exists(destinationFile.FileName), errorCode => false);
            Assert.True(doesFileExist);
        }


        [Fact]
        public async Task FailToMoveFileToExistingFileLocation()
        {
            //var sourceFile
            using var sourceFile = SafeFile.Create(GetNewFileFullPath());
            using var destinationFile = SafeFile.Create(GetNewFileFullPath());
            await using var chronicler = Chronicler.Begin();
            var stagingResponse = await chronicler.Move(sourceFile.FileName, destinationFile.FileName);
            var errorCodeString = stagingResponse.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), errorCodeString);
        }

        [Fact]
        public async Task FailToMoveFileToExistingFileLocationOnCommit()
        {
            //var sourceFile
            using var sourceFile = SafeFile.Create(GetNewFileFullPath());
            var destinationFileName = GetNewFileFullPath();
            await using var chronicler = Chronicler.Begin();
            var stagingResponse = await chronicler.Move(sourceFile.FileName, destinationFileName);

            //now create file after the stage and before the commit
            using var destinationFile = SafeFile.Create(destinationFileName);

            var response = await chronicler.Commit();
            var errorCodeString = response.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString() ,errorCodeString);
        }
    }
}
