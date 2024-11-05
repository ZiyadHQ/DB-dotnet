
using System.Data.Common;
using System.Net;
using System.Text;

class DataBaseServer
{

    /// <summary>
    /// for example: http://localhost
    /// </summary>
    public String host;
    public int port;
    public DataBase dataBase;

    public DataBaseServer(String host, int port)
    {
        this.host = host;
        this.port = port;
        dataBase = new();
    }

    public void Run()
    {

        HttpListener listener = new();
        listener.Prefixes.Add($"{host}:{port}/");
        listener.Start();

        Console.WriteLine($"Listening for requests at: {host}:{port}...");

        while (true)
        {

            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            String responseMessage = "";

            if (request.HttpMethod == "POST")
            {
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    String query = reader.ReadToEnd();
                    Console.WriteLine($"Recieved Query: {query}");
                    List<Document> list;
                    String responseString = "";

                    try
                    {
                        list = dataBase.QueryDataBase(query, out _);
                        foreach (var doc in list) { responseString += doc.ToString() + "\n"; }
                    }
                    catch (System.Exception e)
                    {
                        responseString = e.Message;
                    }

                    Byte[] bufferB = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = bufferB.Length;
                    response.OutputStream.Write(bufferB, 0, bufferB.Length);
                    response.OutputStream.Close();
                }
                continue;
            }
            else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/dashboard")
            {
                responseMessage = File.ReadAllText("../../../dashboard.html");
            }
            else
            {
                responseMessage = "wrong http method!\n";
                responseMessage += "use POST and the following queries to interact with this dbms:\n";
                responseMessage += "ADD/SET: SET {data type} {value} TO {field name} IN {docID or _ if creating new doc} IN {collection name}\n";
                responseMessage += "retrieve all: {collection name}\n";
                responseMessage += "conditional retrieve: {collection name} Where {field name} {comparator: >, >=, ==, <=, <} {value}\n";
                responseMessage += "{comparator: >, >=, ==, <=, <} use lexical order on string values\n";
                responseMessage += "adding new collection: ADD {collection name}\n";
            }

            Byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            Console.WriteLine($"Handle request: {request.HttpMethod} {request.Url}");
        }

    }

}