using BogNMB.API.POCOs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BogNMB.API.Controllers
{
    public class ReplyController : ControllerBase
    {
        public ReplyController(ApiConfig config) : base(config)
        {
        }

        public override string ApiMode => "r";

        public async Task<Reply> GetReplyAsync(string id)
        {
            var json = await GetAsync(id, "0");
            return JsonConvert.DeserializeObject<Reply>(json);
        }
    }
}
