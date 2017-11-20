namespace CacheDataAccess.Caching
{
    /// <summary>
    /// Provides functionality for formatting serialized objects.
    /// </summary>
    public interface ISerialize
    {
        /// <summary>
        /// Deserializes <paramref name="stream"/> into an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to unbox the stream to.</typeparam>
        /// <param name="stream">The byte array of the serialized <typeparamref name="T"/>.</param>
        /// <returns>An instance of <typeparamref name="T"/> from <paramref name="stream"/> or null if <paramref name="stream"/> is null.</returns>
        T Deserialize<T>(byte[] stream);

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="o">The object to serialize.</param>
        /// <returns>An array of <paramref name="o"/> serialized</returns>
        byte[] Serialize(object o);
    }
}