using System;
using System.Text;
using log4net;

namespace Obscured.Holdr.BLL
{
    public class EncryptionService : IEncryptionService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(EncryptionService));

        public byte[] ConvertToBinaryWithoutKey(string data)
        {
            var hashedData = GetMd5HashWithoutKey(data);
            return Encoding.UTF8.GetBytes(hashedData);
        }

        public byte[] ConvertToBinaryWithKey(string data, string hashKey)
        {
            var hashedData = GetMd5HashWithKey(data, hashKey);
            return Encoding.UTF8.GetBytes(hashedData);
        }

        public string GetMd5HashWithKey(string data, string hashKey)
        {
            try
            {
                var textBytes = Encoding.UTF8.GetBytes(data + hashKey);
                var cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                var ret = "";
                foreach (var a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch (Exception e)
            {
                _logger.Warn(String.Format("Unable to get md5 hash for data {0}", data));
            }
            return null;
        }

        public string GetMd5HashWithoutKey(string data)
        {
            try
            {
                var textBytes = Encoding.UTF8.GetBytes(data);
                var cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var hash = cryptHandler.ComputeHash(textBytes);
                var ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch (Exception e)
            {
                _logger.Warn(String.Format("Unable to get md5 hash for data {0}", data));
            }
            return null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}