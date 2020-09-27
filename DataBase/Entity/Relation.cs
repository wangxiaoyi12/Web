namespace DataBase
{
    public class Relation
    {
        public string id { get; set; }
        public string name { get; set; }
        public itemStyle itemStyle { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class guanxi
    {
        public string source { get; set; }

        public string target { get; set; }
    }
    //itemStyle:{
    //     normal:{
    //         color: 'blue'
    //            }
    //},
    public class itemStyle
    {
        public normal normal { get; set; }
    }
    public class normal
    {
        public string color { get; set; }
    }
}
