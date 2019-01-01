using Discord.Commands;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HeckBot.Modules
{
    public class HeckModule : ModuleBase<ShardedCommandContext>
    {
        [Command("quests")]
        public async Task QuestsAsync()
        {
            var one = new DateTime(2018, 02, 13, 06, 05, 00, DateTimeKind.Utc).ToLocalTime();
            var ct = DateTime.Now.ToLocalTime();
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

                quests.Dispose();
            }
        }

        private MagickImage CreateCard(DateTime ct, string q, double m, double s)
        {
            MagickImage image = new MagickImage(new MagickColor(QuestBgColor(q)), 365, 100);

            if (m <= 0)
            {
                var mFull = m + 55;
                var min = Math.Floor(mFull);

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
                if (m > 60)
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
                else
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
    }
}
