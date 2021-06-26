using System;
using System.Collections.Generic;
using System.Linq;
//Version 0.31

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        using System.Collections;
        using System.IO;
        using System.Threading;

        //using System.Runtime.InteropServices;
        //using System.Text;
        /*public class DataPack<T> where T:notnull
        {
            public T value { get; set; }
            public string URL { get; private set; }

            public DataPack(string url)
            {
                URL = url;
            }
            public DataPack(string url,T val)
            {
                URL = url;
                value = val;
            }
            private DataPack()
            {

            }

            public void WriteToFile(string url)
            {
                FileStream f = new FileStream(url, FileMode.OpenOrCreate, FileAccess.Write);
                var arr = ToByteArray(this.value);
                f.Write(arr, 0, arr.Length);
            }
            public static DataPack<T> ReadFromFile<T>(string url,int startIndex=0,int endIndex=-1,int timeout=10000) where T:notnull
            {
                DataPack<T> r = new DataPack<T>();
                FileStream f = new FileStream(url, FileMode.Open, FileAccess.Read);
                f.ReadTimeout = timeout;
                if (endIndex == -1)
                    endIndex = (int)f.Length - startIndex - 1;
                byte[] arr = new byte[endIndex - startIndex];
                f.Read(arr, startIndex, arr.Length);
                r = new DataPack<T>();
                r.value = FromByteArray<T>(arr);
                return r;
            }

            private unsafe static byte[] ToByteArray<T>(T data) where T:notnull
            {
                var size = Marshal.SizeOf<T>(data);
                byte[] bytes = new byte[size];
                var ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, ptr, false);
                Marshal.Copy(ptr, bytes, 0, size);
                Marshal.FreeHGlobal(ptr);
                return bytes;
            }
            private static T FromByteArray<T>(byte[] bytes) where T:notnull 
            {
                var ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                T r = (T)Marshal.PtrToStructure(ptr, typeof(T));
                return r;
            }
        }*/

        #region Data abstractions
        /// <summary>
        /// Implements an object that can be read.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IReadable
        {
            /// <summary>
            /// Creates an instance from <paramref name="content"/>.
            /// </summary>
            /// <param name="content">This object representaion in <see cref="string"/>, if <paramref name="content"/> is more than 1 line it might produce unexpected behavior.</param>
            /// <returns></returns>
            public IStoreable FromStringFile(string content);
        }
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
                    File.WriteAllText(path, this.ToStringFile());
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
            public abstract IStoreable FromStringFile(string content);
            public abstract string ToStringFile();
        }
        /// <summary>
        /// A pack that holds any integer value.
        /// </summary>
        public class IntPack : BasePack
        {
            public new long Data { get => (long)base.Data; set => base.Data = value; }
            public IntPack()
            {
                Data = 0;
            }
            public IntPack(long value)
            {
                Data = value;
            }
            public override IntPack FromStringFile(string content) =>
                 new IntPack(Convert.ToInt64(content));
            public override string ToStringFile() =>
                this.Data + "";

            #region casting
            public static implicit operator IntPack(long value)=>
                new IntPack(value);
            public static explicit operator long(IntPack value)=>
                value.Data;
            #endregion
        }
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
            public override StringPack FromStringFile(string content) =>
                 new StringPack(content);
            public override string ToStringFile() =>
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

        public enum DataBaseMsg
        {
            Succesfull_Operation = 0,
            Unknown_Exception = 0xE7707

        }
        public enum WriteMode
        {
            Append = 0,
            Overwrite = 1
        }
        public class DataBase<T> : IEnumerable<T>, IEnumerator<T>, ICollection<T> where T : IStoreable, new()
        {
            #region Instance members
            #region Fields and Properties
            public List<T> Info { get; set; } = new List<T>();
            public string URL { get; set; } = "\\";
            public readonly string FileExtension;
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize an instance of a database with the url and file extension desired.
            /// </summary>
            /// <param name="url">The basic path of the file with no extension</param>
            /// <param name="fileExtention">The file extension.</param>
            private DataBase(string url, string fileExtention)
            {
                this.URL = url;
                this.FileExtension = fileExtention;
            }
            /// <summary>
            /// Initialize an instance of a database with the full path, name and file extension of the data base file.
            /// </summary>
            /// <param name="fullURL">The full URL of the database.</param>
            private DataBase(string fullURL)
            {
                int dotindex = fullURL.LastIndexOf('.');
                string ex = fullURL.Substring(dotindex);
                this.FileExtension = ex;
                this.URL = fullURL.Substring(0, dotindex);
            }
            #endregion

            #region writes
            /// <summary>
            /// Writes all the database information to the machine storage.
            /// </summary>
            /// <param name="mode">The mode in which the file whould be written.</param>
            /// <param name="msg">A variable thats hold the database messages of the operation.</param>
            /// <returns></returns>
            public bool Write(WriteMode mode, out DataBaseMsg msg)
            {
                if (mode == WriteMode.Append)
                    return Write_append(out msg);
                return Write_overwrite(out msg);
            }
            /// <summary>
            /// Adds the file the information of this data base in the end of it.
            /// </summary>
            /// <param name="msg">The message to the user about the operation.</param>
            /// <returns></returns>
            private bool Write_append(out DataBaseMsg msg)
            {
                msg = DataBaseMsg.Succesfull_Operation;
                try
                {
                    foreach (var item in Info)
                    {
                        File.AppendText(item.ToStringFile()+Environment.NewLine);
                    }
                    return true;
                }
                catch (Exception)
                {
                    msg = DataBaseMsg.Unknown_Exception;
                    return false;
                }
            }
            /// <summary>
            /// Overwrites the file with the information in this database.
            /// </summary>
            /// <param name="msg">The message to the user about this operation.</param>
            /// <returns></returns>
            private bool Write_overwrite(out DataBaseMsg msg)
            {
                File.Delete(URL + FileExtension);
                return Write_append(out msg);
            }
            #endregion

            #region BasicMethods
            /// <summary>
            /// Loads the databse with the information from its file.
            /// </summary>
            /// <param name="msg">A variable thats hold the database messages of the operation.</param>
            /// <returns></returns>
            public bool Load(out DataBaseMsg msg)
            {
                msg = DataBaseMsg.Succesfull_Operation;
                try
                {
                    //var type = typeof(DataPack);
                    //var method = type.GetMethod("ReadFromURL");
                    //DataPack[] a = (DataPack[])method.Invoke(null, new object[] { this.URL + this.FileExtension, 0, 2 });
                    //this.Info = a.ToList();
                    //return true;
                    IStoreable storeable = new T();
                    string[] data = File.ReadAllLines(URL + FileExtension);
                    foreach (string s in data)
                        Info.Add((T)storeable.FromStringFile(s));
                    return true;
                }
                catch (Exception)
                {
                    msg = DataBaseMsg.Unknown_Exception;
                    return false;
                }
            }
            /// <summary>
            /// Deletes the file and all of its content.
            /// </summary>
            public void Delete()
            {
                File.Delete(URL + FileExtension);
            }
            /// <summary>
            /// Unites all the information from <paramref name="a"/> and <paramref name="b"/> into <paramref name="a"/>
            /// and calls <seealso cref="DataBase{T}.Delete()"/> in <paramref name="b"/>.
            /// </summary>
            /// <param name="a">The data base to add <paramref name="b"/>'s information into.</param>
            /// <param name="b">The data base to merge with <paramref name="a"/>.</param>
            /// <returns></returns>
            public static DataBase<T> Unite(DataBase<T> a, DataBase<T> b)
            {
                a.Info.AddRange(b.Info);
                DataBaseMsg msg;
                a.Write(WriteMode.Overwrite, out msg);
                if (msg != DataBaseMsg.Succesfull_Operation)
                    throw new Exception("An exception occured");
                b.Delete();
                return (DataBase<T>)a.MemberwiseClone();
            }
            #endregion

            #region Interfaces
            #region IEnumerator
            private int index = 0;
            public T Current => Info[index];
            object IEnumerator.Current => Info[index];

            public bool MoveNext()
            {
                return ++index < Info.Count;
            }
            public void Reset()
            {
                index = 0;
            }
            public void Dispose()
            {

            }
            #endregion
            #region ICollection
            public int Count => Info.Count;
            public bool IsReadOnly => false;
            public void Add(T item)
            {
                Info.Add(item);
            }
            public void Clear()
            {
                Info.Clear();
            }
            public bool Contains(T item)
            {
                return Info.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                Info.CopyTo(array, arrayIndex);
            }
            public bool Remove(T item)
            {
                return Info.Remove(item);
            }
            #endregion
            #region IEnumerable
            public IEnumerator<T> GetEnumerator()
            {
                return Info.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return Info.GetEnumerator();
            }
            #endregion
            #endregion
            #endregion

            #region Static members

            private static List<object> bases;
            public static IReadOnlyList<DataBase<T>> DataBases { get => (IReadOnlyList<DataBase<T>>)bases; }

            public static DataBase<T> GetDataBase(string URL = "\\")
            {
                //if (URL == "\\")
                return new DataBase<T>(URL);
                //return null;
            }
            #endregion
        }
    }
}
