using Discord.Commands;
using System.Threading.Tasks;

namespace HeckBot.Modules
{
    public class PublicModule : ModuleBase<ShardedCommandContext>
    {
        [Command("info")]
        public async Task InfoAsync()
        {
            var msg = $@"Hi {Context.User}! This guild is being served by HeckBot shard number {Context.Client.GetShardFor(Context.Guild).ShardId}";
            await ReplyAsync(msg);
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            var msg = "Below is a list of things I can do:\r\n" +
                "@HeckBot quests - Returns the current and upcoming quests\r\n" +
                "@HeckBot shield <time> - Starts a shield timer for <time> hours\r\n" +
                "@HeckBot shield stop - Stops all shield timers\r\n" +
                "@HeckBot info - Find out what shard you're connected to\r\n" +
                "@HeckBot cat - purrrr\r\n";
            await ReplyAsync(msg);
        }
    }
}
