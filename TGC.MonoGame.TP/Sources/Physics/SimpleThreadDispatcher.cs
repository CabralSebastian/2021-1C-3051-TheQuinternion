using System;
using System.Diagnostics;
using System.Threading;
using BepuUtilities;
using BepuUtilities.Memory;

namespace TGC.MonoGame.TP.Physics
{
    public class SimpleThreadDispatcher : IThreadDispatcher, IDisposable
    {
        readonly int ThreadCountValue;
        public int ThreadCount => ThreadCountValue;
        struct Worker
        {
            public Thread Thread;
            public AutoResetEvent Signal;
        }

        readonly Worker[] Workers;
        readonly AutoResetEvent Finished;

        readonly BufferPool[] BufferPools;

        public SimpleThreadDispatcher(int threadCount)
        {
            this.ThreadCountValue = threadCount;
            Workers = new Worker[threadCount - 1];
            for (int i = 0; i < Workers.Length; ++i)
            {
                Workers[i] = new Worker { Thread = new Thread(WorkerLoop), Signal = new AutoResetEvent(false) };
                Workers[i].Thread.IsBackground = true;
                Workers[i].Thread.Start(Workers[i].Signal);
            }
            Finished = new AutoResetEvent(false);
            BufferPools = new BufferPool[threadCount];
            for (int i = 0; i < BufferPools.Length; ++i)
            {
                BufferPools[i] = new BufferPool();
            }
        }

        void DispatchThread(int workerIndex)
        {
            Debug.Assert(workerBody != null);
            workerBody(workerIndex);

            if (Interlocked.Increment(ref completedWorkerCounter) == ThreadCountValue)
            {
                Finished.Set();
            }
        }

        volatile Action<int> workerBody;
        int workerIndex;
        int completedWorkerCounter;

        void WorkerLoop(object untypedSignal)
        {
            var signal = (AutoResetEvent)untypedSignal;
            while (true)
            {
                signal.WaitOne();
                if (disposed)
                    return;
                DispatchThread(Interlocked.Increment(ref workerIndex) - 1);
            }
        }

        void SignalThreads()
        {
            for (int i = 0; i < Workers.Length; ++i)
            {
                Workers[i].Signal.Set();
            }
        }

        public void DispatchWorkers(Action<int> workerBody)
        {
            Debug.Assert(this.workerBody == null);
            workerIndex = 1; //Just make the inline thread worker 0. While the other threads might start executing first, the user should never rely on the dispatch order.
            completedWorkerCounter = 0;
            this.workerBody = workerBody;
            SignalThreads();
            //Calling thread does work. No reason to spin up another worker and block this one!
            DispatchThread(0);
            Finished.WaitOne();
            this.workerBody = null;
        }

        volatile bool disposed;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                SignalThreads();
                for (int i = 0; i < BufferPools.Length; ++i)
                {
                    BufferPools[i].Clear();
                }
                foreach (var worker in Workers)
                {
                    worker.Thread.Join();
                    worker.Signal.Dispose();
                }
            }
        }

        public BufferPool GetThreadMemoryPool(int workerIndex)
        {
            return BufferPools[workerIndex];
        }
    }

}