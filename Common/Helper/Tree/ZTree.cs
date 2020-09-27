using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ZTree
    {
        #region  属性
        private string Id;
        /// <summary>
        /// 节点的id
        /// </summary>
        public string id
        {
            get { return Id; }
            set { Id = value; }
        }
        private string Pid;
        /// <summary>
        /// 父节点的id
        /// </summary>
        public string pId
        {
            get { return Pid; }
            set { Pid = value; }
        }
        private string Name;
        /// <summary>
        /// 显示的节点名称
        /// </summary>
        public string name
        {
            get { return Name; }
            set { Name = value; }
        }
        private string Icon;
        /// <summary>
        /// 图标
        /// </summary>
        public string icon
        {
            get { return Icon; }
            set { Icon = value; }
        }
        private bool Open;
        /// <summary>
        /// 当前节点是否展开
        /// </summary>
        public bool open
        {
            get { return Open; }
            set { Open = value; }
        }
        private bool isparent;
        /// <summary>
        /// 是否父节点
        /// </summary>
        public bool isParent
        {
            get { return isparent; }
            set { isparent = value; }
        }

        private bool check;
        /// <summary>
        /// 是否选中，当为带复选框或单选框时有用
        /// </summary>
        public bool @checked
        {
            get { return check; }
            set { check = value; }
        }

        private string Title;
        /// <summary>
        /// 节点提示信息Tip
        /// </summary>
        public string title
        {
            get { return Title; }
            set { Title = value; }
        }
        private string F1;
        /// <summary>
        /// 备用属性1
        /// </summary>
        public string f1
        {
            get { return F1; }
            set { F1 = value; }
        }
        private string F2;
        /// <summary>
        /// 备用属性2
        /// </summary>
        public string f2
        {
            get { return F2; }
            set { F2 = value; }
        }

        private int? Sort;
        /// <summary>
        /// 排序
        /// </summary>
        public int? sort
        {
            get { return Sort; }
            set { Sort = value; }
        }
        #endregion

        #region 构造器
        public ZTree()
        {
        }

        public ZTree(string id, string name, string title = null, string pid = null, string icon = null, int? sort = null, string f1 = null, string f2 = null, bool check = false)
        {
            this.id = id;
            this.name = name;
            this.title = title;
            this.pId = pid;
            this.icon = icon;
            this.sort = sort;
            this.f1 = f1;
            this.f2 = f2;
            this.check = check;
        }
        #endregion

        /// <summary>
        /// 展开结点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tree"></param>
        public static void ExpandTree(string id, List<ZTree> tree)
        {
            if (id == null)
            {
                return;
            }
            var t = tree.FirstOrDefault(a => a.id == id);
            if (t != null)
            {
                if (t.pId != null)
                {
                    var p = tree.FirstOrDefault(a => a.id == t.pId);
                    if (p != null)
                        p.open = true;
                }
                ExpandTree(t.pId, tree);
            }
        }
    }
}
