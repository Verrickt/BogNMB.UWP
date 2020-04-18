using BogNMB.API.POCOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BogNMB.API.Controllers
{
    public class ForumController : ControllerBase
    {
        public ForumController(ApiConfig config) : base(config)
        {
        }

        public override string ApiMode => "forum";

        public async Task<List<Forum>> GetForumsAsync()
        {
            var json = await GetAsync("0","0");
            var res = JsonConvert.DeserializeObject(json, typeof(List<Forum>));
            return res as List<Forum>;
        }
    }
}
