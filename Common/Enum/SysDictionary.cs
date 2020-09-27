using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class SysDictionary
    {
        /// <summary>
        /// 权限类型字典 
        /// </summary>
        public static Dictionary<string, string> PermissionsType
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                     {"Show", "显示(Show)"},
                     {"View", "查看(View)"},
                     {"Add", "添加(Add)"},
                     {"Edit", "修改(Edit)"},
                     {"Delete", "删除(Delete)"},
                     {"Audit", "审核(Audit)"},
                     {"Reply", "回复(Reply)"},
                     {"Confirm", "确认(Confirm)"},
                     {"Cancel", "取消(Cancel)"},
                     {"Invalid", "作废(Invalid)"},
                     {"Build", "生成(Build)"},
                     {"Install", "安装(Instal)"},
                     {"Unload", "卸载(Unload)"},
                     {"Backup", "备份(Backup)"},
                     {"Restore", "还原(Restore)"},
                     {"Replace", "替换(Replace)"},
                     {"Clear", "清空(Clear)"}
                };
                return dic;
            }
        }
        /// <summary>
        /// 角色类型字典 
        /// </summary>
        public static Dictionary<string, string> RoleType
        {
            get
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    {"1", "超级用户"},
                    {"2", "系统用户"}
                };
                return dic;
            }
        }

        /// <summary>
        /// 系统操作日志事件类型 
        /// </summary>
        public enum EventType
        {
            Insert = 0,
            Delete = 1,
            Edit = 2,
            Query = 3,
            Restore = 4
        }
        /// <summary>
        /// 系统操作日志事件类型 
        /// </summary>
        public enum BasicType
        {
            Bank = 1,
            Department = 2,
            Position = 3,
            Goods = 4

        }

        public static ArrayList GetWeek()
        {
            ArrayList al = new ArrayList();
            al.Add("星期一");
            al.Add("星期二");
            al.Add("星期三");
            al.Add("星期四");
            al.Add("星期五");
            al.Add("星期六");
            al.Add("星期日");
            return al;
        }

        /// <summary>
        /// 月份标准
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ArrayList GetMonth()
        {
            ArrayList al = new ArrayList();
            al.Add("1");
            al.Add("2");
            al.Add("3");
            al.Add("4");
            al.Add("5");
            al.Add("6");
            al.Add("7");
            al.Add("8");
            al.Add("9");
            al.Add("10");
            al.Add("11");
            al.Add("12");
            al.Add("13");
            al.Add("14");
            al.Add("15");
            al.Add("16");
            al.Add("17");
            al.Add("18");
            al.Add("19");
            al.Add("20");
            al.Add("21");
            al.Add("22");
            al.Add("23");
            al.Add("24");
            al.Add("25");
            al.Add("26");
            al.Add("27");
            al.Add("28");
            al.Add("29");
            al.Add("30");
            al.Add("31");
            return al;
        }
    }
}
