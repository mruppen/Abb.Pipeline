using System.Linq;

namespace Abb.Pipeline
{
    public class IgnoreCaseNamingStrategy : INamingStrategy
    {
        public static INamingStrategy Instance => new IgnoreCaseNamingStrategy();

        public string FindMatch(string input, string[] allNames)
        {
            var comparand = input.ToLowerInvariant();
            return allNames.SingleOrDefault(n => n.ToLowerInvariant() == comparand);
        }
    }
}
