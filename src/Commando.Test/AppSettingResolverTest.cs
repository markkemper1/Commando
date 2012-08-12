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
			ConfigurationManager.AppSettings["none"] = "";
			ConfigurationManager.AppSettings["none"] = "none_value";
			ConfigurationManager.AppSettings["none2"] = "none2_value";
			ConfigurationManager.AppSettings["one"] = "one_then_{none}";
			ConfigurationManager.AppSettings["two"] = "{none}_{none2}";
			ConfigurationManager.AppSettings["two_levels"] = "{none}_{two}";
			ConfigurationManager.AppSettings["invalid"] = "{none}_{two_NOT here}";
		}

		[Test]
		public void missing_appSetting_should_return_null()
		{
			Assert.That(AppSettingResolver.Setting("nothing"), Is.EqualTo(null));
		}

		[Test]
		public void when_unable_to_resolve_appSeting_token_should_throw()
		{
			Assert.That(() => { AppSettingResolver.Setting("invalid"); }, Throws.ArgumentException);
		}

		[Test]
		public void should_resolve_appSeting_normal_setting()
		{
			Assert.That(AppSettingResolver.Setting("none"), Is.EqualTo("none_value"));
		}

		[Test]
		public void should_resolve_appSeting_one_level_of_tokens()	
		{
			Assert.That(AppSettingResolver.Setting("one"), Is.EqualTo("one_then_none_value"));
		}

		[Test]
		public void should_resolve_appSeting_two_one_level_of_tokens()
		{
			Assert.That(AppSettingResolver.Setting("two"), Is.EqualTo("none_value_none2_value"));
		}

		[Test]
		public void should_resolve_appSeting__one_with_two_levels_of_tokens()
		{
			Assert.That(AppSettingResolver.Setting("two_levels"), Is.EqualTo("none_value_none_value_none2_value"));
		}

		[Test]
		public void should_resolve_a_string()
		{
			Assert.That(AppSettingResolver.Resolve("{two}"), Is.EqualTo("none_value_none2_value"));
		}

		[Test]
		public void should_resolve_connection_string()
		{
			Assert.That(AppSettingResolver.Resolve(ConfigurationManager.ConnectionStrings[0].ConnectionString), Is.EqualTo("none_value_none2_value"));
		}

	}
}
