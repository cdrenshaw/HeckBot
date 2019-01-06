using HeckBot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace HeckBot.Services
{
    public class DbService
    {
        private readonly HeckBotContext _context;

        public DbService(HeckBotContext context)
        {
            _context = context;
        }

        public async Task<bool> SaveShield(ShieldTimer timer)
        {
            try
            {
                _context.ShieldTimers.Add(new ShieldDetails()
                {
                    UserId = timer.User.Id,
                    ChannelId = timer.Channel.Id,
                    ShieldEndTime = timer.ShieldEndTime,
                    NextNotificationTime = timer.NextNotificationTime
                });
                await _context.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task UpdateShield(int id, DateTime nextNotificationTime)
        {
            var timer = _context.ShieldTimers.Where(t => t.Id == id).FirstOrDefault();
            if (timer != null)
            {
                timer.NextNotificationTime = nextNotificationTime;
                await _context.SaveChangesAsync();
            }            
        }

        public async Task DeleteShield(int id)
        {
            var shield = _context.ShieldTimers.Where(s => s.Id == id).FirstOrDefault();
            if (shield != null)
            {
                _context.ShieldTimers.Remove(shield);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ShieldDetails>> GetSavedShields()
        {
            List<ShieldDetails> shields = new List<ShieldDetails>();

            try
            {
                await Task.Run(() => shields = (from s in _context.ShieldTimers select s).ToList());
            }
            catch
            {
                return null;
            }

            if (shields.Count > 0)
                return shields;
            else
                return null;
        }
    }
}
