using System.Linq;

namespace Abb.Pipeline
{
    public class StrictNamingStrategy : INamingStrategy
    {
        public static INamingStrategy Instance { get; } = new StrictNamingStrategy();

        public string FindMatch(string input, string[] allNames)
        {
            if (!allNames.Any(n => n == input))
                return null;

            return input;
        }
    }
}
