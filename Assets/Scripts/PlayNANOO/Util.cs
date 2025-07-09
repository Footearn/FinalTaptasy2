using System;
using System.Security.Cryptography;
using System.Text;

namespace PlayNANOO
{
	public static class Util
	{
		public static string HashMessage(string message, string secret)
		{
			secret = (secret ?? string.Empty);
			ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
			byte[] bytes = aSCIIEncoding.GetBytes(secret);
			byte[] bytes2 = aSCIIEncoding.GetBytes(message);
			using (HMACSHA256 hMACSHA = new HMACSHA256(bytes))
			{
				byte[] inArray = hMACSHA.ComputeHash(bytes2);
				return Convert.ToBase64String(inArray);
				IL_0043:
				string result;
				return result;
			}
		}
	}
}
