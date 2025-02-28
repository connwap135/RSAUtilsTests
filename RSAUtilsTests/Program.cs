using System;

namespace RSAUtilsTests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 示例私钥（实际使用时请替换为自己的私钥）
                string privateKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAK2sNFBlj1H9aDvlSz24TlN80qpVYSrO1PXt66hlNMRt/+sRAslj4Y69CjIlsePmByrCGdN4g6AipT/WyUydqpZWDcWbjyK4SEyU+dII9MXuP1cpCjCKAOsk6VdCeEVntByqQRKjqGNIq1s5XU3tZAOXNUL21Dy6MIaGesE69NNpAgMBAAECgYBkoi8iEudMNBEs+71wgxZnzCFp79VA7954rqdpyVMdKzwqoo3B0m2Fv0ZkLnF4w/aNMTGz1tY2eTzV1AiKq6WHRmfatWM0Azo7hCg3QB827ffH4a88XJyI3S9mhxpwyCErvldzIXAYjLDwCj/dhBVaFWUafB3BNqwvr7VCF5143QJBAPOPZorxikQiqRnTYTLZvmVw7J29/62rfM0gxnnvZQlr5r+AQk9FJRzMG0+6RfCqKIkd3s5qyYsSyhA5dlltudsCQQC2iwKuSbYLhVACCAxgJMG6LLUbM+94qGtwdiRThstOAJCvUh5r3bK/7W2qGB2DMDs8C58tH1j8DFyKf93LYrULAkEA5sxRuIKQqmZJ5d4nsj8iLBBpOEVufo0Nk3hme++9x8LHA1sv+twkAfjsPI3gbuFfzidPFj2dRLuGXP+GxdGzlwJAUswOtTsd5W/ccG9yHZHOhUGOC/6smg/aW7Jam8BCKuk6tysKPWbbkw6AdWxmxoBz/bJPysmzNO/ucau50Gy/LQJBAL5a8dnFPOminqd4cTHLgrZ14sdurtqL7gONnBdvSe02US775Vuf06WvSZPH+jPy9wOPcl9RB3HPV4pCcnrX/es=";

                Console.WriteLine("RSA私钥加解密示例");
                Console.WriteLine(new string('-', 60));

                // GET请求参数加密示例
                Console.WriteLine("【GET请求参数加密示例】");
                string getParams = "departNos=BM0001&containsDeletedPerson=true&beginDate=2024-08-01&endDate=2024-08-15";
                Console.WriteLine($"待加密GET参数：{getParams}");
                
                string encryptedGetParams = RSAUtils.EncryptByPrivateKey(getParams, privateKey);
                Console.WriteLine($"加密后结果：{encryptedGetParams}");
                Console.WriteLine($"最终GET请求：querySecret={Uri.EscapeDataString(encryptedGetParams)}");

                // 解密示例
                Console.WriteLine("【解密示例】");
                string decryptedGetParams = RSAUtils.DecryptByPrivateKey(encryptedGetParams, privateKey);
                Console.WriteLine($"解密后结果：{decryptedGetParams}");



                Console.WriteLine(new string('-', 60));
              

                // POST请求参数加密示例
                Console.WriteLine("【POST请求参数加密示例】");
                string postParams = "{\"holidayType\":\"测试请假类型\",\"holidayUnit\":1,\"hourForDay\":8,\"holidayRule\":1,\"durationCalculateType\":0}";
                Console.WriteLine($"待加密POST参数：{postParams}");
                
                string encryptedPostParams = RSAUtils.EncryptByPrivateKey(postParams, privateKey);
                Console.WriteLine($"加密后结果：{encryptedPostParams}");
                Console.WriteLine($"最终POST请求：{{\"bodySecret\":\"{encryptedPostParams}\"}}");
                Console.WriteLine(new string('-', 60));

                // 响应结果解密示例
                Console.WriteLine("【响应结果解密示例】");
                string encryptedResponse = "AaA/rgVcYp+Ek+tiKk9Q5dRUJI46s4SJL+5n/qHM2IvPndmiUTcbwM6+bz/AHMhHBgfnzUpRZdG27Q79Wf3+gSxI6kWIdXJM3aPgUoWyMwC0AhVPiKW7bZ42n8ykuYJj9BzQfkrsLEVWVi1NGCbgWePySQJkHL4p3PxrcmrzM+xfws1hZ4h4YEmQj131QCNh6Z4UvcSxcrWRrHjoSCXaPWH5nX4AJjeCTz0/OAEVFx3lXx3hd0xNq+EBJ71bNcaRGcmuCgc4IrIbbdIks+hvjO0S6BrTFPDwMT20Rm+CoJaj8EBT1Indpyap7+TLuUQBxznNqPNTS4MwHI5Cdq6u1TdZky8GJOx8wn8Ik4DM1tlTm8oehh1EXHG8/GBh1WppBqDNiF5nuzsZitq3Sm6uncyxmseqYDJyeyl69A/9H6/Ej9J6CxsAZwIaujt65YoBc/8l75Rj54vP/V+tdcdkpOq+kaAmWBtQOiC04xwF0jVJu99rYUPCCJr38Kf2WII1RKjFrUJ1iGgzRAdu6m99jFxOBVtLJ1ImL4Ydj1YCkj+P1o7+rNn4Sp4zgBikFXrClddWMUr+jmYRSUgdlYBHYIQIAARBsQWhyWDauPB+rGQSeDPQVV/y0K8ktWCzVO0V8aXs9m5u4s++1q3ZZ+QvvNOPDnj9VCyAyqwpRoY+VzUJtO7Zzs2pElk9Wvk6Xr6n5Ny8kALHv4e5FZAQcWWQyxCafzKbc6X/Ff5RLOUWrTOW4/0mLSnuSDBHWPF2aJm4CwIoVTo6mwMHpHD82WQavDCyppSWbvLKySBtJJg2h8BL5+oZm/DlroAv05Seg6ra+axHGrnSb5cos/hjNic5RjWcSIDIYFCBrLBR2S9NJUsTIuzxVmBOoijcqa8zmM3uEJ1lroXWxCjXMdngMw7hUZYe5QtDH5B177Bxa7lnmzGSHK37h5B2AR4y4DfqkyJBJhCnQpAf0NRxfHPGaFaVX63FQFAR9WUPOBv9sXb5laJDmsVz4M8F76kVGCsIRPc7";
                Console.WriteLine($"待解密响应：{encryptedResponse}");
                
                try
                {
                    string decryptedResponse = RSAUtils.DecryptByPrivateKey(encryptedResponse, privateKey);
                    Console.WriteLine($"解密后结果：{decryptedResponse}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"解密失败：{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发生错误：{ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}
