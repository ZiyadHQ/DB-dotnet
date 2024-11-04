
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

int delayTime = 10;

Console.WriteLine(addRandomDocsBenchmark(dataBase, delayTime) / delayTime);
// Console.WriteLine(dataBase);

String query = "Camry Where";

String make = "Camry";

dataBase.QueryDataBase(query, out _);
