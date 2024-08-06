using LinePutScript;
using LinePutScript.Converter;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static CloudSaves.Client.DataStructure;
using static CloudSaves.Client.SavesClient.Struct;

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
        public SavesClient(string serverUrl, string userName, string passKey)
        {
            ServerUrl = serverUrl;
            UserName = userName;
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
                var content = new System.Net.Http.StringContent(LPSConvert.SerializeObject(data).ToString());
                var response = await client.PostAsync(ServerUrl + "/" + URL, content);
                return new LPS(await response.Content.ReadAsStringAsync());
            }
        }
        public async Task<ServerInfo> ServerInfo()
        {
            var lps = await ConnectServer("", new LoginData() { SteamID = SteamID, PassKey = PassKey });
            return new ServerInfo()
            {
                Version = lps[0][(gstr)"v"],
                TotalUser = lps[0][(gint)"TotalUser"],
                TotalSave = lps[0][(gint)"TotalSave"],
                ContactInformation = lps[0][(gstr)"ContactInformation"]
            };
        }

        /// <summary>
        /// 返回的数据结构
        /// </summary>
        public static class Struct
        {
            /// <summary>
            /// 服务器信息
            /// </summary>
            public struct ServerInfo
            {
                /// <summary>
                /// 服务器版本
                /// </summary>
                public string Version;
                /// <summary>
                /// 所有用户数量
                /// </summary>
                public int TotalUser;
                /// <summary>
                /// 所有存档数量
                /// </summary>
                public int TotalSave;
                /// <summary>
                /// 联系信息/公告
                /// </summary>
                public string ContactInformation;
            }
        }

    }
}
