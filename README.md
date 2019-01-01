# HeckBot
A sharded Discord bot that reports current and upcoming quests for the mobile game, Kingdoms of Heckfire.

Add it to your own server by clicking here: https://discordapp.com/api/oauth2/authorize?client_id=529505070941995009&scope=bot&permissions=35840

Note that permissions=35840 is the minimum required for the bot to work since it sends the quests as a message with a .png attachment.

<img src="https://www.nerdarray.net/Images/HeckBot/screenshot.png" alt="sample application output" />

To run from source, you'll need a Discord bot token.

1) Register a new bot here: https://discordapp.com/developers/applications/  <br/>Give it a name and make note of the Client ID.  Save changes.

2) With your app selected, go to Bot settings and click Add Bot.  Make note of the token.

3) Create a new user environment variable called HECKBOT_TOKEN and paste the bot token from step 2 as the value.  Restart your computer.

4) Navigate to https://discordapp.com/api/oauth2/authorize?client_id=your_client_id&scope=bot&permissions=35840  <br/>Replace the value with your Client ID from step 1.

5) Compile and run the bot.  @yourbotname quests to see upcoming quests.


Quest timers calculated using https://github.com/christirichards algorithm.
