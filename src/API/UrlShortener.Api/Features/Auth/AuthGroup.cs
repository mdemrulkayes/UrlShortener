using FastEndpoints;

public class AuthGroup : Group
{
    public AuthGroup()
    {
        Configure("", ep =>
        {
            ep.Description(x => x.WithTags("Authentication"));
        });
    }
}