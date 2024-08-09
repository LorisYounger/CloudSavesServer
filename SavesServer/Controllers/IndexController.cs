using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;

namespace SavesServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet("")]
        public string Get(string? lang) => $"Saves Server#{Program.Version}:|ContactInformation#{ContactInformation(lang)}:|\n" + ServerInfo;
        [HttpPost("")]
        public string Post(string? lang) => $"Saves Server:|v#{Program.Version}:|ContactInformation#{ContactInformation(lang)}:|\n" + ServerInfo;

        public string ContactInformation(string? lang)
        {
            if (lang == null || Program.Set.ContactInformationTrans.Count == 0)
                return Program.Set.ContactInformation;
            if (Program.Set.ContactInformationTrans.TryGetValue(lang.ToLower(), out string? cit))
                return cit;
            return Program.Set.ContactInformation;
        }

        private string? serverInfo;
        private DateTime serverInfoTime = DateTime.MinValue;
        public string ServerInfo
        {
            get
            {
                if (serverInfo == null || DateTime.Now > serverInfoTime)
                {
                    serverInfoTime = DateTime.Now.AddHours(1);
                    serverInfo = "ServerInfo:|" +
                        "TotalUser#" + Program.FSQL.Select<db_User>().Count() +
                        ":|TotalSaves#" + Program.FSQL.Select<db_Save>().Count() +
                        ":|";
                }
                return serverInfo;
            }
        }
    }
}
