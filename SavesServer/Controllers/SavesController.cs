using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SavesServer.Controllers
{
    [Route("Saves")]
    [ApiController]
    public class SavesController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Saves Server v1.0\n" + Program.Set.ContactInformation;


    }
}
