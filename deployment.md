# Deployment

 Until an automatic deployment is created here is the steps to deploy the TinyBlazorAdmin app into Azure. You can run it somewhere else and even locally.

## Create the Backend

This project is a frontend only so you will need to deploy the [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener) in "headless mode". Do to it click the blue button below and make sure to select **none** as Frontend

[![Deploy Backend to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/?WT.mc_id=urlshortener-github-frbouche#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUrlShortener%2Fmaster%2Fdeployment%2FazureDeploy.json)

![CreateBackend][CreateBackend]

## Create Azure Active DIrectory (AAD) Components

From the Azure Portal (portal.azure.com), open the **Azure Active Directory** page. From the left option menu select **App registrations**, then create a new registration. **Note the ClientID and TenantID.**

![Create a new registration][newRegistration]



## Configure (AAD) Components



## Deploy TinyBlazorAdmin

- Copy artifact from /deployment folder.

## Configure Backend and Frontend to Work Together


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



[CreateBackend]: ../medias/CreateBackend.png
[newRegistration]: ../medias/newRegistration.png
[AddPolicy]: ../medias/AddPolicy.png
[EditKeyVault]: ../medias/EditKeyVault.png
[CreateSecrets]: ../medias/CreateSecrets.png