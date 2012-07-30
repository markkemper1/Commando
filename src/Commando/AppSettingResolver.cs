using System.Configuration;
using System.Text.RegularExpressions;

namespace Commando
{
	public class AppSettingResolver
	{
		public static Regex regex = new Regex("{([^}]*)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static string Resolve(string key)
		{
			var raw = ConfigurationManager.AppSettings[key];

			if (raw.IndexOf('{') < 0) return raw;

			var matches = regex.Matches(raw);

			foreach(Match m in matches)
				raw = raw.Replace(m.Groups[0].Value, Resolve(m.Groups[1].Value));

			return raw;
		}
	}
}
