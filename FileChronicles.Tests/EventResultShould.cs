using FunkyBasics.Either;
using Xunit;

namespace FileChronicles.Tests
{
    public class EitherResultShould
    {
        [Fact]
        public void HandleSuccess()
        {
            var successEvent = new EitherResult<string, string>.Right("YAY");
            var answer = successEvent.Match(x => x, errorInfo => errorInfo);
            Assert.Equal("YAY", answer);
        }


        [Fact] 
        public void HandleFailure()
        {
            var successEvent = new EitherResult<string, ErrorCode>.Right(ErrorCode.FileAlreadyExists);
            var answer = successEvent.Match(x => x, errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), answer);
        }
    }
}
