using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Commando
{
	public class AppSettingResolver
	{
		public static Regex regex = new Regex("{([^}]*)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static string Setting(string key)
		{
			var raw = ConfigurationManager.AppSettings[key];
			return Resolve(raw);
		}

		public static string Resolve(string raw)
		{
			if (string.IsNullOrWhiteSpace(raw))
				return raw;

			if (raw.IndexOf('{') < 0) return raw;

			var matches = regex.Matches(raw);

			foreach (Match m in matches)
			{
				var token = m.Groups[0].Value;
				var resolved = Setting(m.Groups[1].Value);
				if (resolved == null)
					throw new ArgumentException("The token: \"{}\" failed to resolve to an appSetting value", token);
				raw = raw.Replace(token, resolved);
			}

			return raw;
		}
	}
}
