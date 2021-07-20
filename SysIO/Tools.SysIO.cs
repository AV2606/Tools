using System;
using System.Collections.Generic;
using System.Linq;
//Version 0.4

namespace Tools
{
    /// <summary>
    /// Encapuslate communication with the system and files.
    /// </summary>
    namespace SysIO
    {
        using System.Collections;
        using System.IO;

        /// <summary>
        /// Writes the data with the desired method.
        /// </summary>
        public enum WriteMode
        {
            /// <summary>
            /// Appends this data to the end of the data base.
            /// </summary>
            Append = 0,
            /// <summary>
            /// Overwrites the whole database with this data.
            /// </summary>
            Overwrite = 1
        }

        /// <summary>
        /// A base class for <see cref="DataBase{T}"/>
        /// </summary>
        public class DataBase
        {
            protected static List<DataBase> bases = new List<DataBase>();
            public static IReadOnlyList<DataBase> Bases { get => bases; }
            /// The location of the database.
            /// </summary>
            public string URL { get; set; } = "\\";
            /// <summary>
            /// The file extension of this database file.
            /// </summary>
            public readonly string FileExtension;
            /// <summary>
            /// Returns the full URL of the DataBase.
            /// </summary>
            public string FullURL => URL + FileExtension;

            protected DataBase(string url,string fileExtension)
            {
                this.URL = url;
                this.FileExtension = fileExtension;
            }
            protected DataBase(string fullURL)
            {
                int dotindex = fullURL.LastIndexOf('.');
                string ex = fullURL.Substring(dotindex);
                this.FileExtension = ex;
                this.URL = fullURL.Substring(0, dotindex);
            }
        }

