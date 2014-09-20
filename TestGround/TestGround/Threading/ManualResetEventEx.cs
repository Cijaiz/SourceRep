using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestGround.Threading
{
    public class ManualResetEventEx
    {
        double baseNumber, firstNumber, secondNumber, thirdNumber = 0;
        Random randomGenerator;

        private AutoResetEvent[] autoResetEvents;
        private ManualResetEvent manualResetEvent;

        public ManualResetEventEx()
        {
            autoResetEvents = new AutoResetEvent[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

            manualResetEvent = new ManualResetEvent(false);

        }

        internal void CalculateBase(object stateInfo)
        {
            Console.WriteLine("Calcualting base");
            Thread.Sleep(1000);
            baseNumber = randomGenerator.NextDouble();

            //Singnalling that base is ready..
            manualResetEvent.Set();
        }

        internal void CalculateFirst(object stateInfo)
        {
            Console.WriteLine("Calcualting first");
            Thread.Sleep(1000);
            double precalc = randomGenerator.NextDouble();

            manualResetEvent.WaitOne();

            firstNumber = precalc * baseNumber * randomGenerator.NextDouble();

            //Singnal task completion..
            autoResetEvents[0].Set();
        }

        internal void CalculateSecond(object stateInfo)
        {
            Console.WriteLine("Calcualting second");
            Thread.Sleep(1000);
            double precalc = randomGenerator.NextDouble();

            manualResetEvent.WaitOne();

            secondNumber = precalc * baseNumber * randomGenerator.NextDouble();

            //Singnal task completion..
            autoResetEvents[1].Set();
        }

        internal void CalculateThird(object stateInfo)
        {
            Console.WriteLine("Calcualting third");
            Thread.Sleep(1000);
            double precalc = randomGenerator.NextDouble();

            manualResetEvent.WaitOne();

            thirdNumber = precalc * baseNumber * randomGenerator.NextDouble();

            //Singnal task completion..
            autoResetEvents[2].Set();
        }


        internal double DisplayResult(int seed)
        {
            randomGenerator = new Random(seed);

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CalculateBase));

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CalculateFirst));

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CalculateSecond));

            ThreadPool.QueueUserWorkItem(
                new WaitCallback(CalculateThird));

            WaitHandle.WaitAll(autoResetEvents);

            manualResetEvent.Reset();

            return firstNumber + secondNumber + thirdNumber;
        }

    }
}
