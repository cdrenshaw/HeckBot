using Discord.WebSocket;
using System;
using System.Timers;

namespace HeckBot.Models
{
    public class ShieldTimer : IDisposable
    {
        public delegate void TimerTickedEventHandler(ShieldTimer shieldTimer);
        public event TimerTickedEventHandler TimerTicked;

        /// <summary>
        /// Creates a new shield timer
        /// </summary>
        /// <param name="user">The user who initiated the shield.</param>
        /// <param name="channel">The channel that the request originated from.</param>
        /// <param name="hours">The number of hours until the shield ends.</param>
        public ShieldTimer(SocketUser user, ISocketMessageChannel channel, int hours)
        {
            User = user;
            Channel = channel;

            ShieldEndTime = DateTime.Now.AddHours(hours);

            // the first notification is 3 hours before the shield expires.
            NextNotificationTime = ShieldEndTime.AddHours(-3);

            Timer = new Timer((NextNotificationTime - DateTime.Now).TotalMilliseconds);
            Timer.Elapsed += Timer_Elapsed;
            Timer.AutoReset = true;
            Timer.Start();
        }

        /// <summary>
        /// Creates a new shield timer and matches it to an existing shield timer in the database.
        /// </summary>
        /// <param name="user">The user who initiated the shield.</param>
        /// <param name="channel">The channel that the request originated from.</param>
        /// <param name="shieldEndTime">The predefined end time of the shield</param>
        /// <param name="nextNotificationTime">The time that the next shield notification is due.</param>
        /// <param name="dbId">The ID of the shield in the database.</param>
        public ShieldTimer(SocketUser user, ISocketMessageChannel channel, DateTime shieldEndTime, DateTime nextNotificationTime, int dbId)
        {
            User = user;
            Channel = channel;
            ShieldEndTime = shieldEndTime;
            NextNotificationTime = nextNotificationTime;
            DbId = dbId;
            
            Timer = new Timer((NextNotificationTime - DateTime.Now).TotalMilliseconds);
            Timer.Elapsed += Timer_Elapsed;
            Timer.AutoReset = true;
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerTicked?.Invoke(this);
        }

        // Track the ID of this record in the database.
        public int DbId { get; set; }

        public SocketUser User { get; private set; }

        public ISocketMessageChannel Channel { get; private set; }

        public DateTime ShieldEndTime { get; private set; }

        public DateTime NextNotificationTime { get; set; }

        public Timer Timer { get; set; }

        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}
