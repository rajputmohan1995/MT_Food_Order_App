namespace MT.Web.Utility;

public class StringExtensions
{
    public static string ToHumanize(string strValue)
    {
        return string.Concat(strValue.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }
}