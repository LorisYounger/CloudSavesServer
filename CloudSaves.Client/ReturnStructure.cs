using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using LinePutScript;
using LinePutScript.Converter;

namespace CloudSaves.Client
{
    public class ReturnStructure
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
        /// <summary>
        /// 游戏存档
        /// </summary>
        public class GameSaveData
        {
            /// <summary>
            /// 存档ID
            /// </summary>
            [Line] public virtual long SaveID { get; set; }
            /// <summary>
            /// 玩家ID
            /// </summary>
            [Line] public virtual int Uid { get; set; }
            /// <summary>
            /// 游戏名称
            /// </summary>
            [Line] public virtual string GameName { get; set; } = "";
            /// <summary>
            /// 存档名称
            /// </summary>
            [Line] public virtual string SaveName { get; set; } = "";

            /// <summary>
            /// 存档时间
            /// </summary>
            [Line] public virtual DateTime SaveTime { get; set; }
            /// <summary>
            /// 游戏存档
            /// </summary>
            [Line] public virtual string SaveData { get; set; } = "";

            /// <summary>
            /// 存档介绍 (eg: 存档版本,存档内容)
            /// </summary>
            [Line] public virtual string Introduce { get; set; } = "";
        }
    }
}
