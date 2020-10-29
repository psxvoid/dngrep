using System;
using dngrep.tool.Abstractions.System;
using Moq;

namespace dngrep.tool.xunit.TestHelpers
{
    public static class MoqConsoleAsserts
    {
        public static void VerifyWrite(this Mock<IConsole> mock, string? text, Times times)
        {
            _ = mock ?? throw new ArgumentNullException(nameof(mock));

            mock.Verify(x => x.Write(It.Is<string>(it => it == text)), times);
        }
        
        public static void VerifyWriteLine(this Mock<IConsole> mock, string? text, Times times)
        {
            _ = mock ?? throw new ArgumentNullException(nameof(mock));

            mock.Verify(x => x.WriteLine(It.Is<string>(it => it == text)), times);
        }
        
        public static void VerifyWriteLineAny(this Mock<IConsole> mock, Times times)
        {
            _ = mock ?? throw new ArgumentNullException(nameof(mock));

            mock.Verify(x => x.WriteLine(It.IsAny<string>()), times);
        }
        
        public static void VerifyWriteLine(this Mock<IConsole> mock, Times times)
        {
            _ = mock ?? throw new ArgumentNullException(nameof(mock));

            mock.Verify(x => x.WriteLine(), times);
        }
    }
}
