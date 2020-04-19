using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace BogNMB.API.Test
{
    class Config
    {
        public static ApiConfig ApiConfig => ApiConfig.Test;
    }
    [TestClass]
    public class ForumTest
    {
        [TestMethod]
        public async Task ForumRetriveTest()
        {
            var config = Config.ApiConfig;
            var controller = new ForumController(config);
            var forums = await controller.GetForumsAsync();
            foreach (var item in forums)
            {
                Assert.IsInstanceOfType(item, typeof(Forum));
            }
        }
    }

    [TestClass]
    public class PostTest
    {
        [TestMethod]
        public async Task TestRetrivePost()
        {
            var config = Config.ApiConfig;
            var pc = new PostController(config);
            var fc = new ForumController(config);
            var forums = await fc.GetForumsAsync();
            var posts = await pc.GetPostAsync(forums.First().Id, 1);
            foreach (var item in posts)
            {
                Assert.IsInstanceOfType(item, typeof(Post));
            }
        }
    }

    [TestClass]
    public class ThreadTest
    {
        [TestMethod]
        public async Task ThreadRetriveTest()
        {
            var config = Config.ApiConfig;
            var pc = new PostController(config);
            var fc = new ForumController(config);
            var forums = await fc.GetForumsAsync();
            var posts = await pc.GetPostAsync(forums.First().Id, 1);
            var post = posts.FirstOrDefault(p => p.Hr + p.Replys.Count > 0);
            if (post!=null)
            {
                var tc = new ThreadController(config);
                var threads = await tc.GetThreadsAsync(post.No, 1);
                foreach (var item in threads)
                {
                    Assert.IsInstanceOfType(item, typeof(Thread));
                }
            }
        }
    }

    [TestClass]
    public class ReplyTest
    {
        [DataTestMethod]
        [DataSource]
        public async Task RetriveReplyTest(ApiConfig config)
        {
            var pc = new PostController(config);
            var fc = new ForumController(config);
            var forums = await fc.GetForumsAsync();
            var posts = await pc.GetPostAsync(forums.First().Id, 1);
            var post = posts.FirstOrDefault(p => p.Hr + p.Replys.Count > 0);
            if (post != null)
            {
                var rc = new ReplyController(config);
                var reply = await rc.GetReplyAsync(post.No);
                Assert.IsInstanceOfType(reply, typeof(Reply));
            }
        }
    }
}
