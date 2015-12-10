using System.Security.Claims;
using System.Web.Http;

namespace webko.sapi
{
    [Route("test")]
    public class TestController : ApiController
    {
        public IHttpActionResult Get()
        {
            var caller = User as ClaimsPrincipal;

            var subjectClaim = caller.FindFirst("sub");
            if (subjectClaim != null)
            {
                return Json(new
                {
                    message = "OK user",
                    client = caller.FindFirst("client_id").Value,
                    orgUnit = caller.FindFirst("orgUnit").Value,
                    subject = subjectClaim.Value
                });
            }
            else
            {
                return Json(new
                {
                    message = "OK service",
                    orgUnit = caller.FindFirst("orgUnit").Value,
                    client = caller.FindFirst("client_id").Value
                });
            }
        }
    }
}
