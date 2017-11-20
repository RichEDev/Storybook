using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagement.AutomatedTests
{
    public class FindString
    {
        public static string GetID(string extracted)
        {
            return FindString.GetID(extracted, "(", ")", 1);
        }

        public static string GetID(string extracted, string firstchar, string lastchar, int distance)
        {
            string id = "";
            string current;
            bool found = false;
            int x = 1;

            while (found == false)
            {
                current = extracted.Substring(extracted.Length - x, 1);
                if (current == firstchar)
                {
                    x = x - distance;
                    while (extracted.Substring(extracted.Length - x, 1) != lastchar)
                    {
                        id += extracted.Substring(extracted.Length - x, 1);
                        x--;
                    }
                    found = true;
                }
                else
                {
                    x++;
                }
                if (x == extracted.Length) { found = true; }
            }
            return id;
        }
    }
}
