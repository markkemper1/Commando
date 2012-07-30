using System.Configuration;
using NUnit.Framework;

namespace Commando.Test
{
	[TestFixture]
	public class AppSettingResolverTest
	{
		[SetUp]
		public void SetUp()
		{
			ConfigurationManager.AppSettings["none"] = "none_value";
			ConfigurationManager.AppSettings["none2"] = "none2_value";
			ConfigurationManager.AppSettings["one"] = "one_then_{none}";
			ConfigurationManager.AppSettings["two"] = "{none}_{none2}";
			ConfigurationManager.AppSettings["two_levels"] = "{none}_{two}";
			ConfigurationManager.AppSettings["none"] = "none_value";
			ConfigurationManager.AppSettings["none"] = "none_value";
		}

		[Test]
		public void should_resolve_normal_setting()
		{
			Assert.That(AppSettingResolver.Resolve("none"), Is.EqualTo("none_value"));
		}

		[Test]
		public void should_resolve_one_level_of_tokens()
		{
			Assert.That(AppSettingResolver.Resolve("one"), Is.EqualTo("one_then_none_value"));
		}

		[Test]
		public void should_resolve_two_one_level_of_tokens()
		{
			Assert.That(AppSettingResolver.Resolve("two"), Is.EqualTo("none_value_none2_value"));
		}

		[Test]
		public void should_resolve__one_with_two_levels_of_tokens()
		{
			Assert.That(AppSettingResolver.Resolve("two_levels"), Is.EqualTo("none_value_none_value_none2_value"));
		}
	}
}
