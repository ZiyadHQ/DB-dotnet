using System.Text.Json;

/// <summary>
/// Query Froms:
/// Selection:
///     {collection name} return all docs of collection {collection name}
///     {collection name} Where {fieldName} {comparator: >, ==, <} {value}
/// 
/// Adding/Setting:
///     SET {valueType} {value} TO {field name} IN {docID} IN {collection name}
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
}

public class Parser
{

    public static List<string> Tokenize(string query)
    {
        return new List<string>(query.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public static ParsedQuery Parse(List<String> tokens)
    {
        ParsedQuery query = new();
        if(tokens.Count == 1)
        {
            //Selection Query, select all docs of collection
            return new ParsedQuery()
            {
                Collection = tokens[0],
                IsSelectionQuery = true
            };
        }
        else if(tokens.Contains("Where"))
        {
            //Selection Query, form {collection name} Where {fieldName} {comparator: >, ==, <} {value}
            query.IsSelectionQuery = true;
            query.Collection = tokens[0];
            query.Field = tokens[2];
            query.Comparator = tokens[3];
            query.Value = tokens[4];
            query.IsSelectionQuery = true;
        }
        else if(tokens[0] == "SET")
        {
            query.IsSelectionQuery = false;
            query.DataType = tokens[1];
            query.Value = tokens[2];
            query.Field = tokens[4];
            query.DocumentId = tokens[6];
            query.Collection = tokens[8];
        }

        Console.WriteLine(JsonSerializer.Serialize(query));
        return query;
    }

}