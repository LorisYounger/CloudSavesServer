using System.Data;
using FreeSql.DataAnnotations;

namespace SavesServer.DataBase
{

    /// <summary>
    /// 日志
    /// </summary>
    [Table]
    public class db_Log
    {
        /// <summary>
        /// 历史ID
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long ID { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        [Column(DbType = "DATETIME")]
        public DateTime SendTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 类型
        /// </summary>
        [Column(StringLength = 32)]
        public string Type { get; set; } = "";
        /// <summary>
        /// 内容
        /// </summary>
        [Column(DbType = "TEXT")]
        public string Content { get; set; } = "";
    }
}
