using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySamplePro.Services;
using System.Collections.Generic;
using MySamplePro.Model;

namespace MySamplePro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IValueService _valueService;

        public ValuesController(ILogger<ValuesController> logger, IValueService valueService)
        {
            _logger = logger;
            _valueService = valueService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _logger.LogDebug("test get");
            return new string[] { "value1", "from mysamplepro 1" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return $"from mysamplepro {id}";
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

        /// <summary>
        /// 测试模型验证
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check_model")]
        public IActionResult TestModelCheck(DaiBanParaPoco model)
        {
            return Ok("");
        }
    }
}
