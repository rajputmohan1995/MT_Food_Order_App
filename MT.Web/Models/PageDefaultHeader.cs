namespace MT.Web.Models;

public class PageDefaultHeader
{
    public PageDefaultHeader(string title)
    {
        Title = title;
        PageLinks = new List<PageLinks>();
    }
    public string Title { get; set; }
    public List<PageLinks> PageLinks { get; set; }
}

public class PageLinks
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public string PageName { get; set; }
    public string Param { get; set; }
    public bool IsCurrentPage { get; set; }
}