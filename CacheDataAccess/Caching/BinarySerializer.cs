namespace CacheDataAccess.Caching
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;

    /// <summary>
    /// Helper class to serialize/deserialize objects using Binary serialization.
    /// </summary>
    public class BinarySerializer : ISerialize
    {
        /// <summary>
        /// Deserialize <param name="stream" /> using a <see cref="BinaryFormatter"/>
        /// </summary>
        /// <typeparam name="T">The type this method should return after deserializing <paramref name="stream"/>.</typeparam>
        /// <param name="stream">The <see langword="byte[]" /> to be deserialized using <see cref="BinaryFormatter"/>.</param>
        /// <returns>An instance of <typeparam name="T" /></returns>
        /// <exception cref="SerializationException">The <paramref name="stream" /> supports seeking, but its length is 0. -or-The target type is a <see cref="T:System.Decimal" />, but the value is out of range of the <see cref="T:System.Decimal" /> type.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }

        /// <summary>
        /// Serialize <param name="o" /> using a <see cref="BinaryFormatter"/>
        /// </summary>
        /// <param name="o">The <see langword="object"/> to be serialized.</param>
        /// <returns>A <see cref="BinaryFormatter"/> serialized <see langword="string"/> of the <param name="o" /></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="o" /> is null.</exception>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the <paramref name="o" /> parameter is not marked as serializable. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        public byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }
    }
}