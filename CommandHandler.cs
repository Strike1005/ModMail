using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace ModMail
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider provider;
        private readonly DiscordSocketClient client;
        private readonly CommandService service;
        private readonly IConfiguration configuration;
        private IUser recUser;
        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration configuration)
        {
            this.provider = provider;
            this.client = client;
            this.service = service;
            this.configuration = configuration;
            recUser = client.GetUser(937862407336898580);
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            this.client.MessageReceived += OnMessageReceived;
            await this.service.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
        }
        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != Discord.MessageSource.User) return;
            var argPos = 0;

            var context = new MMSocketCommandContext(this.client, message);
            context.AddRecUser(recUser);

            if (message.Channel is IDMChannel && !message.HasStringPrefix(this.configuration["Prefix"], ref argPos))
            {
                List<string> links = context.Message.Attachments.Select(x => x.Url).ToList();

                if (links.Count > 4)
                {
                    await message.ReplyAsync("Please send less than 5 attachments at a time");
                    return;
                }

                IUser user = context.Message.Author;
                ITextChannel Modchannel = context.Client.GetChannel(912513440696905819) as ITextChannel;
                if (recUser != user)
                {
                    recUser = user;
                    context.AddRecUser(recUser);
                    await Modchannel.SendMessageAsync("**New user DM**\n`Username:` " + recUser.Username);
                    await Modchannel.SendMessageAsync("`ID:` " + recUser.Id);
                }
                
                await message.AddReactionAsync(Emote.Parse("<:IVElogo:909627344472383498>"));
                var EmbedBuilder = new EmbedBuilder();
                EmbedBuilder.AddField("ModMail:", $"{user.Mention} - {message}")

                .WithAuthor(user)
                .WithColor(Color.DarkerGrey)
                .WithFooter(footer =>
                {
                    footer
                    .WithText($"\n{context.User.Id}");
                }
                )
                .WithCurrentTimestamp();

                if (links.Count > 0) { EmbedBuilder.WithDescription("Attachments below:"); }

                Embed embed = EmbedBuilder.Build();

                string Attachments = null;

                if (links.Count > 0 && links.Count < 5)
                {
                    foreach (var z in links)
                    {
                        Attachments += z + Environment.NewLine;
                    }
                    await Modchannel.SendMessageAsync(embed: embed);
                    await Modchannel.SendMessageAsync(Attachments);
                    return;
                }
                await Modchannel.SendMessageAsync(embed: embed);

            }
            else if (!message.HasStringPrefix(this.configuration["Prefix"], ref argPos) && !message.HasMentionPrefix(this.client.CurrentUser, ref argPos)) return;

            context.AddRecUser(recUser);
            await this.service.ExecuteAsync(context, argPos, this.provider);
        }
    }
    public class MMSocketCommandContext : ICommandContext
    {
        public DiscordSocketClient Client { get; }
        public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel { get; }
        public SocketUser User { get; }
        public SocketUserMessage Message { get; }
        public bool IsPrivate => Channel is IPrivateChannel;
        public IUser recUser;
        public void AddRecUser(IUser recU)
        {
            recUser = recU;
        }
        IDiscordClient ICommandContext.Client => Client;

        IGuild ICommandContext.Guild => Guild;

        IMessageChannel ICommandContext.Channel => Channel;

        IUser ICommandContext.User => User;

        IUserMessage ICommandContext.Message => Message;
        public MMSocketCommandContext(DiscordSocketClient client, SocketUserMessage msg)
        {
            Client = client;
            Guild = (msg.Channel as SocketGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }
    }
}
