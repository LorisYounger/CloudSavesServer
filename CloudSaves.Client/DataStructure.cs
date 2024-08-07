using LinePutScript.Converter;
using System;
using static CloudSaves.Client.ReturnStructure;

namespace CloudSaves.Client
{
    public class DataStructure
    {
        /// <summary>
        /// 登录数据
        /// </summary>
        public class LoginData
        {
            public ulong SteamID { get; set; }
            public ulong PassKey { get; set; }
        }
        /// <summary>
        /// 带游戏名字
        /// </summary>
        public class GameData : LoginData
        {
            public string GameName { get; set; }
        }
        /// <summary>
        /// 带存档IDs
        /// </summary>
        public class SaveIDsData : LoginData
        {
            public long[] SaveIDs { get; set; }
        }
        /// <summary>
        /// 带游戏数据
        /// </summary>
        public class SaveData : LoginData
        {
            /// <summary>
            /// 游戏存档
            /// </summary>
            public virtual string GameSaveData { get; set; } = "";
            /// <summary>
            /// 游戏名称
            /// </summary>
            public virtual string GameName { get; set; } = "";
            /// <summary>
            /// 存档介绍 (eg: 存档版本,存档内容)
            /// </summary>
            public virtual string Introduce { get; set; } = "";
            /// <summary>
            /// 存档名称
            /// </summary>
            public virtual string SaveName { get; set; } = "";
        }
    }
}
