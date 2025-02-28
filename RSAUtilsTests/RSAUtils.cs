using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RSAUtilsTests
{
    /// <summary>
    /// RSA加解密工具类
    /// </summary>
    public class RSAUtils
    {
        private const string RSA_ALGORITHM = "RSA";
        private const string CHARSET = "UTF-8";
        private const int MAX_ENCRYPT_BLOCK = 117;
        private const int MAX_DECRYPT_BLOCK = 128;

        /// <summary>
        /// 使用私钥加密数据
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="privateKeyString">私钥字符串(Base64编码)</param>
        /// <returns>加密后的Base64字符串</returns>
        public static string EncryptByPrivateKey(string data, string privateKeyString)
        {
            try
            {
                // URL编码数据
                string urlEncodedData = HttpUtility.UrlEncode(data);
                byte[] dataBytes = Encoding.UTF8.GetBytes(urlEncodedData);

                // 加载私钥
                RSA rsa = CreateRSAProviderFromPrivateKey(privateKeyString);
                
                // 分块加密
                using (MemoryStream ms = new MemoryStream())
                {
                    int bufferSize = MAX_ENCRYPT_BLOCK;
                    int offset = 0;
                    int inputLen = dataBytes.Length;
                    byte[] cache;
                    
                    while (inputLen - offset > 0)
                    {
                        if (inputLen - offset > bufferSize)
                        {
                            cache = rsa.Encrypt(dataBytes.AsSpan(offset, bufferSize).ToArray(), RSAEncryptionPadding.Pkcs1);
                        }
                        else
                        {
                            cache = rsa.Encrypt(dataBytes.AsSpan(offset, inputLen - offset).ToArray(), RSAEncryptionPadding.Pkcs1);
                        }
                        
                        ms.Write(cache, 0, cache.Length);
                        offset += bufferSize;
                    }
                    
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加密过程中发生错误", ex);
            }
        }

        /// <summary>
        /// 使用私钥解密数据
        /// </summary>
        /// <param name="encryptedData">加密的Base64字符串</param>
        /// <param name="privateKeyString">私钥字符串(Base64编码)</param>
        /// <returns>解密后的原始字符串</returns>
        public static string DecryptByPrivateKey(string encryptedData, string privateKeyString)
        {
            try
            {
                byte[] dataBytes = Convert.FromBase64String(encryptedData);
                
                // 加载私钥
                RSA rsa = CreateRSAProviderFromPrivateKey(privateKeyString);
                
                // 分块解密
                using (MemoryStream ms = new MemoryStream())
                {
                    int bufferSize = MAX_DECRYPT_BLOCK;
                    int offset = 0;
                    int inputLen = dataBytes.Length;
                    byte[] cache;
                    
                    while (inputLen - offset > 0)
                    {
                        if (inputLen - offset > bufferSize)
                        {
                            cache = rsa.Decrypt(dataBytes.AsSpan(offset, bufferSize).ToArray(), RSAEncryptionPadding.Pkcs1);
                        }
                        else
                        {
                            cache = rsa.Decrypt(dataBytes.AsSpan(offset, inputLen - offset).ToArray(), RSAEncryptionPadding.Pkcs1);
                        }
                        
                        ms.Write(cache, 0, cache.Length);
                        offset += bufferSize;
                    }
                    
                    string result = Encoding.UTF8.GetString(ms.ToArray());
                    return HttpUtility.UrlDecode(result);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解密过程中发生错误", ex);
            }
        }

        /// <summary>
        /// 从Base64编码的私钥字符串创建RSA对象
        /// </summary>
        private static RSA CreateRSAProviderFromPrivateKey(string privateKeyString)
        {
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyString);
            
            var rsa = RSA.Create();
            
            // 导入PKCS#8格式的私钥
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            
            return rsa;
        }
    }
}
