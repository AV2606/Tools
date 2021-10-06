using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.SysIO
{
    /// <summary>
    /// A string of characters of only one line.
    /// </summary>
    public class SingleLineString : IEnumerable<char>, IEnumerable, ICloneable, IComparable, IComparable<SingleLineString>, IConvertible, IEquatable<SingleLineString>
    {
        private string inValue;

        /// <summary>
        /// The length of the <see cref="char"/> sequence.
        /// </summary>
        public int Length => inValue.Length;
        /// <summary>
        /// Gets the <see cref="char"/> in the <paramref name="index"/> position
        /// </summary>
        /// <param name="index">The location in zero-based numbering of the item.</param>
        /// <returns></returns>
        public char this[int index] => inValue[index];

        public SingleLineString(string str)
        {
            inValue = ToOneLine(str);
        }
        #nullable enable
        public SingleLineString(char[]? str)
        {
            inValue = ToOneLine(new string(str));
        }
        #nullable disable
        public unsafe SingleLineString(char* str):this(new string(str))
        {
        }
        public unsafe SingleLineString(sbyte* str):this(new string(str)) { }
        public SingleLineString(ReadOnlySpan<char> str):this(new string(str)) { }
        public SingleLineString(char c,int count):this(new string(c, count)) { }
        public unsafe SingleLineString(char* c, int startIndex,int length):this(new string(c, startIndex, length)) { }
        public SingleLineString(char[] c,int startIndex,int length):this(new string(c, startIndex, length)) { }
        public unsafe SingleLineString(sbyte* c, int startIndex, int length) : this(new string(c, startIndex, length)) { }
        public unsafe SingleLineString(sbyte* c, int startIndex, int length,Encoding enc) : this(new string(c, startIndex, length,enc)) { }

        public SingleLineString(SingleLineString str)
        {
            inValue = str.inValue;
        }

        /// <summary>
        /// Converts a potentially multiline string to one line (cannot be undone).
        /// </summary>
        /// <param name="s">The string to convert.</param>
        /// <returns></returns>
        private string ToOneLine(string s)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in s)
            {
                if (item == '\r' || item == '\n')
                    continue;
                builder.Append(item);
            }
            return builder.ToString();
        }

        #region IEnumerable
        public IEnumerator<char> GetEnumerator()
        {
            return inValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inValue.GetEnumerator();
        }
        #endregion
        #region ICloneable
        public object Clone()
        {
            return new SingleLineString(this);
        }
        #endregion
        #region IComparable
        public int CompareTo(object obj)
        {
            return inValue.CompareTo(obj);
        }

        public int CompareTo(SingleLineString other)
        {
            return inValue.CompareTo(other.inValue);
        }
        #endregion
        #region IConvertible
        public TypeCode GetTypeCode()
        {
            return inValue.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToBoolean(provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToByte(provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToChar(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToDateTime(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToDecimal(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToDouble(provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToInt16(provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToInt32(provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToInt64(provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToSByte(provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToString(provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToType(conversionType,provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToUInt16(provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToUInt32(provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible)inValue).ToUInt64(provider);
        }

        #endregion
        #region IEquatable
        public bool Equals(SingleLineString other)
        {
            return inValue.Equals(other.inValue);
        }
        #endregion
        public static SingleLineString operator+(SingleLineString left,SingleLineString right)
        {
            return left.inValue + right.inValue;
        } 
        public static bool operator==(SingleLineString left,SingleLineString right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(SingleLineString left, SingleLineString right)
        {
            return !left.Equals(right);
        }
        public static implicit operator SingleLineString(string str)
        {
            return new SingleLineString(str);
        }
        public static explicit operator string(SingleLineString SLS)
        {
            return SLS.inValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (obj is SingleLineString other)
            {
                return other.Equals(this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ((object)this).GetHashCode();
        }
    }
}
