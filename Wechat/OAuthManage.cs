using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace Wechat
{
    /// <summary>
    /// 网页授权逻辑处理，
    /// 处理三步操作，处理成功，返回用户基本信息
    /// </summary>
    public class OAuthManage
    {
        #region 基本信息定义
        /// <summary>
        /// 公众号的唯一标识
        /// </summary>
        private string appid;
        /// <summary>
        /// 公众号的appsecret
        /// </summary>
        private string secret;
        /// <summary>
        /// 回调url地址
        /// </summary>
        private string redirect_uri;
        /// <summary>
        /// 获取微信用户基本信息使用snsapi_userinfo模式
        /// 如果使用静默授权，无法获取用户基本信息但可以获取到openid
        /// </summary>
        private string scope;
        public OAuthManage(string appid, string appsecret, string redirect_uri, bool IsUserInfo = true)
        {
            this.appid = appid;
            this.secret = appsecret;
            this.redirect_uri = redirect_uri;
            this.scope = IsUserInfo ? "snsapi_userinfo" : "snsapi_base";
        }
        #endregion

        #region 请求过程信息
        /// <summary>
        /// 第一步获取到的Code 值
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 第二步获取到的access_token及相关数据
        /// </summary>
        public OAuthAccess_Token TokenData = null;
        #endregion

        #region 事件定义
        /// <summary>
        /// 当处理出现异常时，触发
        /// </summary>
        public Action<Exception> OnError = null;
        /// <summary>
        /// 当获取AccessToken成功是触发
        /// </summary>
        public Action<OAuthAccess_Token> OnGetTokenSuccess = null;
        /// <summary>
        /// 当获取用户信息成功时触发
        /// </summary>
        public Action<OAuthUser> OnGetUserInfoSuccess = null;
        #endregion

        #region 第二步,回调处理
        /// <summary>
        /// 第二步，通过code换取网页授权access_token
        /// </summary>
        public void GetAccess_Token()
        {
            try
            {
                //1.处理跳转
                this.Code = ReqHelper.GetString("code");
                if (string.IsNullOrEmpty(this.Code))
                    throw new Exception("获取code参数失败或用户禁止授权获取基本信息");
                //1.发送获取access_token请求
                string url = GetAccess_TokenUrl();
                string result = NetHelper.Get(url);

                //2.解析相应结果
                TokenData = JsonConvert.DeserializeObject<OAuthAccess_Token>(result);
                if (TokenData == null)
                    throw new Exception("反序列化结果失败，返回内容为：" + result);
                //3.获取成功
                if (OnGetTokenSuccess != null)
                    OnGetTokenSuccess(TokenData);
            }
            catch (Exception ex)
            {
                Error("第二步，通过code换取网页授权access_token异常", ex);
            }
        }
        /// <summary>
        /// 刷新当前access_token
        /// </summary>
        public OAuthAccess_Token RefreshAccess_Token()
        {
            try
            {
                //1.发送请求
                string url = GetReferesh_TokenUrl();
                string result = NetHelper.Get(url);
                //2.解析结果
                OAuthAccess_Token token = JsonConvert.DeserializeObject<OAuthAccess_Token>(result);
                if (token == null)
                    throw new Exception("反序列化结果失败，返回内容：" + result);
                return token;
            }
            catch (Exception ex)
            {
                Error("刷新当前access_token失败", ex);
                return null;
            }
        }
        #endregion

        #region 第三步，获取用户基本信息
        /// <summary>
        /// 第三步，获取基本信息
        /// </summary>
        public void GetUserInfo()
        {
            try
            {
                //1.发送get请求
                string url = GetUserInfoUrl();
                string result = NetHelper.Get(url);
                if (result.Contains("errcode"))
                    throw new Exception("获取用户信息失败：" + result);
                //2.解析结果
                OAuthUser user = JsonConvert.DeserializeObject<OAuthUser>(result);
                if (user == null)
                    throw new Exception("反序列化结果失败，返回内容：" + result);
                //3.获取成功
                if (OnGetUserInfoSuccess != null)
                    OnGetUserInfoSuccess(user);
            }
            catch (Exception ex)
            {
                Error("第三步、获取用户基本信息异常", ex);
            }
        }
        #endregion

        #region 静态方法
        /// <summary>
        /// 验证授权凭证是否有效
        /// </summary>
        /// <param name="access_token">access_token</param>
        /// <param name="openid">用户针对当前公众号的openid</param>
        /// <returns></returns>
        public static bool CheckWebAccess_Token(string access_token, string openid)
        {
            try
            {
                string url = string.Format("https://api.weixin.qq.com/sns/auth?access_token={0}&openid={1}",
             access_token,
             openid);
                string result = NetHelper.Get(url);
                JObject obj = JObject.Parse(result);
                int errcode = (int)obj["errcode"];
                return errcode == 0;
            }
            catch (Exception ex)
            {
                throw new Exception("，" + ex.Message);
            }
        }
        #endregion

        #region 获取请求连接
        /// <summary>
        /// 获取Code的url 地址
        /// </summary>
        /// <returns></returns>
        public string GetCodeUrl()
        {
            string url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state=STATE#wechat_redirect",
                this.appid,
                SecurityHelper.UrlEncode(this.redirect_uri),
                this.scope);
            return url;
        }
        /// <summary>
        /// 获取access_token的url地址
        /// </summary>
        /// <returns></returns>
        private string GetAccess_TokenUrl()
        {
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                this.appid,
                this.secret,
                this.Code);
            return url;
        }
        /// <summary>
        /// 获取刷新AccessToke的地址
        /// </summary>
        /// <returns></returns>
        private string GetReferesh_TokenUrl()
        {
            string url = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}",
                this.appid,
                this.TokenData.refresh_token
                );
            return url;
        }
        /// <summary>
        /// 获取用户基本信息地址
        /// </summary>
        /// <returns></returns>
        private string GetUserInfoUrl()
        {
            string url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN",
                this.TokenData.access_token,
                this.TokenData.openid);
            return url;
        }
        #endregion
        private void Error(string msg, Exception inner)
        {
            if (this.OnError != null)
            {
                this.OnError(new Exception(msg, inner));
            }
        }
    }
    /// <summary>
    /// 获取Access_Token或者刷新返回的数据对象
    /// </summary>
    public class OAuthAccess_Token
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        /// <summary>
        /// 用户针对当前公众号的唯一标识
        /// 关注后会产生，返回公众号下页面也会产生
        /// </summary>
        public string openid { get; set; }
        public string scope { get; set; }
        /// <summary>
        /// 当前用户的unionid，只有在用户将公众号绑定到微信开放平台帐号后
        /// </summary>
        public string unionid { get; set; }
    }
}
