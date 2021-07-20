//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        //using System.Runtime.InteropServices;
        //using System.Text;

        #region Data abstractions

        /// <summary>
        /// Implements an object that can be written
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IWriteable
        {
            /// <summary>
            /// Converts this instance to its <see cref="string"/> representaion.
            /// </summary>
            /// <returns></returns>
            public string ToStringFile();
        }
        #endregion
    }
}
