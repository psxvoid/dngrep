using System;
using Microsoft.Extensions.Logging;
using Moq;

namespace dngrep.tool.xunit.TestHelpers
{
    public static class MicrosoftLoggingMoqAsserts
    {
        public static void VerifyLogLevel(this Mock<ILogger> loggerMock, LogLevel logLevel, Times times)
        {
            _ = loggerMock ?? throw new ArgumentNullException(nameof(loggerMock));

            loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == logLevel),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            times);
        }
    }
}
