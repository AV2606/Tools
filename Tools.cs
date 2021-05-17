using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
//using System.Windows.Media.Imaging;
using System.Threading;
//using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Windows;
using Tools.Extensions;
//Version 0.2

/// <summary>
/// Holds a veriaty of tools to handle efficent programms.
/// </summary>
namespace Tools
{
    /// <summary>
    /// Holds a verity of extension methods that can be found very helpful.
    /// </summary>
    namespace Extensions
    {
        /// <summary>
        /// Holds a sorting style.
        /// </summary>
        public enum SortStyle
        {
            /// <summary>
            /// Avarage case: O(n*n)<para>Worst case: O(n*n)</para>
            /// </summary>
            Bubble_Sort = 0xB0B1E,
            /// <summary>
            /// Avarage case: O(n*logn)<para>Worst case: O(n*logn)</para>
            /// </summary>
            Merge_Sort=0x17E78E,
            /// <summary>
            /// Avarage case: O(n*logn)<para>Worst case: O(n*logn)</para>
            /// </summary>
            Insertion_Sort = 0x1CE7,
            /// <summary>
            /// Randomizing the list until its sorted.
            /// <para>Best case: O(1)<para>Avarage case: O(n!)</para></para>
            /// </summary>
            Bogo_Sort=0xB080

        }
        public static class Extensions
        {
            #region ArrayRanking
            /// <summary>
            /// returns a singel dimensional array that is an equivilant to the two dimensional array 'target'
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public static T[] Reduce<T>(this T[,] target)
            {
                var arr = new T[target.GetLength(0) * target.GetLength(1)];
                int index = 0;
                foreach (var item in target)
                {
                    arr[index++] = item;
                }
                return arr;
            }
            /// <summary>
            /// tries to return the one dimensional array 'targert' to its original two dimensional array.
            /// </summary>
            /// <param name="target"></param>
            /// <returns></returns>
            public static T[,] Expande<T>(this T[] target, int firstDimensionSize)
            {
                T[,] r = new T[firstDimensionSize, Tools.Algebra.Round(target.Length / firstDimensionSize)];
                int target_index = 0; ;
                for (int i = 0; i < firstDimensionSize; i++)
                    for (int j = 0; j < r.GetLength(1); j++, target_index++)
                        r[i, j] = target[target_index];
                return r;
            }
            #endregion
            #region Sorting
            /// <summary>
            /// A method to use in the <see cref="Sort(T[], Compare)"/> which sorts two elements by the prefered propety.
            /// <para>
            /// should return True if item1 is "smaller" than item2.
            /// </para>
            /// <para>
            /// If returns the opposite it may reverse the order of the list.
            /// </para>
            /// </summary>
            /// <param name="item1"></param>
            /// <param name="item2"></param>
            /// <returns></returns>
            public delegate bool Compare<T>(T item1, T item2);        
            /// <summary>
            /// Sorting the array with <see cref="Compare"/> as the logic of the comparing method.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="method"></param>
            /// <returns></returns>
            public static IList<T> Sort<T>(this IList<T> array, Compare<T> method)
            {
                for (int i = 0; i < array.Count; i++)
                    for (int j = i; j < array.Count; j++)
                        if (method(array[j], array[i]))
                        {
                            array.Swap(j, i);
                        }
                return array;
            }
            /// <summary>
            /// Sorting this <see cref="IList{T}"/> object using <see cref="IComparable.CompareTo(T?)"/>
            /// </summary>
            /// <typeparam name="T">The type if this <see cref="IList{T}"/></typeparam>
            /// <param name="list">this object.</param>
            /// <param name="style">The sorting algoritm to use.</param>
            /// <returns></returns>
            public static IList<T> Sort<T>(this IList<T> list,SortStyle style) where T : IComparable
            {
                if (style == SortStyle.Merge_Sort)
                    return MergeSort(list);
                if (style == SortStyle.Insertion_Sort)
                    return InsertionSort(list);
                if (style == SortStyle.Bogo_Sort)
                    return BogoSort(list);
                return BubbleSort(list);
            }
            #region BogoSort
            private static IList<T> BogoSort<T>(this IList<T> list) where T: IComparable
            {
                Random rnd = new();
                for (int i = 0; i < (int)SortStyle.Bogo_Sort; i++)
                {
                    if (list.IsSorted())
                        return list;
                    list.Shuffle(rnd);
                }
                return list;
            }
            private static bool IsSorted<T>(this IList<T> list) where T: IComparable
            {
                if (list.Count == 0)
                    return true;
                var prev = list[0];
                for (int i = 1; i < list.Count; i++)
                    if (list[i].CompareTo(prev) < 0)
                        return false;
                return true;
            }
            #endregion
            #region InsertionSort
            /// <summary>
            /// Handles an insertion sort.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            private static IList<T> InsertionSort<T>(IList<T> list) where T : IComparable
            {
                List<T> r = new();
                foreach (var item in list)
                {
                    r.InsertToSorted(item);
                }
                return r;
            }
            /// <summary>
            /// Inserting a variable to a sorted list.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <param name="item"></param>
            private static void InsertToSorted<T>(this IList<T> list, T item) where T : IComparable
            {
                int ub = list.Count, lb = -1;
                while (ub - lb > 1)
                {
                    int index = (ub + lb) / 2;
                    int result = list[index].CompareTo(item);
                    if(result==0)
                    {
                        list.Insert(index, item);
                        return;
                    }
                    if (result > 0)
                        ub = index;
                    else
                        lb = index;
                }
                list.Insert(lb+1, item);
            }
            #endregion
            #region MergeSort
            /// <summary>
            /// Merge sorts <paramref name="list"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            /// <returns></returns>
            private static IList<T> MergeSort<T>(IList<T> list) where T:IComparable
            {
                return mergesort(list);
            }
            /// <summary>
            /// <see cref="MergeSort{T}(IList{T})"/> helper.
            /// </summary>
            private static IList<T> mergesort<T>(IList<T> l1) where T:IComparable
            {
                if (l1.Count==1)
                    return l1;
                List<T> left = new();
                left.AddRange(l1.SubList(0, l1.Count / 2));
                List<T> right = new();
                right.AddRange(l1.SubList(l1.Count / 2));
                left = mergesort(left) as List<T>;
                right = mergesort(right) as List<T>;
                return merge(right, left);
            }
            private static IList<T> merge<T>(List<T> l1, List<T> l2) where T:IComparable
            {
                List<T> r = new();
                while (l1.Count > 0)
                {
                    if(l2.Count==0)
                    {
                        r.AddRange(l1);
                        return r;
                    }
                    if (l1[0].CompareTo(l2[0]) < 0)
                    {
                        r.Add(l1[0]);
                        l1.RemoveAt(0);
                    }
                    else
                    {
                        r.Add(l2[0]);
                        l2.RemoveAt(0);
                    }
                }
                r.AddRange(l2);
                return r;
            }
            #endregion
            #region BubbleSort
            /// <summary>
            /// bubble sorts the <paramref name="list"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="list"></param>
            private static IList<T> BubbleSort<T>(IList<T> list) where T:IComparable
            {
                for (int i = list.Count-1;  i>-1; i--)
                {
                    int maxI = i;
                    for (int j = i; j >-1; j--)
                    {
                        if (list[maxI].CompareTo(list[j]) < 0)
                            maxI = j;
                    }
                    list.Swap(maxI, i);
                }
                return list;
            }
            #endregion
            #endregion
            /// <summary>
            /// Returns an array which his elements are a sub-sequence of this array.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="me"></param>
            /// <param name="startIndex">The index to start to copy this array from.</param>
            /// <param name="length">The length of the sub array.</param>
            /// <returns></returns>
            public static IEnumerable<T> SubList<T>(this IEnumerable<T> me, int startIndex = 0, int length = -1)
            {
                if (length < 0)
                    length = me.Count() - startIndex;
                T[] r = new T[length];
                for (int i = 0; i < length; i++)
                    r[i] = me.ElementAt(i + startIndex);
                return r;
            }
            /// <summary>
            /// Swaps the elements in <paramref name="index1"/> and <paramref name="index2"/>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="me"></param>
            /// <param name="index1"></param>
            /// <param name="index2"></param>
            public static void Swap<T>(this IList<T> me, int index1, int index2)
            {
                var temp = me[index1];
                me[index1] = me[index2];
                me[index2] = temp;
            }
            /// <summary>
            /// Shuffles this List.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="me"></param>
            /// <param name="rnd"></param>
            public static void Shuffle<T>(this IList<T> me, Random rnd = null)
            {
                if (rnd is null)
                    rnd = new Random();
                int length = me.Count;
                for (int i = 0; i < length; i++)
                {
                    me.Swap(i, rnd.Next(0, length));
                }
            }
            /// <summary>
            /// Returns a string that represents this <see cref="IEnumerable{T}"/> with human-friendly syntax.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="toStringLogic">Overloading the string represntive method of each element.</param>
            /// <returns></returns>
            public static string ToString<T>(this IEnumerable<T> me, Func<T, string> toStringLogic = null)
            {
                string r = " {";
                if (toStringLogic is null)
                    foreach (var item in me)
                    {
                        if (item is ValueType)
                            r += item + ", ";
                        else
                            r += item.ToString() + ", ";
                    }
                else
                    foreach (var item in me)
                    {
                        r += toStringLogic(item) + ", ";
                    }
                return r + "}";
            }
            #region getters
            /// <summary>
            /// Returns the Maximum value of the <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <typeparam name="T">Should implement the <see cref="IComparable"/></typeparam>
            /// <param name="me"></param>
            /// <returns></returns>
            public static T GetMax<T>(this IEnumerable<T> me) where T : IComparable
            {
                T max = me.ElementAt(0);
                foreach (var item in me)
                    if (item.CompareTo(max) > 0)
                        max = item;
                return max;
            }
            /// <summary>
            /// Returns the Minumum value of the <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <typeparam name="T">Should implement the <see cref="IComparable"/></typeparam>
            /// <param name="me"></param>
            /// <returns></returns>
            public static T GetMin<T>(this IEnumerable<T> me) where T : IComparable
            {
                T min = me.ElementAt(0);
                foreach (var item in me)
                    if (item.CompareTo(min) < 0)
                        min = item;
                return min;
            }
            /// <summary>
            /// Returns the index of the 'biggest' element of the <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <typeparam name="T">Should implement the <see cref="IComparable"/></typeparam>
            /// <param name="me"></param>
            /// <returns></returns>
            public static int GetMaxIndex<T>(this IEnumerable<T> me) where T : IComparable
            {
                int maxIndex = 0;
                int length = me.Count();
                for (int i = 1; i < length; i++)
                {
                    if (me.ElementAt(i).CompareTo(me.ElementAt(maxIndex)) > 0)
                        maxIndex = i;

                }
                return maxIndex;
            }
            /// <summary>
            /// Returns the index of the 'minumum' value of the <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <typeparam name="T">Should implement the <see cref="IComparable"/></typeparam>
            /// <param name="me"></param>
            /// <returns></returns>
            public static int GetMinIndex<T>(this IEnumerable<T> me) where T : IComparable
            {
                int minIndex = 0;
                int length = me.Count();
                for (int i = 1; i < length; i++)
                {
                    if (me.ElementAt(i).CompareTo(me.ElementAt(minIndex)) < 0)
                        minIndex = i;

                }
                return minIndex;
            }
            #endregion
            #region ColorHandling
            /// <summary>
            /// Multiplies all <paramref name="color"/>'s values (except opacity) by <paramref name="number"/>
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Color Multiply(this Color color, double number)
            {
                double[] d = new double[] { color.R * number, color.G * number, color.B * number };
                for (int i = 0; i < 3; i++)
                {
                    if (d[i] > 255)
                        d[i] = 255;
                    if (d[i] < 0)
                        d[i] = 0;
                }
                return Color.FromArgb(color.A, (int)d[0], (int)d[1], (int)d[2]);
            }
            /// <summary>
            /// Divides all <paramref name="color"/>'s values (except opacity) by <paramref name="number"/>
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Color Divide(this Color color, double number)
            {
                return Multiply(color, 1 / number);
            }
            /// <summary>
            /// Adds <paramref name="number"/> to all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Color Add(this Color color, int number)
            {
                int[] colors = new int[] { color.R + number, color.G + number, color.B + number };
                for (int i = 0; i < 3; i++)
                {
                    if (colors[i] > 255)
                        colors[i] = 255;
                    if (colors[i] < 0)
                        colors[i] = 0;
                }
                return Color.FromArgb(color.A, colors[0], colors[1], colors[2]);
            }
            /// <summary>
            /// Adds <paramref name="color1"/> to all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="color1"></param>
            /// <returns></returns>
            public static Color Add(this Color color, Color color1)
            {
                int[] colors = new int[] { color.R + color1.R, color.G + color1.G, color.B + color1.B };
                for (int i = 0; i < 3; i++)
                {
                    if (colors[i] > 255)
                        colors[i] = 255;
                    if (colors[i] < 0)
                        colors[i] = 0;
                }
                return Color.FromArgb(color.A, colors[0], colors[1], colors[2]);
            }
            /// <summary>
            /// Subs <paramref name="number"/> from all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Color Sub(this Color color, int number)
            {
                return Add(color, 0 - number);
            }
            /// <summary>
            /// Subs <paramref name="color1"/> from all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="color1"></param>
            /// <returns></returns>
            public static Color Sub(this Color color, Color color1)
            {
                return Add(color, color1.Multiply(-1));
            }
            #endregion
            /// <summary>
            /// returns the distance between the two points.
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point"></param>
            /// <returns></returns>
            public static double Distance(this Point point1, Point point)
            {
                double dx = point.X - point1.X;
                double dy = point.Y - point1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }
    }
    public static class ThreadPool
    {
        private static List<Thread> _pool;
        private static bool _keep_finished_threads;
        private static Thread _service_thread;
        private static CancellationTokenSource source;

