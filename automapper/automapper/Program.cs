using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace automapper
{
    public class Program
    {
        public InClass In { get; set; }
        public OutClass Out { get; set; }

        public static Program Instance()
        {
            return new Program();
        }

        static void Main(string[] args)
        {
            var Instances = Instance();
            Instances.Map(null, null);
        }

        public void Map(object input, object output)
        {
            In = new InClass() { Name = "CIAJI", LastnAME = "ANANTHNAS", Age = 29 };
            Out = new OutClass() { ID = 1 };

            MapperManager.Map<OutClass, InClass>(In, Out);
        }
    }
}
