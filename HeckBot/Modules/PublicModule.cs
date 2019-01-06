using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
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
            var msg = $@"Below is a list of things I can do:
                @HeckBot quests - Returns the current and upcoming quests
                @HeckBot shield <time> - Starts a shield timer for <time> hours
                @HeckBot shield stop - Stops all shield timers";
            await ReplyAsync(msg);
        }
    }
}
