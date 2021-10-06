using System;
//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {

        public interface IWriteable
        {
            /// <summary>
            /// Converts this instance to its <see cref="SingleLineString"/> representaion.
            /// </summary>
            /// <returns></returns>
            public SingleLineString ToStringFile();
        }
    }
}
