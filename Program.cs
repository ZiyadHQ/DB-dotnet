
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Random random = new();

        int addRandomDocsBenchmark(DataBase dataBase, int miliseconds)
        {
            string[] names =
            [
                "Ahmad",
        "Saleh",
        "Khaled",
        "Omar",
        "Muhammad"
            ];

            string[] cars =
            [
                "Camry",
        "Civic",
        "Accord",
        "Accord",
        "Accord",
        "Accord",
        "Accord",
        "Accord"
            ];

            string[] collections =
            [
                "User",
        "Car"
            ];

            int count = 0;
            bool running = true;

            Task.Delay(miliseconds).ContinueWith((e) =>
            {
                running = false;
            });

            while (running)
            {
                if (random.Next(2) == 0)
                {
                    string collection = "User";
                    Document newDoc = new();
                    newDoc.addField("Name", names[random.Next() % names.Length]);
                    newDoc.addField("Age", random.Next(12, 80));
                    dataBase.AddDoc(collection, newDoc);
                    count++;
                }
                else
                {
                    string collection = "Car";
                    Document newDoc = new();
                    newDoc.addField("Make", cars[random.Next() % cars.Length]);
                    newDoc.addField("Age", random.Next(0, 20));
                    dataBase.AddDoc(collection, newDoc);
                    count++;
                }
            }

            return count;
        }

        void addRandomDocsCount(DataBase dataBase, int count)
        {
            string[] names =
            [
                "Ahmad",
        "Saleh",
        "Khaled",
        "Omar",
        "Muhammad"
            ];

            string[] cars =
            [
                "Camry",
        "Civic",
        "Accord",
        "Accord",
        "Accord",
        "Accord",
        "Accord",
        "Accord"
            ];

            string[] collections =
            [
                "User",
        "Car"
            ];

            for (int i = 0; i < count; i++)
            {
                if (random.Next(2) == 0)
                {
                    string collection = "User";
                    Document newDoc = new();
                    newDoc.addField("Name", names[random.Next() % names.Length]);
                    newDoc.addField("Age", random.Next(12, 80));
                    dataBase.AddDoc(collection, newDoc);
                }
                else
                {
                    string collection = "Car";
                    Document newDoc = new();
                    newDoc.addField("Make", cars[random.Next() % cars.Length]);
                    newDoc.addField("Age", random.Next(0, 20));
                    dataBase.AddDoc(collection, newDoc);
                }
            }

        }

        void addRandomDocs(Collection collection, int docCount)
        {

            string[] names =
            [
                "Ahmad",
        "Saleh",
        "Khaled",
        "Omar",
        "Muhammad"
            ];

            for (int i = 0; i < docCount; i++)
            {

                Document newDoc = new();
                newDoc.addField("Name", names[random.Next() % names.Length]);
                newDoc.addField("Age", random.Next(12, 80));

                collection.AddDoc(newDoc, out _);
            }

        }
        
        DataBase dataBase = new();
        DataBaseServer server;
        if(args.Length == 2)
        {
            foreach(string arg in args){Console.WriteLine(arg);}
            server = new(args[0], int.Parse(args[1]));
        }else
        {
            server = new("http://localhost", 8080);
        }
        server.dataBase = dataBase;
        server.Run();
    }
}