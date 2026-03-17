using FastEndpoints;

public class AnalyticsGroup : Group
{
    public AnalyticsGroup()
    {
        Configure("", ep =>
        {
            ep.Description(x => x.WithTags("Analytics"));
        });
    }
}