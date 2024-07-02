
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace SavesServer
{
    public static class Checking
    {
        /// <summary>
        /// 检查是否有错误, 若有则返回错误信息
        /// </summary>
        /// <param name="res">错误处理结果</param>
        /// <param name="result">错误信息</param>
        public static bool Check(this string? res, out string result)
        {
            if (res == null)
            {
                result = "";
                return false;
            }
            else
            {
                result = $"Error#{res}:|";
                return true;
            }
        }

        /// <summary>
        /// 对IP进行检查
        /// </summary>
        /// <returns>IP String</returns>
        public static string? Base(HttpContext context)
        {
            if (relstime != DateTime.Now.Day)
            {
                IPuploadtimes.Clear();
                IPids.Clear();
            }
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress;
            return ip?.MapToIPv4().ToString();
        }
        public static Dictionary<string, HashSet<ulong>?> IPids = new();
        public static Dictionary<string, int> IPuploadtimes = new Dictionary<string, int>();
        private static int relstime = DateTime.Now.Day;
        /// <summary>
        /// 检查单个ip中ID是否有大于8个
        /// </summary>
        /// <returns>TRUE: 失败</returns>
        public static string? IDsCheck(HttpContext context, ulong steamid, ulong passkey)
        {
            string? ip = Base(context);
            if (ip == null)
            {
                return "Unknown IP";
            }
            if (!IPids.ContainsKey(ip))
            {
                IPids[ip] = new();
            }
            var id = steamid + passkey;
            HashSet<ulong>? ids = IPids.GetValueOrDefault(ip, null);
            if (ids == null)
            {
                ids = new();
                IPids[ip] = ids;
            }
            else if (ids.Count >= 8)
            {//太多了,扔了
                Program.Log("UserWarn", $"IP: {ip} IDS.Length={ids.Count} ID='{string.Join(',', ids)}'");
                return "IP MAX IDS=8 CHECK";
            }
            ids.Add(id);
            return null;
        }
        /// <summary>
        /// 对IP进行检查
        /// </summary>
        /// <returns>IP String</returns>
        public static string? TimesCheck(HttpContext context)
        {
            string? ip = Base(context);
            if (ip == null)
            {
                return "Unknown IP";
            }
            int time = IPuploadtimes.GetValueOrDefault(ip, 0);
            if (time >= 100)
            {
                if (time < 10000)
                {
                    Program.Log("UserWarn", $"IP: {ip} Times Max {time}");
                    IPuploadtimes[ip] = 100000;
                }
                return "IP times Max";
            }
            return null;
        }
        /// <summary>
        /// 增加次数
        /// </summary>
        /// <returns>IP String</returns>
        public static void AddTimes(HttpContext context, int addcount = 1, string content = "")
        {
            string? ip = Base(context);
            if (ip == null)
            {
                return;
            }
            int time = IPuploadtimes.GetValueOrDefault(ip, 0);
            IPuploadtimes[ip] = time + addcount;
            if (content != "")
                Program.Log("UserWarn", $"IP: {ip} Times: {time + addcount} Content:{content}");
        }
    }
}
