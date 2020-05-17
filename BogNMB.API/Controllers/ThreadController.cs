using BogNMB.API.POCOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BogNMB.API.Controllers
{
    public class ThreadController : PagedController
    {
        public ThreadController(ApiConfig config) : base(config)
        {
        }

        public override string ApiMode => "t";
        public async Task<List<Thread>> GetThreadsAsync(string postId,int page)
        {
            var json = await GetAsync(postId, page.ToString());
            var res = JsonConvert.DeserializeObject(json, typeof(ThreadContainer));
            return (res as ThreadContainer).Replys;
        }
    }

    
}
