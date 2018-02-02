using System;
using System.IO;

namespace PasswordEncryptToHash
{
    public static class Logger
    {
        private static int Count;

        static Logger()
        {
            Count = 0;
            try
            {
                File.Delete(@".\PasswordEncryptToHash.log");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void WriteLine(string message)
        {
            try
            {
                File.AppendAllText(@".\PasswordEncryptToHash.log", message + Environment.NewLine);
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void WriteLine(string message, bool error)
        {
            
        }

        public static void Processing()
        {
            Count++;
            if (Count <= 10) return;
            Console.Write(".");
            Count = 0;

        }
        public static void ProcessingEnd()
        {
            Console.WriteLine(".");
        }
    }
}
