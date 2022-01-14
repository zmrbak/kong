using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

/*********************** Kong Server jwt Configuration *********************
consumer：空
uri param names：空
cookie names：空
key claim name：iss
secret is base64：false
claims to verify：exp
anonymous：空
run on preflight：true
maximum expiration：10
header names：authorization

JWT{
  "secret": "dpN4ZsbxNrZoS8j9dC4zFCzTIvmbfzKT",
  "created_at": 1641547093,
  "key": "5p8qb6bR5vOypmB0mXA2pVuaDwnn1bbU",
  "algorithm": "HS256",
  "id": "fbecba2d-1831-4bd9-b8d4-38e96d638dda",
  "tags": null,
  "consumer": {
    "id": "a1785aa2-72e7-474d-9591-1564095a48bf"
  },
  "rsa_public_key": null
}
****************************************************************************/

var key = "5p8qb6bR5vOypmB0mXA2pVuaDwnn1bbU";
var secret = "dpN4ZsbxNrZoS8j9dC4zFCzTIvmbfzKT";
var url = "http://192.168.250.246/yuntu/v1/wechat/token";

//请求有效期10秒（ 3 -> 11）
var exp = 10;

//校正服务器时间，此数据需要进行缓存
var clockSkew = 0;
using (var client = new HttpClient())
{
    var request = new HttpRequestMessage(HttpMethod.Options, "http://192.168.250.246/");
    var result = client.SendAsync(request).GetAwaiter().GetResult();
    if (result.Headers.Date.HasValue)
    {
        clockSkew = (int)((result.Headers.Date.Value.UtcDateTime - DateTime.Now.ToUniversalTime()).TotalSeconds);
    }
}

//服务器上当前时间(校正后的时间,Linux格式)
var currentTimestamp = (int)((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds) + clockSkew;

var header = new { typ = "JWT", alg = "HS256" };
var payload = new { iss = key, exp = currentTimestamp + exp };

var headerData = Base64UrlEncode(JsonSerializer.Serialize(header));
var payloadData = Base64UrlEncode(JsonSerializer.Serialize(payload));

//拼凑认证信息
var authorization =
    headerData + "." +
    payloadData + "." +
    HMACSHA256(headerData, payloadData, secret);

//向服务器发送请求
using (var client = new HttpClient())
{
    //在header中添加认证信息
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
    var result = client.GetAsync(url).GetAwaiter().GetResult();
    Console.WriteLine(result.StatusCode);
    //得到服务器返回信息
    Console.WriteLine(result.Content.ReadAsStringAsync().Result);
}

string HMACSHA256(string headerData, string payloadData, string secret)
{
    var data = Encoding.UTF8.GetBytes(secret);
    using (var hmac = new HMACSHA256(data))
    {
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(headerData + "." + payloadData));
        return Convert.ToBase64String(hash).Base64Trim();
    }
}

string Base64UrlEncode(string text)
{
    var data = Encoding.UTF8.GetBytes(text);
    return Convert.ToBase64String(data).Base64Trim();
}

public static class StringExtension
{
    //处理Base64的三个特殊字符+、/和=
    //= 被省略
    //+ 替换成 -
    /// 替换成 _ 
    public static string Base64Trim(this string base64String)
    {
        return base64String
            .Replace("=", "")
            .Replace("+", "-")
            .Replace("/", "_");
    }
}



