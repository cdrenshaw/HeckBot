using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace HeckBot.Models
{
    public class ShieldTimer : IDisposable
    {
        public delegate void TimerTickedEventHandler(ShieldTimer shieldTimer);
        public event TimerTickedEventHandler TimerTicked;

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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerTicked?.Invoke(this);
        }

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
