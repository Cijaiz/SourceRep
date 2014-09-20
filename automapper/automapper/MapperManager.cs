using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace automapper
{
    public static class MapperManager
    {
        public static T Map<T, X>(X inputObject, T outputObject)
        {
            MapperEngine.Instance().MapEngine(inputObject, outputObject);
            return outputObject;
        }
    }
}
