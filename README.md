# Simple I18n

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)

## Introduction

It's a small library to provide .Net Core I18n translations from JSON files.


## Platforms Supported

Simplei18n itself targets .NET 5, and works with all applications
based on .NET 5 and above, including:

- ASP.NET MVC
- ASP.NET Web API
- .NET Windows Forms/WPF
- .NET Console Application

## Installation

Beta version:

```
    dotnet add package Simplei18n -v 1.0.4
```

## Project Configuration

### 1 - Create translation files

After nuget package is installed, you need to create a directory in your project to provide JSONs for all languages that you want to support. For each language, you will need to create a JSON file according to the respective culture name. E.g. for french from France the JSON name will be fr-FR.json, as well for Brazilian Portuguese it will be pt-BR.json.

```
+-Locales
    |__ en-US.json
    |__ fr-FR.json
    |__ pt-BR.json
```
    
### 2 - Configuring appsettings.json

Add the code below in your appsettings.json and configure the parameters according to your scenario:

```
  "SimpleI18n": {
    "LocaleFilesPath": "bin/Debug/netcoreapp5.0/LocaleFiles",
    "DefaultCultureName": "pt-BR",
    "UseCurrentThreadCulture": true
  }
```
### 3 - Configuring Dependency Injection

In your startup.cs code, just add the extension method AddSimpleI18n() as below:

```
    \\Import
    using SimpleI18n.DependencyInjection;
    
    public class Startup
    {    
        \\ ...
        
        public void ConfigureServices(IServiceCollection services)
        {
                \\ ...
                
                services.AddSimpleI18n();
                
                \\ ...
        }
        
        \\ ...
        
    }
```

## Translation Files Example

en-US.json

```
{
    "Welcome": "Welcome",
    "Welcome {0}": "Welcome {0}",
    "Mary": "Mary"
}
```

pt-BR.json

```
{
    "Welcome" : "Bem-vindo",
    "Welcome {0}": "Bem-vindo {0}",
    "Welcome {0}{f}": "Bem-vinda {0}",
    "Mary": "Maria[f]"
}

```

fr-FR.json

```
{
    "Welcome" : "Bienvenue"
    "Welcome {0}": "Bienvenu {0}",
    "Welcome {0}{f}": "Bienvenue {0}",
    "Mary": "Marie[f]"
}

```






