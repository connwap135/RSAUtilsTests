using RSAUtilsTests;
using System;
using System.Text;
using System.Security.Cryptography;

Console.WriteLine("海康威视开放平台API调用加密测试");

// 私钥
var privateKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAK2sNFBlj1H9aDvlSz24TlN80qpVYSrO1PXt66hlNMRt/+sRAslj4Y69CjIlsePmByrCGdN4g6AipT/WyUydqpZWDcWbjyK4SEyU+dII9MXuP1cpCjCKAOsk6VdCeEVntByqQRKjqGNIq1s5XU3tZAOXNUL21Dy6MIaGesE69NNpAgMBAAECgYBkoi8iEudMNBEs+71wgxZnzCFp79VA7954rqdpyVMdKzwqoo3B0m2Fv0ZkLnF4w/aNMTGz1tY2eTzV1AiKq6WHRmfatWM0Azo7hCg3QB827ffH4a88XJyI3S9mhxpwyCErvldzIXAYjLDwCj/dhBVaFWUafB3BNqwvr7VCF5143QJBAPOPZorxikQiqRnTYTLZvmVw7J29/62rfM0gxnnvZQlr5r+AQk9FJRzMG0+6RfCqKIkd3s5qyYsSyhA5dlltudsCQQC2iwKuSbYLhVACCAxgJMG6LLUbM+94qGtwdiRThstOAJCvUh5r3bK/7W2qGB2DMDs8C58tH1j8DFyKf93LYrULAkEA5sxRuIKQqmZJ5d4nsj8iLBBpOEVufo0Nk3hme++9x8LHA1sv+twkAfjsPI3gbuFfzidPFj2dRLuGXP+GxdGzlwJAUswOtTsd5W/ccG9yHZHOhUGOC/6smg/aW7Jam8BCKuk6tysKPWbbkw6AdWxmxoBz/bJPysmzNO/ucau50Gy/LQJBAL5a8dnFPOminqd4cTHLgrZ14sdurtqL7gONnBdvSe02US775Vuf06WvSZPH+jPy9wOPcl9RB3HPV4pCcnrX/es=";

// 期望结果
string expectedResult = "pmttDL8cTHDp8SRRFnBL69EdlyDVkASZ1MheaGGv38jduOw7ye5MdT1Vtd+fThQkrSHUuihMphZXbiGGHGov2N0YWmcmi9QX+kFUlNmzafU7MTaUijaPEgku8xHTWXAcP02arqs3RYNNJpfaI86Ruaq/bNwT8j/nb1CTuiYbzvw=";

try
{
    // GET请求参数测试
    var originalGetParams = "departNos=BM0001&containsDeletedPerson=true&beginDate=2024-08-01&endDate=2024-08-15";
    
    Console.WriteLine("1. 测试加密功能:");
    var encryptedGet = RSAUtils.EncryptByPrivateKey(originalGetParams, privateKey);
    Console.WriteLine($"加密结果: {encryptedGet}");
    Console.WriteLine($"与预期结果一致: {encryptedGet == expectedResult}");
    
    // 测试不同的解密方法
    Console.WriteLine("\n2. 测试解密功能:");
   var d= RSAUtils.DecryptByPrivateKey(encryptedGet, privateKey);
    Console.WriteLine($"解密结果: {d}");


    // POST请求参数测试
    Console.WriteLine("\n4. POST 请求参数加密测试:");
    var originalPostParams = "{\"holidayType\":\"测试请假类型\",\"holidayUnit\":1,\"hourForDay\":8,\"holidayRule\":1,\"durationCalculateType\":0}";
    var encryptedPost = RSAUtils.EncryptByPrivateKey(originalPostParams, privateKey);
    Console.WriteLine($"加密结果: {encryptedPost}");
}
catch (Exception ex)
{
    Console.WriteLine($"发生错误: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

Console.WriteLine("\n测试完成，按任意键退出...");
Console.ReadKey();

