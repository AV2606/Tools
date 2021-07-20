using System;
//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        public class TypeMismatchException : Exception
        {
            public TypeMismatchException(string message) : base(message) { }
        }
    }
}
