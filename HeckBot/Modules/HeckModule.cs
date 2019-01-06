using Discord;
using Discord.Commands;
using HeckBot.Models;
using HeckBot.Services;
using ImageMagick;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HeckBot.Modules
{
    public class HeckModule : ModuleBase<ShardedCommandContext>
    {
        private readonly ShieldService _shieldService;

        public HeckModule(ShieldService shieldService)
        {
            _shieldService = shieldService;
        }

        #region Quests Command Related

        [Command("quests")]
        public async Task QuestsAsync()
        {
            // First quest.
            var one = new DateTime(2018, 02, 13, 06, 05, 00, DateTimeKind.Utc).ToLocalTime();
            // Current time.
            var ct = DateTime.Now.ToLocalTime();
            // Hours between first quest and now.
            var interval = (ct - one).TotalHours;
            var i = Math.Ceiling(interval - (11 * (Math.Floor(interval / 11))));
            var t2 = (Math.Ceiling(interval) - interval) * 60;
            var s = Math.Round((t2 - Math.Floor(t2)) * 60);
            t2 = Math.Floor(t2);

            using (MagickImageCollection images = new MagickImageCollection())
            {
                for (var j = 0; j < 11; j++)
                {
                    var k = i + j;
                    if (k > 11)
                    {
                        k = k - 11;
                    }
                    var q = FindQuest(k);
                    double m;
                    if (k == 0)
                    {
                        m = 0;
                    }
                    else
                    {
                        m = t2 + (60 * (j - 1));
                    }

                    images.Add(CreateCard(ct, q, m, s));
                }

                var quests = images.AppendVertically();

                var stream = new MemoryStream();
                quests.Write(stream, MagickFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "quests.png");
                // Clean up after ourselves
                stream.Close();
                quests.Dispose();
            }
        }

        private MagickImage CreateCard(DateTime ct, string q, double m, double s)
        {
            MagickImage image = new MagickImage(new MagickColor(QuestBgColor(q)), 365, 100);

            if (m <= 0) // this is the current quest.
            {
                var mFull = m + 55;
                var min = Math.Floor(mFull);  // how many minutes left on the quest.

                new Drawables()
                    .FontPointSize(36)
                    .StrokeColor(new MagickColor("white"))
                    .FillColor(new MagickColor("white"))
                    .TextAlignment(TextAlignment.Left)
                    .Text(20, 45, q).Draw(image);

                new Drawables()
                    .FontPointSize(22)
                    .StrokeColor(new MagickColor("white"))
                    .FillColor(new MagickColor("white"))
                    .TextAlignment(TextAlignment.Left)
                    .Text(20, 80, "Ends in " + min + " minutes").Draw(image);
            }
            else
            {
                if (m > 60)  // more than an hour until quest starts.
                {
                    var hFull = m / 60;
                    var h = Math.Floor(hFull);
                    m = Math.Round((hFull - h) * 60);
                    new Drawables()
                        .FontPointSize(36)
                        .StrokeColor(new MagickColor("white"))
                        .FillColor(new MagickColor("white"))
                        .TextAlignment(TextAlignment.Left)
                        .Text(20, 45, q).Draw(image); // quest name

                    new Drawables()
                        .FontPointSize(22)
                        .StrokeColor(new MagickColor("white"))
                        .FillColor(new MagickColor("white"))
                        .TextAlignment(TextAlignment.Left)
                        .Text(20, 80, "Starts in " + h + " hours " + m + " minutes").Draw(image);
                }
                else // less than an hour until quest starts.
                {
                    new Drawables()
                        .FontPointSize(36)
                        .StrokeColor(new MagickColor("white"))
                        .FillColor(new MagickColor("white"))
                        .TextAlignment(TextAlignment.Left)
                        .Text(20, 45, q).Draw(image); // quest name


                    new Drawables()
                        .FontPointSize(22)
                        .StrokeColor(new MagickColor("white"))
                        .FillColor(new MagickColor("white"))
                        .TextAlignment(TextAlignment.Left)
                        .Text(20, 80, "Starts in " + m + " minutes").Draw(image);
                }
            }

            return image;
        }

        private string QuestBgColor(string q)
        {
            string c;
            switch (q)
            {
                case "Troop Training":
                    c = "#6041ec"; // purple
                    break;
                case "Monster Slaying":
                    c = "#f60829"; // red
                    break;
                case "Resource Gathering":
                    c = "#08f61e"; // green
                    break;
                case "Research":
                    c = "#288ee6"; // blue
                    break;
                case "Might Growth":
                    c = "#e6a428"; // orange
                    break;
                case "Construction":
                    c = "#926c52"; // brown
                    break;
                default:
                    c = "#ffffff"; // white
                    break;
            }

            return c;
        }

        private string FindQuest(double i)
        {
            string q;
            switch (i)
            {
                case 1:
                    q = "Troop Training";
                    break;
                case 2:
                    q = "Monster Slaying";
                    break;
                case 3:
                    q = "Resource Gathering";
                    break;
                case 4:
                    q = "Research";
                    break;
                case 5:
                    q = "Troop Training";
                    break;
                case 6:
                    q = "Monster Slaying";
                    break;
                case 7:
                    q = "Might Growth";
                    break;
                case 8:
                    q = "Resource Gathering";
                    break;
                case 9:
                    q = "Troop Training";
                    break;
                case 10:
                    q = "Monster Slaying";
                    break;
                case 11:
                    q = "Construction";
                    break;
                default:
                    q = "ERROR";
                    break;
            }
            return q;
        }

        #endregion // Quests Command Related


        #region Shield Command Related

        [Command("shield")]
        public async Task ShieldTimer(string time = null)
        {
            // the user didn't send any input so give them instructions.
            if (time == null)
            {
                await ReplyAsync("You need to tell me how many hours to set the timer for.  Valid values are 4, 8, 12, 24, 72, and 168.  For instance: @HeckBot shield 24");
                return;
            }

            if (time.ToUpper() == "STOP" || time.ToUpper() == "CANCEL")
            {
                var success = await _shieldService.StopShieldTimers(Context.User);
                if (success)
                {
                    await ReplyAsync("I've cancelled all of your running shield timers, " + Context.User.Username + ".");
                }
                else
                {
                    await ReplyAsync("I'm sorry, but I didn't find any shield timers for you, " + Context.User.Username + ".");
                }

                return;
            }

            // validate the input
            int hours = 0;
            try
            {
                // try to convert the value to an integer.
                hours = Convert.ToInt32(time);
            }
            catch
            {
                await ReplyAsync("Please enter a value in hours between 4 and 168.  Valid shield timers are 4, 8, 12, 24, 72, or 168 hours long.");
                return;
            }

            // max shield time is 7 days
            if (hours > 168)
            {
                await ReplyAsync("You can only shield for a max of 7 days or 168 hours.  Valid shield timers are 4, 8, 12, 24, 72, or 168 hours long.");
                return;
            }

            // min shield time is 4 hours
            if (hours < 4)
            {
                await ReplyAsync("You can't shield for less than 4 hours.  Valid shield timers are 4, 8, 12, 24, 72, or 168 hours long.");
                return;
            }

            // user supplied a value that doesn't equate to a shield in Heckfire.
            if (hours != 4 && hours != 8 && hours != 12 && hours != 24 && hours != 72 && hours != 168)
            {
                await ReplyAsync("You can't shield for " + hours + " hours.  Valid shield timers are 4, 8, 12, 24, 72, or 168 hours long.");
                return;
            }

            // Create a new timer and start it.
            ShieldTimer timer = new ShieldTimer(Context.User, Context.Channel, hours);
            await _shieldService.StartShieldTimer(timer, true);

            // Let the user know.
            await ReplyAsync("I've started a " + hours + " hour shield timer for you, " + Context.User.Username + ".  Make sure you have 'Allow direct messages from server members' enabled so that I can send you reminders about your shield.");
            // DM them too.
            await Context.User.SendMessageAsync("I've started your " + hours + " hour shield timer.  I'll DM you with increasing frequency beginning 3 hours before your shield expires.");
        }

        #endregion  // Shield Command Related
    }
}
