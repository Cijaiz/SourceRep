using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Runtime.Serialization;

namespace Octane.NotificationUtility
{
    /// <summary>
    /// Helper methods for Json.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Serializes the clr object to json string
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>Json string</returns>
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }

        public static string SerializeToXML<T>(T obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                string retVal = Encoding.Default.GetString(ms.ToArray());
                return retVal;
            }
        }

        /// <summary>
        /// Deserializes the json string to clr object of given Type T
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object</typeparam>
        /// <param name="json">Json string</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(string json)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                T obj = (T)serializer.ReadObject(ms);
                return obj;
            }
        }

        /// <summary>
        /// Newton soft's JSON Serializer for Web Calls. Advantages: High speed, no needs to specify datacontracts and datamembers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Newton soft's JSON De-Serializer for Web Calls. Advantages: High speed, no needs to specify datacontracts and datamembers.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
