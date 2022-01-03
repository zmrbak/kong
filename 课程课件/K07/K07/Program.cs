using System.Net;
using System.Text.Json;

HttpListener listener = new HttpListener();
listener.Prefixes.Add("http://*:80/");
listener.Start();
listener.BeginGetContext(HttpCallBack, listener);

void HttpCallBack(IAsyncResult ar)
{
    var state = ar.AsyncState;
    if (state != null)
    {
        var httpListener = (HttpListener)state;
        var context = httpListener.EndGetContext(ar);
        httpListener.BeginGetContext(HttpCallBack, httpListener);

        var result = new
        {
            Time=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Url= context.Request.Url,
        };

        var outString=JsonSerializer.Serialize(result);
        var buffer = System.Text.Encoding.UTF8.GetBytes(outString);
        context.Response.OutputStream.Write(buffer);
        context.Response.OutputStream.Flush();
        context.Response.OutputStream.Close();
    }
}

Console.WriteLine("程序已经启动，请按回车退出...");
Console.ReadLine();
listener.Stop();
