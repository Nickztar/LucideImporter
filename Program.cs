const string url = "https://raw.githubusercontent.com/lucide-icons/lucide/refs/heads/main/icons/{0}.svg";
var httpClient = new HttpClient();

var svgWriter = File.CreateText("Svg.cs");
await svgWriter.WriteLineAsync("public class SVG {");
Dictionary<string, string> Svgs = [];

while (true)
{
    Console.WriteLine("Enter a lucide icon name (Enter to finish)");
    var iconName = Console.ReadLine();
    if (string.IsNullOrEmpty(iconName)) break;
    try
    {
        var toFetch = string.Format(url, iconName.ToLowerInvariant());
        var svgStr = await httpClient.GetStringAsync(toFetch);
        iconName = string.Join("", iconName.Split("-").Select(x => ToUpper(x, 0).ToString()));
        Console.WriteLine($"Found file with name: {iconName}");
        Console.WriteLine("Write to override, empty to use");
        var newName = Console.ReadLine();
        if (!string.IsNullOrEmpty(newName))
        {
            iconName = newName;
        }
        Svgs.TryAdd(iconName, svgStr);
    }
    catch
    {
        Console.WriteLine("Failed");
    }
}

foreach (var (name, content) in Svgs)
{
    await svgWriter.WriteLineAsync($"""" 
    public const string {name} = """
        {content}
    """;
    """");
}
await svgWriter.WriteLineAsync("}");
await svgWriter.FlushAsync();

Console.WriteLine($"Added {Svgs.Count} svgs to Svg.cs");
return;

static ReadOnlySpan<char> ToUpper(string stringValue, int index)
{
    Span<char> stringContent = stringValue.ToCharArray();
    stringContent[index] = char.ToUpper(stringContent[index]);
    return stringContent;
}