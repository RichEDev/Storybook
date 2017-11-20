using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Class to override the Username password validator used for the WCF Authentication 
    /// </summary>
    public class UserNamePassValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        /// <summary>
        /// Overridden Validate method that contains custom authentication for the Spend anagement API
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public override void Validate(string userName, string password)
        {
            if (userName == null || password == null)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "test1" && password == "1tset") && !(userName == "test2" && password == "2tset"))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }

        }
    }
}
