# Food Fite Door Game for Microsoft Teams

![.NET Core](https://github.com/cse-southeast-devcrew/microsoft-teams-bot-food-fite/workflows/.NET%20Core/badge.svg?branch=main)
![Continuous Deployment](https://github.com/cse-southeast-devcrew/microsoft-teams-bot-food-fite/workflows/Continuous%20Deployment/badge.svg?branch=main)

## Description

Reference video for [Food Fite](https://youtu.be/cpmsHO9JeCI)

## How to develop this Bot

1. Open VSCode
2. File > Open Workspace
3. Browse to and select `./microsoft-teams-bot-food-fite/FoodFite/FoodFite.code-workspace`
4. Opening the workspace should ask for you to `reopen in container`, do that.

### Including your SSH key for Git push

You'll basically want to add your private key to the SSH agent with `ssh-add /path/to/private_key`. [More info](https://code.visualstudio.com/docs/remote/containers#_using-ssh-keys)

## How to run locally

### VS Code

1. Click "Run" from the top menu then "Run without Debugging". This will prompt you to create a `launch.json`.
2. Click "View" from the top menu then "Run". This should open the debug pane.
3. Select from the drop down the configuration created in step 1, probably ".NET Core Launch (web)".

*Note: This will fail until environment variables are set up in the following step.

## How to setup environment variables

Create an `appsettings.json` file using the template:

```json
{
    "MicrosoftAppId": "",
    "MicrosoftAppPassword": "",
    "CosmosDbEndpoint": "",
    "CosmosDbAuthKey": "",
    "CosmosDbDatabaseId": "",
    "CosmosDbContainerId": ""
}
```

Add the environment variable `BOT_TYPE` with values `gamebot` or `fitebot`. This can be added in the `env` section if using a VS Code `launch.json` file in the `.vscode` folder.

```json
"env": {
    "BOT_TYPE": "fitebot"
}
```

*Note: These settings may also be stored as environment variables rather than in an `appsettings.json` file.

## Create Azure Resources

Create the following Azure resources:

0. Resource Group
1. [Azure Cosmos DB Account and container](https://docs.microsoft.com/en-us/azure/cosmos-db/create-cosmosdb-resources-portal)
2. [Azure Web App and Bot Service](https://docs.microsoft.com/en-us/azure/bot-service/abs-quickstart?view=azure-bot-service-4.0)

Update the values in the settings listed above using the resources just created.

## How to debug

- Install the [Bot Framework Emulator](https://github.com/Microsoft/BotFramework-Emulator/blob/master/README.md)
- Follow the guidance here: [Debug with the emulator](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-4.0&tabs=csharp)

After running the program locally, the url that the bot is running at will be outputted. Enter that with `/api/messages` at the end in the Bot Framework Emulator, probably `http://localhost:3978/api/messages`.

You can also grab the url from the Web App in Azure to test live.

*Note: You will need to open multiple chats to enable a fight.

## How to manually deploy to Azure Web App

1. Open the project folder where the `.csproj` file is located.
2. [Follow the directions to zip deploy](https://docs.microsoft.com/en-us/azure/app-service/deploy-zip).

## Setup environment variables in Azure

1. Open the Bot Web App in the Azure Portal
2. Open the Configuration pane
3. Insert all environment variables and their values

Another option is to use the [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/webapp/config/appsettings?view=azure-cli-latest#az_webapp_config_appsettings_set).

## How to configure continuous deployment

There is a GitHub Action workflow that enables continuous deployment to an Azure Web App. This file is at `.github/workflows/webappdeploy.yml`.

A service prinicpal needs to be created and stored in GitHub Secrets. Follow the [guidelines here](https://docs.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=userlevel#generate-deployment-credentials).

## How to access the Bot in Microsoft Teams

Follow the guidelines to [Connect a Bot to Microsoft Teams](https://docs.microsoft.com/en-us/azure/bot-service/channel-connect-teams?view=azure-bot-service-4.0#:~:text=To%20add%20the%20Microsoft%20Teams,Get%20bot%20embed%20code%20dialog.).
