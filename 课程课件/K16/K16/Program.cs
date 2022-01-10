using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

var key = "z9CmeM7IYVZjDtmJtfqQ473U5RxHbbNN";
var secret = "gzbxbz3PoGaih7Pg6FSQU4nY7b9PbcAA";
var url = "http://192.168.240.77/s16r1/123/test";

//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
//eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJpc3MiOiJ6OUNtZU03SVlWWmpEdG1KdGZxUTQ3M1U1UnhIYmJOTiJ9.
//cBQYTO5sWvrDC0Ox7Y0l7t4UcaFIiaEgyx0v1XyXHms
var header = new
{
    typ = "JWT",
    alg = "HS256"
};

var payload = new
{
    iss = key
};

var headerData = Base64UrlEncode(JsonSerializer.Serialize(header));
var payloadData = Base64UrlEncode(JsonSerializer.Serialize(payload));
var authorization =
    headerData + "." +
    payloadData + "." +
    HMACSHA256(headerData, payloadData, secret);


using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorization);
    var result = client.GetAsync(url).GetAwaiter().GetResult();
    Console.WriteLine(result.StatusCode);
    Console.WriteLine(result.Content.ReadAsStringAsync().Result);
}

string HMACSHA256(string headerData, string payloadData, string secret)
{
    var data = Encoding.UTF8.GetBytes(secret);
    using (var hmac = new HMACSHA256(data))
    {
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(headerData + "." + payloadData));
        return Convert.ToBase64String(hash);
    }
}

string Base64UrlEncode(string text)
{
    var data = Encoding.UTF8.GetBytes(text);
    var content = Convert.ToBase64String(data);
    return HttpUtility.UrlEncode(content);
}

