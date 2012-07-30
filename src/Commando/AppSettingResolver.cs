using System;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Commando
{
	public class AppSettingResolver
	{
		public static Regex regex = new Regex("{([^}]*)}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		/// <summary>
		///		Retrieve an appSetting an resolve any tokens.
		/// </summary>
		/// <param name="key">The app Setting key</param>
		/// <returns>The resolved appSetting value.</returns>
		public static string Setting(string key)
		{
			var raw = ConfigurationManager.AppSettings[key];
			return Resolve(raw);
		}

		/// <summary>
		///		Resolve appSettings tokens in the string passed
		/// </summary>
		/// <param name="input">The input string to resolve appSettings for. e.g. "{test}" => "the_value_of_test_app_setting"</param>
		/// <returns>The resolved string</returns>
		public static string Resolve(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return input;

			if (input.IndexOf('{') < 0) return input;

			var matches = regex.Matches(input);

			foreach (Match m in matches)
			{
				var token = m.Groups[0].Value;
				var resolved = Setting(m.Groups[1].Value);
				if (resolved == null)
					throw new ArgumentException("The token: \"{}\" failed to resolve to an appSetting value", token);
				input = input.Replace(token, resolved);
			}

			return input;
		}
	}
}
