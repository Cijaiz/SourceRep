using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace automapper
{
    public class MapperEngine
    {
        public static MapperEngine Instance()
        {
            return new MapperEngine();
        }

        public T MapEngine<T, X>(X input, T output)
        {
            Type inType = input.GetType();
            if (inType != null)
            {
                PropertyInfo[] inProperties = inType.GetProperties();
                foreach (PropertyInfo inInfo in inProperties)
                {
                    object propValue = inInfo.GetValue(input);

                    var outProperty = typeof(T).GetProperty(inInfo.Name);
                    if (outProperty == null)
                        continue;

                    outProperty.SetValue(output, propValue, null);
                }
            }
            return output;
        }
    }
}
