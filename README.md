# Web Portal for Automating Azure environment deployments
This repository includes reference code created by Microsoft, RISCO and U-BTech as part of a hackfest centered around DevOps. Please read the detailed technical case study to better understand the context and usage of this application:

[*Automating cloud deployments using Azure Resource Manager and Puppet with RISCO*](https://microsoft.github.io/techcasestudies/devops/2017/03/14/risco.html)

The code in this repository is meant to be used as a reference to showcase how to automate Azure environment deployments using a web application. 

The repository includes the code of an ASP.NET web portal which allows users to login using Azure Active Directory credentials and choose a predefined cloud environment to deploy. The web application then uses the Azure .NET SDK to request a deployment of a Resource Manager JSON template from the Azure Resource Manager.

To make use of this code, users need to edit the `web.config` file, specifically the `<appSettings>` section:

```xml
<appSettings>
    <add key="webpages:Version" value="3.0.0.0" />
    <add key="webpages:Enabled" value="false" />
    <add key="ClientValidationEnabled" value="true" />
    <add key="UnobtrusiveJavaScriptEnabled" value="true" />
    <add key="ida:ClientId" value="[Your Azure application client ID]" />
    <add key="ida:AADInstance" value="https://login.microsoftonline.com/" />
    <add key="ida:ClientSecret" value="[Your Azure application client secret]" />
    <add key="ida:Domain" value="[Azure AD domain]" />
    <add key="ida:TenantId" value="[Your tenant id]" />
    <add key="ida:PostLogoutRedirectUri" value="[Azure application reply URL]" />    
</appSettings>
```

To better understand how this project implements user authentication and makes use of the Azure SDK to interact with the Azure Resource Manager please see:

- [Creating Active Directory application and service principal](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal).
- [ASP.NET Web App Sign In & Sign Out with Azure AD](https://docs.microsoft.com/en-gb/azure/active-directory/develop/active-directory-devquickstarts-webapp-dotnet).
- [Deploy an Azure Virtual Machine using C# and a Resource Manager template](https://docs.microsoft.com/en-us/azure/virtual-machines/virtual-machines-windows-csharp-template?toc=/azure/virtual-machines/windows/toc.json).

## License ##

This project is licensed under MIT and Apache-2.0.

## Appreciation ##

Special thanks to Elad Hayun (U-BTech) who wrote most of the code published here.

