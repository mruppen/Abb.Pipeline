using System.Linq;

namespace Abb.Pipeline
{
    public class IgnoreCaseNamingStrategy : INamingStrategy
    {
        public static INamingStrategy Instance { get; } = new IgnoreCaseNamingStrategy();

        public string FindMatch(string input, string[] allNames)
        {
            var comparand = input.ToLowerInvariant();
            return allNames.SingleOrDefault(n => n.ToLowerInvariant() == comparand);
        }
    }
}
