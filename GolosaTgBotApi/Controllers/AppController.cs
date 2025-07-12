using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.TelegramService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static GolosaTgBotApi.Controllers.CommentController;

namespace GolosaTgBotApi.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AppController : ControllerBase
    {
        private readonly ITelegramService _telegram;

        public AppController(ITelegramService telegram)
        {
            _telegram = telegram;
        }

        [HttpGet]
        [Route("test")]
        public async Task<ActionResult> Test()
        {
            var res = await _telegram.GetFileById("AgACAgIAAx0CTxROOQAD5WgyM9iDNAM41tkd1dC2CqprRuibAAJE-DEb6c2RSRY09aAdHRlWAQADAgADeQADNgQ");
            Console.WriteLine(res.Length);
            return Ok();
        }
    }
}