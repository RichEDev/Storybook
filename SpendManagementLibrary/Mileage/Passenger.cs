using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SpendManagementLibrary.Mileage
{

    /// <summary>
    /// Represents a passenger of a journey step
    /// </summary>
    [Serializable]
    public class Passenger
    {
        /// <summary>
        /// The employee ID if it is a known employee (in which case Name will be null)
        /// </summary>
        public int? EmployeeId { get; set; }

        public int? StepNumber { get; set; }

        /// <summary>
        /// The Name if it is not a known employee (in which case EmployeeId will be null)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a passenger from  the string that is returned from the web form
        /// which should be either "0:A.N.Other" for an unknown passenger, or "42:" for a known employee.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Passenger p)
        {
            p = new Passenger();
            Match m;
            int employeeId;
            if ((m = Regex.Match(s, "(?<=Name=)[^=,&]+", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)).Success)
            {
                p.Name = m.Value;
            }
            if ((m = Regex.Match(s, "(?<=ID=)[^=,&]+", RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase)).Success && int.TryParse(m.Value, out employeeId))
            {
                p.EmployeeId = employeeId;
            }
            return p.EmployeeId.HasValue || !string.IsNullOrEmpty(p.Name);
        }

        public static IEnumerable<Passenger> ParsePassengersString(string s)
        {
            foreach (var passengerString in s.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries))
            {
                Passenger p;
                if (TryParse(passengerString, out p)) yield return p;
            }
        }

        public override string ToString()
        {
            List<string>  parts = new List<string>();
            parts.Add("ID=" + (EmployeeId ?? 0));
            parts.Add("Name=" + Regex.Replace(Name, @"[&=]", ""));
            return string.Join("&", parts);
        }
    }
}