using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System.Text;
using System.Web;

namespace RSAUtilsTests
{
    /// <summary>
    /// 完全模拟Java的RSA加密实现
    /// </summary>
    public static class RSAUtils
    {
        private const int MAX_ENCRYPT_BLOCK = 117;
        private const int MAX_DECRYPT_BLOCK = 128;
        private const string CHARSET = "UTF-8";

        /// <summary>
        /// 私钥加密 - 完全按照Java的实现方式
        /// </summary>
        /// <param name="data">要加密的数据</param>
        /// <param name="privateKey">Base64编码的私钥</param>
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
                Console.WriteLine($"加密失败：{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 私钥解密 - 完全按照Java的实现方式
        /// </summary>
        /// <param name="encryptedData">要解密的Base64编码数据</param>
        /// <param name="privateKey">Base64编码的私钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptByPrivateKey(string encryptedData, string privateKey)
        {
            try
            {
                Console.WriteLine("开始解密...");

                // 从Base64字符串加载私钥
                AsymmetricKeyParameter privateKeyParam = GetPrivateKeyFromBase64(privateKey);
                Console.WriteLine("私钥加载成功");

                // 将加密数据转换为字节数组
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                Console.WriteLine($"加密数据长度: {encryptedBytes.Length}");

                // 使用BouncyCastle提供的解密引擎
                var cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
                cipher.Init(false, privateKeyParam);
                Console.WriteLine("解密引擎初始化成功");

                // 分段解密处理
                using (var outputStream = new MemoryStream())
                {
                    int dataLength = encryptedBytes.Length;
                    int offSet = 0;
                    byte[] cache;
                    int i = 0;

                    // 对数据分段解密
                    while (dataLength - offSet > 0)
                    {
                        try
                        {
                            if (dataLength - offSet > MAX_DECRYPT_BLOCK)
                            {
                                cache = cipher.DoFinal(encryptedBytes, offSet, MAX_DECRYPT_BLOCK);
                            }
                            else
                            {
                                cache = cipher.DoFinal(encryptedBytes, offSet, dataLength - offSet);
                            }
                            outputStream.Write(cache, 0, cache.Length);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"解密块失败：{ex.Message}");
                            Console.WriteLine($"解密块偏移量: {offSet}");
                            return null; // 处理解密块失败的情况
                        }
                        i++;
                        offSet = i * MAX_DECRYPT_BLOCK;
                    }

                    // URL解码，与Java保持一致
                    string outStr = Encoding.UTF8.GetString(outputStream.ToArray());
                    Console.WriteLine("解密成功");
                    return HttpUtility.UrlDecode(outStr, Encoding.GetEncoding(CHARSET));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"解密失败：{ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 从Base64字符串加载私钥
        /// </summary>
        private static AsymmetricKeyParameter GetPrivateKeyFromBase64(string privateKey)
        {
            // 直接解析私钥，与Java行为一致
            byte[] keyBytes = Convert.FromBase64String(privateKey);
            return PrivateKeyFactory.CreateKey(keyBytes);
        }
    }
}
