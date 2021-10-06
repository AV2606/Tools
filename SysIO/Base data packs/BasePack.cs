//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        #region Base data packs
        /// <summary>
        /// A basic pack class that holds a variable which can be stored on a hard drive.
        /// </summary>
        public abstract class BasePack : IStoreable 
        {
            /// <summary>
            /// The object that is the data which should be stored.
            /// </summary>
            protected object data;
            /// <summary>
            /// The data that this pack holds.
            /// </summary>
            public virtual object Data { get => data; set => data = value; }
            public BasePack()
            {
                data = new object();
            }
            public abstract IStoreable FromStringFile(SingleLineString content);
            public abstract SingleLineString ToStringFile();
        }
        #endregion
    }
}
