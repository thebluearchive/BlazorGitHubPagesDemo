
namespace BlazorGitHubPagesDemo;

public class Generator {
    private string[] Words { get; set; }
    private HashSet<string> TopLevelDomains { get; set; }
    private List<string> AllPossibleUrls { get;set; }
    private Random random = new();
    private HttpClient _client;

    /// <summary>
    /// Instantiates the Generator class and loads the word lists
    /// required for the generation of urls.
    /// </summary>
    public Generator(HttpClient client) {
        _client = client;
        Console.WriteLine($"client initialized with base address '{client.BaseAddress}'");
    }

    public async Task Initialize() {
        Console.WriteLine("Initializing Generator...");
        Words = await LoadWordList();
        TopLevelDomains = await LoadTopLevelDomains();
        AllPossibleUrls = GetAllPossibleUrls(Words, TopLevelDomains);
        Console.WriteLine("Generator initialization complete.");
    }

    private async Task<string[]> LoadWordList() {
        string wordListPath = $"word-lists/words.txt";
        Console.WriteLine($"Trying to load the wordlist at '{wordListPath}'");
        string words = await _client.GetStringAsync(wordListPath);
        string[] result = words.Split("\r\n");
        Console.WriteLine($"Loaded wordlist. {result.Length} words loaded.");
        PrettyPrint(result);
        return result;
    }

    private async Task<HashSet<string>> LoadTopLevelDomains() {
        string tldListPath = $"word-lists/tlds.txt";
        Console.WriteLine($"Trying to load the tld list at at '{tldListPath}'");
        string tlds = await _client.GetStringAsync(tldListPath);
        HashSet<string> result = tlds.Split("\r\n").ToHashSet<string>();
        Console.WriteLine($"Loaded tldList. {result.Count} tlds loaded.");
        PrettyPrint(result);
        return result;
    }

    private void PrettyPrint(IEnumerable<string> strings) {
        Console.Write("[");
        foreach (string elem in strings) {
            Console.Write($"\"{elem}\", ");
        }
        Console.WriteLine("]");
    }

    public List<string> GetAllPossibleUrls(string[] words, HashSet<string> tlds) {
        List<string> overlaps = new();

        Console.WriteLine("Generating all possible urls...");
        foreach (string word in words)
        {
            for (int i = 1; i < word.Length; i++)
            {
                string suffix = word[i..];
                if (tlds.Contains(suffix))
                {
                    overlaps.Add($"{word[..i]}.{suffix}");
                }
            }
        }
        
        Console.WriteLine($"Generated all possible urls. {overlaps.Count} urls generated.");
        return overlaps;
    }

    public string GetRandomUrl() {
        Console.WriteLine("Getting a random url.");
        return AllPossibleUrls[random.Next(AllPossibleUrls.Count - 1)];
    }
}