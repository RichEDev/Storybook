namespace SpendManagementLibrary.Exceptions
{
    using System;

    public class ValidationException: Exception
    {
        public string Field { get; set; }

        public ValidationException(string message) : this(message, null)
        { }

        public ValidationException(string message, string field) : base(message)
        {
            this.Field = field;
        }
    }
}
