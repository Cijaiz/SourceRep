#region References
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
#endregion

namespace C2C.Core.Helper
{
    public static class SerializationHelper
    {
        /// <summary>
        /// Serializes the CLR object to JSON string
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns>JSON string</returns>
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

        /// <summary>
        /// Serializes the CLR object to XML string
        /// </summary>
        /// <typeparam name="T">Entity</typeparam>
        /// <param name="obj">Object to be serialized</param>
        /// <returns></returns>
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
        /// Desterilizes the JSON string to CLR object of given Type T
        /// </summary>
        /// <typeparam name="T">The type of the desterilized object</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>Desterilized object</returns>
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
        /// Newton soft's JSON Serialize for Web Calls. Advantages: High speed, no needs to specify [DataContracts] and [DataMembers].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Newton soft's JSON De-Serialize for Web Calls. Advantages: High speed, no needs to specify [DataContracts] and [DataMembers].
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
