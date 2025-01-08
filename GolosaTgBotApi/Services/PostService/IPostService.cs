﻿using GolosaTgBotApi.Models.Dtos;
using Telegram.Bot.Types;
namespace GolosaTgBotApi.Services.PostService
{
    public interface IPostService
    {
        public Task HandlePost(Message post);
        public Task<PostWithCommentsDto> GetPostWithComments(long postId, int commentCount);
        public Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId);

    }
}
