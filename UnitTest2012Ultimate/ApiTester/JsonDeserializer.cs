namespace APITester
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Script.Serialization;

    public class JsonDeserializer
    {
        internal IDictionary<string, object> jsonData { get; set; }

        /// <summary>
        /// Deserializers a JSON string
        /// </summary>
        /// <param name="json">The json string</param>
        public JsonDeserializer(string json)
        {
            var json_serializer = new JavaScriptSerializer();

            this.jsonData = (IDictionary<string, object>)json_serializer.DeserializeObject(json);
        }

        /// <summary>
        /// Gets a string from the GetOjbect
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns></returns>
        public string GetString(string path)
        {
            return (string)this.GetObject(path);
        }

        /// <summary>
        /// Counts the number of items in an array
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The count</returns>
        public int ArrayItemCount(string path)
        {
            int result = 0;

            object o = this.GetObject(path);
            if (o == null)
            {
                return result;
            }

            if (o is Array)
            {
                result = ((Array)o).GetUpperBound(0) + 1;
            }
            else
            {
                result = 0;
            }

            return result;
        }

  
        /// <summary>
        /// Gets the object based on the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The object</returns>
        public object GetObject(string path)
        {
            object result = null;

            var curr = this.jsonData;
            var paths = path.Split('.');
            var pathCount = paths.Count();

            try
            {
                for (int i = 0; i < pathCount; i++)
                {
                    var key = paths[i];
                    if (i == (pathCount - 1))
                    {
                        result = curr[key];
                    }
                    else
                    {
                        if (curr[key] is Array)
                        {
                            var newcurr = new Dictionary<string, object>();
                            var idx = 0;
                            foreach (var obj in (object[])curr[key])
                            {
                                newcurr.Add(idx.ToString(), obj);
                                idx++;
                            }
                            curr = newcurr;
                        }
                        else
                        {
                            curr = (IDictionary<string, object>)curr[key];    
                        }
                        
                    }
                }
            }
            catch
            {
                // Probably means an invalid path (ie object doesn't exist)
            }

            return result;
        }
    }
}