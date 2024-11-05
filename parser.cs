using System.Text.Json;

/// <summary>
/// Query Froms:
/// Selection:
///     {collection name} return all docs of collection {collection name}
///     {collection name} Where {fieldName} {comparator: >, ==, <} {value}
/// 
/// Adding/Setting:
///     SET {valueType} {value} TO {field name} IN {docID} IN {collection name}
///     ADD {collection name} //add a new collection with name {collection name}
/// </summary>
public class ParsedQuery
{
    public string Operation { get; set; }
    public string DataType { get; set; }
    public string Value { get; set; }
    public string Field { get; set; }
    public string DocumentId { get; set; }
    public string Collection { get; set; }
    public string Comparator { get; set; }
    public bool IsSelectionQuery { get; set; }
    public bool IsADDQuery { get; set; }
    public bool IsGETQuery { get; set; }
}

public class Parser
{

    public static List<string> Tokenize(string query)
    {
        if (query == null)
        {
            throw new Exception("query string is empty!");
        }
        return new List<string>(query.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public static ParsedQuery Parse(List<String> tokens)
    {
        ParsedQuery query = new();
        if (tokens[0] == "GET")
        {
            if (tokens.Count != 2)
            {
                throw new Exception("incorrect GET query, GET qeuries should only include the term 'GET' and the target collection name(or '_' if you want to get the name of all collections)");
            }
            query.IsGETQuery = true;
            query.Collection = tokens[1];
        }
        else if (tokens.Contains("Where"))
        {
            if (tokens.Count != 5)
            {
                throw new Exception("the selection query was written incorrectly!");
            }
            //Selection Query, form {collection name} Where {fieldName} {comparator: >, ==, <} {value}
            query.IsSelectionQuery = true;
            query.Collection = tokens[0];
            query.Field = tokens[2];
            query.Comparator = tokens[3];
            query.Value = tokens[4];
            query.IsSelectionQuery = true;
        }
        else if (tokens[0] == "SET")
        {
            if(tokens.Count != 9)
            {
                throw new Exception("incorrect SET query, SET query must be in form: SET {valueType} {value} TO {field name} IN {docID} IN {collection name}");
            }
            //upsertion query
            query.IsSelectionQuery = false;
            query.DataType = tokens[1];
            query.Value = tokens[2];
            query.Field = tokens[4];
            query.DocumentId = tokens[6];
            query.Collection = tokens[8];
        }
        else if (tokens[0] == "ADD")
        {
            if(tokens.Count != 2)
            {
                throw new Exception("ADD query must be in form: ADD {collection name}");
            }
            query.IsADDQuery = true;
            query.Collection = tokens[1];
        }
        else
        {
            throw new Exception($"query format: {JsonSerializer.Serialize(query)} not recognized!");
        }

        Console.WriteLine(JsonSerializer.Serialize(query));
        return query;
    }

}