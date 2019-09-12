namespace Abb.Pipeline
{
    public interface INamingStrategy
    {
        string FindMatch(string input, string[] allNames);
    }
}
