using Microsoft.AspNetCore.Mvc;
using SavesServer.DataBase;

namespace SavesServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet("")]
        public string Get() => "Saves Server#v1.0:|ContactInformation#" + Program.Set.ContactInformation + ":|\n" + ServerInfo;
        [HttpPost("")]
        public string Post() => "Saves Server:|v#1.0:|ContactInformation#" + Program.Set.ContactInformation + ":|\n" + ServerInfo;

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