        /// <summary>
        /// The number of thread currently pooled.
        /// </summary>
        public static int ThreadsCount
        {
            get
            {
                return _pool.Count;
            }
        }
        /// <summary>
        /// Indicates wether to keep managing threads that finished execution or discard them, <seealso cref="true"/>  by default.
        /// </summary>
        public static bool KeepFinishedThreads
        {
            get
            {
                return _keep_finished_threads;
            }
            set
            {
                _keep_finished_threads = value;
                if (_keep_finished_threads == false)
                {
                    try
                    {
                        if (_service_thread.IsAlive == false)
                            _service_thread.Start();
                    }
                    catch(Exception)
                    {
                        ReHandleGCS();
                        _service_thread.Start();
                    }
                }
                else
                {
                    source.Cancel(true);
                }
            }
        }
        public static IReadOnlyList<Thread> Pool
        {
            get
            {
                return _pool;
            }
        }


        /// <summary>
        /// Initialize the pool.
        /// if it is already initialized does nothing.
        /// </summary>
        public static void Initialize()
        {
            if (_pool is null)
                _pool = new List<Thread>();
            else
                return;
            source = new();
            _service_thread = new Thread(()=>GCService(source.Token));
            _service_thread.Name = "GCService";
            KeepFinishedThreads = true;
        }
        private static void ReHandleGCS()
        {
            _service_thread = new Thread(() => GCService(source.Token));
            _service_thread.Name = "GCService";
        }

