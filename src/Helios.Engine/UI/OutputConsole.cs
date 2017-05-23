using System.Text;
using System.Text.RegularExpressions;

namespace Helios.Engine.UI
{
    public class OutputConsole : IOutputFormatter
    {
        public string Write(string input)
        {
            DoColorsAndLineBreaks(ref input);

            var sb = new StringBuilder();
            sb.AppendLine(input);
            sb.AppendLine();

            return sb.ToString();
        }

        private void DoColorsAndLineBreaks(ref string input)
        {
            var colorTokens = Regex.Matches(input, "<#\\w{3,}>");

            foreach (Match match in colorTokens)
            {
                //var clr = match.Value.Substring(2, match.Value.Length - 3);
                //input = input.Replace(match.Value, $"<span style=\"color: {clr}\">");
                input = input.Replace(match.Value, "");
            }

            input = input.Replace("<#>", "");
            //input = input.Replace("\n", "<br />");
            input = input.Replace("<tab>", "   ");
        }
    }
}
