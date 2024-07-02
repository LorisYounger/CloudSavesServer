using FreeSql.DataAnnotations;

namespace SavesServer.DataBase
{
    /// <summary>
    /// 游戏存档
    /// </summary>
    [Table]
    public class db_Save
    {
        /// <summary>
        /// 存档ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long SaveID { get; set; }
        /// <summary>
        /// 玩家ID
        /// </summary>
        [Column]
        public int Uid { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        [Column(StringLength = 255)]
        public string GameName { get; set; } = "";
        /// <summary>
        /// 存档名称
        /// </summary>
        [Column(StringLength = 255)]
        public string SaveName { get; set; } = "";

        /// <summary>
        /// 存档时间
        /// </summary>
        [Column]
        public DateTime SaveTime { get; set; }
        /// <summary>
        /// 游戏存档
        /// </summary>
        [Column(DbType = "LONGTEXT")]
        public string SaveData { get; set; } = "";

        /// <summary>
        /// 存档介绍 (eg: 存档版本,存档内容)
        /// </summary>
        [Column(StringLength = 10000)]
        public string Introduce { get; set; } = "";
    }
}
