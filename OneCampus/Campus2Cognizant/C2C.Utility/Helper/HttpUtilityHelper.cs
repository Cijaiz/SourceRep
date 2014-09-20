#region References
using System;
using System.Web;
#endregion

namespace C2C.Core.Helper
{
    public class HttpUtilityHelper
    {
        /// <summary>
        /// Deserializes the cookie value to entity T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName">Name of the cookie</param>
        /// <returns>Entity of type T</returns>
        public static T GetCookie<T>(string cookieName)
        {
            Guard.IsNotBlank(cookieName, "CookieName");

            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            object deserializedObject = SerializationHelper.JsonDeserialize<T>(cookie.Value);
            return (T)deserializedObject;
        }

        /// <summary>
        /// Encrypt the key and retrieve corresponding value. Deserialize the same to of type entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cookieName">Name of the cookie</param>
        /// <param name="key">Key of teh coookie</param>
        /// <returns>Entity of type T</returns>
        public static T GetDecryptedCookie<T>(string cookieName, string key)
        {
            Guard.IsNotBlank(cookieName, "CookieName");
            Guard.IsNotBlank(key, "CookieKey");

            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];

            // Encrypt the key and retrieve the corresponding value
            string decryptValue = Encryptor.DecryptStringAES(cookie[key]);

            //Deserialize the decrypted value to an object of type T
            T deserializedObject = SerializationHelper.JsonDeserialize<T>(decryptValue);

            return deserializedObject;
        }

        /// <summary>
        /// Will serialise the entity T and store it in Cookie. Returns cookie.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Parameter of type T</param>
        /// <param name="cookieName">Name of the cookie</param>
        /// <returns></returns>
        public static HttpCookie SetCookie<T>(T entity, string cookieName)
        {
            Guard.IsNotBlank(cookieName, "CookieName");
            Guard.IsNotNull(entity, "EntityValue");

            // Initialize a cookie and save the serialised entity as Cookie value.
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = SerializationHelper.JsonSerialize<T>(entity);

            HttpContext.Current.Response.Cookies.Add(cookie);
            return cookie;
        }

        /// <summary>
        /// Will create a new HttpCookie based on the parameters passed
        /// </summary>
        /// <param name="cookiename">Name of the cookie</param>
        /// <param name="key"></param>
        /// <param name="value">Value assigned to the cookie</param>
        /// <returns></returns>
        public static HttpCookie SetCookie(string cookieName, string key, string value, int expiryMinutes = 0)
        {
            Guard.IsNotBlank(cookieName, "CookieName");
            Guard.IsNotBlank(value, "CookieValue");
            HttpCookie newcookie;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];

            if (cookie == null)
                // Initialize a cookie 
                newcookie = new HttpCookie(cookieName);
            else
                newcookie = cookie;

            // Delets the key value from cookie if key exists.
            if (newcookie.HasKeys)
            {
                foreach (string cookieKey in newcookie.Values.AllKeys)
                {
                    if (key == cookieKey)
                        newcookie.Values.Remove(cookieKey);
                }
            }

            cookie[key] = value;
            if (expiryMinutes > 0)
            {
                newcookie["ExpiresBy"] = DateTime.Now.AddMinutes(expiryMinutes).ToString();
            }
            HttpContext.Current.Response.Cookies.Add(cookie);

            return cookie;
        }

        /// <summary>
        /// Encrypt kay, value and save it in cookies
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="cookieName">Name of the cookie to be added</param>
        /// <param name="key">Key for the cookie</param>
        /// <param name="value">Value for the key supplied</param>
        /// <returns>Cookie</returns>
        public static HttpCookie SetEncryptedCookie<T>(string cookieName, string key, T value, int expiryMinutes = 0)
        {
            Guard.IsNotBlank(cookieName, "CookieName");
            Guard.IsNotNull(value, "EntityValue");
            HttpCookie newcookie;

            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];

            if (cookie == null)
                // Initialize a cookie 
                newcookie = new HttpCookie(cookieName);
            else
                newcookie = cookie;

            string cookieValue = string.Empty;

            Type paramType = typeof(T);

            //Serialize value of cookie if its not of type string or int
            if (paramType != typeof(string) || paramType != typeof(int))
                cookieValue = SerializationHelper.JsonSerialize<T>(value);
            else
                cookieValue = value.ToString();

            if (!string.IsNullOrEmpty(cookieValue))
            {
                // Encrypt key value pair and insert into the cookie.
                string encryptValue = Encryptor.EncryptStringAES(cookieValue);

                // Delets the key value from cookie if key exists.
                if (newcookie.HasKeys)
                {
                    foreach (string cookieKey in newcookie.Values.AllKeys)
                    {
                        if (key == cookieKey)
                            newcookie.Values.Remove(cookieKey);
                    }
                }

                newcookie[key] = encryptValue;
                if (expiryMinutes > 0)
                {
                    newcookie["ExpiresBy"] = DateTime.Now.AddMinutes(expiryMinutes).ToString();
                }
                HttpContext.Current.Response.Cookies.Add(newcookie);
            }

            return cookie;
        }
    }
}
