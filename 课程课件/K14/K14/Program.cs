using System.Security.Cryptography;
using System.Text;

var username = "u14";
var password = "McSEv3tWpTx5cnq8K38jn49b4LFls5ij";
var url = "http://192.168.240.77/s14r1/123";
var body = "123456";
var datetime = DateTime.Now;

using (var client = new HttpClient())
{
    var result1 = client.GetAsync(url).Result;
    var tempDate= result1.Headers.Date;
    if(tempDate.HasValue)
    {
        datetime = tempDate.Value.UtcDateTime;
    }

    var data = datetime.ToString("r");
    var bodyDigest = "SHA-256=" + BodyDigest(body);

    //  signing_string = "date: Thu, 22 Jun 2017 17:15:21 GMT\nGET /requests HTTP/1.1"
    var signing_string = "date: " + data + "\n" + "digest: " + bodyDigest;

    var signature = Signature(signing_string, password);
    var authorization = "hmac username=\"" + username + "\", algorithm=\"hmac-sha256\", headers=\"date digest\", signature=\"" + signature + "\"";

    //-H "Date: Thu, 22 Jun 2017 21:12:36 GMT" \
    client.DefaultRequestHeaders.Add("Date", data);
    //-H "Digest: SHA-256=SBH7QEtqnYUpEcIhDbmStNd1MxtHg2+feBfWc1105MA=" \
    client.DefaultRequestHeaders.Add("Digest", bodyDigest);
    //-H 'Authorization: hmac username="alice123", algorithm="hmac-sha256", headers="date request-line digest", signature="gaweQbATuaGmLrUr3HE0DzU1keWGCt3H96M28sSHTG8="' \
    client.DefaultRequestHeaders.Add("Authorization", authorization);

    var result = client.GetAsync(url).Result;
    if (result.StatusCode == System.Net.HttpStatusCode.OK)
    {
        Console.WriteLine(result.Content.ReadAsStringAsync().Result);
    }
    else
    {
        Console.WriteLine(result.StatusCode);
        Console.WriteLine(result.Content.ReadAsStringAsync().Result);
    }
}

string Signature(string text, string password)
{
    //digest = HMAC - SHA256(< signing_string >, "secret")
    //base64_digest = base64(< digest >)
    var content = Encoding.UTF8.GetBytes(text);
    var key = Encoding.UTF8.GetBytes(password);
    using (var hmac = new HMACSHA256(key))
    {
        var hash = hmac.ComputeHash(content);
        return Convert.ToBase64String(hash);
    }
}

string BodyDigest(string body)
{
    //body="A small body"
    //digest=SHA-256(body)
    //base64_digest=base64(digest)
    var bytes = Encoding.UTF8.GetBytes(body);
    var hash = SHA256.Create().ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}