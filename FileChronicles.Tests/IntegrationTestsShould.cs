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
    }
}
