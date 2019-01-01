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
            var msg = $@"Hi {Context.User}! There are currently {Context.Client.Shards} shards!
                This guild is being served by shard number {Context.Client.GetShardFor(Context.Guild).ShardId}";
            await ReplyAsync(msg);
        }
    }
}
