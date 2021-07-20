using System;
using System.Collections.Generic;
using System.Linq;
//using System.Windows.Media.Imaging;
using System.Threading;
//using System.Windows.Media;
//Version 0.3

//You sould import from nuget the System.Drawing.Common
///Allow unsafe code in the project properties for the <seealso cref="Tools.Imageing.GenericImage.ToBitmap"/>()

/// <summary>
/// Holds a veriaty of tools to handle efficent programms.
/// </summary>
namespace Tools
{
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
}