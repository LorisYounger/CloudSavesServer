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
        /// 游戏存档
        /// </summary>
        public class GameSaveData
        {
            /// <summary>
            /// 存档ID
            /// </summary>
            public virtual long SaveID { get; set; }
            /// <summary>
            /// 玩家ID
            /// </summary>
            public virtual int Uid { get; set; }
            /// <summary>
            /// 游戏名称
            /// </summary>
            public virtual string GameName { get; set; } = "";
            /// <summary>
            /// 存档名称
            /// </summary>
            public virtual string SaveName { get; set; } = "";

            /// <summary>
            /// 存档时间
            /// </summary>
            public virtual DateTime SaveTime { get; set; }
            /// <summary>
            /// 游戏存档
            /// </summary>
            public virtual string SaveData { get; set; } = "";

            /// <summary>
            /// 存档介绍 (eg: 存档版本,存档内容)
            /// </summary>
            public virtual string Introduce { get; set; } = "";
        }
    }
}
