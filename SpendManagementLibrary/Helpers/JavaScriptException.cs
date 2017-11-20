using System.Runtime.Serialization;

namespace SpendManagementLibrary.Helpers
{
    using System;

    [Serializable]
    public class JavaScriptException : Exception
    {
        private readonly int _lineNumber;
        private readonly string _pageUrl;

        /// <summary>
        /// The line number which caused the JavaScript exception (this is the line number on their client from the DOM at the time it occured which may be different from initially sent.
        /// </summary>
        public int LineNumber
        {
            get
            {
                return _lineNumber;
            }
        }

        /// <summary>
        /// A string that describes the immediate frames of the call stack.
        /// </summary>
        public override string StackTrace
        {
            get 
            { 
                return string.Format("Url: {0} Line: {1}", _pageUrl, _lineNumber);
            }
        }

        /// <summary>
        /// Represents JavaScript exceptions which occcur during application execution.
        /// </summary>
        /// <param name="pageUrl"></param>
        /// <param name="lineNumber"></param>
        public JavaScriptException(string pageUrl, int lineNumber)
        {
            _pageUrl = pageUrl;
            _lineNumber = lineNumber;
        }

        /// <summary>
        /// Represents JavaScript exceptions which occcur during application execution.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pageUrl"></param>
        /// <param name="lineNumber"></param>
        public JavaScriptException(string message, string pageUrl, int lineNumber) : base(message)
        {
            _pageUrl = pageUrl;
            _lineNumber = lineNumber;
        }

        /// <summary>
        /// Represents JavaScript exceptions which occcur during application execution.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="pageUrl"></param>
        /// <param name="lineNumber"></param>
        /// <param name="inner"></param>
        public JavaScriptException(string message, string pageUrl, int lineNumber, Exception inner) : base(message, inner)
        {
            _pageUrl = pageUrl;
            _lineNumber = lineNumber;
        }

        /// <summary>
        /// Represents JavaScript exceptions which occcur during application execution.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected JavaScriptException(SerializationInfo info, StreamingContext context)
        {

        }
    }

}
