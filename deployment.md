# Deployment

 Until an automatic deployment is created here is the steps to deploy the TinyBlazorAdmin app into Azure. You can run it somewhere else and even locally.

## Create the Backend

This project is a frontend only so you will need to deploy the [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener) in "headless mode". Do to it click the blue button below and make sure to select **none** as Frontend

[![Deploy Backend to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/?WT.mc_id=urlshortener-github-frbouche#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUrlShortener%2Fmaster%2Fdeployment%2FazureDeploy.json)

![CreateBackend][CreateBackend]

## Create Azure Active Directory (AAD) Components

### Create App for the Fontend

We need a Service Principal that we will use to authenticate our user to Azure Active Directory (AAD). To accheive this we will create a application registration in Azure.

From the Azure Portal (portal.azure.com), open the **Azure Active Directory** page. From the left option menu select **App registrations**, then create a new registration. Use a name that will helps you to remember that it's for the TinyBlazorAdmin website (ex: 
TinyAdminApp)

**Note the ClientID and TenantID.**

If you need to retreive the ClientID and TenantID, they will be diplay at the top of the page once you select an app in the portal.

![Create a new registration][newRegistration]


### Create App for the Azure Function

We need a second App registration, this time to let the Azure Function validate that user information contain into the token is valid.

![First steps to create the AD App registration][azFunction_Auth_step1]

From the Azure Portal, go to your Azure Function. From the left panel select the *Authentication / Authorization* (1) option. Enable the *App Service Authentication* (2) and click on *Azure Active Directory*.

![Configuring the App registration][azFunction_Auth_step2]

1. Select Express.
2. Make sure *Create New AD App* is selected.
3. Give the AD App a name.
4. Click Ok.
5. DON"T FORGET TO CLICK THE SAVE BUTTON

Now, we need to configure the brand new Ad App registration. Still from the Azure portal open the *Active Directory* blade. Select the *App registration* option from the left menu. Then select the application you just created.

![ConfigAzFuncADapp][ConfigAzFuncADapp]

1. From the left panel click the *Expose an API* option.
2. Click the *Add a client application*.
3. Enter the ClientID of the Frontend App (the first one created).
4. Check the Impersonation checkbox.
5. Save by clicking *Add application*.


## Deploy TinyBlazorAdmin

(more details to come but here are the steps)

- create a storage account 
- set it to static website
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





[CreateBackend]: medias/CreateBackend.png
[newRegistration]: medias/newRegistration.png
[AddPolicy]: medias/AddPolicy.png
[EditKeyVault]: medias/EditKeyVault.png
[CreateSecrets]: medias/CreateSecrets.png
[azFunction_Auth_step1]: medias/azFunction_Auth_step1.png
[azFunction_Auth_step2]: medias/azFunction_Auth_step2.png
[ConfigAzFuncADapp]: medias/ConfigAzFuncADapp.png

