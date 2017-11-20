namespace Common.Logging.Log4Net
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Util;
    using Newtonsoft.Json;

    /// <summary>
    /// Layout for log4net to output in JSON format.
    /// </summary>
    public class JsonLayout : PatternLayout
    {
        /// <summary>
        /// Default members which will be output unless removed
        /// </summary>
        private static readonly List<string> DefaultMembers = new List<string>()
        {
            "logid",
            "level",
            "timestamputc",
            "logger",
            "machinename",
            "pid",
            "thread",
            "location"
        };

        /// <summary>
        /// A colection of custom members which will be available to the <c>Format</c> method.
        /// </summary>
        private static readonly List<string> PropertyMembers = new List<string>();

        /// <summary>Flag indicating if this layout handles exceptions</summary>
        /// <value><c>false</c> if this layout handles exceptions</value>
        /// <remarks>
        /// <para>
        /// If this layout handles the exception object contained within
        /// <see cref="T:log4net.Core.LoggingEvent" />, then the layout should return
        /// <c>false</c>. Otherwise, if the layout ignores the exception
        /// object, then the layout should return <c>true</c>.
        /// </para>
        /// <para>
        /// Set this value to override a this default setting. The default
        /// value is <c>true</c>, this layout does not handle the exception.
        /// </para>
        /// </remarks>
        public override bool IgnoresException => false;

        /// <summary>Initialize layout options</summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="T:log4net.Core.IOptionHandler" /> delayed object
        /// activation scheme. The <see cref="M:log4net.Layout.PatternLayout.ActivateOptions" /> method must
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="M:log4net.Layout.PatternLayout.ActivateOptions" /> is called this
        /// object is in an undefined state and must not be used.
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then
        /// <see cref="M:log4net.Layout.PatternLayout.ActivateOptions" /> must be called again.
        /// </para>
        /// </remarks>
        public override void ActivateOptions()
        {
        }

        /// <summary>
        /// Add a single member that can be plain pattern string. 
        /// </summary>
        /// <param name="value">the member</param>
        public virtual void AddMember(string value)
        {
            if ((value.StartsWith("properties{") || value.StartsWith("property{")) && value.EndsWith("}"))
            {
                value = value.Remove(value.Length - 1).Replace("properties{", string.Empty).Replace("property{", string.Empty);

                // Prevents duplicate entries when config is changed and reloaded
                if (PropertyMembers.Contains(value) == false)
                {
                    PropertyMembers.Add(value);
                }
            }
            else
            {
                // Prevents duplicate entries when config is changed and reloaded
                if (DefaultMembers.Contains(value) == false)
                {
                    DefaultMembers.Add(value);
                }
            }
        }

        /// <summary>
        /// Remove a member
        /// </summary>
        /// <param name="value">the removal</param>
        public virtual void AddRemove(string value)
        {
            DefaultMembers.Remove(value);
            PropertyMembers.Remove(value);
        }

        /// <summary>
        /// Produces a JSON string containing common logging context
        /// </summary>
        /// <param name="writer">
        /// The writer to append too.
        /// </param>
        /// <param name="e">
        /// The event being logged.
        /// </param>
        /// <exception cref="IOException">An I/O error occurs when attempting to write the log entry.</exception>
        public override void Format(TextWriter writer, LoggingEvent e)
        {
            var outputMembers = new Dictionary<string, object>();

            foreach (string defaultMember in DefaultMembers)
            {
                string value;
                switch (defaultMember)
                {
                    case "logid":
                        value = Guid.NewGuid().ToString();
                        break;
                    case "level":
                        value = e.Level.DisplayName;
                        break;
                    case "timestamputc":
                        value = e.TimeStamp.ToUniversalTime().ToString("O");
                        break;
                    case "pid":
                        value = Process.GetCurrentProcess().Id.ToString();
                        break;
                    case "machinename":
                        value = Environment.MachineName;
                        break;
                    case "logger":
                        value = e.LoggerName;
                        break;
                    case "thread":
                        value = e.ThreadName;
                        break;
                    case "location":
                        value = $"{e.LocationInformation.StackFrames[1].ClassName}:{e.LocationInformation.StackFrames[1].LineNumber}";
                        break;
                    default:
                        value = "no value";
                        break;
                }

                outputMembers.Add(defaultMember, value);
            }

            if (PropertyMembers.Count > 0)
            {
                PropertiesDictionary propertiesDictionary = e.GetProperties();
                foreach (string propertyMember in PropertyMembers)
                {
                    if (propertiesDictionary.Contains(propertyMember))
                    {
                        object propertyValue = propertiesDictionary[propertyMember];

                        if (propertyValue != null)
                        {
                            // Check to see if its a collection
                            if (propertyValue.GetType().GetInterface("IEnumerable") != null)
                            {
                                outputMembers.Add(propertyMember, propertyValue);
                            }
                            else
                            {
                                outputMembers.Add(propertyMember, propertyValue.ToString());
                            }


                        }
                    }
                }
            }

            // if message is null and exception is set use e.ExceptionObject.Message otherwise if e.MessageObject is a string then use e.RenderedMessage otherwise use e.MessageObject
            object message;

            if (string.IsNullOrWhiteSpace(e.RenderedMessage) && e.ExceptionObject != null)
            {
                message = e.ExceptionObject.Message;
            }
            else if (e.MessageObject is string || e.MessageObject is SystemStringFormat)
            {
                message = e.RenderedMessage;
            }
            else
            {
                message = $"See JSONObject";
                outputMembers.Add("JSONObject", e.MessageObject);
            }

            outputMembers.Add("message", message);

            if (e.ExceptionObject != null)
            {
                outputMembers.Add("exceptionObject", e.ExceptionObject);
            }

            string json = JsonConvert.SerializeObject(outputMembers, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

            writer.Write(json);
            writer.Write(writer.NewLine);
        }
    }
}