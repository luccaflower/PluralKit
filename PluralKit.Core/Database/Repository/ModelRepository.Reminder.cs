using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluralKit.Core
{
    //TODO: Method to mark reminders as unseen
    public partial class ModelRepository
    {
        public async Task AddReminder(IPKConnection conn, PKReminder reminder) 
        {
            await conn.ExecuteAsync("insert into reminders(mid, channel, guild, receiver, seen) values (@Mid, @Channel, @Guild, @Receiver, @Seen)", reminder);
            _logger.Debug("Added reminder for {@Receiver}", reminder.Receiver);
        }

    }

    public class PKReminder {
        public ulong Mid { get; set; }
        public ulong Channel { get; set; }
        public ulong Guild { get; set; }
        public MemberId Receiver { get; set; }
        public bool Seen { get; set; }
    }
}
