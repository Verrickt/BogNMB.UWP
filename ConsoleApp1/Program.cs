using BogNMB.API;
using BogNMB.API.Controllers;
using BogNMB.API.POCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                var str = Console.ReadLine();
                var ans = GetIPAddr(str);
                ans.ForEach(Console.WriteLine);

            }
            GetIPAddr("88881");
            /*
            var config = new ApiConfig();
            ForumController f = new ForumController(config);
            var res = await f.GetForumsAsync();
            var p = new PostController(config);
            var posts = await p.GetPostAsync(res[0].Id, 1);
            var t = new ThreadController(config);
            var threads = await t.GetThreadsAsync(posts[0].No, 1);
            Console.ReadLine();
            */
        }
        private static List<int> len = new List<int>();
        private static void dfs(int level,int length,string str,List<string> ans)
        {
            len.Add(length);
            if(level==3)
            {
                if(len.Sum()>=str.Length) { len.RemoveAt(len.Count - 1); return; }
                string[] ips = new string[4];
                int start = 0;
                for(int i=0;i<3;i++)
                {
                    ips[i] = str.Substring(start, len[i]);
                    start += len[i];
                }
                ips[3] = str.Substring(start);
                if(ips.Select(i=>int.Parse(i)).All(i=>i>=0&&i<=255))
                {
                    ans.Add(string.Join(".", ips));
                }
                len.RemoveAt(len.Count - 1);
                return;
            }
            for(int i=1;i<str.Length;i++)
            {
                dfs(level + 1, i, str,ans);
            }
            len.RemoveAt(len.Count - 1);
        }
        static List<string> GetIPAddr(string str)
        {
            List<string> ans = new List<string>();
            for(int i=1;i<str.Length;i++)
            {
                dfs(1, i, str,ans);
            }
            return ans;
        }
       
    }
}
