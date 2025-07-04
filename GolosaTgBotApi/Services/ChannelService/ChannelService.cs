﻿using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.ChannelService
{
    public class ChannelService : IChannelService
    {
        private readonly ITelegramService _telegram;
        private readonly IMariaService _mariaService;

        public ChannelService(ITelegramService telegramService, IMariaService mariaService)
        {
            _telegram = telegramService;
            _mariaService = mariaService;
        }

        public async Task<Channel> CreateNewChannel(long channelId)
        {
            var channel = await _mariaService.GetChannelById(channelId);
            if (channel != null)
            {
                await UpdateChannelInfo(channel);
                return channel;
            }
            var newChannel = new Channel();
            var chatInfo = await _telegram.GetChatInfoById(channelId);
            newChannel.Id = channelId;
            newChannel.OwnerId = await _telegram.GetChannelOwnerId(channelId);
            newChannel.Title = chatInfo.Title;
            await _mariaService.CreateNewChannel(newChannel);
            return newChannel;
        }

        public async Task AddLinkedChat(ChatFullInfo chatInfo)
        {
            var linkedChannel = await _mariaService.GetChannelById(chatInfo.LinkedChatId ?? 0);
            if (linkedChannel == null)
            {
                linkedChannel = await CreateNewChannel(chatInfo.LinkedChatId ?? 0);
            }
            if (chatInfo.LinkedChatId != linkedChannel.LinkedChatId)
            {
                LinkedChat chat = new LinkedChat();
                chat.Name = chatInfo.Title;
                chat.ChannelID = linkedChannel.Id;
                linkedChannel.LinkedChat = chat;
                linkedChannel.LinkedChatId = chatInfo.Id;
                _mariaService.UpdateChannelInfo(linkedChannel);
            }
        }

        public async Task<Channel> CheckOnChannelExisting(long channelId)
        {
            var channel = await _mariaService.GetChannelById(channelId);
            if (channel == null)
            {
                channel = await CreateNewChannel(channelId);
            }
            return channel;
        }

        public async Task UpdateChannelInfo(Channel channel)
        {
            var chatInfo = await _telegram.GetChatInfoById(channel.Id);
            channel.OwnerId = await _telegram.GetChannelOwnerId(channel.Id);
            channel.Title = chatInfo.Title;
            await _mariaService.UpdateChannelInfo(channel);
        }
    }
}