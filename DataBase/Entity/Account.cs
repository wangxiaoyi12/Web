using System.Collections.Generic;

namespace DataBase
{
    /// <summary>
    /// 通用用户登录类，简单信息
    /// </summary>
    public class Account
    {
        #region Attribute
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登录的用户名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 支付密码
        /// </summary>
        public string PassWord2 { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 是否后台登录
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 用户可操作的模块集合
        /// </summary>
        //public List<Sys_Role_Nav> Role_Nav { get; set; }
        /// <summary>
        /// 登陆类型（判断是管理员登陆还是会员登录）
        /// 管理员：admin 会员：member
        /// </summary>
        public string LoginType { get; set; }

        #endregion
    }
}
