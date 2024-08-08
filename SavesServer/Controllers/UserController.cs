using CloudSaves.Client;
using LinePutScript;
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
        /// <summary>
        /// 删除该游戏所有数据
        /// </summary>
        /// <param name="data">游戏数据</param>
        [HttpPost("DeleteGame")]
        public string DeleteGame(DataStructure.GameData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            if (user.ListGame.Contains(data.GameName))
            {
                var lg = user.ListGames;
                lg.Remove(data.GameName);
                FSQL.Update<db_User>().Set(a => a.ListGames, lg).Where(a => a.Uid == user.Uid).ExecuteAffrows();
                FSQL.Delete<db_Save>().Where(a => a.Uid == user.Uid).Where(a => a.GameName == data.GameName).ExecuteAffrows();
                Log("UserDelete", $"Delete Game:{user.SteamID} {user.PassKey} {data.GameName}\n{HttpContext.Request.HttpContext.Connection.RemoteIpAddress}");
                return new Line("Success", "Delete Game Success").ToString();
            }
            return new Line("Error", "Game Not Found").ToString();
        }
        /// <summary>
        /// 删除游戏内账号所有存档和信息
        /// </summary>
        /// <param name="data">登录数据</param>
        [HttpPost("DeleteAccount")]
        public string DeleteAccount(DataStructure.LoginData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            Log("UserDelete", $"Delete User:{user.SteamID} {user.PassKey}\n{HttpContext.Request.HttpContext.Connection.RemoteIpAddress}");
            FSQL.Delete<db_User>().Where(a => a.Uid == user.Uid).ExecuteAffrows();
            FSQL.Delete<db_Save>().Where(a => a.Uid == user.Uid).ExecuteAffrows();
            return new Line("Success", "Delete Account Success").ToString();
        }
    }
}
