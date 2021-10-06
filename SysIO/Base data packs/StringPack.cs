//Version 0.5

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        #region Base data packs
        /// <summary>
        /// A pack that holds a string of characters.
        /// </summary>
        public class StringPack : BasePack
        {
            public new string Data { get => (string)base.Data; set => base.Data = value; }
            public StringPack()
            {
                Data = "";
            }
            public StringPack(string value)
            {
                Data = value;
            }
            public StringPack(SingleLineString value)
            {
                Data = (string)value;
            }
            public override StringPack FromStringFile(SingleLineString content) =>
                 new StringPack(content);
            public override SingleLineString ToStringFile() =>
                this.Data;

            #region casting
            public static implicit operator StringPack(string value) =>
                new StringPack(value);
            public static implicit operator StringPack(char value) =>
                new StringPack(value + "");
            public static implicit operator StringPack(char[] value)
            {
                string s = "";
                foreach (char c in value)
                    s += c;
                return new StringPack(s);
            }
            public unsafe static implicit operator StringPack(char* str)
            {
                int counter = 0;
                string s = "";
                while (str[counter] != 3)
                    s += str[counter++];
                return new StringPack(s);
            }

            public static explicit operator string(StringPack value) =>
                value.Data;
            #endregion
        }
        #endregion
    }
}
