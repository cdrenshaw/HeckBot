using Discord;
using Discord.WebSocket;
using HeckBot.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeckBot.Services
{
    public class ShieldService
    {
        private readonly DiscordShardedClient _client;
        private readonly DbService _db;
        private readonly IServiceProvider _services;

        private List<ShieldTimer> _shieldTimers;

        public ShieldService(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordShardedClient>();
            _db = services.GetRequiredService<DbService>();
            _services = services;

            _shieldTimers = new List<ShieldTimer>();

            RestoreSavedShields();
        }

        private async Task RestoreSavedShields()
        {
            var shields = await _db.GetSavedShields();

            if (shields != null)
            {
                foreach (var s in shields)
                {
                    var channel = _client.GetChannel(s.ChannelId);
                    var user = _client.GetUser(s.UserId);
                    
                    // Check if the shield expired
                    if (s.ShieldEndTime < DateTime.Now)
                    {
                        try
                        {
                            // DM the user an update on their shield.
                            await user.SendMessageAsync("Your shield has expired!");
                        }
                        catch
                        {
                            // Something went wrong.  Send the message to the channel the timer request originated from.
                            await ((ISocketMessageChannel)channel).SendMessageAsync(user.Username + "'s shield expired, but something is preventing us from sending them a DM to let them know.");
                        }

                        // clean up this timer.
                        await _db.DeleteShield(s.Id);

                        continue;
                    }

                    ShieldTimer timer;

                    if (s.NextNotificationTime < DateTime.Now)
                    {
                        timer = new ShieldTimer(user, (ISocketMessageChannel)channel, s.ShieldEndTime, DateTime.Now.Add(new TimeSpan(0, 1, 0)), s.Id);
                        await StartShieldTimer(timer, false);

                        continue;
                    }

                    timer = new ShieldTimer(user, (ISocketMessageChannel)channel, s.ShieldEndTime, s.NextNotificationTime, s.Id);
                    await StartShieldTimer(timer, false);
                }
            }
        }

        public async Task<bool> StartShieldTimer(ShieldTimer timer, bool saveTimer)
        {
            bool success = true;

            if (saveTimer)
                success = await _db.SaveShield(timer);

            if (success)
            {
                timer.TimerTicked += Timer_TimerTicked;
                _shieldTimers.Add(timer);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> StopShieldTimers(SocketUser user)
        {
            var timers = _shieldTimers.Where(t => t.User.Id == user.Id).ToList();
            if (timers.Count > 0)
            {
                foreach (var timer in timers)
                {
                    timer.Timer.Stop();
                    _shieldTimers.Remove(timer);
                    await _db.DeleteShield(timer.DbId);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private async void Timer_TimerTicked(ShieldTimer shieldTimer)
        {
            // stop the timer while we're working.
            shieldTimer.Timer.Stop();
            
            // calculate how much time is left until the shield expires.
            var timeLeft = shieldTimer.ShieldEndTime - DateTime.Now;

            // check if the shield has expired.
            if (timeLeft.TotalSeconds <= 0)
            {
                try
                {
                    // DM the user to let them know their shield has expired.
                    await shieldTimer.User.SendMessageAsync("Your shield has expired!");
                }
                catch
                {
                    // Something went wrong.  Send the message to the channel the timer request originated from.
                    await shieldTimer.Channel.SendMessageAsync(shieldTimer.User.Username + "'s shield expired, but something is preventing us from sending them a DM to let them know.");
                }

                // clean up this timer.
                shieldTimer.Timer.Stop();
                shieldTimer.Timer.Dispose();
                _shieldTimers.Remove(shieldTimer);
                await _db.DeleteShield(shieldTimer.DbId);
                shieldTimer.Dispose();
                
                return;
            }

            // track the time until the next notification
            TimeSpan timeUntilNotification;

            // more than an hour until the shield expires.
            if (timeLeft.TotalMinutes > 60) 
            {
                // notify again in an hour
                shieldTimer.NextNotificationTime = shieldTimer.NextNotificationTime.AddHours(1);
                // set the timer interval.
                timeUntilNotification = shieldTimer.NextNotificationTime - DateTime.Now;
                shieldTimer.Timer.Interval = timeUntilNotification.TotalMilliseconds;
                // start the timer.
                shieldTimer.Timer.Start();

                await _db.UpdateShield(shieldTimer.DbId, shieldTimer.NextNotificationTime);

                try
                {
                    // DM the user an update on their shield.
                    await shieldTimer.User.SendMessageAsync("Your shield expires in " + timeLeft.Hours + " hours " + timeLeft.Minutes + " minutes.");
                }
                catch
                {
                    // Something went wrong.  Send the message to the channel the timer request originated from.
                    await shieldTimer.Channel.SendMessageAsync(shieldTimer.User.Username + "'s shield expires in " + timeLeft.Hours + " hours " + timeLeft.Minutes + " minutes, but something is preventing us from sending them a DM to let them know.");
                }

                return;
            }

            // More than 30 minutes until the shield expires.
            if (timeLeft.TotalMinutes > 30)
            {
                shieldTimer.NextNotificationTime = shieldTimer.NextNotificationTime.AddMinutes(30);
            }

            // More than 15 minutes until the shield expires.
            if (timeLeft.TotalMinutes > 15)
            {
                shieldTimer.NextNotificationTime = shieldTimer.NextNotificationTime.AddMinutes(15);
            }

            // More than 5 minutes until the shield expires.
            if (timeLeft.TotalMinutes > 5)
            {
                shieldTimer.NextNotificationTime = shieldTimer.NextNotificationTime.AddMinutes(5);
            }
            else // Less than 5 minutes until the shield expires.
            {
                // Check if there's still time before the shield expires.
                if (shieldTimer.ShieldEndTime > DateTime.Now)
                {
                    shieldTimer.NextNotificationTime = shieldTimer.ShieldEndTime;
                }
                else // Shield has expired.
                {
                    try
                    {
                        // DM the user an update on their shield.
                        await shieldTimer.User.SendMessageAsync("Your shield has expired!");
                    }
                    catch
                    {
                        // Something went wrong.  Send the message to the channel the timer request originated from.
                        await shieldTimer.Channel.SendMessageAsync(shieldTimer.User.Username + "'s shield expired, but something is preventing us from sending them a DM to let them know.");
                    }

                    // clean up this timer.
                    shieldTimer.Timer.Stop();
                    shieldTimer.Timer.Dispose();
                    _shieldTimers.Remove(shieldTimer);
                    await _db.DeleteShield(shieldTimer.DbId);
                    shieldTimer.Dispose();

                    return;
                }
            }

            // set the timer interval.
            timeUntilNotification = shieldTimer.NextNotificationTime - DateTime.Now;
            shieldTimer.Timer.Interval = timeUntilNotification.TotalMilliseconds;
            // start the timer.
            shieldTimer.Timer.Start();

            await _db.UpdateShield(shieldTimer.DbId, shieldTimer.NextNotificationTime);

            try
            {
                // DM the user an update on their shield.
                await shieldTimer.User.SendMessageAsync("Your shield expires in " + timeLeft.Minutes + " minutes.");
            }
            catch
            {
                // Something went wrong.  Send the message to the channel the timer request originated from.
                await shieldTimer.Channel.SendMessageAsync(shieldTimer.User.Username + "'s shield expires in " + timeLeft.Minutes + " minutes, but something is preventing us from sending them a DM to let them know.");
            }
        }
    }
}
