using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;

namespace Common.Alipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.4
    /// 修改日期：2016-03-08
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class Config
    {
        // 应用ID,您的APPID
        public static string app_id = "2021002107635402";

        // 支付宝网关
        public static string gatewayUrl = "https://openapi.alipay.com/gateway.do";

        // 商户私钥，您的原始格式RSA私钥    MIIEowIBAAKCAQEAwhbq/6fm3FDOH1rEVJzOKzB2uLcVJaewZpLKyVt8yVWelh1mz8JCo/kah1h4R1mICl5T+LNFgW52w57CTONx5pyhxa5NBfimEd2f9AQZhfDk7NSy6GO/El4OvEYz+oxQyAuyR+N2m/dRFaf4oSYE7Lk/mNQLETFhkCUaRQGa4Xr3fepnaj6uXRVxOU8k8RfpbmGO41iIjHPctUUAHrCDodJ1lFAGxKgzy42ZdTXeGwBMMn+oyome+AE6qvPVcM9gyzxgKwTc1f7KtOd/4FOBfOTLhZ5YsBGigFGnRX99PgUMPKseynIGbXe88rIrS4wJLi+0jpAmWbr+f0V6x0sMzQIDAQABAoIBAH3SB5OmT9WhGI5g9Ad6A3kiGMSUA0R+2a8VGBrCkTmfpIfiFYU3hKXx1Y3J/2cZlkRKdBs0dCl858bWw1S+2VbLUV/qJ4pob5gDWS8B/V3EFVOmaziVeQc6LElHJWkVz0RvuKo7vedLw6fLVTV6hRTE3oJ/a5FAtuOP+eCkMrhaMNH/cGb1BrYB3LoPeljHi//g9BhNe0+zf123YhL93SMB3mPNFIZAJRWmhMsl6hDP1JS04A1JNAIh2YT6TlYTIOydfusMx+9YG7NZC07n0DP3/+FKSLcrv83qK5IdU5NGwWtC+aTIBoZoetCvMk7Ed8ctlbKZQi2mcZ4lEXle8m0CgYEA+t6xSm1xy03SoAcVu+JXf6VPvrv45HXUHTtx4/yep7vqxjo6pfpjQUjw3nNGbFixg6BzR6reGdy8ffrz6qG1TC8T11GK1bWPWzlKVEHL8pqyhd8HPgMJTqlG8WApiYQu940e0NWJD/FBpCfXCngzqUk+RAtplBqgp97G1tFVUQcCgYEAxg76uJ8tWP/kYnszIp28LCiSC5VTiB9ItoU9Htq7fhLHLiCuJo5h39yuGxayCqlkbN5ruYybNnayVv5Yp7ka/YoPjhUgX0gC9JARCjBwp2pTp7Xk+UXgY7zav9k4AaQ9AcJQy7OYCVMnCActwhKFANqQk+yKJxGWXOPbffp6AosCgYEA96WUEGvf1JkbXfwAeXujco+HHDtFJIoofT7sW+pJ2D0KMurScHyvNIhDtuTG1Tfs7IocotfK7m7X5+kuliVR6kXi6UBGtFbJkH0KoghdwzzxFHQpGw82Rd+W97o85tKE5rz5eaAB8KnxaZImluA351o1TEiTza9Hfs/NK8xz00sCgYAGgOdquI9cpMpzEWiUcmmDabGh/71GcwCIPcfMA5K0iRlRQnF1gqdkGnL9ILmn7/gEOyl+ZkeSekNQ5/kxSLrCKI5qM/dkKZEvRqvIkd6hILSuhSKHrOu5WrI0KoXUR/hY3nCYv8T2SWy2rsWp4cHMTWEB+npu3fz5/+FtOZRYEQKBgCtKdbFCOvAcIBTzS+H3GvrYxKuT2LgDRUjoqt8jVa7ZD2RIxHqozZk4KKSULZp7X7+SZGCf4dt1wR4VOONBApyx8qVWJgEhXpnbFtS8jr9+Hut1YUwTQSIqy+ybBnWQyDB46mCOsl/BYospv22/LKkzzCCfTc40+Jnq3Mxt4Zpx
        public static string private_key = "MIIEpQIBAAKCAQEA7CZ1BIo3LQ7oD3nGjCZNPLYKsQm1N9B1QotMkvfQe+5c4ZaUsMycDOl1mmTv5G39UDIObeiHFwP92UzNG69Kmo2E0xpzLlKcQCbVeBNJsxDgQULRoUPqGScIqZRKdLnR1mqBszAUJwIqA5omp8c9F0wGfdzwK89ytkkNH9rAM9VewpVKzlmB8WQQQWmpJlnUWAi9hJbia6/b1Xk/zqf6ph5enRKr9bTsmWxzjQHP/XXyqD/kG93gOdPFeX4vKHG7BfbaTVrtmD5N9183MUsTSX6brOmPPWdECq3LvKWbmT1PHOJUZz4iD00uCP6swE0rdhSaQy5pwsDX+Oj12JDLwwIDAQABAoIBAQCsA89VQUCzx5nuO+wOho/YW1oqX0HrqCLiGAntPCpXO+fYQxGIP2KkXSJXFpNPt3a/hdbQ5IISl8u/D5yQP/192vwXsbqY0YATkrYIQzR6BR71KrD8YTYSoEZFEEm0AO8C2o4jcjAbwg3Mler2Z8DuSWipIdiocPHEKi4vN+6tUabH8QIX0521AzNrgoQJEm29gc3DZkOZWC8qjt0LT+Qp5LHPR0h6DAEw8sjy/OazUufsnIugd7eqbBNrTc20lNejUQ+wm912XPmSvh+K44U/HuQqK5WvhYHKdsFNkgJ6jWJMUKXqR+5HGKYaxotR0nOEfoV+0TxlBlgyDtmN4nkRAoGBAP7dfpEtSbPqDGgUnS7KP5ZcaSIFp9t67vbRDbRe3lL7MJo1AcziYP/VJ2jFW9ybzDZFM2aG8NAG0wANnEoJfMOdPdn9WyhI3ZUuPabuMJoaQGuDcaVECMN5K9qT1L/LkmB2WZmwApsvm68Tx87v1x0B+mMuDi7vt/ApwzevdiBNAoGBAO0zoW8d3KBpNz43g3lSVw930CdTHD/9a88MTY8izBIo0RFyIYooKhGHD6GRGcNfz0c0s+CqoGuTxyf9ScjhH4XcRbXnG+RQv5CvhVws0krAbeiOB4D6plfmB6/G3cAC6ItC+kahwDHWvllY/4jbo1eCMfM6SOorXReDQs9p2iRPAoGBAJEr9TYYh8zVWi/G3imSxlmNVc0tT4mzIn6wYZAGIbk+n1aXLulXYU0YSxRgpoyoqc+X50lVP8IewP1cw1NgyBJcdKjAqbXum1ioODHnpwOARIV5H7i1YhCF6rbcUejVuhD8GNlz6mge3B0DtWJoNy2svMTVrPqLTUidYTdM2IpZAoGBALKjC5TvXFTCaIRGspEP3dkcVnsHMGNQq3yGu3JstluGucz1sySlEumYWcFH0as94JCztDQizCW4aWhIbOUIBgF93lIb9vl/hoWkaj780nLRcPSO4j/UhKn15xwOHLqadh9LFclTz7a5DymiyO982grxVkTjRD524ejvRcWG5m4pAoGARs7OASza1TlsnW4fBvQjUPmtcfT6T4kulhhST1EhXjXECcsWygc/8ut1I7A3eDAsTnfbAxe0dYsz01rGxdFK4GMGTPhsWJUynFxS6NB8OeaU7W0zy/qddS9Ld+ZZen0pip72BScfcUbGLFbFX2oditDTwSA4MS2E2US5pP0b9EI=";
                            
        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        //public static string alipay_public_key  MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsZgWxHgG50XwAHwxFycttQB7KqF0pVh03h56B7SSqA/XZgsue5Diq0HB0jPs8LQASL8R7pTwszJ1R2gE6ghAJlg88TJcfjyJREfNVRQvjvWNhF/0DrCrg5em0Ap3XMQ69pjTasuC/SfK1C/mKsup/ucgBA2fymaPxQ5JMdDn5WsJvkhjJi+RLVZ3VPiEnNhqPh9cyybqQhmAFKDuxg9J/Vwu/6dSb1vKmCnt2qNnAXhXQBjSBwkvFFSbKxJVsiWj+iJtHo1TYfHPdqL11jR56lrpnFUZ19P20I+8Fm2Ith6rmAXWqGeqYSkYWUkpOHtMhH5pAlk0w34Xdu395/+bUQIDAQAB
        public static string alipay_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo5TxqA66pqNTBBBcDTvp+TPkvCDCTSBFDjiRzVnZGKUqhwf669fLSkBkmLysnEhA4zr+wWjGQDbu2RUryt8YSpzyBhvB1g2FGI94RYxKTO8gf/av7bUmfExbRlFe8P/ZeGoBqgTl1PesLE4XwIIjz3FJB2d6O3w/pcAvhA4OrvQLhTHICWm/WmFSZNcEEWjOmXGTMoDeoMIKuVnGfB7niXBeHEVUJIqaZl8ICmzb7kaRBRX32syOzCGArwNG1GDpEWKV5medsiJgmHl+B7uRit6nXM7XISgdnISltDvxP/JryGYomf4O7lUuZm/0cqioW4MbrGgu7RZbVo7jATwmLQIDAQAB";
                                                  //MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo5TxqA66pqNTBBBcDTvp+TPkvCDCTSBFDjiRzVnZGKUqhwf669fLSkBkmLysnEhA4zr+wWjGQDbu2RUryt8YSpzyBhvB1g2FGI94RYxKTO8gf/av7bUmfExbRlFe8P/ZeGoBqgTl1PesLE4XwIIjz3FJB2d6O3w/pcAvhA4OrvQLhTHICWm/WmFSZNcEEWjOmXGTMoDeoMIKuVnGfB7niXBeHEVUJIqaZl8ICmzb7kaRBRX32syOzCGArwNG1GDpEWKV5medsiJgmHl+B7uRit6nXM7XISgdnISltDvxP/JryGYomf4O7lUuZm/0cqioW4MbrGgu7RZbVo7jATwmLQIDAQAB
        // 签名方式
        public static string sign_type = "RSA2";

        // 编码格式
        public static string charset = "UTF-8";

    }
}