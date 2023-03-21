# Etherscan Worker Service

This service will scan all block that is configured and it was built in window platform

You need install .NET CORE 3.1 packages to run demo
## Installation

Install env
<br/>
 - [Install NET CORE 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-3.1.426-windows-x64-installer)
 - [Install NET CORE 3.1 Hosting Bundle](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-3.1.32-windows-hosting-bundle-installer)
 - [Install Mysql](https://dev.mysql.com/downloads/file/?id=516927)
 - Install redis cli in Install folder of this repository
 - Restore etherscan db, file .sql inside Install folder
 
## Buiding and running

To build tests, run the following command

```bash
  dotnet build
```

## Run
Run Etherscan.WorkerService.exe in bin\Debug\netcoreapp3.1 folder and trace log
<br/>
If any config is changed, change it in file bin\Debug\netcoreapp3.1\appsettings.json
