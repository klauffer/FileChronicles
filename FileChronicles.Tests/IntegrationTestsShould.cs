using System.IO;
using System.Threading.Tasks;
using FileChronicles.Tests.Infrastructure;
using Xunit;

namespace FileChronicles.Tests
{
    public class IntegrationTestsShould : TestFixture
    {
        public IntegrationTestsShould()
        {
        }

        [Fact]
        public async Task HandleMultipleOfTheSameEvents()
        {
            var fileName1 = GetNewFileFullPath();
            using var safeFile1 = SafeFile.Clear(fileName1);

            var fileName2 = GetNewFileFullPath();
            using var safeFile2 = SafeFile.Clear(fileName2);

            await using var chronicler = Chronicler.Begin();

            var result1 = await chronicler.Create(fileName1, _fileContentsBytes, default);
            var result2 = await chronicler.Create(fileName2, _fileContentsBytes, default);

            var commitResult = await chronicler.Commit();

            var actualFileContents1 = File.ReadAllText(fileName1);
            var actualFileContents2 = File.ReadAllText(fileName2);

            Assert.Equal(_fileContents, actualFileContents1);
            Assert.Equal(_fileContents, actualFileContents2);
        }

        [Fact]
        public async Task HandleCreatingAndDeletingSeperateFiles()
        {
            var fileName1 = GetNewFileFullPath();
            using var safeFile1 = SafeFile.Clear(fileName1);

            var fileName2 = GetNewFileFullPath();
            using var safeFile2 = SafeFile.Create(fileName2);

            await using var chronicler = Chronicler.Begin();

            var result1 = await chronicler.Create(fileName1, _fileContentsBytes, default);
            var result2 = await chronicler.Delete(fileName2, default);

            var commitResult = await chronicler.Commit();

            var actualFileContents1 = File.ReadAllText(fileName1);
            var doesFile2Exist = File.Exists(fileName2);

            Assert.Equal(_fileContents, actualFileContents1);
            Assert.False(doesFile2Exist);
        }

        [Fact]
        public async Task HandleCreatingAndDeletingTheSameFiles()
        {
            var fileName1 = GetNewFileFullPath();
            using var safeFile1 = SafeFile.Clear(fileName1);

            await using var chronicler = Chronicler.Begin();

            var result1 = await chronicler.Create(fileName1, _fileContentsBytes, default);
            var result2 = await chronicler.Delete(fileName1, default);

            var commitResult = await chronicler.Commit();

            var doesFile2Exist = File.Exists(fileName1);

            Assert.False(doesFile2Exist);
        }

        [Fact]
        public async Task CreateMoveThenDeleteFile()
        {
            using var sourceFile = SafeFile.Clear(GetNewFileFullPath());
            using var destinationFile = SafeFile.Clear(GetNewFileFullPath());

            await using var chronicler = Chronicler.Begin();
            var createResult = await chronicler.Create(sourceFile.FileName, _fileContentsBytes, default);
            var moveResult = await chronicler.Move(sourceFile.FileName, destinationFile.FileName, default);
            var deleteResult = await chronicler.Delete(destinationFile.FileName, default);
            var commitResult = await chronicler.Commit();

            var isCreated = createResult.Match(x => true, errorCode => false);
            var isMoved = moveResult.Match(x => true, errorCode => false);
            var isDeleted = deleteResult.Match(x => true, errorCode => false);
            var isCommitted = commitResult.Match(x => true, errorCode => false);

            Assert.True(isCreated);
            Assert.True(isMoved);
            Assert.True(isDeleted);
            Assert.True(isCommitted);
        }
    }
}
