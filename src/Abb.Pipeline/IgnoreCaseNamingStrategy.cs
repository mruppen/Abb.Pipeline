using System.Linq;

namespace Abb.Pipeline
{
    public class IgnoreCaseNamingStrategy : INamingStrategy
    {
        public static INamingStrategy Instance => new IgnoreCaseNamingStrategy();

        public string FindMatch(string input, string[] allNames)
        {
            var comparand = input?.ToUpperInvariant();
            return allNames?.SingleOrDefault(n => n.ToUpperInvariant() == comparand);
        }
    }
}
