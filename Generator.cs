
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
    }

    public async Task Initialize() {
        Console.WriteLine("Initializing Generator...");
        Words = await LoadWordList();
        TopLevelDomains = await LoadTopLevelDomains();
        AllPossibleUrls = GetAllPossibleUrls(Words, TopLevelDomains);
        Console.WriteLine("Generator initialization complete.");
    }

    private async Task<string[]> LoadWordList() {
        string wordListPath = "/BlazorGitHubPagesDemo/word-lists/words.txt";
        Console.WriteLine($"Trying to load the wordlist at '{wordListPath}'");
        string words = await _client.GetStringAsync(wordListPath);
        Console.WriteLine($"Loaded wordlist. {words.Length} words loaded.");
        return words.Split("\r\n");
    }

    private async Task<HashSet<string>> LoadTopLevelDomains() {
        string tldListPath = "/BlazorGitHubPagesDemo/word-lists/tlds.txt";
        Console.WriteLine($"Trying to load the tld list at at '{tldListPath}'");
        string tlds = await _client.GetStringAsync(tldListPath);
        Console.WriteLine($"Loaded tldList. {tlds.Length} tlds loaded.");
        return tlds.Split("\r\n").ToHashSet<string>();
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
                if (i % 100 == 0) {
                    Console.WriteLine($"Generated {i*100} urls...");
                }
            }
        }
        
        Console.WriteLine($"Generated all possible urls. {AllPossibleUrls.Count} urls generated.");
        return overlaps;
    }

    public string GetRandomUrl() {
        Console.WriteLine("Getting a random url.");
        return AllPossibleUrls[random.Next(AllPossibleUrls.Count - 1)];
    }
}