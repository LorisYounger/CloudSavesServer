using CloudSaves.Client;
using LinePutScript;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;
using static SavesServer.Program;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SavesServer.Controllers
{
    [Route("User")]
    [ApiController]
    public class UserController : ControllerBase
    {

        /// <summary>
        /// 登录并获取数据
        /// </summary>
        /// <param name="data">登录数据</param>
        /// <returns></returns>
        public static db_User Login(DataStructure.LoginData data)
        {
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
            return user;
        }

        /// <summary>
        /// 列出所有游戏
        /// </summary>
        /// <param name="data">登录数据</param>
        [HttpPost("ListGames")]
        public string ListGames(DataStructure.LoginData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            return new Line("Success", user.ListGame).ToString();
        }

    }
}
