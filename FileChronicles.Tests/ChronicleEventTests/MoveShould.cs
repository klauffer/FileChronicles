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

        [Fact]
        public async Task MoveTheSameFileTwice()
        {
            using var sourceFile = SafeFile.Create(GetNewFileFullPath());
            using var destinationFile1 = SafeFile.Clear(GetNewFileFullPath());
            using var destinationFile2 = SafeFile.Clear(GetNewFileFullPath());

            await using var chronicler = Chronicler.Begin();
            var stagingResponse1 = await chronicler.Move(sourceFile.FileName, destinationFile1.FileName);
            var stagingResponse2 = await chronicler.Move(destinationFile1.FileName, destinationFile2.FileName);
            var response = await chronicler.Commit();
            var doesFileExist = response.Match(() => File.Exists(destinationFile2.FileName), errorCode => false);
            Assert.True(doesFileExist);
        }

        [Fact]
        public async Task FailToMoveTheSameFileTwiceFromSameLocation()
        {
            using var sourceFile = SafeFile.Create(GetNewFileFullPath());
            using var destinationFile1 = SafeFile.Clear(GetNewFileFullPath(1));
            using var destinationFile2 = SafeFile.Clear(GetNewFileFullPath(2));

            await using var chronicler = Chronicler.Begin();
            var stagingResponse1 = await chronicler.Move(sourceFile.FileName, destinationFile1.FileName);
            var stagingResponse2 = await chronicler.Move(sourceFile.FileName, destinationFile2.FileName);

            var errorCodeString = stagingResponse2.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileDoesNotExist.ToString(), errorCodeString);
        }

        [Fact]
        public async Task FailToMoveThatDoesNotExist()
        {
            using var sourceFile = SafeFile.Clear(GetNewFileFullPath());
            using var destinationFile = SafeFile.Clear(GetNewFileFullPath());

            await using var chronicler = Chronicler.Begin();
            var stagingResponse = await chronicler.Move(sourceFile.FileName, destinationFile.FileName);

            var errorCodeString = stagingResponse.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileDoesNotExist.ToString(), errorCodeString);
        }
    }
}
