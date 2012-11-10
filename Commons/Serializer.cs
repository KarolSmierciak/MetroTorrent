namespace Commons
{
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Text;

    /// <summary>
    /// Provides methods for serialization and deserialization.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Performs deserialization of the json string into an instance object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="json">json string.</param>
        /// <returns>Deserialized object.</returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        /// <summary>
        /// Performs serialization of the instance object into a json string.
        /// </summary>
        /// <param name="instance">Object to be serialized.</param>
        /// <returns>json string.</returns>
        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                {
                    return _Reader.ReadToEnd();
                }
            }
        }
    }
}
