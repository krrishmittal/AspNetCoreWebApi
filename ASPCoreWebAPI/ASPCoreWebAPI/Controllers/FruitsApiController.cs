using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FruitsApiController : ControllerBase
    {
        public List<string> fruits = new List<string>
        {
            "Apple",
            "Banana",
            "Orange",
            "Mango",
            "Grapes"
        };
        [HttpGet]
        public List<string> GetFruits()
        {
            return fruits;
        }
        [HttpGet("{id}")]
        public string GetFruitsByIndex(int id)
        {
            return fruits.ElementAt(id);
        }
         
    }
}
