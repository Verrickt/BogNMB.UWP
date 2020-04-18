using System.Collections.Generic;

namespace BogNMB.API.POCOs
{
    public class Post
    {
        public string No { get; set; }
        public int Res { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Com { get; set; }
        public string Time { get; set; }
        public string Id { get; set; }
        public string Img { get; set; }
        public string Src { get; set; }
        public string Ext { get; set; }
        public int @Lock { get; set; }
        public int Htmb { get; set; }
        public int Sage { get; set; }
        public int Admin { get; set; }
        public int Hr { get; set; }
        public int Top { get; set; }
        public List<Thread> Replys { get; set; }
    }
}
