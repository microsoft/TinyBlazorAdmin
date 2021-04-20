# Deployment

## 1- First thing first: Copy the repository

### 👉 **Copy this repository** into your own account

> To Copy a GitHub repository click on the use this template button on the top right of the screen.

![Click on the button Use this template][CreateACopy]

Provide a name. It can be anything, it just need to be unique in your account (you can keep TinyBlazorAdmin if you want). Add a description it you want, and click the button **Create repository from template**. 

![Give it a name description and click Create][NameYourCopy]

After a few seconds, you should now be in your version of the TinyBlazorAdmin project.

> Make sure you are currently in YOUR GitHub repository.
>
>![This should be YOUR repo][NotFBoucherRepo]

## 2- Deploy AzUrlShortener (the backend)

This project is a frontend only so you will need to deploy the [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener) in "headless mode". Do to it, click the blue button below and make sure to select **none** as Frontend

[![Deploy Backend to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/?WT.mc_id=urlshortener-github-frbouche#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUrlShortener%2Fmain%2Fdeployment%2FazureDeploy.json)

![CreateBackend][CreateBackend]


## 3- Deploy TinyBlazorAdmin (the frontend)

There are many ways you could run Tiny Blazor Admin website. In this deployment, we will use the new [Azure Static Web App (SWA)](https://azure.microsoft.com/en-ca/services/app-service/static/?WT.mc_id=tinyblazoradmin-github-frbouche). However, because the TinyBlazorAdmin use Azure Active Directory (AAD), we need a standalone Azure Function (deployed at the previous step).

Open Azure portal (portal.azure.com), open the **resource group** where you created the backend (ex: streamDemo is our case). Click the "**+**" and search **Static Web App**, and click the *Create* button. 

![Creating swa][swa_create1]

> Note: You will need to **Authorize Azure Static Web Apps**, to have access to _your_ GitHub repository (the one created when you forked the project). This is required because SWA uses GitHub Action to deploy.

![Creating swa part 2][swa_create2]

Select your organization, repository and branch (ex: main).

![Creating swa part 3][swa_create3]

Select **Blazor** as your *Build Presets*. The *App location* needs to be the location of the project file; in our case `src/TinyBlazorAdmin/`. The *App artifact location* can be left to wwwroot. 

>Note: We don't need the Api location, because AzURLShortener is deployed in a full standalone Azure Function.

Once it's all filled, click the Review, and create button. It will takes a few minutes to get deployed. During this time let's create and configure our security components.

## 4- Create Azure Active Directory (AAD) Components

### Create AAD App for the Fontend

We need a Service Principal that we will use to authenticate our user to Azure Active Directory (AAD). To achieve this we will create an application registration in Azure.

From the Azure Portal (portal.azure.com), open the **Azure Active Directory** page. From the left option menu select **App registrations**, then create a new registration. Use a name that will help you to remember that it's for the TinyBlazorAdmin website (ex: TinyAdminApp) (1)

![Create a new registration][newRegistration]

For the Redirect URL use **Single-page application (SPA)** (3) and enter the URL of the Azure Static WebApp deployed previously and add `/authentication/login-callback`. It should look lsomething like this:

```
https://bolly-tiger-04a15beef.azurestaticapps.net/authentication/login-callback

```

**Note the ClientID and TenantID.**

You will need to retrieve the ClientID and TenantID, they will be display at the top of the page once you select an app in the portal.

Go back in the Authentication and in the section Implicit grant check the checkbox `Access Token` and `ID Tokens`

![tokensaccess][tokensaccess]

### Create App for the Azure Function

We need a second App registration, this time to let the Azure Function validate that user information contained in the token is valid.

![First steps to create the AD App registration][azFunction_Auth_step1]

From the Azure Portal, go to your Azure Function. From the left panel select the *Authentication / Authorization* (1) option. Enable the *App Service Authentication* (2) and click on *Azure Active Directory*.

![Configuring the App registration][azFunction_Auth_step2]

1. Select Express.
2. Make sure *Create New AD App* is selected.
3. Give the AD App a name.
4. Click Ok.
5. DON'T FORGET TO CLICK THE SAVE BUTTON

Now, we need to configure the brand-new Ad App registration. Still from the Azure portal open the *Active Directory* blade. Select the *App registration* option from the left menu. Then select the application you just created.

![ConfigAzFuncADapp][ConfigAzFuncADapp]

1. From the left panel click the *Expose an API* option.
2. Click the *Add a client application*.
3. Enter the ClientID of the Frontend App (the first one created).
4. Check the Impersonation checkbox.
5. Save by clicking *Add application*.



## Configure Backend and Frontend to Work Together

Now in your GitHub it's time to update the settings. The code needs to know the AD app to use and the Azure Function to call. Update those values inside `TinyBlazorAdmin\wwwroot\appsettings.json`

> The **Endpoint** _must_ ends with a `/`

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxxxx",
    "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxxxx",
    "ValidateAuthority": true
  },
  "UrlShortenerSecuredService": {
    "Endpoint": "https://__azFunction_URL__.azurewebsites.net/"
  }
}
```

## Enabeling Cross-Origin Resource Sharing (CORS)


First we need the url of the caller (aka TinyBlazorAdmin). From the Azure Portal, open the TinyBlazorAdmin SWA blade. Note the URL display in the top right of the page this is the URL of your admin page.

![swa_URL][swa_URL]

Now we need to add this URL to the list in the CORS of the Azure Function that run AzUrlShortener. From the Azure portal open the blade of AzUrlShortener. From the left menu search CORS, and click it.  Add the URL of the SWA and don't forget to save.


![azFunction_CORS][azFunction_CORS]

## Try it!

Voila, the deployment is now completed. You can test it by creating a new short URL using the admin SWA. 


## Adding Custom Domain

To add a custom domain to your AzUrlShortener & TinyBlazorAdmin, [follow these steps](https://github.com/FBoucher/AzUrlShortener/blob/main/doc/post-deployment-configuration.md#add-a-custom-domain) from the the AzUrlShortener repo.


[CreateBackend]: medias/CreateBackend.png
[newRegistration]: medias/newRegistration.png
[AddPolicy]: medias/AddPolicy.png
[EditKeyVault]: medias/EditKeyVault.png
[CreateSecrets]: medias/CreateSecrets.png
[azFunction_Auth_step1]: medias/azFunction_Auth_step1.png
[azFunction_Auth_step2]: medias/azFunction_Auth_step2.png
[ConfigAzFuncADapp]: medias/ConfigAzFuncADapp.png
[tokensaccess]: medias/tokensaccess.png
[swa_create1]: medias/swa_create1.png
[swa_create2]: medias/swa_create2.png
[swa_create3]: medias/swa_create3.png
[swa_URL]: medias/swa_URL.png
[azFunction_CORS]: medias/azFunction_CORS.png
[RegisterClientApp]: medias/RegisterClientApp.png
[NotFBoucherRepo]: medias/NotFBoucherRepo.png
[CreateACopy]: medias/CreateACopy.png
[NameYourCopy]: medias/NameYourCopy.png
