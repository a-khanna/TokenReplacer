# TokenReplacer

[![Build Status](https://travis-ci.org/a-khanna/TokenReplacer.svg?branch=master)](https://travis-ci.org/a-khanna/TokenReplacer)

A Microsoft.Extensions.Configuration based Configuration Token Replacer, which scans all previous ConfigurationProviders and replaces the tokens accordingly.
---
### Basic Example
1. Add tokens to your configuration. 
Here's an example appsettings.json. The `url` re-uses other configuration settings in the form of tokens by enclosing the keys in `{}`
```csharp
    {
      "ConnectionSettings": {
         "protocol": "https",
          "hostname": "somehost",
          "id": "id1234",
          "url": "{ConnectionSettings:protocol}://{ConnectionSettings:hostname}/{ConnectionSettings:id}"
        }
    }
```
2. Use AddTokenReplacement() on your ConfigurationBuilder
```csharp
    var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json"); // from Microsoft.Extensions.Configuration.Json
                    .AddTokenReplacement();
                    
    var Configuration = builder.Build();
    var connectionsettings = Configuration.GetSection("ConnectionSettings")["url"];
    // returns "https://somehost/id1234"
```

### Advanced Usage
##### Toggle token replacement from json configuration
Add the following to your json configuration and set `Replace` to `true` or `false`:
```json
    "TokenReplacementSettings": {
        "Replace": true
    }
```
##### Replace only specific tokens
Set `OnlySpecific` to `true` and add the tokens to be replaced in the `Tokens` array:
```json
    "TokenReplacementSettings": {
        "Replace": true,
        "OnlySpecific": true,
        "Tokens": [
            "ConnectionSettings:protocol",
            "ConnectionSettings:hostname"
        ]
    }
```