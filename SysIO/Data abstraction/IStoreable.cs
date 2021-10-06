using System;
//Version 0.5

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        using System.IO;
        using System.Collections.Generic;
        #region Data abstractions
        /// <summary>
        /// Allows The object to be stored in the machine memory.
        /// </summary>
        public interface IStoreable: IWriteable, IReadable 
        {
            /// <summary>
            /// Writes to the file in the desired <paramref name="path"/>.
            /// <para>
            /// returns true if succed, or false if isn't.
            /// </para>
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public virtual bool WriteToFile(string path)
            {
                try
                {
                    File.WriteAllText(path, (string)this.ToStringFile());
                }
                catch(Exception)
                {
                    return false;
                }
                return true;
            }
            /// <summary>
            /// Tries to get the object from the file in <paramref name="path"/>
            /// </summary>
            /// <param name="path">The path to the files which should contain the object.</param>
            /// <param name="obj">An instance of the object to try to recover.</param>
            /// <returns></returns>
            public static IStoreable ReadFromFile(string path,IStoreable obj)
            {
                return obj.FromStringFile(File.ReadAllText(path));
            }
        }
        #endregion
    }
}
