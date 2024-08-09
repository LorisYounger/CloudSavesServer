using FreeSql.DataAnnotations;

namespace SavesServer.DataBase
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table]
    public class db_User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Uid { get; set; }

        //之所以不拿SteamID做主键是因为你根本没法判断SteamID是不是真用户的

        /// <summary>
        /// Steam ID
        /// </summary>
        [Column]
        public ulong SteamID { get; set; }
        /// <summary>
        /// PassKey
        /// </summary>
        [Column]
        public ulong PassKey { get; set; }
        /// <summary>
        /// 游戏列表,用逗号分隔
        /// </summary>
        [Column]
        public string ListGame { get; set; } = "";
        /// <summary>
        /// 游戏列表
        /// </summary>
        public List<string> ListGames
        {
            get => ListGame.Length == 0 ? new List<string>() : ListGame.Split(',').ToList();
            set => ListGame = string.Join(',', value);
        }
    }
}
