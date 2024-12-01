using FreeSql.DataAnnotations;
using static CloudSaves.Client.ReturnStructure;

namespace SavesServer.DataBase
{
    /// <summary>
    /// 游戏存档
    /// </summary>
    [Table]
    [Index("idx_game_uid", "GameName,Uid")]
    public class db_Save : GameSaveData
    {
        /// <summary>
        /// 存档ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public override long SaveID { get; set; }
        /// <summary>
        /// 玩家ID
        /// </summary>
        [Column]
        public override int Uid { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        [Column(StringLength = 255)]
        public override string GameName { get; set; } = "";
        /// <summary>
        /// 存档名称
        /// </summary>
        [Column(StringLength = 255)]
        public override string SaveName { get; set; } = "";

        /// <summary>
        /// 存档时间
        /// </summary>
        [Column]
        public override DateTime SaveTime { get; set; }
        /// <summary>
        /// 游戏存档
        /// </summary>
        [Column(DbType = "LONGTEXT")]
        public override string SaveData { get; set; } = "";

        /// <summary>
        /// 存档介绍 (eg: 存档版本,存档内容)
        /// </summary>
        [Column(StringLength = 10000)]
        public override string Introduce { get; set; } = "";
        /// <summary>
        /// 是否是自动存档
        /// </summary>
        [Column]
        public override bool IsAutoSave { get; set; } = true;
    }
}
