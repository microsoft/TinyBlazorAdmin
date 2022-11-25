# Deployment

Until a "full automatic" deployment is created, here are all the steps to deploy the TinyBlazorAdmin app into Azure. You can run it somewhere else and even locally.

## First thing first

You need to **fork this repo** into your own account. You will need to update the configuration (this document will explain when and what), therefore it needs to be yours.

To fork a GitHub repository click on the fork button on the top right of the screen. If you need more detail have a look to this GitHub doc: [Fork a repo
](https://docs.github.com/en/free-pro-team@latest/github/getting-started-with-github/fork-a-repo).

## Deploy AzUrlShortener (the Backend)

This project is a frontend for [Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener). If it's not already done you will need to deploy this project in your Azure subscription. 

## Deploy TinyBlazorAdmin (the Frontend)

There are many ways you could run Tiny Blazor Admin website. In this deployment, we will use the new [Azure Static Web App (SWA)](https://azure.microsoft.com/en-ca/services/app-service/static/?WT.mc_id=tinyblazoradmin-github-frbouche). 

Open Azure portal (portal.azure.com), open the **resource group** where you created the backend (ex: streamDemo is our case). Click the "**+**" and search **Static Web App**, and click the *Create* button. 

![Creating swa][swa_create1]

> Note: You will need to **Authorize Azure Static Web Apps**, to have access to _your_ GitHub repository (the one created when you forked the project). This is required because SWA uses GitHub Action to deploy.

![Creating swa part 2][swa_create2]

Select your organization, repository and branch (ex: main).

![Creating swa part 3][swa_create3]

Select **Blazor** as your *Build Presets*. The *App location* needs to be the location of the project file; in our case `src/admin`.  The Api is in the `src/api`. The *App artifact location* can be left to wwwroot. 

Once it's all filled, click the Review, and create button. It will takes a few minutes to get deployed. During this time let's create and configure our security components.

## Create Invite to add users to the Admin role

Users need to be part of the of the role **admin** (all lower case). To add them you need to use the *Role management* interface from in the Azure portal.

From the [portal](https://portal.azure.com/), open your static web app and select the *Role management* from the left options list. From there click the Invive button make sure you type **admin** all lower case without extra spaces in the Role field.

![Create invire][create_invire]

This will create an invite that you can share with the recipient. Once the invite is accepted, the name should be visible in the list.

## Connect the Data

Let's add the connection to the Azure Storage table. In your AzUrlShortener resource group us look for the Azure storage starting by "urldata" and grab the connectionstring.

![get connectionstring][grab_connstring]

Now that we have the connectionstring, add it the configuration of the Azure static web App under the name `UlsDataStorage`.

![add_config][add_config]

## Try it!

Voila, the deployment is now completed. You can test it by creating a new short URL using the admin SWA. 

[swa_create1]: medias/swa_create1.png
[swa_create2]: medias/swa_create2.png
[swa_create3]: medias/swa_create3.png
[swa_URL]: medias/swa_URL.png
[create_invire]: medias/create_invite.png
[add_config]: medias/add_config.png
[grab_connstring]: medias/grab_connstring.png
