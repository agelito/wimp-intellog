<p align="center" style="background: rgb(36,36,36);
background: radial-gradient(circle, rgba(6,6,6,1) 14%, rgba(11,11,11,1) 17%, rgba(255,255,255,0) 23%);">
  <a href="" rel="noopener">
 <img width=200px height=200px src="https://images.evetech.net/alliances/99010468/logo" alt="WOMP Logo"></a>
</p>

<h3 align="center">WIMP Intel Log</h3>

<div align="center">

  ![Builds](https://github.com/agelito/wimp-intellog/actions/workflows/ci.yml/badge.svg?branch=master)
  [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=agelito_wimp-intellog&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=agelito_wimp-intellog)
  [![License](https://img.shields.io/github/license/agelito/wimp-intellog)](/LICENSE)

</div>

---

<p align="center"> This ia a console application reading EVE Online intel chat logs. 
    <br> 
</p>

## 📝 Table of Contents
- [About](#about)
- [Getting Started](#getting_started)
- [Deployment](#deployment)
- [Usage](#usage)
- [Built Using](#built_using)
- [Authors](#authors)
- [Acknowledgments](#acknowledgement)

## 🧐 About <a name = "about"></a>
This console application is part of a tool called WIMP (WOMP Intel Management Program). The purpose of this application is to watch and read the in-game intel channel chat logs. Once any message was added the application will send the message data to [WIMP-Server](https://github.com/agelito/wimp-server) for processing.

## 🏁 Getting Started <a name = "getting_started"></a>
These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
Install the following:
* [Git](https://git-scm.com/downloads). A distributed version control system.
* [Visual Studio Code](https://code.visualstudio.com/). Open source code editor.
* [NET Core SDK](https://dotnet.microsoft.com/download). The SDK also includes the Runtime.
* The [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) from the VS Code Marketplace.

After installing the prerequisites you can verify the installed dotnet version by typing this command in the terminal:
```
dotnet --version
```

### Installing
Follow these steps to get started working with the project:

Clone the repository:
```
git clone git@github.com:agelito/wimp-intellog.git
```

Open the cloned project with Visual Studio Code:

```
code wimp-intellog
```

Open the terminal in Visual Studio Code:

```
ctrl+shift+`
```

Install any missing NuGet packages:
```
dotnet restore
```

Build and run the application:
```
dotnet run
```

The application should now run with default configuration options. See [usage](#usage) section for information about how to configure the application. 

## 🎈 Usage <a name="usage"></a>

### Running the application <a name="running"></a>
The application can run from command line or by double clicking the executable file, or within editor using `dotnet run`:
```
./WIMP-IntelLog.exe
```

### Configuring the application <a name="configuration"></a>
The application is configuring using a configuration file. The configuration file should be located next to the application binary, or in the root project folder if developing and running using `dotnet run`:
```
# Configuration file
Config.json

# Application executable
WIMP-IntelLog.exe
```

The following configuration options is available:
Key | Description | Example
---|---|---
EveLogDirectory | The directory on disk where EVE log files are stored. | C:\\Users\\user\\Documents\\EVE\\logs\\Chatlogs
IntelChannelName | The name of in game intel channel. The application will try to find log files matching this name. The log files will begin with the name specified here followed by `_` and date. *Deprecation Warning: This field might become deprecated in the future.* | MyIntelChannel
IntelChannelNames | Array of in game intel channel names. The application will try to find log files matching each one of the names specified in this list. | ["MyIntelChannel", "MyOtherIntelChannel"]
IntelEndpoint | The application will send parsed log messages to the endpoint specified using `HTTP POST`.| http://localhost:5000/intel/

***Config.json Example***
```
{
    "EveLogDirectory": "C:\\Users\\user\\Documents\\EVE\\logs\\Chatlogs",
    "IntelChannelName": "MyIntelChannel",
    "IntelChannelNames": ["MyIntelChannel", "MyOtherIntelChannel"],
    "IntelReportService": "http://localhost:5000"
}
```

## ⛏️ Built Using <a name = "built_using"></a>
- [.Net Core](https://docs.microsoft.com/en-us/dotnet/core/introduction) - Development Platform

## ✍️ Authors <a name = "authors"></a>
- [@agelito](https://github.com/agelito) - Idea & Initial work

See also the list of [contributors](https://github.com/agelito/wimp-intellog/contributors) who participated in this project.

## 🎉 Acknowledgements <a name = "acknowledgement"></a>
- [EVE Online](https://www.eveonline.com/) - The fantastic game this tool is used with.
- [WOMP](https://evewho.com/alliance/99010468) - The great corp I'm part of in EVE Online.