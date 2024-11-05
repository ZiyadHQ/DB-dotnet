
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

struct DataBase
{

    public int numberOfElements = 0;
    public DateTime lastChanged;
    public long dataSize = 0;
    Dictionary<String, Collection> collections;

    public DataBase()
    {
        this.collections = new();
        this.lastChanged = DateTime.Now;
    }

    public bool AddCollection(String collectionName)
    {
        Console.WriteLine($"adding new collection: {collectionName}");
        if (collections.ContainsKey(collectionName))
        {
            return false;
        }

        collections[collectionName] = new Collection(collectionName);

        return true;
    }

    public bool AddDoc(String collectionName, Document document)
    {

        if (!collections.ContainsKey(collectionName))
        {
            return false;
        }

        if (collections[collectionName].AddDoc(document, out _))
        {
            numberOfElements++;
            return true;
        }
        return false;
    }

    public bool AddDoc(String collectionName, String docID, Document document)
    {

        if (!collections.ContainsKey(collectionName))
        {
            return false;
        }

        if (collections[collectionName].AddDoc(document, out _))
        {
            numberOfElements++;
            return true;
        }
        return false;
    }

    public List<Document> QueryDataBase(String query, out String errorString)
    {
        errorString = String.Empty;

        var parsed = Parser.Parse(Parser.Tokenize(query));

        if (parsed.IsSelectionQuery)
        {
            return QueryWhere(parsed);
        }
        else if (parsed.IsADDQuery)
        {
            return QueryADD(parsed);
        }
        else if (parsed.IsGETQuery)
        {
            if (parsed.Collection == "_")
            {
                QueryGET(parsed);
                return [];
            }
            else
            {
                return QueryWhere(parsed);
            }
        }
        else
        {
            return QuerySET(parsed);
        }
    }

    public List<Document> QueryWhere(ParsedQuery parsedQuery)
    {
        if (parsedQuery.Field == null)
        {
            List<Document> documents = collections[parsedQuery.Collection].documents.Values.ToList();
            Console.WriteLine($"docs found: {documents.Count}");
            return documents;
        }
        else
        {
            Console.WriteLine(parsedQuery.Collection);
            var documents = collections[parsedQuery.Collection].documents.Values.ToList();

            var filteredDocuments = documents
                .Where(doc => doc.dataFields.ContainsKey(parsedQuery.Field))
                .Where(doc =>
                {
                    var field = doc.dataFields[parsedQuery.Field];

                    if (parsedQuery.Comparator == "==")
                    {
                        if (field.dataType == "string")
                            return field.stringData == parsedQuery.Value;
                        else if (field.dataType == "number")
                            return field.numberData == double.Parse(parsedQuery.Value);
                    }
                    else if (parsedQuery.Comparator == ">")
                    {
                        if (field.dataType == "string")
                            return String.Compare(field.stringData, parsedQuery.Value, StringComparison.Ordinal) > 0;
                        else if (field.dataType == "number")
                            return field.numberData > double.Parse(parsedQuery.Value);
                    }
                    else if (parsedQuery.Comparator == "<")
                    {
                        if (field.dataType == "string")
                            return String.Compare(field.stringData, parsedQuery.Value, StringComparison.Ordinal) < 0;
                        else if (field.dataType == "number")
                            return field.numberData < double.Parse(parsedQuery.Value);
                    }
                    else if (parsedQuery.Comparator == ">=")
                    {
                        if (field.dataType == "string")
                            return String.Compare(field.stringData, parsedQuery.Value, StringComparison.Ordinal) >= 0;
                        else if (field.dataType == "number")
                            return field.numberData >= double.Parse(parsedQuery.Value);
                    }
                    else if (parsedQuery.Comparator == "<=")
                    {
                        if (field.dataType == "string")
                            return String.Compare(field.stringData, parsedQuery.Value, StringComparison.Ordinal) <= 0;
                        else if (field.dataType == "number")
                            return field.numberData <= double.Parse(parsedQuery.Value);
                    }

                    return false; // Default if no condition is met
                }).ToList();
            Console.WriteLine($"docs found: {filteredDocuments.Count}");
            return filteredDocuments;
        }
    }

