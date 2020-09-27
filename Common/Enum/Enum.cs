using System.Collections.Generic;

namespace System
{
    public class Enums
    {
        /// <summary>
        /// 获取指定类型的int值得枚举对象
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="t">类型</param>
        /// <returns></returns>
        public static T ToObject<T>(int value)
        {
            Type t = typeof(T);
            return (T)System.Enum.ToObject(t, value);
        }
        /// <summary>
        /// 指定枚举的string值，获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse<T>(string value)
        {
            //T result = default(T);
            //if(System.Enum.TryParse<T>(value, out result))
            return (T)System.Enum.Parse(typeof(T), value);
        }
        /// <summary>
        /// 获取指定类型的枚举列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<int, string> GetList<T>()
        {
            Type t = typeof(T);
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var item in System.Enum.GetValues(t))
            {
                dic.Add((int)item, System.Enum.GetName(t, item));
            }
            return dic;
        }




        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 查看
            /// </summary>
            View,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Edit,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 回复
            /// </summary>
            Reply,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel,
            /// <summary>
            /// 作废
            /// </summary>
            Invalid,
            /// <summary>
            /// 生成
            /// </summary>
            Build,
            /// <summary>
            /// 安装 
            /// </summary>
            Install,
            /// <summary>
            /// 卸载
            /// </summary>
            Unload,
            /// <summary>
            /// 备份
            /// </summary>
            Backup,
            /// <summary>
            /// 还原
            /// </summary>
            Restore,
            /// <summary>
            /// 替换
            /// </summary>
            Replace,
            /// <summary>
            /// 清空
            /// </summary>
            Clear,
            /// <summary>
            /// 其他事件
            /// </summary>
            OtherEvent
        }
        /// <summary>
        /// 系统登录类型
        /// </summary>
        public enum LoginType
        {
            /// <summary>
            /// 未登录
            /// </summary>
            nologin=0,
            /// <summary>
            /// 后台账号
            /// </summary>
            admin = 1,
            /// <summary>
            /// 前台会员账号
            /// </summary>
            member = 2,
            /// <summary>
            /// 商城用户
            /// </summary>
            shop=3
        }



        /// <summary>
        /// 会员星级，定义
        /// </summary>
        public enum StarLevel
        {
            一星 = 1,
            二星 = 2,
            三星 = 3,
            四星 = 4,
            五星 = 5,
            六星 = 6
        }



        /// <summary>
        /// 结算类型
        /// </summary>
        public enum SettleType
        {
            周结算 = 1,
            月计算 = 2
        }


        /// <summary>
        /// 积分充值状态
        /// </summary>
        public enum ScoreState
        {
            待付款 = 1,
            已成功 = 2
        }
    }    
}
