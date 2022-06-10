
namespace DomainGenerator;

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
        Words = await LoadWordList();
        TopLevelDomains = await LoadTopLevelDomains();
        AllPossibleUrls = GetAllPossibleUrls(Words, TopLevelDomains);
    }

    private async Task<string[]> LoadWordList() {
        string wordListPath = $"word-lists/words.txt";
        string words = await _client.GetStringAsync(wordListPath);
        string[] result = words.Split("\n");
        return result;
    }

    private async Task<HashSet<string>> LoadTopLevelDomains() {
        string tldListPath = $"word-lists/tlds.txt";
        string tlds = await _client.GetStringAsync(tldListPath);
        HashSet<string> result = tlds.Split("\n").ToHashSet<string>();
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
        
        return overlaps;
    }

    public string GetRandomUrl() {
        return AllPossibleUrls[random.Next(AllPossibleUrls.Count - 1)];
    }
}