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
        /// A pack that holds any number value.
        /// </summary>
        public class NumberPack : BasePack
        {
            public new decimal Data { get => (decimal)base.Data; set => base.Data = value; }
            public NumberPack()
            {
                Data = 0;
            }
            public NumberPack(decimal value)
            {
                Data = value;
            }
            public override NumberPack FromStringFile(string content) =>
                 new NumberPack(Convert.ToDecimal(content));
            public override string ToStringFile() =>
                this.Data + "";

            #region casting
            public static implicit operator NumberPack(decimal value) =>
                new NumberPack(value);
            public static implicit operator NumberPack(IntPack value)=>
                new NumberPack(value.Data);

            public static explicit operator decimal(NumberPack value) =>
                value.Data;
            #endregion
        }
        #endregion
    }
}
