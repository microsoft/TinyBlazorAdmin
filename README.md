# TinyBlazorAdmin

 Admin tools for [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener) using Blazor Single Page Application (webassembly).


 # Manual Deployment

 Until an automatic deployment is created here is the steps to deploy the TinyBlazorAdmin app into Azure. You can run it somewhere else and even locally.

### Create the backend.

This project is a frontend only so you will need to deploy the [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener) in "headless mode". Do to it click the blue button below and make sure to select **none** as Frontend

[![Deploy Backend to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/?WT.mc_id=urlshortener-github-frbouche#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUrlShortener%2Fmaster%2Fdeployment%2FazureDeploy.json)

![CreateBackend][CreateBackend]

## Create a Service Principal in Azure

From the Azure Portal (portal.azure.com), open the **Azure Active Directory** page. From the left option menu select **App registrations**, then create a new registration. Note the ClientID and TenantID 

![Create a new registration][newRegistration]

And update those values inside `TinyBlazorAdmin\wwwroot\appsettings.json`

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxxxx",
    "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxxxx",
    "ValidateAuthority": true
  }
}
```

## Create an Azure KeyVault

From the Azure Portal (portal.azure.com), click the **Create a new resource** button in the top left corner. Type **Key Vault**, and create a new one. 

Once it created, open it to add a policy and let the App Registration have access to the secrets. Click the Add Policies button,

![AddPolicy.png][AddPolicy.png]

Give Secret Permissions Read and Get to the App Registration created previously.

![EditKeyVault][EditKeyVault]

Don't forget to save.

## Create a Secret

We need to create two secrets to save the Azure Function URL and the security code/ token.

![CreateSecrets][CreateSecrets]

[more to come... ]



# Contributing

If you find a bug or would like to add a feature, check out those resources:

To see the current work in progress: [GLO boards](https://app.gitkraken.com/glo/board/XtpDU2ZLuQARV8y7) 'kanban board'



[CreateBackend]: medias/CreateBackend.png
[newRegistration]: medias/newRegistration.png
[AddPolicy]: medias/AddPolicy.png
[EditKeyVault]: medias/EditKeyVault.png
[CreateSecrets]: medias/CreateSecrets.png

