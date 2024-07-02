namespace SavesServer
{

    public class SessionCache
    {
        /// <summary>
        /// SessionKey字典 失效时间+SteamID
        /// </summary>
        public static Dictionary<ulong, (DateTime, ulong)> Session = new Dictionary<ulong, (DateTime, ulong)>();

        public static ulong AddSession(ulong SteamID)
        {
            ulong sid = (ulong)Random.Shared.NextInt64() + (ulong)Random.Shared.NextInt64();
            if (Session.TryGetValue(sid, out var value))
            {
                if (value.Item1 < DateTime.Now)
                {
                    Session[sid] = (DateTime.Now.AddDays(2), SteamID);
                }
                else
                {
                    return AddSession(SteamID);
                }
            }
            else
            {
                Session.Add(SteamID, (DateTime.Now, sid));
            }
            return sid;
        }
        /// <summary>
        /// Steam数据缓存
        /// </summary>
        public static Dictionary<ulong, SessionCache> SteamCache = new Dictionary<ulong, SessionCache>();
    }
}
