using FastEndpoints;

public class UrlsGroup : Group
{
    public UrlsGroup()
    {
        Configure("", ep =>
        {
            ep.Description(x => x.WithTags("URLs"));
        });
    }
}