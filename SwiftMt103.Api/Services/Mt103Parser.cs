using System.Text.RegularExpressions;

namespace SwiftMt103.Api.Services;

public class Mt103Parser
{
    public Dictionary<string, string> Parse(string input)
    {
        var result = new Dictionary<string, string>();

        var match = Regex.Match(input, @"\{4:(?<block>.*?)-\}", RegexOptions.Singleline);

        if (!match.Success)
            return result;

        var block = match.Groups["block"].Value;

        var fieldRegex = new Regex(
            @":(?<tag>\d{2}[A-Z]?):(?<value>.*?)(?=\r?\n:\d{2}[A-Z]?:|\z)",
            RegexOptions.Singleline);

        var matches = fieldRegex.Matches(block);

        foreach (Match m in matches)
        {
            var tag = m.Groups["tag"].Value;
            var value = m.Groups["value"].Value.Trim();

            result[tag] = value;
        }

        return result;
    }
}