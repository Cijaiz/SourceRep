using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestGround.Abstract_Class_Example;
using TestGround.Threading;

namespace TestGround
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press Any key to continue...");
            Console.ReadLine();

            #region Abstract Class Testing
            //Do Operations.
            //Hotel hotel = new Hotel(); 
            #endregion

            #region Threading test Ground

            TestGround.Threading.AutoResetEventEx.AllocateTask();

            //Manual Reset Event
            ManualResetEventEx calc = new ManualResetEventEx();
            Console.WriteLine("Result = {0}.",
                calc.DisplayResult(1).ToString());
            Console.WriteLine("Result = {0}.",
                calc.DisplayResult(2).ToString());


            #endregion
            

            Console.Read();
        }
    }
}
