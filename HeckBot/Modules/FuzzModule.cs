using Discord.Commands;
using HeckBot.Services;
using System.IO;
using System.Threading.Tasks;

namespace HeckBot.Modules
{
    public class FuzzModule : ModuleBase<ShardedCommandContext>
    {
        private readonly PictureService _picService;

        public FuzzModule(PictureService picService)
        {
            _picService = picService;
        }

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await _picService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
            // Clean up after ourselves.
            stream.Close();
        }
    }
}
