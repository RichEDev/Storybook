using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Employees;

    public class cPassword
	{
		public static string Crypt(string pw, string p)
		{
			int x;
			string retVal;

			// Encrypt or decrypt passwords
			// Errors returned prefixed with '$'

			try
			{

				int Q;
				char CH;
				string op;
				int EC;
				int n;
				n = pw.Length;

				for (x = 0; x < n; x++)
				{
					if ((int)Convert.ToChar((pw.Substring(n, 1))) > 122 || (int)Convert.ToChar((pw.Substring(n, 1))) < 48)
					{
						retVal = "$" + "Password contains an invalid character";
						break;
					}
				}

				switch (int.Parse(p))
				{
					case 1: // Encrypt
						op = "";
						for (Q = 0; Q < pw.Length; Q++)
						{
							CH = Convert.ToChar(pw.Substring(Q, 1));
							EC = (int)CH + n;
							if (EC > 122)
							{
								EC = EC - 75;
							}
							char[] characters = System.Text.Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(EC) });

							op = op + characters[0];
							n = n + 1;
						}
						retVal = op;
						break;
					case 2:  // Decrypt
						op = "";
						for (Q = 0; Q < pw.Length; Q++)
						{
							CH = Convert.ToChar(pw.Substring(Q, 1));
							EC = (int)CH - n;
							if (EC < 48)
							{
								EC = EC + 75;
							}
							char[] characters = System.Text.Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(EC) });
							op = op + characters[0];
							n = n + 1;
						}
						retVal = op;
						break;
					default:
						retVal = "$Unknown param issued to Crypt() function";
						break;
				}
			}
			catch
			{
				retVal = "$Err: Crypt() failed";
			}
			return retVal;
		}

		public static string HashPassword(string inpwd)
		{
			string hashedPwd;

			hashedPwd = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(inpwd, "md5");
			return hashedPwd;
		}

		public static string SHA_HashPassword(string inpwd)
		{
			System.Security.Cryptography.SHA512Managed SHA = new System.Security.Cryptography.SHA512Managed();

			byte[] dataSHA = SHA.ComputeHash(System.Text.Encoding.Default.GetBytes(inpwd));

			StringBuilder sb = new StringBuilder();

			for (int x = 0; x < dataSHA.Length; x++)
			{
				sb.AppendFormat("{0:x2}", dataSHA[x]);
			}

			return sb.ToString();
		}

	}
}
