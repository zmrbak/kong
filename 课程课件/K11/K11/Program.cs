
using System.Text;

var username = "u1";
var password = "p1";
using (HttpClient httpClient = new HttpClient())
{
    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"))
        );
    var result = httpClient.GetStringAsync("http://192.168.240.77/my-path").GetAwaiter().GetResult();
    //var result = httpClient.GetStringAsync("http://192.168.240.77/path").GetAwaiter().GetResult();
    Console.WriteLine(result);
}

Console.ReadLine();