        /// <summary>
        /// Adds the <see cref="Thread"/> and by default dont start it.
        /// <para>The other overloads might do start it by default.</para>
        /// </summary>
        /// <param name="t">The <see cref="Thread"/> to add.</param>
        /// <param name="Start">Indicates wether to start the <see cref="Thread"/> or not.</param>
        public static void Add(Thread t, bool Start = false)
        {
            _pool.Add(t);
            if (Start)
                t.Start();
        }
        /// <summary>
        /// Adds thread secified by the <see cref="ThreadStart"/> 'ts' and by default starts it.
        /// <para>The other overloads might not start it by default.</para>
        /// </summary>
        /// <param name="t">The <see cref="Thread"/> to add.</param>
        /// <param name="Name">The name of the thread.</param>
        /// <param name="Start">Indicates wetherto start the <see cref="Thread"/> or not.</param>
        public static void Add(ThreadStart ts,ThreadPriority priority=ThreadPriority.Normal, bool Start = true, string Name = "Unamed thread")
        {
            Thread t = new Thread(ts);
            t.Name = Name;
            t.Priority = priority;
            Add(t, Start);
        }
        /// <summary>
        /// Removes the thread from the pool and retrives it.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Thread Remove(Thread t)
        {
            var r = _pool.Remove(t);
            return r ? t : null;
        }
        /// <summary>
        /// Removes the thread from the pool and retrives it.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Thread Remove(int index)
        {
            var r = _pool.ElementAt(index);
            _pool.RemoveAt(index);
            return r;
        }
        /// <summary>
        /// Waits for the whole pool to finish.
        /// </summary>
        public static void Join()
        {
            for (int i = 0; i < _pool.Count; i++)
                _pool[i].Join();
        }
        /// <summary>
        /// Waits until <see cref="Thread"/> t finishes if the thread exists in the pool.
        /// </summary>
        /// <param name="t">The thread to wait until finish execution.</param>
        public static void Join(Thread t)
        {
            var th = _pool.Find(t1 => { return t1 == t; } );
            th.Join();
        }
        /// <summary>
        /// Waits until the <see cref="Thread"/> in the index location finishes execution.
        /// </summary>
        /// <param name="index">The thread's index to wait until finish execution.</param>
        public static void Join(int index)
        {
            _pool[index].Join();
        }
        /// <summary>
        /// Waits until the <see cref="Thread"/> with the name 'name' finishes.
        /// </summary>
        /// <param name="name">The thread's name to wait until finish execution.<para>
        /// if there are nultiple threads with the same name it wait till they all are finishd.</para></param>
        public static void Join(string name)
        {
            int length = _pool.Count;
            for (int i = 0; i < length; i++)
                if (_pool[i].Name == name)
                {
                    _pool[i].Join();
                    return;
                }
        }
        /// <summary>
        /// Removes all the threads that finished execution and reteives them.
        /// <para>
        /// Might be interrupted or give wrong data if <see cref="ThreadPool.KeepFinishedThreads"/> is set
        /// to <see langword="false"/>.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static List<Thread> RemoveAllFinished()
        {
            var p = _pool;
            var r = new List<Thread>();
            _pool = new List<Thread>();
            foreach (Thread t in p)
                if (t.IsAlive)
                    _pool.Add(t);
                else
                    r.Add(t);
            return r;
        }

