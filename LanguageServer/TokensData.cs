using Newtonsoft.Json;

public class TokensData
{
    public List<string> tokenNames = new();

    [JsonIgnore]
    public Dictionary<string, Type> typeByName = new();
}