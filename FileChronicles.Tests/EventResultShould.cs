using Xunit;

namespace FileChronicles.Tests
{
    public class EventResultShould
    {
        [Fact]
        public void HandleSuccess()
        {
            var successEvent = new EventResult<string>.Success();
            var answer = successEvent.Match(() => "YAY", errorInfo => errorInfo);
            Assert.Equal("YAY", answer);
        }

        [Fact] 
        public void HandleFailure()
        {
            var successEvent = new EventResult<ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            var answer = successEvent.Match(() => "Doh!", errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), answer);
        }
    }
}
