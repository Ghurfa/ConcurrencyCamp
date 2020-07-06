using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AsyncWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            SqlConnection connection = new SqlConnection(builder.ToString());
            SqlCommand command = new SqlCommand();

            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "waitfor delay '00:00:02'; select * from userData";

            await connection.OpenAsync();

            SqlDataReader reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {

            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
