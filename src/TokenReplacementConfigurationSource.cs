using Microsoft.Extensions.Configuration;

namespace ConfigurationExtension
{
    internal class TokenReplacementConfigurationSource : IConfigurationSource
    {
        private IConfigurationRoot configurationRoot;

        public TokenReplacementConfigurationSource(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new TokenReplacementConfigurationProvider(configurationRoot);
        }
    }
}