        /// <summary>
        /// Checks whether a thread finished execution and needs to to be discarted.
        /// </summary>
        private static void GCService(CancellationToken c)
        {
            try
            {
            Start:
                c.ThrowIfCancellationRequested();
                while (!KeepFinishedThreads)
                {
                    c.ThrowIfCancellationRequested();
                    RemoveAllFinished();
                    Thread.Sleep(4);
                }
                while (KeepFinishedThreads)
                {
                    c.ThrowIfCancellationRequested();
                    Thread.Sleep(4);
                }
                goto Start;
            }
            catch (OperationCanceledException)
            {
                
            }
        }   
    }
    public class Algebra
    {
        /// <summary>
        /// returns true if the two <seealso cref="double"/>s are the same number untill the 3rd
        /// decimal digit.
        /// <para>
        /// for example:
        /// </para>
        /// <para>
        /// 1.3334 == 1.3335     =>true
        /// </para><para>
        /// 1.334 == 1.335       =>false
        /// </para>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool AlmostEquals(double left, double right)
        {
            //todo
            return false;
        }
        /// <summary>
        /// return the upper rounding product of d.
        /// </summary>
        /// <param name="d">examples of inputs-outputs
        /// <para>
        /// 2=>2, 2.2=>3, 2.5=>3, 2.99=>3, -2=>-2, -2.5=>-2....
        /// </para>
        /// </param>
        /// <returns></returns>
        public static int Round(double d)
        {
            if (((int)d) - d != 0)
                return d < 0 ? (int)d : (int)(d + 1);
            return (int)d;
        }
    }
    public static class Imageing
    {
        public class Resolution
        {
            public int Height { get; private set; }
            public int Width { get; private set; }

