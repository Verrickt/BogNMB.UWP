using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BogNMB.API.Test
{
    public class DataSourceAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            yield return new[] { ApiConfig.Default };
            yield return new[] { ApiConfig.Test };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if(data[0] is ApiConfig ap)
            {
                if (ap.BaseUrl == ApiConfig.Default.BaseUrl) return "Default";
                else return "Test";
            }
            return "Unknown";
        }
    }
    [TestClass]
    public class ForumTest
    {
        [DataTestMethod]
        [DataSource]
        public async Task ForumRetriveTest(ApiConfig config)
        {
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
        [DataTestMethod]
        [DataSource]
        public async Task TestRetrivePost(ApiConfig config)
        {
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
        [DataTestMethod]
        [DataSource]
        public async Task ThreadRetriveTest(ApiConfig config)
        {
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
