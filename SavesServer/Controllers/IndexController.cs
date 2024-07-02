using Microsoft.AspNetCore.Mvc;

namespace SavesServer.Controllers
{
    [Route("/")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpGet]
        public string Get() => "Saves Server v1.0\n" + Program.Set.ContactInformation;
    }
}
