using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using Common;
namespace Wechat
{
    /// <summary>
    /// 链接管理使用
    /// </summary>
    public class LinkManage : BaseManage
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public LinkManage()
        {
        }

        #region 配置信息
        /// <summary>
        /// 获取AccessToke链接地址
        /// </summary>
        public string GetAccessToken()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                config.AppID, config.AppSecret);
            return url;
        }
        /// <summary>
        /// 微信服务器IP地址列表
        /// </summary>
        /// <returns></returns>
        public string GetWXIP()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 获取JsApi_Ticket 地址
        /// </summary>
        /// <returns></returns>
        public string GetJsApi_Ticket(string access_token = null)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi",
             access_token == null ? config.Access_Token : access_token
                );
            return url;
        }
        #endregion

        #region 菜单管理
        /// <summary>
        /// 自定义菜单查询接口
        /// </summary>
        /// <returns></returns>
        public string GetQueryMenu()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 创建菜单接口地址
        /// </summary>
        /// <returns></returns>
        public string GetCreateMenu()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 删除菜单接口地址
        /// </summary>
        /// <returns></returns>
        public string GetRemoveMenu()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}",
                config.Access_Token);
            return url;
        }
        #endregion

        #region 用户管理
        /// <summary>
        /// 获取用户列表，用户的OPENID 列表
        /// </summary>
        /// <returns></returns>
        public string GetUserList(string Next_OpenID = null)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}",
                config.Access_Token,
                Next_OpenID);
            return url;
        }
        /// <summary>
        /// 获取单个用户的基本信息
        /// </summary>
        /// <param name="OpenID"></param>
        /// <returns></returns>
        public string GetUserInfo(string OpenID)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN",
                config.Access_Token,
                OpenID
                );
            return url;
        }
        /// <summary>
        /// 批量获取用户信息链接地址
        /// </summary>
        /// <returns></returns>
        public string GetUserInfo()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token={0}", config.Access_Token);
            return url;
        }
        /// <summary>
        /// 获取设置用户备注地址
        /// </summary>
        /// <returns></returns>
        public string GetUser_UpdateRemark()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info/updateremark?access_token={0}",
                config.Access_Token);
            return url;
        }
        #endregion

        #region 分组管理
        /// <summary>
        /// 创建分组
        /// </summary>
        /// <returns></returns>
        public string GetCreateGroup()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/create?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 查询所有分组
        /// </summary>
        /// <returns></returns>
        public string GetGroupList()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/get?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 查询用户所在的分组
        /// </summary>
        /// <returns></returns>
        public string GetUser_Group()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/getid?access_token={0}"
                , config.Access_Token);
            return url;
        }
        /// <summary>
        /// 修改分组名
        /// </summary>
        /// <returns></returns>
        public string GetUpdateGroup()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/update?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 移动用户到分组
        /// </summary>
        /// <returns></returns>
        public string GetMoveUser_Group()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/members/update?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 批量移动用户分组
        /// </summary>
        /// <returns></returns>
        public string GetMoveUsers_Group()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/members/batchupdate?access_token={0}",
                config.Access_Token);
            return url;
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <returns></returns>
        public string GetDeleteGroup()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/groups/delete?access_token={0}",
                config.Access_Token);
            return url;
        }
        #endregion

        #region 素材管理
        /// <summary>
        /// 获取上传素材链接地址
        /// </summary>
        /// <returns></returns>
        public string GetPostMaterial()
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}",
                config.Access_Token);
            return url;
        }
        #endregion


        #region 二维码管理

        #endregion


        #region 本站地址定义
        public string GetUserOAthHandle()
        {
            string url = string.Format("http://{0}/mobile/login/OAuthHandle", ReqHelper.req.Url.Host);
            return url;
        }
        #endregion


        #region 支付管理
        public string GetPayUnifiedOrder()
        {
            string url = string.Format("https://api.mch.weixin.qq.com/pay/unifiedorder");
            return url;
        }
        #endregion
    }
}
