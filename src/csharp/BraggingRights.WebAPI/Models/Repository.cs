namespace BraggingRights.WebAPI;

public class Repository
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public bool Private { get; set; }
    public string HtmlUrl { get; set; }
    public string Description { get; set; }
    public string Homepage { get; set; }
    public int Size { get; set; }
    public string Language { get; set; }

    public IList<Language> Languages { get; set; }
}