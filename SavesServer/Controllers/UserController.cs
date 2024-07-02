using CloudSaves.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;
using static SavesServer.Program;

namespace SavesServer.Controllers
{
    [Route("User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public string Login(DataStructure.LoginData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;

            db_User user = Program.FSQL.Select<db_User>().Where(a => a.SteamID == data.SteamID && a.PassKey == a.PassKey).ToOne();
            //如果用户不存在则创建
            if (user == null)
            {//创建用户
                user = new db_User()
                {
                    SteamID = data.SteamID,
                    PassKey = data.PassKey
                };
                FSQL.Insert(user).ExecuteAffrows();
            }


            return "TODO";
        }


    }
}
