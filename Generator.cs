
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
        Words = await LoadWordList();
        TopLevelDomains = await LoadTopLevelDomains();
        AllPossibleUrls = GetAllPossibleUrls(Words, TopLevelDomains);
    }

    private async Task<string[]> LoadWordList() {
        string words = await _client.GetStringAsync("/BlazorGitHubPagesDemo/word-lists/words.txt");
        return words.Split("\r\n");
    }

    private async Task<HashSet<string>> LoadTopLevelDomains() {
        string words = await _client.GetStringAsync("/BlazorGitHubPagesDemo/word-lists/tlds.txt");
        return words.Split("\r\n").ToHashSet<string>();
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