            public Resolution(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }
        }
        /// <summary>
        /// Represent an image of the basic pixel format 32-bit ARGB
        /// </summary>
        public class GenericImage
        {
            private uint[,] Canvas;
            public int Width { get { return Canvas.GetLength(0); } }
            public int Height { get { return Canvas.GetLength(1); } }
            /// <summary>
            /// Returns the pixel color in the index 1 and index2 position
            /// </summary>
            /// <param name="index1"></param>
            /// <param name="index2"></param>
            /// <returns></returns>
            public Color this[int index1, int index2]
            {
                get
                {
                    return Color.FromArgb((int)Canvas[index1, index2]);
                }
            }

            #region Constructors
            /// <summary>
            /// creates an empty 0 sized image
            /// </summary>
            public GenericImage()
            {
                Canvas = new uint[0, 0];
            }
            /// <summary>
            /// creates an image from the color 2 dimensional array
            /// </summary>
            /// <param name="image"></param>
            public GenericImage(Color[,] image)
            {
                Canvas = new uint[image.GetLength(0), image.GetLength(1)];
                for (int i = 0; i < Canvas.GetLength(0); i++)
                {
                    for (int j = 0; j < Canvas.GetLength(1); j++)
                    {
                        Canvas[i, j] = (uint)image[i, j].ToArgb();
                    }
                }
            }
            /// <summary>
            /// creates an image from the array which each cell represents a 32 bit color of ARGB
            /// </summary>
            /// <param name="image"></param>
            public GenericImage(int[,] image)
            {
                Canvas = new uint[image.GetLength(0), image.GetLength(1)];
                for (int i = 0; i < Canvas.GetLength(0); i++)
                    for (int j = 0; j < Canvas.GetLength(1); j++)
                        Canvas[i, j] = (uint)image[i, j];
            }
            /// <summary>
            /// creates an image from the byte[,] that every 4 adjacent cells represent a value of Alpha, Red, Green and Blue between 0-255.
            /// </summary>
            /// <param name="image"></param>
            public GenericImage(byte[,] image)
            {
                Canvas = new uint[image.GetLength(0) / 4, image.GetLength(1)];
                for (int i = 0; i < Canvas.GetLength(0); i++)
                {
                    for (int j = 0; j < Canvas.GetLength(1); j++)
                    {
                        Canvas[i, j] = 0;
                        Canvas[i, j] += (256 * 256 * 256 * (uint)image[i * 4, j] + 256 * 256 * (uint)image[i * 4 + 1, j] + 256 * (uint)image[i * 4 + 2, j] + image[i * 4 + 3, j]);
                    }
                }
            }
            /// <summary>
            /// creates an image from the array which each cell represents a 32 bit color of ARGB
            /// </summary>
            /// <param name="image"></param>
            public GenericImage(uint[,] image)
            {
                Canvas = new uint[image.GetLength(0), image.GetLength(1)];
                for (int i = 0; i < Canvas.GetLength(0); i++)
                    for (int j = 0; j < Canvas.GetLength(1); j++)
                        Canvas[i, j] = image[i, j];
            }
            /// <summary>
            /// Creates a black image with the attributes above.
            /// </summary>
            /// <param name="Width"></param>
            /// <param name="Height"></param>
            public GenericImage(int Width, int Height)
            {
                Canvas = new uint[Width, Height];
            }
            /// <summary>
            /// Creates Black image with theresulotion above.
            /// </summary>
            /// <param name="r"></param>
            public GenericImage(Resolution r)
            {
                Canvas = new uint[r.Width, r.Height];
            }
            #endregion
            #region Setters
            /// <summary>
            /// Sets the pixel int (x,y) to the color represented by <seealso cref="uint"/> color.
            /// </summary>
            /// <param name="color"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void SetPixel(uint color, int x, int y)
            {
                Canvas[x, y] = color;
            }
            /// <summary>
            /// Sets the pixel int (x,y) to the color represented by <seealso cref="Color"/> color.
            /// </summary>
            /// <param name="color"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void SetPixel(Color color, int x, int y)
            {
                Canvas[x, y] = (uint)color.ToArgb();
            }
            #endregion
            #region ImageMethods
            /// <summary>
            /// Makes the whole image the same color
            /// </summary>
            /// <param name="color"></param>
            public void Wipe(Color color)
            {
                uint c = (uint)color.ToArgb();
                int w = Width, h = Height;
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        Canvas[i, j] = c;
                    }
                }
            }
            /// <summary>
            /// Creates a gradient from up to down of the image.
            /// </summary>
            /// <param name="up">The color to be at the upper line of the image.</param>
            /// <param name="down">The color to be at the lower line of the image.</param>
            public void GradientUpDown(Color up, Color down)
            {
                int h = Height, w = Width;
                for (int j = 0; j < h; j++)
                {
                    double r = (down.R - up.R) * (j / (double)w) + up.R;
                    double g = (down.G - up.G) * (j / (double)w) + up.G;
                    double b = (down.B - up.B) * (j / (double)w) + up.B;
                    uint c = (uint)Color.FromArgb((int)r, (int)g, (int)b).ToArgb();
                    for (int i = 0; i < w; i++)
                    {
                        Canvas[i, j] = c;
                    }
                }
            }
            /// <summary>
            /// Creates a gradient from left to right of the image.
            /// </summary>
            /// <param name="left">The color to be at the most left line of the image.</param>
            /// <param name="right">The color to be at the most right line of the image.</param>
            public void GradientLeftRight(Color left, Color right)
            {
                int h = Height, w = Width;
                for (int i = 0; i < w; i++)
                {
                    double r = (right.R - left.R) * (i / (double)w) + left.R;
                    double g = (right.G - left.G) * (i / (double)w) + left.G;
                    double b = (right.B - left.B) * (i / (double)w) + left.B;
                    uint c = (uint)Color.FromArgb((int)r, (int)g, (int)b).ToArgb();
                    for (int j = 0; j < h; j++)
                    {
                        Canvas[i, j] = c;
                    }
                }
            }
            /// <summary>
            /// Creates gradient between 2 points
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="color1"></param>
            /// <param name="point2"></param>
            /// <param name="color2"></param>
            public void Gradient(Point point1, Color color1, Point point2, Color color2)
            {
                int width = this.Width;
                int height = this.Height;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        Point p = new Point(i, j);
                        double d1 = p.Distance(point1), d2 = p.Distance(point2);
                        double dis = d1 + d2;
                        Color c = Color.Black;
                        double ratio1 = d2 / dis;
                        double ratio2 = 1 - ratio1;
                        c = c.Add(color1.Multiply(ratio1));
                        c = c.Add(color2.Multiply(ratio2));
                        Canvas[i, j] = (uint)c.ToArgb();
                    }
                }
            }
            /// <summary>
            /// Replace every pixel with the old color with the new one
            /// </summary>
            /// <param name="_old">The old color to replace</param>
            /// <param name="_new">The new color to put in.</param>
            public void Replace(Color _old, Color _new)
            {
                uint old = (uint)_old.ToArgb();
                uint n = (uint)_new.ToArgb();
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        if (Canvas[i, j] == old)
                            Canvas[i, j] = n;
            }
            #endregion

            #region PrivateMethods
            /// <summary>
            /// Multiplies all <paramref name="color"/>'s values (except opacity) by <paramref name="number"/>
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            private Color Multiply(Color color, double number)
            {
                double[] d = new double[] { color.R * number, color.G * number, color.B * number };
                for (int i = 0; i < 3; i++)
                {
                    if (d[i] > 255)
                        d[i] = 255;
                    if (d[i] < 0)
                        d[i] = 0;
                }
                return Color.FromArgb(color.A, (int)d[0], (int)d[1], (int)d[2]);
            }
            /// <summary>
            /// Divides all <paramref name="color"/>'s values (except opacity) by <paramref name="number"/>
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            private Color Divide(Color color, double number)
            {
                return Multiply(color, 1 / number);
            }
            /// <summary>
            /// Adds <paramref name="number"/> to all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            private Color Add(Color color, int number)
            {
                int[] colors = new int[] { color.R + number, color.G + number, color.B + number };
                for (int i = 0; i < 3; i++)
                {
                    if (colors[i] > 255)
                        colors[i] = 255;
                    if (colors[i] < 0)
                        colors[i] = 0;
                }
                return Color.FromArgb(color.A, colors[0], colors[1], colors[2]);
            }
            /// <summary>
            /// Subs <paramref name="number"/> from all <paramref name="color"/>'s values. (except opecity)
            /// </summary>
            /// <param name="color"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            private Color Sub(Color color, int number)
            {
                return Add(color, 0 - number);
            }
            #endregion

            /*
            public WriteableBitmap ToWriteableBitmap()
            {
                WriteableBitmap bitmap = new WriteableBitmap(Width, Height, 1, 1, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
                Int32Rect rect = new Int32Rect(0, 0, Width, Height);
                bitmap.WritePixels(rect, Canvas, Width * 4, 0);
                return bitmap;
            }*/

            public unsafe Bitmap ToBitmap()
            {
                int w = Width, h = Height;
                Bitmap img = new Bitmap(w, h);
                var data = img.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int* ip = (int*)data.Scan0.ToPointer();
                uint* p = (uint*)ip;
                int index = 0;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        p[index++] = Canvas[j, i];
                    }
                }
                img.UnlockBits(data);
                var test = img.GetPixel(0, 0);
                return img;
            }
            #region Equalities
            public override bool Equals(object obj)
            {
                if (obj is GenericImage)
                    return this.Equals((GenericImage)obj);
                return base.Equals(obj);
            }
            private bool Equals(GenericImage img)
            {
                for (int i = 0; i < Canvas.GetLength(0); i++)
                    for (int j = 0; j < Canvas.GetLength(1); j++)
                        if (img.Canvas[i, j] != Canvas[i, j])
                            return false;
                return true;
            }
            /// <summary>
            /// Checks if the two images are considered equal
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator ==(GenericImage left, GenericImage right)
            {
                return left.Equals(right);
            }
            /// <summary>
            /// Check whether the two images are'nt considered equal
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator !=(GenericImage left, GenericImage right)
            {
                return !left.Equals(right);
            }
            public static bool IsNull(GenericImage image)
            {
                return image is null;
            }
            #endregion
            public static GenericImage From(System.Drawing.Image image)
            {
                GenericImage im = new GenericImage(image.Width, image.Height);
                Bitmap b = (Bitmap)image;
                int w = im.Width, h = im.Height;
                for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                        im.SetPixel(b.GetPixel(i, j), i, j);
                return im;
            }

            public override string ToString()
            {
                return "32 bit image: " + Width + "*" + Height;
            }
        }
    }    
}