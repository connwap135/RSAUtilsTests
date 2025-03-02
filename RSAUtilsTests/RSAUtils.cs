using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

namespace RSAUtilsTests
{
    /// <summary>
    /// RSA加解密工具类
    /// </summary>
    public class RSAUtils
    {
        private const int MAX_ENCRYPT_BLOCK = 117;
        private const int MAX_DECRYPT_BLOCK = 128;

        /// <summary>
        /// 使用私钥加密数据
        /// </summary>
        /// <param name="data">需要加密的字符串</param>
        /// <param name="privateKeyString">私钥字符串(Base64编码)</param>
        /// <returns>加密后的Base64字符串</returns>
        public static string EncryptByPrivateKey(string data, string privateKey)
        {
            try
            {
                // 准确模拟Java的URL编码方式
                string urlEncodedData = Uri.EscapeDataString(data);

                // 将URL编码后的数据转为字节数组
                byte[] dataBytes = Encoding.UTF8.GetBytes(urlEncodedData);

                // 从Base64字符串中加载私钥
                AsymmetricKeyParameter privateKeyParam = GetPrivateKeyFromBase64(privateKey);

                // 使用BouncyCastle提供的加密引擎进行加密
                var cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
                cipher.Init(true, privateKeyParam);

                // 分段加密处理
                using (var outputStream = new MemoryStream())
                {
                    int dataLength = dataBytes.Length;
                    int offSet = 0;
                    byte[] cache;
                    int i = 0;

                    // 对数据分段加密
                    while (dataLength - offSet > 0)
                    {
                        if (dataLength - offSet > MAX_ENCRYPT_BLOCK)
                        {
                            cache = cipher.DoFinal(dataBytes, offSet, MAX_ENCRYPT_BLOCK);
                        }
                        else
                        {
                            cache = cipher.DoFinal(dataBytes, offSet, dataLength - offSet);
                        }
                        outputStream.Write(cache, 0, cache.Length);
                        i++;
                        offSet = i * MAX_ENCRYPT_BLOCK;
                    }

                    // 将加密结果转为Base64字符串
                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("加密过程中发生错误", ex);
            }
        }

        // EncryptByPrivateKey加密后的密文海康互联验证通过，我无法解密
        // EncryptByPrivateKey2加密后的密文DecryptByPrivateKey能解密，但海康互联验证不通过，EncryptByPrivateKey加密后的密文DecryptByPrivateKey无法解密
        public static string EncryptByPrivateKey2(string data, string privateKeyString)
        {
            try
            {
                // URL编码数据
                string urlEncodedData = Uri.EscapeDataString(data);
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
                    return Uri.UnescapeDataString(result);
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
        private static AsymmetricKeyParameter GetPrivateKeyFromBase64(string privateKey)
        {
            // 直接解析私钥，与Java行为一致
            byte[] keyBytes = Convert.FromBase64String(privateKey);
            return PrivateKeyFactory.CreateKey(keyBytes);
        }

        /// <summary>
        /// 从Base64编码的私钥字符串创建RSA对象
        /// </summary>
        private static RSA CreateRSAProviderFromPrivateKey(string privateKeyString)
        {
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyString);
            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

            return rsa;
        }
    }
}
