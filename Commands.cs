using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ModMail
{
    public class Commands : ModuleBase<MMSocketCommandContext>
    {
        [Command("modmail")]
        public async Task ModMail([Remainder] string message)
        {
            IUser user = Context.Message.Author;
            List<string> links = new List<string>();

            foreach (var x in Context.Message.Attachments)
            {
                links.Add(x.Url);
            }
            
            var EmbedBuilder = new EmbedBuilder();
            EmbedBuilder.AddField("ModMail:", $"{user.Mention} - {message}")

            .WithAuthor(user)
            .WithColor(Color.DarkerGrey)
            .WithFooter(footer =>
            {
                footer
                .WithText($"\n{Context.Message.Author.Id}");
            }
            )
            .WithCurrentTimestamp();
            if (links.Count > 0) { EmbedBuilder.WithDescription("Attachments below:"); }
            
            Embed embed = EmbedBuilder.Build();

            ITextChannel Modchannel = Context.Client.GetChannel(912513440696905819) as ITextChannel;

            await Modchannel.SendMessageAsync(embed: embed);

            string Attachments = null;
            string Attachments2 = null;

            if (links.Count > 0 && links.Count < 5)
            {
                foreach (var z in links)
                {
                    Attachments += z + Environment.NewLine;
                }
                await Modchannel.SendMessageAsync(Attachments);
            }
            else if (links.Count > 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    Attachments += links[i] + Environment.NewLine;
                }
                for (int o = 0; o < links.Count - 5; o++)
                {
                    Attachments2 += links[o + 5] + Environment.NewLine;
                }
                await Modchannel.SendMessageAsync(Attachments);
                await Modchannel.SendMessageAsync(Attachments2);
            }
            await ReplyAsync("Message sent through modmail.");
        }
       
        [Command("dm"), Alias("message", "msg")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task DM(IUser userID, [Remainder] string input)
        {
            try
            {
                await UserExtensions.SendMessageAsync(userID, input);
            }
            catch (Discord.Net.HttpException x)
            {
                await ReplyAsync($"DM failed with the error message:\n`{x.Reason}`");
                return;
            }
            await ReplyAsync($"**Message sent to** `{userID.Username}#{userID.Discriminator}`");
        }
       
        [Command("dm-embed"), Alias("edm")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task EmbedDm(IUser userID, [Remainder] string input)
        {
            IUser Author = Context.Client.CurrentUser;
            var EmbedBuilder = new EmbedBuilder();
            EmbedBuilder.AddField("Mod Reply:", $"{input}")

            .WithAuthor(Author)
            .WithColor(Color.Blue)
            .WithFooter(footer =>
            {
                footer
                .WithText($"\nAespa Mod-Team");
            }
            )
            .WithCurrentTimestamp();

            Embed embed = EmbedBuilder.Build();
            try
            {
                await UserExtensions.SendMessageAsync(userID, embed: embed);
            }
            catch (Discord.Net.HttpException x)
            {
                await ReplyAsync($"DM failed with error message:\n`{x.Reason}`");
                return;
            }
            await ReplyAsync($"**Message sent to** `{userID.Username}#{userID.Discriminator}`");
        }

       
        [Command("recent"), Alias("dmr", "reply")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task DMRecent([Remainder] string input)
        {
            try
            {
                await UserExtensions.SendMessageAsync(Context.recUser, input);
            }
            catch (Discord.Net.HttpException x)
            {
                await ReplyAsync($"DM failed with the error message:\n`{x.Reason}`");
                await UserExtensions.SendMessageAsync(Context.Client.GetUser(218445513547186176), "An error has occurred with ModMail\n Message: " + x.Message + "\nDiscord Code: " + x.DiscordCode + "\nSource: " + x.Source);
                return;
            }
            await ReplyAsync($"**Message sent to** `{Context.recUser.Username}#{Context.recUser.Discriminator}`");
        }
        
        [Command("cmdlist"), Alias("info", "cmdinfo")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task CmdList()
        {
            IUser Author = Context.Client.CurrentUser;
            var EmbedBuilder = new EmbedBuilder();
            EmbedBuilder.AddField("$dm [userID/@User] [input]", "- Sends a DM to the user containing the input");
            EmbedBuilder.AddField("$dm-embed ($edm) [userID/@User] [input]", "- Sends a DM to the user containing the input as a rich embed");
            EmbedBuilder.AddField("$reply ($recent, $dmr) [input]", "- Sends a DM to the most recent user containing the input");
            EmbedBuilder.AddField("$faq", "- Lists the FAQ answers and their command names")

            .WithColor(Color.Blue);

            Embed embed = EmbedBuilder.Build();
            
            await ReplyAsync(embed: embed);
        }

        [Command("faq")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task FAQ()
        {
            IUser Author = Context.Client.CurrentUser;
            var EmbedBuilder = new EmbedBuilder();
            EmbedBuilder.AddField("$ans1", "- For random stuff not suitable for modmail")

            .WithColor(Color.Blue);

            Embed embed = EmbedBuilder.Build();

            await ReplyAsync(embed: embed);
        }

        [Command("ans1")]
        [RequireUserPermission(GuildPermission.ManageMessages, ErrorMessage = "You are not able to use this command")]
        public async Task Answer1()
        {
            string input;
            input = "Hi!\nIf you have a query or concern to express to the IVE cord moderation team, please message this bot with it." +
                "\nHowever if you do not have a question about the server or a concern about a user or similar, please do not message this bot account." +
                "\nThank you!";
            try
            {
                await UserExtensions.SendMessageAsync(Context.recUser, input);
            }
            catch (Discord.Net.HttpException x)
            {
                await ReplyAsync($"DM failed with the error message:\n`{x.Reason}`");
                return;
            }
            await ReplyAsync($"**Message sent to** `{Context.recUser.Username}#{Context.recUser.Discriminator}`");
        }
    }
}
