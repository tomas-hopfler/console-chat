using System.Security.Cryptography;
using System.Text;

namespace ConsoleChat
{
	public static class RSAEncryption
	{
		private static RSACryptoServiceProvider _rsaProvider = new RSACryptoServiceProvider();

		public static string Encrypt(string plainText, string publicKey)
		{
			byte[] bytesPlainTextData = Encoding.Unicode.GetBytes(plainText);
			using (RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider())
			{
				rsaProvider.FromXmlString(publicKey);
				byte[] bytesEncryptedData = rsaProvider.Encrypt(bytesPlainTextData, false);
				return Convert.ToBase64String(bytesEncryptedData);
			}
		}
		public static void GenerateKeys()
		{
			_rsaProvider = new RSACryptoServiceProvider();
		}

		public static string Decrypt(string encryptedText)
		{
			byte[] bytesEncryptedData = Convert.FromBase64String(encryptedText);
			byte[] bytesDecryptedData = _rsaProvider.Decrypt(bytesEncryptedData, false);
			return Encoding.Unicode.GetString(bytesDecryptedData);
		}

		public static string GetPublicKey()
		{
			string publicKey = _rsaProvider.ToXmlString(false);
			return publicKey;
		}
	}
}