    public List<Document> QuerySET(ParsedQuery parsedQuery)
    {

        Document doc = new();
        // "_" means create a new document
        if (parsedQuery.DocumentId == "_")
        {
            if (parsedQuery.DataType == "string")
            {
                doc.addField(parsedQuery.Field, parsedQuery.Value);
            }
            else if (parsedQuery.DataType == "number")
            {
                doc.addField(parsedQuery.Field, double.Parse(parsedQuery.Value));
            }

            collections![parsedQuery.Collection].AddDoc(doc, out String assignedID);
            doc.docID = assignedID;
            Console.WriteLine($"Assigned ID of new Doc: {assignedID}");
        }
        else
        {
            Collection collection = collections[parsedQuery.Collection];
            doc = collection.documents[parsedQuery.DocumentId];
            if (parsedQuery.DataType == "string")
            {
                doc.addField(parsedQuery.Field, parsedQuery.Value);
            }
            else if (parsedQuery.DataType == "number")
            {
                doc.addField(parsedQuery.Field, double.Parse(parsedQuery.Value));
            }
            collections![parsedQuery.Collection].AddDoc(parsedQuery.DocumentId, doc);
        }
        Console.WriteLine(doc);
        return [doc];
    }

    public List<Document> QueryADD(ParsedQuery parsed)
    {
        if (!AddCollection(parsed.Collection))
        {
            throw new Exception("failed to add new collection, check whether your query was correct or if the collection already exists.");
        }
        return [];
    }

    public void QueryGET(ParsedQuery parsed)
    {

        if (collections.Count == 0)
        {
            throw new Exception("there are no collections in this data base, use ADD {collection name} to add a new collection");
        }
        else
        {
            List<String> list = new();
            foreach (var kvp in collections) { list.Add($"{kvp.Key}"); };
            throw new Exception(JsonSerializer.Serialize(list));
        }

    }

    public override string ToString()
    {
        String buffer = "";

        foreach (var kvp in collections)
        {
            buffer += kvp.Value.ToString();
        }

        return buffer;
    }

}

struct Collection
{

    public Collection(String collectionName)
    {
        this.collectionName = collectionName;
    }

    public String collectionName;
    public Dictionary<String, Document> documents = new();

    public bool AddDoc(String docID, Document document)
    {

        if (documents.ContainsKey(docID))
        {
            return false;
        }

        document.docID = docID;
        documents[docID] = document;
        return true;
    }

    public bool AddDoc(Document document, out String assignedID)
    {
        assignedID = String.Empty;
        String docID = Guid.NewGuid().ToString();

        if (documents.ContainsKey(docID))
        {
            return false;
        }

        document.docID = docID;
        documents[docID] = document;
        assignedID = docID;
        return true;
    }

    public override string ToString()
    {
        //only display the first 10 docs
        int docCount = 0;

        String buffer = $"Collection Name: {this.collectionName}\n";
        foreach (var kvp in documents)
        {
            if (docCount > 10) break;
            buffer += kvp.Value;
            docCount++;
        }

        buffer += "\n";
        return buffer;
    }

}

struct Document
{

    public String docID;
    public Dictionary<String, DocumentField> dataFields;

    public Document()
    {
        this.dataFields = new();
    }

    public bool addField(String fieldName, String stringData)
    {

        if (dataFields.ContainsKey(fieldName))
        {
            return false;
        }

        dataFields[fieldName] = new DocumentField(fieldName, stringData);

        return true;
    }

    public bool addField(String fieldName, double numberData)
    {

        if (dataFields.ContainsKey(fieldName))
        {
            return false;
        }

        dataFields[fieldName] = new DocumentField(fieldName, numberData);

        return true;
    }

    public bool setFieldData(String fieldName, String stringData)
    {
        if (dataFields.ContainsKey(fieldName))
        {
            return false;
        }

        if (dataFields[fieldName].dataType == "string")
        {
            return false;
        }



        return true;
    }

    public override string ToString()
    {
        String buffer = $"Document ID: {docID}_";
        foreach (var kvp in dataFields)
        {
            buffer += kvp.Value + "_";
        }

        return buffer;
    }

}

struct DocumentField
{

    public DocumentField(String fieldName, String stringData)
    {
        this.fieldName = fieldName;
        this.dataType = "string";
        this.stringData = stringData;
    }

    public DocumentField(String fieldName, double numberData)
    {
        this.fieldName = fieldName;
        this.dataType = "number";
        this.numberData = numberData;
    }

    public readonly String fieldName;
    public readonly String dataType;

    //data fields
    public String? stringData;
    public double? numberData;

    public override string ToString()
    {
        String dataString;

        switch (dataType)
        {

            case "string":
                dataString = stringData;
                break;

            case "number":
                dataString = numberData + "";
                break;

            default:
                dataString = "error!";
                break;
        }

        return $"fieldName: {fieldName}, dataType: {dataType}, value: {dataString}";
    }

}