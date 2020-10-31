using System;

namespace dngrep.tool.Core.Exceptions
{
    public class GrepException : Exception
    {

        public GrepException()
        {
        }

        public GrepException(string message) : base(message)
        {
        }

        public GrepException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