        /// <summary>
        /// Handles a communication connection with system storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class DataBase<T> : DataBase, IEnumerable<T>, IEnumerator<T>, ICollection<T>, IList<T> where T : IStoreable, new()
        {
            #region Instance members

            #region Fields and Properties
            /// <summary>
            /// Field for <see cref="DataBase{T}.Info"/>.
            /// </summary>
            private List<T> _info = new List<T>();

            /// <summary>
            /// <summary>
            /// A map of all the initializtion IStorables types.
            /// </summary>
            private Dictionary<string, IStoreable> Types = new Dictionary<string, IStoreable>();

            /// <summary>
            /// The data base data.
            /// </summary>
            public IReadOnlyList<T> Info { get => _info; }

            //From IList<T>
            public T this[int index] { get => ((IList<T>)Info)[index]; set => ((IList<T>)Info)[index] = value; }
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize an instance of a database with the url and file extension desired.
            /// </summary>
            /// <param name="url">The basic path of the file with no extension</param>
            /// <param name="fileExtention">The file extension.</param>
            private DataBase(string url, string fileExtention) : base(url, fileExtention)
            {
                PostConstruct();
            }
            /// <summary>
            /// Initialize an instance of a database with the full path, name and file extension of the data base file.
            /// </summary>
            /// <param name="fullURL">The full URL of the database.</param>
            private DataBase(string fullURL) : base(fullURL)
            {
                PostConstruct();
            }
            /// <summary>
            /// Some initializing procedures.
            /// </summary>
            private void PostConstruct()
            {
                //Info = new List<T>();
                bases.Add(this);
            }
            #endregion

            #region writes
            /// <summary>
            /// Writes all the database information to the machine storage.
            /// </summary>
            /// <param name="mode">The mode in which the file whould be written.</param>
            /// <returns></returns>
            public bool Write(WriteMode mode)
            {
                if (mode == WriteMode.Append)
                    return Write_append();
                return Write_overwrite();
            }
            /// <summary>
            /// Adds the file the information of this data base in the end of it.
            /// </summary>
            /// <returns></returns>
            private bool Write_append()
            {
                try
                {
                    foreach (var item in Info)
                    {
                        //Item's type.
                        var vl = item.GetType().FullName + ":";
                        //Item's string+new line.
                        vl += item.ToStringFile() + Environment.NewLine;
                        //Saves.
                        File.AppendAllText(URL + FileExtension, vl);
                    }
                    return true;
                }
                catch (DirectoryNotFoundException e)
                {
                    throw;
                    return false;
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            /// <summary>
            /// Overwrites the file with the information in this database.
            /// </summary>
            /// <returns></returns>
            private bool Write_overwrite()
            {
                File.Delete(URL + FileExtension);
                return Write_append();
            }
            #endregion

            #region BasicMethods
            /// <summary>
            /// Loads the databse with the information from its file.
            /// </summary>
            /// <returns></returns>
            public bool Load()
            {
                try
                {
                    if (File.Exists(URL + FileExtension) == false)
                    {
                        var s = File.Create(URL + FileExtension);
                        s.Close();
                    }
                    //IStoreable storeable = new T();
                    string[] data = File.ReadAllLines(URL + FileExtension);
                    foreach (string s in data)
                    {
                        //Info.Add((T)storeable.FromStringFile(s));
                        string TypeName = s.Substring(0, s.IndexOf(':'));
                        string s1 = s.Substring(s.IndexOf(':') + 1);
                        IStoreable storeable;
                        if (this.Types.ContainsKey(TypeName))
                            storeable = Types[TypeName];
                        else
                        {
                            var lvl1 = Type.GetType(TypeName);
                            var lvl2 = lvl1.GetConstructor(Type.EmptyTypes);
                            var lvl3 = lvl2.Invoke(null);
                            storeable = (IStoreable)lvl3;
                            Types.Add(TypeName, storeable);
                        }
                        this.Add((T)storeable.FromStringFile(s1));
                    }
                    return true;
                }
                catch (Exception e)
                {
                    throw;
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
                a._info.AddRange(b.Info);
                a.Write(WriteMode.Overwrite);
                b.Delete();
                return a;
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
                bases.Remove(this);
            }
            #endregion
            #region ICollection
            public int Count => Info.Count;
            public bool IsReadOnly => false;
            public void Add(T item)
            {
                if (item.GetType().GetConstructor(Type.EmptyTypes) is null)
                    throw new TypeMismatchException($"The item '{item.GetType().FullName}' must have a constructor with 0 arguments!");
                _info.Add(item);
            }
            public void Clear()
            {
                _info.Clear();
            }
            public bool Contains(T item)
            {
                return Info.Contains(item);
            }
            public void CopyTo(T[] array, int arrayIndex)
            {
                _info.CopyTo(array, arrayIndex);
            }
            public bool Remove(T item)
            {
                return _info.Remove(item);
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
            #region IList
            public int IndexOf(T item)
            {
                return _info.IndexOf(item);
            }
            public void Insert(int index, T item)
            {
                if (item.GetType().GetConstructor(Type.EmptyTypes) is null)
                    throw new TypeMismatchException($"The item '{item.GetType().FullName}' must have a constructor with 0 arguments!");
                _info.Insert(index, item);
            }
            public void RemoveAt(int index)
            {
                _info.RemoveAt(index);
            }
            #endregion
            #endregion
            #endregion

            #region Static members
            /// <summary>
            /// All the <seealso cref="DataBase{T}"/> that are available.
            /// </summary>
            public static IReadOnlyList<DataBase<T>> DataBases
            {
                get => GetDataBases(bases);
            }

            /// <summary>
            /// Helper method for <seealso cref="DataBases"/>
            /// </summary>
            /// <param name="bases">The <seealso cref="List{Tools.SysIO.DataBase}"/> </param>
            /// <returns></returns>
            private static IReadOnlyList<DataBase<T>> GetDataBases(IEnumerable<DataBase> bases)
            {
                var r = new List<DataBase<T>>();
                foreach (var item in bases)
                    if (item.GetType() == typeof(DataBase<T>))
                        r.Add(item as DataBase<T>);
                return r;
            }

            /// <summary>
            /// Returns the data bases the user asked for, if the data base already exists returns a reference to it.
            /// </summary>
            /// <param name="fullURL">The URL of the data base.</param>
            /// <param name="IgnoreMissMatchExistingDBs">Indicates whether to ignore already existing 
            /// different data bases types with the same URL
            /// or not.</param>
            /// <returns>The specified <seealso cref="DataBase{T}"/></returns>
            public static DataBase<T> GetDataBase(string fullURL = "\\",bool IgnoreMissMatchExistingDBs=false)
            {
                if (fullURL == "\\")
                    throw new InvalidURLException("The URL MUST be defined!");

                var relevants = DataBase.bases.Where((db) => { return db.FullURL == fullURL; });
                int count = relevants.Count();

                if (count == 0)
                    return new DataBase<T>(fullURL);

                if (IgnoreMissMatchExistingDBs)
                    return ReturnRelevant(fullURL, relevants);

                if(count>1)
                    throw new InterferingDataBasesException("There are already existeing data bases with the same URL but with different types!");

                if (relevants.ElementAt(0) is DataBase<T>)
                    return relevants.ElementAt(0) as DataBase<T>;
                throw new InterferingDataBasesException("There are already existeing data bases with the same URL but with different types!");
            }
            /// <summary>
            /// Returns the relevant <see cref="DataBase{T}"/>.
            /// </summary>
            /// <param name="URL">The URL of the data base.</param>
            /// <param name="relevants">A list of all the databases with the specified URL.</param>
            /// <returns></returns>
            private static DataBase<T> ReturnRelevant(string URL, IEnumerable<DataBase> relevants)
            {
                var r = GetDataBases(relevants);
                if (r.Count > 0)
                    return r[0];
                return new DataBase<T>(URL);
            }
            #endregion
        }
    }
}
