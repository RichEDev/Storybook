namespace BusinessLogic.Cache
{
    /// <summary>
    /// Defined a custom set of hashName and field for retrieving data from a hash in cache.
    /// </summary>
    public abstract class GetByCustom
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetByCustom"/> class. 
        /// </summary>
        /// <param name="hashName">The name of the hash to access in cache.</param>
        /// <param name="field">The name of the field to access in cache.</param>
        protected GetByCustom(string hashName, string field)
        {
            Guard.ThrowIfNullOrWhiteSpace(hashName, nameof(hashName));
            Guard.ThrowIfNullOrWhiteSpace(field, nameof(field));

            this.HashName = hashName;
            this.Field = field;
        }

        /// <summary>
        /// Gets the name of the field to access in cache.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets the name of the hash to access in cache.
        /// </summary>
        public string HashName { get; }
    }
}
