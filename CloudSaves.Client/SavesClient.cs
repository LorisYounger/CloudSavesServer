using LinePutScript;
using LinePutScript.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static CloudSaves.Client.DataStructure;
using static CloudSaves.Client.ReturnStructure;

namespace CloudSaves.Client
{
    /// <summary>
    /// 存档客户端
    /// </summary>
    public class SavesClient
    {
        /// <summary>
        /// 新建 存档客户端
        /// </summary>
        public SavesClient(string serverUrl, ulong steamID, ulong passKey)
        {
            ServerUrl = serverUrl;
            SteamID = steamID;
            PassKey = passKey;
        }

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 用户SteamID
        /// </summary>
        public ulong SteamID { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public ulong PassKey { get; set; }

        public void SetLoginData(LoginData loginData)
        {
            loginData.SteamID = SteamID;
            loginData.PassKey = PassKey;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public async Task<LPS> ConnectServer(string URL, LoginData data)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var content = new System.Net.Http.StringContent(LPSConvert.SerializeObject(data, convertNoneLineAttribute: true).ToString());
                var response = await client.PostAsync(ServerUrl + "/" + URL, content);
                return new LPS(await response.Content.ReadAsStringAsync());
            }
        }
        /// <summary>
        /// 服务器信息
        /// </summary>
        public async Task<ServerInfo> ServerInfo()
        {
            var lps = await ConnectServer("", new LoginData() { SteamID = SteamID, PassKey = PassKey });
            return new ServerInfo()
            {
                Version = lps.FirstOrDefault()?[(gstr)"v"],
                TotalUser = lps[1][(gint)"TotalUser"],
                TotalSave = lps[1][(gint)"TotalSave"],
                ContactInformation = lps.FirstOrDefault()?[(gstr)"ContactInformation"]
            };
        }
        /// <summary>
        /// 返回当前游戏的所有存档列表(不包括数据)
        /// </summary>
        /// <param name="gamename">游戏名字</param>
        public async Task<List<GameSaveData>> ListGameSaves(string gamename)
        {
            GameData gameData = new GameData() { GameName = gamename };
            SetLoginData(gameData);
            var lps = await ConnectServer("Saves/ListGameSaves", gameData);
            List<GameSaveData> results = new List<GameSaveData>();
            foreach (var item in lps)
            {
                results.Add(LPSConvert.DeserializeObject<GameSaveData>(item));
            }
            return results;
        }

        /// <summary>
        /// 获得游戏存档(包括数据)
        /// </summary>
        /// <param name="saveIDs">存档id</param>
        public async Task<List<GameSaveData>> GetGameSave(params long[] saveIDs)
        {
            SaveIDsData saveIDsData = new SaveIDsData() { SaveIDs = saveIDs };
            SetLoginData(saveIDsData);
            var lps = await ConnectServer("Saves/GetGameSave", saveIDsData);
            List<GameSaveData> results = new List<GameSaveData>();
            foreach (var item in lps)
            {
                results.Add(LPSConvert.DeserializeObject<GameSaveData>(item));
            }
            return results;
        }
        /// <summary>
        /// 删除游戏存档
        /// </summary>
        /// <param name="saveIDs">存档id</param>
        /// <returns></returns>
        public async Task<bool> RemoveGameSave(params long[] saveIDs)
        {
            SaveIDsData saveIDsData = new SaveIDsData() { SaveIDs = saveIDs };
            SetLoginData(saveIDsData);
            var lps = await ConnectServer("Saves/RemoveGameSave", saveIDsData);
            return lps.FirstOrDefault()?.Name == "Success";
        }
        /// <summary>
        /// 添加游戏存档
        /// </summary>
        /// <param name="gamename">游戏名称</param>
        /// <param name="introduce">存档介绍/信息</param>
        /// <param name="saveName">存档名字</param>
        /// <param name="data">存档数据</param>
        /// <returns></returns>
        public async Task<bool> AddGameSave(string gamename, string introduce, string saveName, string data)
        {
            SaveData saveData = new SaveData() { GameName = gamename, GameSaveData = data, SaveName = saveName, Introduce = introduce };
            SetLoginData(saveData);
            var lps = await ConnectServer("Saves/AddGameSave", saveData);
            return lps.FirstOrDefault()?.Name == "Success";
        }
        /// <summary>
        /// 列出当前用户存档过的游戏
        /// </summary>
        /// <returns>游戏名列表</returns>
        public async Task<List<string>> ListGames()
        {
            var lps = await ConnectServer("User/ListGames", new LoginData() { SteamID = SteamID, PassKey = PassKey });
            return lps.First().GetInfos().ToList();
        }
        /// <summary>
        /// 删除该游戏所有数据
        /// </summary>
        /// <param name="gameName">游戏名称</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteGame(string gameName)
        {
            GameData gameData = new GameData() { GameName = gameName };
            SetLoginData(gameData);
            var lps = await ConnectServer("User/DeleteGame", gameData);
            return lps.FirstOrDefault()?.Name == "Success";
        }
        /// <summary>
        /// 删除游戏内账号所有存档和信息
        /// </summary>
        /// <returns>登录数据</returns>
        public async Task<bool> DeleteAccount()
        {
            var lps = await ConnectServer("User/DeleteAccount", new LoginData() { SteamID = SteamID, PassKey = PassKey });
            return lps.FirstOrDefault()?.Name == "Success";
        }


    }
}
