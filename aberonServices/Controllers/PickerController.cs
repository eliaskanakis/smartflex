using aberonServices.Helpers;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace aberonServices.Controllers
{
    public class PickerController : ApiController
    {
        // GET api
        public IHttpActionResult Get(string nameShort)
        {
            var r = SqlHelper.GetRow("Picker",
                new OracleParameter("pNameShort",nameShort));

            if (r == null) return NotFound();

            return Ok(new {
                pickerId = r["EmlpoyeeId"],
                Name=r["Name1"]
            });
        }
    }
}
