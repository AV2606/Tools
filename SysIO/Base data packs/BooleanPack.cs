using System;
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
        /// A pack that holds boolean integer value.
        /// </summary>
        public class BooleanPack : BasePack
        {
            public new bool Data { get => (bool)base.Data; set => base.Data = value; }
            public BooleanPack()
            {
                Data = false;
            }
            public BooleanPack(bool value)
            {
                Data = value;
            }
            public override BooleanPack FromStringFile(string content) =>
                 new BooleanPack(Convert.ToBoolean(content));
            public override string ToStringFile() =>
                this.Data + "";

            #region casting
            public static implicit operator BooleanPack(bool value) =>
                new BooleanPack(value);
            public static explicit operator bool(BooleanPack value) =>
                value.Data;
            #endregion
        }
        #endregion
    }
}
