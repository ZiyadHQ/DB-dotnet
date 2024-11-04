
using System.Text.Json;

Random random = new();

int addRandomDocsBenchmark(DataBase dataBase, int miliseconds)
{
    String[] names =
    [
        "Ahmad",
        "Saleh",
        "Khaled",
        "Omar",
        "Muhammad"
    ];

    String[] cars =
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

    String[] collections =
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
            String collection = "User";
            Document newDoc = new();
            newDoc.addField("Name", names[random.Next() % names.Length]);
            newDoc.addField("Age", random.Next(12, 80));
            dataBase.AddDoc(collection, newDoc);
            count++;
        }
        else
        {
            String collection = "Car";
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
    String[] names =
    [
        "Ahmad",
        "Saleh",
        "Khaled",
        "Omar",
        "Muhammad"
    ];

    String[] cars =
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

    String[] collections =
    [
        "User",
        "Car"
    ];

    for(int i=0; i<count; i++)
    {
        if (random.Next(2) == 0)
        {
            String collection = "User";
            Document newDoc = new();
            newDoc.addField("Name", names[random.Next() % names.Length]);
            newDoc.addField("Age", random.Next(12, 80));
            dataBase.AddDoc(collection, newDoc);
        }
        else
        {
            String collection = "Car";
            Document newDoc = new();
            newDoc.addField("Make", cars[random.Next() % cars.Length]);
            newDoc.addField("Age", random.Next(0, 20));
            dataBase.AddDoc(collection, newDoc);
        }
    }

}

void addRandomDocs(Collection collection, int docCount)
{

    String[] names =
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
dataBase.AddCollection("User");
dataBase.AddCollection("Car");

// Console.WriteLine(addRandomDocsBenchmark(dataBase, delayTime) / delayTime);
addRandomDocsCount(dataBase, 1000);
// Console.WriteLine(dataBase);

var addedDoc = dataBase.QueryDataBase("SET number -20 TO Age IN _ IN Car", out _);
Console.WriteLine($"addedDoc's docID: {addedDoc[0].docID}");
dataBase.QueryDataBase($"SET string S680 TO Make IN {addedDoc[0].docID} IN Car", out _);

String query = "Car Where Age == -20";

String make = "Camry";

List<Document> list = dataBase.QueryDataBase(query, out _);

Console.WriteLine($"recieved list size: {list.Count}");

foreach(Document doc in list){Console.WriteLine(doc.ToString());}

Console.WriteLine("Finished Program!");