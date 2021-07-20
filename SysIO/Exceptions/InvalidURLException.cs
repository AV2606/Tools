using System;
//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        public class InvalidURLException : Exception
        {
            public InvalidURLException(string message) : base(message)
            {
            }
        }
    }
}
