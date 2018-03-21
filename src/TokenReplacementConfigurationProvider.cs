using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ConfigurationExtension
{
    internal class TokenReplacementConfigurationProvider : ConfigurationProvider
    {
        private IConfigurationRoot configurationRoot;
        private TokenReplacementSettings tokenReplacementSettings;

        public TokenReplacementConfigurationProvider(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;

            var configSettings = configurationRoot.GetSection("TokenReplacementSettings");
            tokenReplacementSettings = new TokenReplacementSettings
            {
                Replace = configSettings["Replace"] != null ? bool.Parse(configSettings["Replace"]) : true,
                OnlySpecific = configSettings["OnlySpecific"] != null ? bool.Parse(configSettings["OnlySpecific"]) : false,
                Tokens = configSettings.AsEnumerable().Where(kvp => kvp.Key.StartsWith("TokenReplacementSettings:Tokens:")).Select(kvp => kvp.Value)
            };
        }

        public override void Load()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (tokenReplacementSettings.Replace)
            {
                foreach (var provider in configurationRoot.Providers)
                {
                    var keys = GetAllKeys(provider, null, Enumerable.Empty<string>().ToList());
                    foreach (var key in keys)
                    {
                        var initialValue = configurationRoot[key];
                        var value = initialValue != null ? ReplaceTokensIfAny(initialValue) : null;

                        if (value != null)
                            data.Add(key, value);
                    }
                }
            }
            Data = data;
        }

        private string ReplaceTokensIfAny(string value)
        {
            var valueCopy = string.Copy(value);
            bool replacementPerformed = false;
            int offset = 0;

            while (value.IndexOf('}', offset) > value.IndexOf('{', offset))
            {
                var token = value.Substring(value.IndexOf('{', offset) + 1, value.IndexOf('}', offset) - value.IndexOf('{', offset) - 1);

                if (tokenReplacementSettings.OnlySpecific)
                {
                    if (tokenReplacementSettings.Tokens.SingleOrDefault(t => t.Equals(token)) != null)
                        Replace(token, ref valueCopy, ref replacementPerformed);
                    else
                    {
                        offset = value.IndexOf('}', offset) + 1;
                        continue;
                    }
                }
                else
                    Replace(token, ref valueCopy, ref replacementPerformed);

                offset = value.IndexOf('}', offset) + 1;
            }

            return replacementPerformed ? valueCopy : null;
        }

        private void Replace(string token, ref string value, ref bool replacementPerformed)
        {
            if (configurationRoot[token] != null)
            {
                value = value.Replace(string.Format("{{{0}}}", token), configurationRoot[token]);
                replacementPerformed = true;
            }
        }

        private List<string> GetAllKeys(IConfigurationProvider provider, string parentPath, List<string> initialKeys)
        {
            foreach (var key in provider.GetChildKeys(Enumerable.Empty<string>(), parentPath).Distinct())
            {
                var currentKey = string.IsNullOrEmpty(parentPath) ? key : $"{parentPath}:{key}";
                GetAllKeys(provider, currentKey, initialKeys);

                if (!initialKeys.Any(k => k.StartsWith(currentKey)))
                    initialKeys.Add(currentKey);
            }
            return initialKeys;
        }
    }
}