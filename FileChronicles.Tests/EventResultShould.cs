using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileChronicles.Tests
{
    public class EventResultShould
    {
        [Fact]
        public void HandleSuccess()
        {
            var successEvent = new EventResult<string, string>.Success("YAY");
            var answer = successEvent.Match(successInfo => successInfo, errorInfo => errorInfo);
            Assert.Equal("YAY", answer);
        }

        [Fact] 
        public void HandleFailure()
        {
            var successEvent = new EventResult<string, ErrorCode>.Error(ErrorCode.FileAlreadyExists);
            var answer = successEvent.Match(success => success, errorCode => errorCode.ToString());
            Assert.Equal(ErrorCode.FileAlreadyExists.ToString(), answer);
        }
    }
}
