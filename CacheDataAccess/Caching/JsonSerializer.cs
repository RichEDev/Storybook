namespace CacheDataAccess.Caching
{
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper class to serialize/deserialize objects using JSON via Newtonsoft.Json
    /// </summary>
    public class JsonSerializer : ISerialize
    {
        /// <summary>
        /// Encoding to use to convert string to byte[] and the other way around.
        /// </summary>
        /// <remarks>
        /// StackExchange.Redis uses Encoding.UTF8 to convert strings to bytes,
        /// hence we do same here.
        /// </remarks>
        private static readonly Encoding Encoder = Encoding.UTF8;

        /// <summary>
        /// The settings for the <see cref="Newtonsoft.Json.JsonSerializer"/>
        /// </summary>
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Creates a new instance of <see cref="JsonSerializer"/>
        /// </summary>
        public JsonSerializer()
        {
            this._settings = this._settings ?? new JsonSerializerSettings();
        }

        /// <summary>
        /// Deserializes the specified serialized object.
        /// </summary>
        /// <param name="stream">The serialized object.</param>
        /// <returns>An instance of <typeparam name="T" /> from <paramref name="stream"/></returns>
        public T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            var jsonString = Encoder.GetString(stream);
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// Serializes <paramref name="o"/> into JSON.
        /// </summary>
        /// <param name="o">The item to serialize into JSON.</param>
        /// <returns>A JSON serialized instance of <paramref name="o"/></returns>
        public byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            var type = o.GetType();
            var jsonString = JsonConvert.SerializeObject(o, type, this._settings);
            return Encoder.GetBytes(jsonString);
        }
    }
}
