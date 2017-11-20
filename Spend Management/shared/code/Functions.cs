namespace Spend_Management.shared.code
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Text;

    using Infragistics.WebUI.CalcEngine;

    /// <summary>
    /// The functions.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Get a list of functions that inherit from the UltraCalcFunction base class.
        /// </summary>
        /// <returns>
        /// The <see><cref>ReadOnlyCollection</cref></see>
        ///  List of Function.
        /// </returns>
        public static ReadOnlyCollection<Function> Get()
        {
            var cache = MemoryCache.Default;
            var existing = cache.Get("UltraCalcFunctions");
            if (existing != null)
            {
                return (ReadOnlyCollection<Function>)existing;
            }

            var type = typeof(UltraCalcFunction);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type.IsAssignableFrom);

            var result = types.Where(type1 => !type1.IsAbstract).Select(type1 => GetDataFromType(type1)).Where(currentFunction => currentFunction != null).ToList().AsReadOnly();
            cache.Add("UltraCalcFunctions", result, new CacheItemPolicy());
            return result;
        }

        private static Function GetDataFromType(Type type1)
        {
            
            var obj = Activator.CreateInstance(type1);
            if (obj is UltraCalcFunction)
            {
                var function = (UltraCalcFunction)obj;
                if (function.Category != null && function.Category != "Engineering")
                {
                    var example = new StringBuilder();
                    var syntax = new StringBuilder();
                    foreach (string descriptor in function.ArgDescriptors)
                    {
                        example.Append(descriptor);
                        example.Append(Environment.NewLine);
                    }

                    var idx = 0;

                    foreach (string argument in function.ArgList)
                    {
                        var argumentValue = string.Empty;
                        if (argument.EndsWith("{0}"))
                        {
                            switch (argument)
                            {
                                case "number{0}":
                                    argumentValue = "[number,number...]";
                                    break;
                                default:
                                    argumentValue = "[argument,argument...]";
                                    break;
                            }
                        }
                        else
                        {
                            switch (argument.ToLower())
                            {
                                case "date":
                                case "date1":
                                case "date2":
                                case "end_date":
                                case "enddate":
                                case "fv":
                                case "settlement":
                                case "start_date":
                                case "startdate":
                                case "date_value":
                                case "logical":
                                case "logical_test":
                                case "method":
                                case "no_commas":
                                case "date_text":
                                case "find_text":
                                case "new_text":
                                case "old_text":
                                case "string":
                                case "text":
                                case "time_text":
                                case "value":
                                case "value_if_false":
                                case "value_if_true":
                                case "interval":
                                case "type":
                                    argumentValue = string.Format("[{0}]", argument);
                                    break;
                                case "number":
                                    argumentValue = string.Format("[{1}]", argument.ToLower(), argument);
                                    break;
                                default:
                                    if (!string.IsNullOrEmpty(argument))
                                    {
                                        argumentValue = string.Format("[{0}]", argument);
                                    }

                                    break;
                            }
                        }

                        if (syntax.Length > 0)
                        {
                            syntax.Append(",");
                        }

                        syntax.Append(argumentValue);
                        idx++;
                    }

                    var result = new Function
                    {
                        Description = function.Description,
                        Example = example.ToString(),
                        FunctionName = function.Name.ToUpper(),
                        Parent = function.Category,
                        Syntax = string.Format("{0}({1})", function.Name.ToUpper(), syntax),
                        Remarks = string.Empty
                    };

                    result.Example = result.Example.Replace(".Net", string.Empty).Replace(".NET", string.Empty);
                    result.Description = result.Description.Replace(".Net", string.Empty).Replace(".NET", string.Empty);

                    return result;
                }
            }

            return null;
        }
    }
}