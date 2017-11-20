using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Auto_Tests.Tools
{
    public static class ExtensionMethods
    {     
            /// <summary>
            /// Creates a Deep copy of the object provided returning a cloned
            /// object at a different memory address.
            /// 
            /// NOTE:  Equals operator won't work if not overloaded once used.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="a"></param>
            /// <returns></returns>
            public static T DeepClone<T>(this T a)     { 
                using (MemoryStream stream = new MemoryStream())    
                {          
                    BinaryFormatter formatter = new BinaryFormatter();      
                    formatter.Serialize(stream, a);     
                    stream.Position = 0;       
                    return (T) formatter.Deserialize(stream);      
                }     
            }
        } 
    }
