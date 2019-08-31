using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord.Commands;

using PluralKit.Core;

namespace PluralKit.Bot.Commands
{
    [Group("group")]
    [Alias("g")]
    public class GroupCommands : ContextParameterModuleBase<PKGroup>
    {
        public SystemStore Systems { get; set; }
        public GroupStore Groups { get; set; }
        public EmbedService Embeds { get; set; }
        
        public override string Prefix => "group";
        public override string ContextNoun => "group";

        [Command("new")]
        [Alias("n", "add", "create", "register")]
        [Remarks("group new <name>")]
        [MustHaveSystem]
        public async Task NewMember([Remainder] string groupName)
        {
            // Hard name length cap
            if (groupName.Length > Limits.MaxGroupNameLength) throw Errors.MemberNameTooLongError(groupName.Length);
            
            // Warn if there's already a group by this name
            var existingGroup = await Groups.GetByName(Context.SenderSystem, groupName);
            if (existingGroup != null)
            {
                var msg = await Context.Channel.SendMessageAsync($"{Emojis.Warn} You already have a group in your system with the name \"{existingGroup.Name.Sanitize()}\" (with ID `{existingGroup.Hid}`). Do you want to create another group with the same name?");
                if (!await Context.PromptYesNo(msg)) throw new PKError("Group creation cancelled.");
            }
            
            // Create the group
            var group = await Groups.Create(Context.SenderSystem, groupName);
            
            // Send confirmation and space hint
            await Context.Channel.SendMessageAsync($"{Emojis.Success} Group \"{groupName.Sanitize()}\" (`{group.Hid}`) registered! See the user guide for commands for editing this group: https://pluralkit.me/guide#group-management");
            if (groupName.Contains(" ")) await Context.Channel.SendMessageAsync($"{Emojis.Note} Note that this group's name contains spaces. You will need to surround it with \"double quotes\" when using commands referring to it, or just use the group's 5-character ID (which is `{group.Hid}`).");
        }

        [Command()]
        [Alias("info", "show")]
        [Remarks("group <id>")]
        public async Task ViewGroup(PKGroup group)
        {
            var system = await Systems.GetById(group.System);
            await Context.Channel.SendMessageAsync(embed: await Embeds.CreateGroupEmbed(system, group));
        }

        public override async Task<PKGroup> ReadContextParameterAsync(string value)
        {
            var res = await new PKGroupTypeReader().ReadAsync(Context, value, _services);
            return res.IsSuccess ? res.BestMatch as PKGroup : null;        
        }
    }
}