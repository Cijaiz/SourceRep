using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestGround.Threading
{
    public static class AutoResetEventEx
    {
        private static AutoResetEvent autoResetEvent;

        public static void DoWork()
        {
            Console.WriteLine("Waiting for the Signal");
            autoResetEvent.WaitOne();
            Console.WriteLine("Starting the Worker Task");
        }

        public static void AllocateTask()
        {
            Console.WriteLine("Start Thread Allocation");
            autoResetEvent = new AutoResetEvent(false);

            Thread myTask = new Thread(DoWork);
            myTask.Start();
            
            Console.WriteLine("Allocated the Task to the Thread");

            Thread.Sleep(10000);

            Console.WriteLine("Signalling the Task to be initiated..");
            autoResetEvent.Set();
        }
    }
}
