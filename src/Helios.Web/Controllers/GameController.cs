using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Helios.Engine.Scripting;
using Helios.Domain.Models;
using Helios.Domain.Contracts;
using Microsoft.AspNetCore.Hosting;
using Helios.Web.Models.GameViewModels;

namespace Helios.Web.Controllers
{
    [Authorize]
    public class GameController : Controller
    {
        private readonly GameMessageHandler _messageHandler;
        private readonly UserManager<User> _userMgr;
        private readonly IRepository<Account> _accounts;
        private IHostingEnvironment _env;


        public GameController(GameMessageHandler msgHandler, UserManager<User> userMgr, IRepository<Account> accounts, IHostingEnvironment env)
        {
            _messageHandler = msgHandler;
            _env = env;
            _userMgr = userMgr;
            _accounts = accounts;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Message(GameMessageModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Message)) return new OkResult();
            //var resp = "Welcome to <#darkcyan>Arcana Aeterna<#>!\n" +
            //          "-=-=-=-=-=-=-=-=-=-=-=-=-=";
            //await _messageHandler.SendText(resp);
            await _messageHandler.ReceiveMessage(model.ConnectionId, model.Message);
            return new OkResult();
        }

        [HttpPost]
        public async Task<ActionResult> Login(GameMessageModel model)
        {
            var user = await _userMgr.GetUserAsync(HttpContext.User);
            var account = _accounts.Find(x => x.UserId == user.Id).SingleOrDefault();
        
            //if (user.Account == null)s
            //{
            //    user.Account = new Account { UserId = user.Id };
            //    var res = await _userMgr.UpdateAsync(user);
            //}

            _messageHandler.Login(model.ConnectionId, user.Account);

            return new OkResult();
        }

        [HttpPost]
        public async Task<ActionResult> Logout(GameMessageModel model)
        {
             _messageHandler.Logout(model.ConnectionId);
             return new OkResult();
        }
    }
}