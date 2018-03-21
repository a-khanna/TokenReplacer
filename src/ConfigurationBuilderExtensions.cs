using Microsoft.Extensions.Configuration;

namespace ConfigurationExtension
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddTokenReplacement(this IConfigurationBuilder configurationBuilder)
        {
            var config = configurationBuilder.Build();
            return configurationBuilder.Add(new TokenReplacementConfigurationSource(config));
        }
    }
}
