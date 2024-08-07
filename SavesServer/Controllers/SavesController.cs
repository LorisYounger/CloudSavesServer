using CloudSaves.Client;
using LinePutScript;
using LinePutScript.Converter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;
using static CloudSaves.Client.ReturnStructure;
using static SavesServer.Controllers.UserController;
using static SavesServer.Program;
namespace SavesServer.Controllers
{
    [Route("Saves")]
    [ApiController]
    public class SavesController : ControllerBase
    {
        [HttpGet("")]
        public string Get() => "Saves Server v1.0\n" + Program.Set.ContactInformation;
        [HttpPost("")]
        public string Post() => "Saves Server:|v#1.0:|\n" + Program.Set.ContactInformation;
        /// <summary>
        /// 列出所有该游戏的数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("ListGameSaves")]
        public string ListGameSaves(DataStructure.GameData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            var saves = FSQL.Select<db_Save>().Where(a => a.Uid == user.Uid).Where(a => a.GameName == data.GameName).ToList();
            LPS lps = new LPS();
            foreach (var save in saves)
            {
                var line = LPSConvert.SerializeObject(save, "Saves", convertNoneLineAttribute: true);
                line.Remove("SaveData");
                lps.Add(line);
            }
            return lps.ToString();
        }
        /// <summary>
        /// 获取指定ID的存档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("GetGameSave")]
        public string GetGameSave(DataStructure.SaveIDsData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            var saves = FSQL.Select<db_Save>().Where(a => a.Uid == user.Uid).Where(a => data.SaveIDs.Contains(a.SaveID)).ToList();
            LPS lps = new LPS();
            foreach (var save in saves)
            {
                lps.Add(LPSConvert.SerializeObject(save, "Saves", convertNoneLineAttribute: true));
            }
            return lps.ToString();
        }
        /// <summary>
        /// 删除指定ID的存档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("RemoveGameSave")]
        public string RemoveGameSave(DataStructure.SaveIDsData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            FSQL.Delete<db_Save>().Where(a => a.Uid == user.Uid).Where(a => data.SaveIDs.Contains(a.SaveID)).ExecuteAffrows();
            return "Success";
        }
        /// <summary>
        /// 添加存档
        /// </summary>
        /// <param name="data"></param>
        [HttpPost("AddGameSave")]
        public string AddGameSave(DataStructure.SaveData data)
        {
            if (Checking.TimesCheck(HttpContext).Check(out string error))//搞桌宠不需要检查ip,敏感信息需要检查
                return error;
            if (Checking.IDsCheck(HttpContext, data.SteamID, data.PassKey).Check(out error))
                return error;
            db_User user = Login(data);
            if (!user.ListGames.Contains(data.GameName))
            {
                var list = user.ListGames;
                list.Add(data.GameName);
                user.ListGames = list;
                FSQL.Update<db_User>().SetSource(user).ExecuteAffrows();
            }
            var save = new GameSaveData()
            {
                GameName = data.GameName,
                SaveData = data.GameSaveData,
                SaveTime = DateTime.Now,
                Introduce = data.Introduce,
                SaveName = data.SaveName,
                Uid = user.Uid
            };
            FSQL.Insert(save).ExecuteAffrows();
            return "Success";
        }
    }
}
