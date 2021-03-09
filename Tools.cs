using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Media;
using Color = System.Drawing.Color;
using System.Windows;

public class Tools
{
    public static void DoNothin() { }

    public class ArrayTools<T>
    {
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
        public delegate bool Compare(T item1, T item2);
        /// <summary>
        /// returns a singel dimensional array that is an equivilant to the two dimensional array 'target'
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T[] Reduce(T[,] target)
        {
            var arr = new T[target.GetLength(0) * target.GetLength(1)];
            int index = 0;
            foreach(var item in target)
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
        public static T[,] Expande(T[] target, int firstDimension)
        {
            T[,] r = new T[firstDimension, Algebra.Round(target.Length/firstDimension)];
            int target_index = 0; ;
            for (int i = 0; i < firstDimension; i++)
                for (int j = 0; j < r.GetLength(1); j++,target_index++)
                    r[i,j] = target[target_index];
            return r;
        }
        /// <summary>
        /// Sorting the array with <see cref="Compare"/> as the logic of the comparing method.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T[] Sort(T[] array,Compare method)
        {
            for (int i = 0; i < array.Length; i++)
                for (int j = i; j < array.Length; j++)
                    if(method(array[j],array[i]))
                    {
                        var temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
            return array;
        }
    }
    public static class ThreadPool
    {
        static Queue<Thread> pool;
        /// <summary>
        /// The number of thread currently pooled.
        /// </summary>
        public static int Threads { get
            {
                return pool.Count;
            } }
        /// <summary>
        /// Indicates wether to keep managing threads that finished execution or discard them, false by default.
        /// </summary>
        public static bool KeepFinishedThreads { get; set; }
        public static Thread[] Pool
        {
            get
            {
                return pool.ToArray();
            }
        }
        private static Thread Service;

        /// <summary>
        /// Adds the <see cref="Thread"/> and by default dont start it.
        /// <para>The other overloads might do start it by default.</para>
        /// </summary>
        /// <param name="t">The <see cref="Thread"/> to add.</param>
        /// <param name="Start">Indicates wether to start the <see cref="Thread"/> or not.</param>
        public static void Add(Thread t,bool Start=false)
        {
            pool.Enqueue(t);
            t.Priority = ThreadPriority.Highest;
            if (Start)
                t.Start();
        }
        /// <summary>
        /// Adds thread secified by the <see cref="ThreadStart"/> 'ts' and by default start it.
        /// <para>The other overloads might not start it by default.</para>
        /// </summary>
        /// <param name="t">The <see cref="Thread"/> to add.</param>
        /// <param name="Name">The name of the thread.</param>
        /// <param name="Start">Indicates wetherto start the <see cref="Thread"/> or not.</param>
        public static void Add(ThreadStart ts, bool Start = true ,string Name="Unamed thread")
        {
            Thread t = new Thread(ts);
            t.Name = Name;
            Add(t,Start);
        }
        public static Thread Remove(Thread t)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (t == pool.Peek())
                    return pool.Dequeue();
                pool.Enqueue(pool.Dequeue());
            }
            return null;
        }
        public static Thread Remove(int index)
        {
            var temp = new Queue<Thread>();
            for (int i = 0; i < index; i++)
                pool.Enqueue(pool.Dequeue());
            var ret = pool.Dequeue();
            foreach (Thread t in pool)
                temp.Enqueue(t);
            pool = temp;
            return ret;
        }
        /// <summary>
        /// Waits for the whole pool to finish.
        /// </summary>
        public static void Join()
        {
            foreach (Thread t in pool)
                t.Join();
        }
        /// <summary>
        /// Waits until <see cref="Thread"/> t finishes.
        /// </summary>
        /// <param name="t">The thread to wait until finish execution.</param>
        public static void Join(Thread t)
        {
            foreach (Thread t1 in pool)
                if (t1 == t)
                    t1.Join();
        }
        /// <summary>
        /// Waits until the <see cref="Thread"/> in the index location finishes.
        /// </summary>
        /// <param name="index">The thread's index to wait until finish execution.</param>
        public static void Join(int index)
        {
            pool.ElementAt(index).Join();
        }
        /// <summary>
        /// Waits until the <see cref="Thread"/> with the name 'name' finishes.
        /// </summary>
        /// <param name="name">The thread's name to wait until finish execution.<para>
        /// if there are nultiple threads with the same name it wait till they all are finishd.</para></param>
        public static void Join(string name)
        {
            int maxi = pool.Count;
            for (int i = 0; i < maxi; i++)
            {
                try
                {
                    if (pool.ElementAt(i).Name == name)
                        pool.ElementAt(i).Join();
                }
                catch (Exception)
                {

                }
            }
                
        }
        public static Thread[] RemoveAllFinished()
        {
            //Queue<Thread> finished = new Queue<Thread>();
            int maxi = pool.Count;
            for (int i = 0; i < maxi; i++)
            {
                if (pool.Peek().IsAlive == false)
                    pool.Dequeue();
                else
                pool.Enqueue(pool.Dequeue());
            }
            return pool.ToArray();
        }
        /// <summary>
        /// Initialize the pool.
        /// if it is already initialized does nothing.
        /// </summary>
        public static void Initialize()
        {
            if (pool == null)
                pool = new Queue<Thread>();
            else
                return;
            KeepFinishedThreads = false;
            Service = new Thread(GCService);
            Service.Name = "GCService";
            Service.Start();
        }
        /// <summary>
        /// Checks whether a thread finished execution and needs to to be discarted.
        /// </summary>
        private static void GCService()
        {
            Start:
            while (!KeepFinishedThreads)
                RemoveAllFinished();
            while (KeepFinishedThreads)
                Tools.DoNothin();
            goto Start;
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
                return d<0?(int)d:(int)(d + 1);
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
            public Color this[int index1,int index2]
            {
                get
                {
                    return Color.FromArgb((int)Canvas[index1, index2]);
                }
            }
            
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
                Canvas = new uint[image.GetLength(0)/4, image.GetLength(1)];
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

            /// <summary>
            /// Sets the pixel int (x,y) to the color represented by <seealso cref="uint"/> color.
            /// </summary>
            /// <param name="color"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public void SetPixel(uint color,int x, int y)
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
            public void GradientUpDown(Color up,Color down)
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
            /// Replace every pixel with the old color with the new one
            /// </summary>
            /// <param name="_old">The old color to replace</param>
            /// <param name="_new">The new color to put in.</param>
            public void Replace(Color _old,Color _new)
            {
                uint old = (uint)_old.ToArgb();
                uint n = (uint)_new.ToArgb();
                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        if (Canvas[i, j] == old)
                            Canvas[i, j] = n;
            }

            public WriteableBitmap ToWriteableBitmap()
            {
                WriteableBitmap bitmap = new WriteableBitmap(Width, Height, 1, 1, PixelFormats.Bgra32, BitmapPalettes.Halftone256Transparent);
                Int32Rect rect = new Int32Rect(0, 0, Width, Height);
                bitmap.WritePixels(rect, Canvas, Width * 4, 0);
                return bitmap;
            }
            public unsafe Bitmap ToBitmap()
            {
                int w = Width, h = Height;
                Bitmap img = new Bitmap(w, h);
                var data= img.LockBits(new Rectangle(0, 0, w, h), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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
                return "32 bit image: "+Width + "*" + Height;
            }
        }
    }
}
