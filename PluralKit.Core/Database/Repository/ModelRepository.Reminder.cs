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

        public IAsyncEnumerable<PKReminder> GetUnseenReminders(IPKConnection conn, PKMember member) 
        {
            return conn.QueryStreamAsync<PKReminder>(@"
UPDATE reminders
SET seen = true
WHERE receiver = @Id AND seen = false
RETURNING *"
            , member);
        }

        public IAsyncEnumerable<PKReminder> GetReminders(IPKConnection conn, PKMember member)
        {
            return conn.QueryStreamAsync<PKReminder>(@"
UPDATE reminders
SET seen = true
WHERE receiver = @Id
RETURNING *"
            , member);
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
