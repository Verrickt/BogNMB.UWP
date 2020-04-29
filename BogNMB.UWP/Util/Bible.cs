using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;

namespace BogNMB.UWP.Util
{
    public class Bible
    {
        private static readonly Random r = new Random(DateTime.Now.Second);
        public static string OnLoading => 
            _storage.Loading[r.Next() % _storage.Loading.Count];
        public static string OnFinished => 
            _storage.LoadingComplete[r.Next() % _storage.LoadingComplete.Count];
        public static string OnError => 
            _storage.Error[r.Next() % _storage.Error.Count];

        private static LocalObjectStorageHelper _helper = new LocalObjectStorageHelper();
        private const string fileName = "bible";
        private static BibleStorage _storage=new BibleStorage();
        private static async Task dumpStorage()
        {
            if (await _helper.FileExistsAsync(fileName))
            {
                BibleStorage content = null;
                try
                {
                    content = await _helper.ReadFileAsync<BibleStorage>(fileName);
                    Interlocked.Exchange(ref _storage, content);
                } catch (Exception) { }
            }
        }
        static Bible()
        {
            dumpStorage();
        }

    }
    public class BibleStorage
    {
        public List<string> Loading { get; set; }
        public List<string> LoadingComplete { get; set; } 
        public List<string> Error { get; set; } 

        public BibleStorage()
        {
            Loading = new List<string>() 
            { 
                "丧尸->朱军->诸君->肥肥",
                "B站未来有可能会倒闭，但绝不会变质",
                "横行霸道,共产共妻",
                "MO主席万碎(＾o＾)ﾉ"
            };
            LoadingComplete = new List<string>() { "加载完毕.点击刷新" };
            Error = new List<string>() { "加载出错.点击重试" };
        }
    }
}
