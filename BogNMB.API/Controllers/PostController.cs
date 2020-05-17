using BogNMB.API.POCOs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BogNMB.API.Controllers
{
    public class PostController : PagedController
    {
        public PostController(ApiConfig config) : base(config)
        {
        }

        public override string ApiMode => "p";

        public async Task<List<Post>> GetPostAsync(int forumId,int page)
        {
            var json = await GetAsync(forumId.ToString(), page.ToString());
            var res = JsonConvert.DeserializeObject(json, typeof(List<Post>));
            return res as List<Post>??new List<Post>();
        }

    }
}
