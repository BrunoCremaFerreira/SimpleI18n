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

## Usage

### Translation Files Example:

en-US.json

```
{
    "Welcome": "Welcome",
    "Welcome {0}": "Welcome {0}",
    "Mary": "Mary",
    "Fork": "Fork",
    "{0} example": 
    {
        "Zero": "no example",
        "One": "{0} example",
        "Other": "{0} examples"
    },
    "Animal": 
    {
        "Zero": "Animal",
        "One": "Animal",
        "Other": "Animals"
    }
}
```

pt-BR.json

```
{
    "Welcome" : "Bem-vindo",
    "Welcome {0}": "Bem-vindo {0}",
    "Welcome {0}{f}": "Bem-vinda {0}",
    "Mary": "Maria[f]",
    "Fork": "Garfo",
    "{0} example": 
    {
        "Zero": "nenhum exemplo",
        "One": "{0} exemplo",
        "Other": "{0} exemplos"
    },
    "Animal": 
    {
        "Zero": "Animal",
        "One": "Animal",
        "Other": "Animais"
    }
}

```

fr-FR.json

```
{
    "Welcome" : "Bienvenue"
    "Welcome {0}": "Bienvenu {0}",
    "Welcome {0}{f}": "Bienvenue {0}",
    "Mary": "Marie[f]",
    "{0} example": 
    {
        "Zero": "pas d'exemple",
        "One": "{0} exemple",
        "Other": "{0} exemples"
    },
    "Animal": 
    {
        "Zero": "Animal",
        "One": "Animal",
        "Other": "Animaux"
    }
}

```

### Usage from IStringLocalizer Interface:

The Simple I18n implements the Microsoft.Extensions.Localization.IStringLocalizer interface. You can use this by injection:

```
using Microsoft.Extensions.Localization;

\\ ...

public class YourClass
{

    protected readonly IStringLocalizer Localizer;
    
    public YourClass(IStringLocalizer localizer)
    {
        Localizer = localizer;
    }
    
    public string GetWelcomeMessage()
    {
        return Localizer["Welcome {0}", Localizer["Mary"]];
        
        // Translations expected for:
        // en-US: "Welcome Mary"
        // pt-BR: "Bem-vinda Maria"   // Genre applied
        // fr-FR: "Bienvenue Marie"   // Genre applied
    }
    
    public string GetAnimals(int i)
    {
        return Localizer["Animal", i];
        
        // Translations expected for:
        // en-US:   for i == 0: "Animal"
        //          for i == 1: "Animal"
        //          for i >  1: "Animals"
        //            
        // pt-BR:   for i == 0: "Animal"
        //          for i == 1: "Animal"
        //          for i >  1: "Animais"
        //            
        // fr-FR:   for i == 0: "Animal"
        //          for i == 1: "Animal"
        //          for i >  1: "Animaux"
    }
    
    public string GetExample(int i)
    {
        return Localizer["{0} example", i];
        
        // Translations expected for:
        // en-US:   for i == 0: "no example"
        //          for i == 1: "1 example"
        //          for i >  1: "i examples"
        //            
        // pt-BR:   for i == 0: "nenhum exemplo"
        //          for i == 1: "1 exemplo"
        //          for i >  1: "i exemplos"
        //            
        // fr-FR:   for i == 0: "pas d'exemple"
        //          for i == 1: "1 exemple"
        //          for i >  1: "i exemples"
    }
    
}

```

### Usage in Razor Pages:

```
@using Microsoft.Extensions.Localization

@inject IStringLocalizer Localizer

@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">@Localizer["Welcome"]</h1>
    <h1 class="display-4">@Localizer["Welcome {0}", Localizer["Mary"]]</h1>
</div>

```






