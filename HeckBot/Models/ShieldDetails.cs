using System;

namespace HeckBot.Models
{
    public class ShieldDetails
    {
        public int Id { get; set; }

        public ulong UserId { get; set; }

        public ulong ChannelId { get; set; }

        public DateTime ShieldEndTime { get; set; }

        public DateTime NextNotificationTime { get; set; }
    }
}
