using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.SMSHelper;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Member_InfoImp : EFBase<DataBase.Member_Info>
    {

        public string GetTextDL(Member_Info m)
        {
            if (m.DLName == "省级代理")
            {
                return m.Sheng;
            }
            else if (m.DLName == "市级代理")
            {
                return m.Sheng + m.Shi;
            }
            else if (m.DLName == "县级代理")
            {
                return m.Sheng + m.Shi + m.Xian;
            }
            else
            {
                return "";
            }
        }
        public JsonHelp ServiceDL(Member_Info entity)
        {
            var json = new JsonHelp() { Status = "n", Msg = "审核失败" };

            if (entity.IsServiceCenter == "否")
            {
                entity.IsServiceCenter = "审核中";
            }
            if (string.IsNullOrEmpty(entity.JingDu.ToString()))
            {
                json.Msg = "请填写正确的经度";
                return json;
            }
            if (string.IsNullOrEmpty(entity.WeiDu.ToString()))
            {
                json.Msg = "请填写正确的纬度";
                return json;
            }


            if (DB.Member_Info.Update(entity))
            {
                //添加操作日志
                DB.SysLogs.setMemberLog("ServiceApply", "申请代理事件已提交，申请人Code：[" + entity.Code + "]");
                json = new JsonHelp() { Status = "y", Msg = "您的申请已提交，请等待管理员的审核！" };
            }

            json.Status = "y";
            json.Msg = "操作成功";
            return json;
        }
        public JsonHelp CancelCenterDL(string id)
        {
            JsonHelp json = new JsonHelp();
            var m = DB.Member_Info.FindEntity(id);
            m.Shi = "";
            m.ShengId = 0;
            m.ShiId = 0;
            m.Sheng = "";
            m.Xian = "";
            m.XianId = 0;
            m.IsDL = "否";
            var r = DB.Member_Info.Update(m);
            if (r)
            {
                json.IsSuccess = true;
                json.Msg = "操作成功";
                DB.SysLogs.setAdminLog("Cancel", "会员：[" + m.Code + "]被取消店主资格");
            }
            return json;
        }
        public JsonHelp IsokDL(string id, string type)
        {
            var json = new JsonHelp();
            var m = DB.Member_Info.FindEntity(id);
            m.IsDL = type;
            //如果是驳回的话,返回各种奖金状态
            if (type.Equals("否"))
            {

                m.Shi = "";
                m.ShengId = 0;
                m.ShiId = 0;
                m.Sheng = "";
                m.Xian = "";
                m.XianId = 0;

            }
            var r = DB.Member_Info.Update(m);
            if (r == true)
            {
                json.IsSuccess = true;
                json.Msg = "操作成功";
            }
            else
            {
                json.IsSuccess = false;
                json.Msg = "操作失败";
            }
            return json;
        }

        #region 本站使用
        /// <summary>
        /// 默认获取第一个用户对象
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public Member_Info GetModelByOpenID(string openid)
        {
            return DB.Member_Info.Where(q => q.OpenID == openid).FirstOrDefault();
        }

        /// <summary>
        /// 获取推荐关系的社区店
        /// </summary>
        /// <param name="curUser"></param>
        /// <returns></returns>
        public string GetDianCode(Member_Info curUser)
        {
            string recID = curUser.RecommendId;
            string code = "空";
            while (string.IsNullOrEmpty(recID) == false)
            {
                Member_Info model = FindEntity(recID);
                if (model == null)
                    break;
                if (model.MemberLevelId == 4)
                {
                    code = model.Code;
                    break;
                }
                //进入下一次循环
                if (model.Code == "admin") break;
                recID = model.RecommendId;
            }

            return code;
        }
        #endregion

        #region 图表

        #region 获得本月的业绩趋势图
        /// <summary>
        /// 获得本月的业绩趋势图
        /// </summary>
        /// <returns></returns>
        public string getActiveMemberLine()
        {
            DataSet ds = new DataSet();
            var data = FindSqlToJson(@"select SUM(ActiveAmount) as count,DATEPART(DAY,ActiveTime)  as activetime from Member_Info 
                                            where DATEDIFF(Month, ActiveTime, GETDATE()) = 0 group by DATEPART(DAY, ActiveTime) order by ActiveTime desc");
            DataTable dt = Common.JsonConverter.JsonToDataTable(data);
            if (dt != null)
            {
                dt.TableName = "本月业绩走势图";
                ds.Tables.Add(dt.Copy());
                return EchartsHelp.EchartJsonToMultipleBar(ds, SysDictionary.GetMonth(), "activetime", "count");
            }
            else
            {
                return "";
            }

        }
        #endregion

        #region 会员分布MAP
        /// <summary>
        /// 会员分布MAP
        /// </summary>
        /// <returns></returns>
        public string getMemberDistribution()
        {
            StringBuilder sb = new StringBuilder();
            var data = FindSqlToJson(@"SELECT COUNT(*) AS count,ProvName FROM dbo.Member_Info WHERE IsActive='已激活' GROUP BY ProvName");
            DataTable dt = Common.JsonConverter.JsonToDataTable(data);
            // { name: '北京', value: 1 },
            sb.Append("[");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows.Cast<DataRow>().OrderByDescending(r => int.Parse(r["count"].ToString())))
                {
                    if (dr["ProvName"].ToString() != "")
                    {
                        sb.Append("{ \"name\":\"" + dr["ProvName"].ToString().Trim().Replace('省', ' ').Replace("特别行政区", " ").Replace("壮族自治区", " ").Replace("回族自治区", " ").Replace("维吾尔自治区", " ").Replace("自治区", " ").Replace("特别行政区", " ").Replace('市', ' ').Trim() + "\",\"value\":" + dr["count"].ToString() + "},");
                        //sb.Append("{ \"name\":\"" + dr["ProvName"].ToString().Trim().Replace('省', ' ').Replace('市', ' ').Trim() + "\",\"value\":" + dr["count"].ToString() + "},");
                    }
                }
                return sb.ToString().TrimEnd(',') + "]";
            }
            else
            {
                return "[{ name: \"\", value: 0 }]";
            }
        }
        #endregion

        #endregion

        #region 网络图
        /// <summary>
        /// 获取网格节点
        /// </summary>
        /// <param name="code">根节点登陆名</param>
        /// <param name="type">轨数类型，1=1轨 2=2轨 3=3轨以此类推</param>
        /// <param name="w">总宽度</param>
        /// <param name="h">每层高度间隔</param>
        /// <param name="c">获取层数</param>
        /// <returns></returns>
        public List<Relation> GetRelation(string code, int type, int w, int h, int c)
        {
            var member = DB.Member_Info.FindEntity(a => a.Code == code);
            var rootPosition = member.Position;
            List<Relation> list = new List<Relation>();
            var len = rootPosition.Length + (c - 1);
            var childs = DB.Member_Info.Where(a => a.Position.StartsWith(rootPosition) && a.Position.Length <= len).Select(a => new
            {
                Position = a.Position,
                //NickName = a.NickName,
                Code = a.Code,
                MemberId = a.MemberId,
                IsActive = a.IsActive
            }).ToList();

            var active = childs.Where(a => a.IsActive == "已激活" && a.Position.Length < rootPosition.Length + c - 1).ToList();
            foreach (var item in active)
            {
                for (int i = 1; i <= type; i++)
                {
                    if (!childs.Any(a => a.Position == item.Position + i))
                        childs.Add(new
                        {
                            Position = item.Position + i,
                            Code = "暂无",
                            MemberId = "",
                            IsActive = ""
                        });
                }
            }

            var layer = childs.Max(p => p.Position.Length) - childs.Min(p => p.Position.Length) + 1;
            foreach (var item in childs)
            {
                #region 生成每个点的  x,y，name,id等信息
                var position2 = item.Position.Substring(rootPosition.Length).Replace("1", "0").Replace("2", "1").Replace("3", "2"); //转成2进制
                int x = w / 2;
                int y = h;
                if (position2.Length > 0)
                {
                    var position10 = Tools.ConvertNTo10(position2, type);  //转成10进制
                    y = (position2.Length + 1) * h;
                    var pingjunkuan = w / ((int)Math.Pow(type, (position2.Length + 1) - 1));
                    x = (position10 + 1) * pingjunkuan - pingjunkuan / 2;
                }
                itemStyle itemstyle = new itemStyle();
                normal normal = new normal();
                var code1 = item.Code;

                //这里颜色按 正式会员，未激活会员，未注册
                if (item.IsActive == "已激活")
                {
                    normal.color = "#D53A35";
                }
                else if (item.IsActive == "未激活")
                {
                    normal.color = "#ff99ee";
                }
                else
                {
                    normal.color = "#36C6D3";
                }
                itemstyle.normal = normal;
                var model = new Relation() { id = item.Position, name = item.Code, x = x, y = y, itemStyle = itemstyle };
                list.Add(model);
                #endregion
            }

            return list;
        }

        /// <summary>
        /// 获取网络图关系
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<guanxi> guanxi(string code, int type, int layer)
        {
            List<guanxi> rlist = new List<guanxi>();
            var member = DB.Member_Info.FindEntity(a => a.Code == code);
            var rootPosition = member.Position;

            var len = rootPosition.Length + layer - 1;
            var childs = DB.Member_Info.Where(a => a.Position.StartsWith(rootPosition) && a.Position.Length <= len).Select(a => new
            {
                Position = a.Position,
                IsActive = a.IsActive
            }).ToList();

            var active = childs.Where(a => a.IsActive == "已激活" && a.Position.Length < len).ToList();
            foreach (var item in active)
            {
                for (int i = 1; i <= type; i++)
                {
                    if (!childs.Any(a => a.Position == item.Position + i))
                        childs.Add(new
                        {
                            Position = item.Position + i,
                            IsActive = ""
                        });
                }
            }
            //设置关系
            foreach (var item in childs.OrderBy(a => a.Position.Length))
            {
                //找它的下级
                var child = childs.Where(a => a.Position.Length == item.Position.Length + 1 && a.Position.StartsWith(item.Position));
                foreach (var item2 in child)
                {
                    guanxi entity = new guanxi();
                    entity.source = item.Position;
                    entity.target = item2.Position;
                    rlist.Add(entity);
                }
            }
            return rlist;
        }
        #endregion

        #region 更改数据

        #region 激活会员
        private static object lockActive = new object();
        /// <summary>
        /// 激活会员
        /// </summary>
        /// <param name="memberId">要激活的会员id</param>
        /// <param name="curUserId">当前操作人员的id</param>
        /// <param name="isNullActive">是否空激活</param>
        /// <param name="isAdmin">是否后台激活，true: 后台激活，不扣店主电子币，也不给保单费</param>
        /// <returns></returns>
        public JsonHelp ActiveMember(string memberId, Account Current, bool isNullActive, bool isAdmin = false)
        {
            JsonHelp json = new JsonHelp();
            lock (lockActive)
            {
                using (var tran = DB.Member_Info.BeginTransaction)
                {
                    try
                    {
                        #region 判断是否已激活
                        var mModel = DB.Member_Info.FindEntity(memberId);
                        var levels = DB.Sys_Level.Where().OrderBy(a => a.Investment).ToList();
                        var mLevel = levels.FirstOrDefault(a => a.LevelId == mModel.MemberLevelId);   //当前会员级别（被激活人）

                        if (mModel.IsActive == "已激活")
                        {
                            json.Msg = "已激活，不能再次激活！";
                            DB.Rollback();
                            return json;
                        }
                        #endregion

                        if (isNullActive == true)
                        {
                            mModel.IsActive = "已激活";
                            mModel.ActiveTime = DateTime.Now;
                            mModel.ActiveAmount = 0;
                            mModel.IsNullActive = true;
                            mModel.Commission = 0;// - mLevel.Investment;//空激活时，收益如需回填，可设为回填同等级别的负数，
                            mModel.CommissionSum = 0;
                            mModel.ShopCoins = 0;
                            var u1 = DB.Member_Info.Update(mModel);
                            if (u1 == false)
                            {
                                json.Msg = "激活会员失败！";
                                DB.Rollback();
                                return json;
                            }
                        }
                        else    //非空单，加收益
                        {

                            #region 查询一些基础数据，后面用的上(当前会员，推荐人，上级，分公司 以及他们分别对应的级别)            
                            //var listMember = DB.Member_Info.Where(a => a.IsActive == "已激活").ToList(); //暂时不缓存所有会员了，后面直接取数据库的
                            //listMember.Add(mModel);
                            //List<Member_Info> listMember = null;
                            var cModel = DB.Member_Info.FindEntity(mModel.ServiceCenterId); //推荐人           
                            #endregion

                            #region 先判断自己账户中报单积分是否大于会员激活金额
                            if (isAdmin == false)
                            {
                                if (DB.XmlConfig.XmlSite.IsJiHuo == false)
                                {
                                    json.Msg = "店主激活权限已关闭！";
                                    DB.Rollback();
                                    return json;
                                }
                                mModel.ActiveAmount = mLevel.Investment; //先给激活金额赋值，这里的判断可以放到推广奖奖那里
                                var coins = mModel.ActiveAmount * DB.XmlConfig.XmlSite.ManagerAwardRate / 100m;
                                var Commission = mModel.ActiveAmount * DB.XmlConfig.XmlSite.StockPrice / 100m;
                                if (isAdmin == false && cModel.Coins < coins)
                                {
                                    json.Msg = "报单积分不足，不能激活会员！";
                                    return json;
                                }
                                if (isAdmin == false && cModel.Commission < Commission)
                                {
                                    json.Msg = "收益不足，不能激活会员！";
                                    return json;
                                }
                                cModel.Coins -= coins;
                                cModel.Commission -= Commission;
                                //流水账单
                                Fin_LiuShui _liushui = new Fin_LiuShui();
                                _liushui.MemberId = cModel.MemberId;
                                _liushui.Code = cModel.Code;
                                _liushui.NickName = cModel.NickName;
                                _liushui.Type = "流水账单";
                                _liushui.Comment = "激活报单积分(-)";
                                _liushui.Amount = coins;
                                _liushui.CreateTime = DateTime.Now;
                                DB.Fin_LiuShui.Insert(_liushui);
                                _liushui.MemberId = cModel.MemberId;
                                _liushui.Code = cModel.Code;
                                _liushui.NickName = cModel.NickName;
                                _liushui.Type = "流水账单";
                                _liushui.Comment = "激活收益(-)";
                                _liushui.Amount = Commission;
                                _liushui.CreateTime = DateTime.Now;
                                DB.Fin_LiuShui.Insert(_liushui);
                                var Geng = DB.Member_Info.Update(cModel);
                                if (Geng == false)
                                {
                                    json.Msg = "激活会员失败！";
                                    DB.Rollback();
                                    return json;
                                }
                            }
                            #endregion

                            #region 1.激活会员本身
                            mModel.IsActive = "已激活";
                            mModel.ActiveTime = DateTime.Now;
                            mModel.ActiveAmount = mLevel.Investment;
                            mModel.ShopCoins = Convert.ToDecimal(mLevel.TeamAwardRange);
                            mModel.IsNullActive = false;
                            var u = DB.Member_Info.Update(mModel);
                            if (u == false)
                            {
                                json.Msg = "激活会员失败！";
                                DB.Rollback();
                                return json;
                            }
                            #endregion

                            #region 2.报单补助
                            if (isAdmin == false)
                            {
                                json = DB.Jiang.BaoDan(cModel, mModel);
                                if (json.IsSuccess == false)
                                {
                                    json.Msg = "报单补助分配失败！";
                                    DB.Rollback();
                                    return json;
                                }
                            }
                            #endregion

                            #region 3.推广奖
                            json = DB.Jiang.Recommend(mModel);
                            if (json.IsSuccess == false)
                            {
                                json.Msg = "推广奖分配失败！";
                                DB.Rollback();
                                return json;
                            }
                            #endregion

                            #region 平级奖(层奖)
                            json = DB.Jiang.FindPoint(mModel, levels);
                            if (json.IsSuccess == false)
                            {
                                json.Msg = "平级奖分配失败！";
                                DB.Rollback();
                                return json;
                            }
                            #endregion

                            //#region 更新结余
                            //json = DB.Jiang.UpdateLRAmount(mModel);
                            //if (json.IsSuccess == false)
                            //{
                            //    json.Msg = "更新上级业绩结余出错！";
                            //    DB.Rollback();
                            //    return json;
                            //}
                            //#endregion

                            #region 拓展奖(对碰)--服务奖拿对碰的
                            json = DB.Jiang.DuiPeng(mModel, levels);
                            if (json.IsSuccess == false)
                            {
                                json.Msg = "拓展奖分配失败！";
                                DB.Rollback();
                                return json;
                            }
                            #endregion


                        }
                        if (Current.LoginType == Enums.LoginType.member.ToString())
                        {
                            DB.SysLogs.setMemberLog("Active", "激活会员成功，会员编号：[" + mModel.Code + "]");
                        }
                        else
                        {
                            DB.SysLogs.setAdminLog("Active", "激活会员成功，会员编号：[" + mModel.Code + "]");
                        }
                        tran.Complete();
                        json.IsSuccess = true;
                        json.Msg = "激活成功";
                    }
                    catch (Exception e)
                    {
                        json.IsSuccess = false;
                        DB.Rollback();
                        LogHelper.Error("激活会员失败：" + e.Message + "，会员id：" + memberId);
                    }
                }
            }
            return json;
        }

        public void SendSMS1(string mobile, string content)
        {

            ZTSMS _sms = new ZTSMS("676767", "JNWZ200915", "JNWZ200915", "【物来惠商城】");
            _sms.OnSuccess = (count, remain) =>
            {
                Console.WriteLine("短信发送成功" + count);
                LogHelper.Info($"zc，短信发送成功[{mobile}]，当前余额：{remain}");
            };
            _sms.OnError = (msg) =>
            {
                Console.WriteLine($"短信发送失败[{mobile}]，原因：{msg}");
                LogHelper.Info($"zc，短信发送失败[{mobile}]，原因：{msg}");
            };
            _sms.Send(mobile, content);
        }
        #endregion

        #region 保存（新增与修改）
        /// <summary>
        /// 保存（新增与修改）
        /// </summary>
        /// <param name="entity">当前实体对象</param>
        /// <param name="loginType">当前session名称</param>
        /// <param name="AddCommission">增减收益</param>
        /// <param name="AddCommissionSum">增减收益累计</param>
        /// <param name="AddCoins">增减电子币</param>
        /// <returns></returns>
        public JsonHelp Save(Member_Info entity, Enums.LoginType loginType, string NiCheng, string Pwd2,
            decimal AddCommission = 0, decimal AddCommissionSum = 0, decimal AddCoins = 0, decimal AddShopCoins = 0, decimal AddScores = 0, decimal AddTourScores = 0)
        {
            JsonHelp json = new JsonHelp();

            if (string.IsNullOrEmpty(entity.MemberId))
            {
                entity.LoginPwd = Common.CryptHelper.DESCrypt.Encrypt(entity.LoginPwd);
                entity.Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(entity.Pwd2);
                entity.MemberId = Guid.NewGuid().ToString();
                entity.LastLogin = DateTime.Now;
                #region 检验数据                   
                var list = DB.Member_Info.Where(a => a.Code == entity.Code || a.Pwd3 == entity.RecommendCode || a.Code == entity.UpperCode || a.Code == entity.ServiceCenterCode).ToList();
                //1. code不为空，不重复
                if (string.IsNullOrEmpty(entity.Code))
                {
                    json.Msg = "会员编号不能为空";
                    return json;
                }
                else if (Any(a => a.Code == entity.Code))
                {
                    json.Msg = "会员编号已存在！";
                    return json;
                }
                entity.Pwd3 = RndNum(6);
                while (Any(a => a.Pwd3 == entity.Pwd3))
                {
                    entity.Pwd3 = RndNum(6);
                }

                //2.推荐人，分公司，安置人 是否正确,位置是否被占用
                var upper = list.FirstOrDefault(a => a.Code == entity.UpperCode);
                var rem = list.FirstOrDefault(a => a.Pwd3 == entity.RecommendCode);
                var center = list.FirstOrDefault(a => a.Code == entity.ServiceCenterCode && a.IsServiceCenter == "是");
                if (rem == null)
                {
                    json.Msg = "推荐人不存在！";
                    return json;
                }
                else if (rem.IsActive != "已激活")
                {
                    json.Msg = "推荐人未激活！";
                    return json;
                }
                else
                {
                    entity.RecommendId = rem.MemberId;
                    entity.RecommendName = rem.NickName;
                    entity.RecommendCode = rem.Code;
                    entity.RPosition = GetPosition(rem);//推荐位置关系
                }
                //if (entity.RecPosition.StartsWith(upper.RecPosition) == true)
                //{
                //    json.Msg = "不允许跨区注册！";
                //    return json;
                //}

                //if (rem.IsServiceCenter == "是")
                //{
                //    entity.ServiceCenterCode = rem.Code;
                //    entity.ServiceCenterId = rem.MemberId;
                //}
                //else
                //{
                //    entity.ServiceCenterCode = rem.ServiceCenterCode;
                //    entity.ServiceCenterId = rem.ServiceCenterId;
                //}


                #endregion


                #region 设置默认值
                entity.MemberLevelId = 0;
                entity.MemberLevelName = "普通会员";
                entity.IsDL = "否";
                entity.Shi = "";
                entity.Xian = "";
                entity.Sheng = "";
                entity.ShiId = 0;
                entity.XianId = 0;
                entity.ShengId = 0;
                entity.IsActive = "已激活";
                entity.IsLockCommission = false;
                entity.IsLockDraw = false;
                entity.IsLock = "否";
                entity.IsServiceCenter = "否";
                entity.Commission = 0;
                entity.CommissionSum = 0;
                entity.Coins = 0;
                entity.Scores = 0;
                entity.ActiveTime = DateTime.Now;
                entity.Layered = string.Empty;
                entity.LAmount = 0;
                entity.ActiveAmount = 0;
                entity.RAmount = 0;
                entity.FHSum = 0;
                entity.InteractionAmount = 0;
                entity.FamousBrandFund = 0;
                entity.IsNullActive = false;
                entity.IsSub = false;
                entity.AppellationLeveId = 0;
                entity.AppellationLeveName = "";
                entity.DLId = 0;
                entity.DLName = "无级别";
                entity.ZCId = 0;
                entity.ZCName = "无级别";
                entity.Position = "";
                entity.ShopCoins = 0;

                entity.JingDu = 0;
                entity.WeiDu = 0;
                entity.ShopImage = "";
                entity.ShopName = "";
                entity.DiZhi = "";
                #endregion

                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "注册成功";
                    //添加操作日志
                    if (loginType == Enums.LoginType.admin)
                    {
                        DB.SysLogs.setAdminLog("Save", "注册会员Code为[" + entity.Code + "]，操作成功");
                    }
                    else if (loginType == Enums.LoginType.member)
                    {
                        DB.SysLogs.setMemberLog("Save", "注册会员Code为[" + entity.Code + "]，操作成功");
                    }
                    else
                    {
                        DB.SysLogs.setNoLoginLog("Reg", "注册会员Code为[" + entity.Code + "]，操作成功");
                    }
                }
            }
            else
            {
                //前台的修改，只修改一些会员信息的字段（姓名，性别，身份证，银行账户信息，联系信息）
                var model = DB.Member_Info.FindEntity(entity.MemberId);

                model.NickName = entity.NickName;
                model.Sex = entity.Sex;
                model.IdCode = entity.IdCode;

                model.BankAccount = entity.BankAccount;
                model.BankAddress = entity.BankAddress;
                model.BankCode = entity.BankCode;
                model.BankName = entity.BankName;
                model.OpenBank = entity.OpenBank;

                model.Mobile = entity.Mobile;
                model.QQ = entity.QQ;
                model.PostAddress = entity.PostAddress;
                model.Alipay = entity.Alipay;
                model.WeiXin = entity.WeiXin;
                model.Email = entity.Email;

                model.ProvId = entity.ProvId;
                model.ProvName = entity.ProvName;
                model.CityId = entity.CityId;
                model.CityName = entity.CityName;
                model.CountyId = entity.CountyId;
                model.CountyName = entity.CountyName;

                #region 后台的会员个人信息保存
                if (loginType == Enums.LoginType.admin)
                {
                    if (AddCommission > 0 || AddCommission < 0 || AddCommissionSum > 0 || AddCommissionSum < 0 || AddCoins > 0 || AddCoins < 0 || AddShopCoins > 0 || AddShopCoins < 0)
                    {
                        //var Pwd = Common.CryptHelper.DESCrypt.Encrypt(Pwd2);
                        //var admin = DB.Member_Info.FindEntity(p => p.Code == "admin");
                        //if (Pwd != admin.Pwd2)
                        //{
                        //    json.Msg = "支付密码不正确,增减失败！";
                        //    return json;
                        //}
                    }
                    //model.ZCId = entity.ZCId;
                    //model.ZCName = DB.Jiang.getService(entity.ZCId.Value);
                    model.MemberLevelId = entity.MemberLevelId;
                    if (model.MemberLevelId == 0)
                    {
                        model.MemberLevelName = "普通会员";
                    }
                    else
                    {
                        model.MemberLevelName = entity.MemberLevelName;
                    }
                    //if (entity.IsServiceCenter != "否")
                    //{
                    //    if (model.AppellationLeveId == 0)
                    //    {
                    //        if (NiCheng == "会员")
                    //        {
                    //            json.Msg = "请选择店主级别！";
                    //            return json;
                    //        }
                    //    }
                    //}
                    //if (NiCheng != "会员")
                    //{
                    //    if (NiCheng == "直营店")
                    //    {
                    //        model.AppellationLeveId = 1;
                    //        model.AppellationLeveName = "直营店";
                    //    }
                    //    else if (NiCheng == "专卖店")
                    //    {
                    //        model.AppellationLeveId = 2;
                    //        model.AppellationLeveName = "专卖店";
                    //    }

                    //}
                    Fin_LiuShui _liushui = new Fin_LiuShui();
                    if (AddCommission != 0)
                    {
                        DB.Fin_LiuShui.AddLS(model.MemberId, AddCommission, "后台增减余额");
                    }
                    if (AddScores > 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "积分(+)";
                        _liushui.Amount = AddScores;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    else if (AddScores < 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "积分(-)";
                        _liushui.Amount = AddScores;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);

                    }
                    if (AddCoins > 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "报单积分(+)";
                        _liushui.Amount = AddCoins;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    else if (AddCoins < 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "报单积分(-)";
                        _liushui.Amount = AddCoins;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }

                    if (AddShopCoins > 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "推广奖(+)";
                        _liushui.Amount = AddShopCoins;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    else if (AddShopCoins < 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "推广奖(-)";
                        _liushui.Amount = AddShopCoins;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }

                    if (AddCommissionSum > 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "收益累计(+)";
                        _liushui.Amount = AddCommissionSum;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    else if (AddCommissionSum < 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "收益累计(-)";
                        _liushui.Amount = AddCommissionSum;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    //if (AddScores > 0)
                    //{
                    //    //流水账单                    
                    //    _liushui.MemberId = model.MemberId;
                    //    _liushui.Code = model.Code;
                    //    _liushui.NickName = model.NickName;
                    //    _liushui.Type = "充值账单";
                    //    _liushui.Comment = "配货价(+)";
                    //    _liushui.Amount = AddScores;
                    //    _liushui.CreateTime = DateTime.Now;
                    //    DB.Fin_LiuShui.Insert(_liushui);
                    //}
                    //else if (AddScores != 0)
                    //{
                    //    //流水账单                    
                    //    _liushui.MemberId = model.MemberId;
                    //    _liushui.Code = model.Code;
                    //    _liushui.NickName = model.NickName;
                    //    _liushui.Type = "充值账单";
                    //    _liushui.Comment = "配货价(-)";
                    //    _liushui.Amount = AddScores;
                    //    _liushui.CreateTime = DateTime.Now;
                    //    DB.Fin_LiuShui.Insert(_liushui);
                    //}
                    if (AddTourScores > 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "商城积分(+)";
                        _liushui.Amount = AddTourScores;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    else if (AddTourScores != 0)
                    {
                        //流水账单                    
                        _liushui.MemberId = model.MemberId;
                        _liushui.Code = model.Code;
                        _liushui.NickName = model.NickName;
                        _liushui.Type = "充值账单";
                        _liushui.Comment = "商城积分(-)";
                        _liushui.Amount = AddTourScores;
                        _liushui.CreateTime = DateTime.Now;
                        DB.Fin_LiuShui.Insert(_liushui);
                    }
                    model.IsLock = entity.IsLock;
                    model.Commission += AddCommission;
                    model.CommissionSum += AddCommissionSum;
                    model.Coins += AddCoins;
                    model.ShopCoins += AddShopCoins;
                    model.Scores += AddScores;
                    model.TourScores += AddTourScores;
                    //model.IsServiceCenter = entity.IsServiceCenter;
                    model.IsLockCommission = entity.IsLockCommission;
                    model.IsLockFen = entity.IsLockFen;
                    model.IsLockDraw = entity.IsLockDraw;
                    model.StarLevel = entity.StarLevel;
                    if (model.Commission < 0 || model.CommissionSum < 0 || model.Coins < 0 || model.ShopCoins < 0 || model.Scores < 0 || model.TourScores < 0)
                    {
                        json.Msg = "增减分后的最终值不能小于0";
                        return json;
                    }
                }
                #endregion

                if (DB.Member_Info.Update(model))
                {
                    json.Status = "y";
                    json.Msg = "修改成功";
                    //添加操作日志
                    if (loginType == Enums.LoginType.admin)
                    {
                        DB.SysLogs.setAdminLog("Save", "修改会员Code为[" + model.Code + "]，操作成功");
                    }
                    else if (loginType == Enums.LoginType.member)
                    {
                        DB.SysLogs.setMemberLog("Save", "修改会员Code为[" + model.Code + "]，操作成功");
                    }
                }
            }
            return json;
        }
        #endregion

        #region 删除       
        public JsonHelp Delete(string idList, Enums.LoginType type = Enums.LoginType.admin)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                var id = idList.TrimEnd(',').Split(',').ToList();

                try
                {
                    //开启事务，可以不使用事务,也可以使用多个事务
                    //db.BeginTran();
                    if (DB.Member_Info.Any(a => id.Contains(a.RecommendId) || a.Commission>0) || id.Contains("C3B57B68-3BBF-45DA-9B16-B3BE88F2A535") )
                    {
                        json.Msg = "不可删除已推荐过会员或者余额大于0的记录";
                        return json;
                    }
                    var codes = DB.Member_Info.Where(a => id.Contains(a.MemberId)).Select(a => a.Code).ToList().Aggregate((m, n) => m + "," + n);
                    if (DB.Member_Info.Delete(a => id.Contains(a.MemberId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                        //添加操作日志
                        if (type == Enums.LoginType.admin)
                        {
                            DB.SysLogs.setAdminLog("Delete", "删除 会员编号为【" + codes + "】的会员信息");
                        }
                        else if (type == Enums.LoginType.member)
                        {
                            DB.SysLogs.setMemberLog("Delete", "删除 会员编号为【" + codes + "】的会员信息");
                        }
                    }
                }
                catch (Exception ex)
                {
                    //回滚事务
                    //DB.Rollback();
                    throw ex;
                }
                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 店主 申请、审核、取消
        /// <summary>
        /// 申请店主
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonHelp ServiceApply(Member_Info entity)
        {
            var json = new JsonHelp() { Status = "n", Msg = "审核失败" };
            if (entity.IsServiceCenter == "否")
            {
                entity.IsServiceCenter = "审核中";
                if (DB.Member_Info.Update(entity))
                {
                    //添加操作日志
                    DB.SysLogs.setMemberLog("ServiceApply", "申请店主事件已提交，申请人Code：[" + entity.Code + "]");
                    json = new JsonHelp() { Status = "y", Msg = "您的申请已提交，请等待管理员的审核！" };
                }
            }
            return json;
        }
        /// <summary>
        /// 审核申请店主通过或驳回
        /// </summary>
        /// <param name="id">会员id</param>
        /// <param name="type">通过：是  驳回：否</param>
        /// <returns></returns>
        public JsonHelp IsokServerCenter(string id, string type)
        {
            var json = new JsonHelp();
            var m = DB.Member_Info.FindEntity(id);
            m.IsServiceCenter = type;
            if (type == "否")
            {
                m.ShopImage = "";
                m.JingDu = 0;
                m.WeiDu = 0;
                m.ShopImage = "";
                m.ShopName = "";
            }
            var r = DB.Member_Info.Update(m);

            //是的话产生跳排奖金
            if (type == "是")
            {
                DB.Jiang.Recommend(m);
                //跳排

                var umodel = GetXiaoGongPaiMember(m);
                if (umodel != null)
                {
                    GetXiaHua(m, umodel, 2);
                }



            }


            if (r == true)
            {
                json.IsSuccess = true;
                json.Msg = "操作成功";
            }
            else
            {
                json.IsSuccess = false;
                json.Msg = "操作失败";
            }
            return json;
        }

        private Member_Info GetXiaoGongPaiMember(Member_Info m)
        {
            var Recommend = DB.Member_Info.FindEntity(m.RecommendId);
            while (Recommend != null)
            {
                if (Recommend.IsServiceCenter == "是")
                {
                    return Recommend;
                }
                else
                {
                    Recommend= DB.Member_Info.FindEntity(Recommend.RecommendId);
                }
            }
            return null;
        }

        public void GetXiaHua(Member_Info mModel, Member_Info member, int count)
        {
            Member_Info uppermodel = new Member_Info();


            uppermodel = GetTiaoUpmodelPosition(count, member);

            string position = "";
            if (uppermodel != null)
            {
                position = GetPosition(uppermodel, count);
            }
            mModel.Position = position;
            mModel.UpperId = uppermodel.MemberId;
            mModel.UpperName = uppermodel.NickName;
            mModel.UpperCode = uppermodel.Code;
            //if (isZi.Equals("否"))
            //{
            DB.Member_Info.Update(mModel);

            //产生奖金
            while (uppermodel != null)
            {
                var uppercount = DB.Member_Info.Count(a => a.Position.StartsWith(uppermodel.Position));
                if (uppercount == 7)
                {
                    DB.Jiang.InsertFin(uppermodel, mModel, DB.XmlConfig.XmlSite.DaiLiJinTie, "代理津贴", "代理津贴");
                    break;
                }
                uppermodel = DB.Member_Info.FindEntity(uppermodel.UpperId);
            }
        }

        public string GetPosition(Member_Info uppermodel, int count)
        {
            string position = "";
            List<Member_Info> ulist = DB.Member_Info.Where(a => a.UpperId == uppermodel.MemberId && a.Position.StartsWith(uppermodel.Position)).ToList();// GetModelList("Upperid='" + uppermodel.MemberId + "'");
            if (ulist != null)
            {
                if (ulist.Count < count)
                {
                    if (ulist.Count == 0)
                    {
                        position = uppermodel.Position + "1";
                    }
                    else
                    {
                        List<int> positionnum = new List<int>();
                        for (int i = 0; i < ulist.Count; i++)
                        {
                            //int po = Convert.ToInt32(ulist[i].Position.Replace("A", ""));
                            string positions = ulist[i].Position.ToString().Substring(ulist[i].Position.ToString().Length - 1, 1);
                            positionnum.Add(Convert.ToInt32(positions));
                        }
                        //获取最小的一个序列差级，是position的数字
                        var result = Enumerable.Range(1, count).Except(positionnum);
                        List<int> resultlist = new List<int>();
                        foreach (int x in result)
                        {
                            resultlist.Add(x);
                        }
                        resultlist.Sort();
                        position = uppermodel.Position + resultlist[0];
                    }
                }
            }
            return position;
        }
        public Member_Info GetTiaoUpmodelPosition(int count, Member_Info rModel)
        {
            //查找下面所有点位安置关系不足3个的

            List<Member_Info> mlist = DB.Member_Info.FindListBySql("select * from (select a.* ,(select count(*) from Member_Info as b where b.UpperId=a.MemberId  and  b.position like '" + rModel.Position + "%') sonscount from Member_Info as a ) as c where  position like '"
                + rModel.Position + "%' and c.sonscount<" + count + " order by len(position),c.sonscount,position").ToList();
            Member_Info model = mlist[0];
            Member_Info uppermodel = DB.Member_Info.Where(a => a.Position == model.Position).FirstOrDefault();
            return uppermodel;
        }
        /// <summary>
        /// 取消店主
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonHelp CancelCenter(string id)
        {
            JsonHelp json = new JsonHelp();
            var m = DB.Member_Info.FindEntity(id);
            if (m.Code == "admin")
            {
                json.Msg = "系统默认不允许取消admin的店主资格";
                return json;
            }
            m.IsServiceCenter = "否";
            var r = DB.Member_Info.Update(m);
            if (r)
            {
                json.IsSuccess = true;
                json.Msg = "操作成功";
                DB.SysLogs.setAdminLog("Cancel", "会员：[" + m.Code + "]被取消店主资格");
            }
            return json;
        }
        #endregion

        #region 修改或重置密码
        /// <summary>
        /// 修改会员密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        public JsonHelp EditPwd(string id, string pwd)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存成功" };
            var m = DB.Member_Info.FindEntity(id);
            m.LoginPwd = Common.CryptHelper.DESCrypt.Encrypt(pwd);
            if (DB.Member_Info.Update(m))
            {
                json.Status = "y";
                json.Msg = "保存成功";
                //添加操作日志
                DB.SysLogs.setAdminLog("Edit", "修改密码,编号为[" + m.Code + "]的会员用户");
            }
            return json;
        }
        /// <summary>
        /// 重置密码（或支付密码）
        /// </summary>
        /// <param name="idList">会员列表id</param>
        /// <param name="PwdField">密码字段</param>
        /// <returns></returns>
        public JsonHelp ResetPwd(string idList, string PwdField)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "操作失败" };

            try
            {
                var newPwd = string.Empty;
                //db.BeginTran();
                string flag = "密码";
                var defaultPwd = string.Empty;
                var list = idList.Split(',').ToList();
                var members = DB.Member_Info.Where(a => list.Contains(a.MemberId)).ToList();
                var r = 0;
                foreach (var item in members)
                {
                    if (PwdField == "LoginPwd")
                    {
                        newPwd = "111111";
                        item.LoginPwd = Common.CryptHelper.DESCrypt.Encrypt(newPwd);
                    }
                    else if (PwdField == "Pwd2")
                    {
                        newPwd = "222222";
                        flag = "支付密码";
                        item.Pwd2 = Common.CryptHelper.DESCrypt.Encrypt(newPwd);
                    }
                }
                r = DB.Member_Info.Update(members);
                if (r > 0)
                {
                    json.Status = "y";
                    json.Msg = "重置" + flag + "成功,新密码为：" + newPwd;
                    var codes = members.Select(a => a.Code).ToList().Aggregate((m, n) => m + "," + n);
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Edit", "重置" + flag + ",编号为[" + codes + "]的会员用户");
                }
            }
            catch (Exception e)
            {
                //DB.Rollback();
                LogHelper.Error("重置密码失败：" + e.Message);
            }
            return json;
        }
        /// <summary>
        /// 前台修改密码
        /// </summary>
        /// <param name="id">会员id</param>
        /// <param name="type">密码类型</param>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="password">新密码</param>
        /// <returns></returns>
        public JsonHelp ModifyPassword(string id, string type, string oldPwd, string password)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "修改密码失败" };
            try
            {
                var member = DB.Member_Info.FindEntity(id);
                var oldpwd2 = Common.CryptHelper.DESCrypt.Encrypt(oldPwd);
                var password2 = Common.CryptHelper.DESCrypt.Encrypt(password);
                if (type == "登录密码")
                {
                    if (oldpwd2 != member.LoginPwd)
                    {
                        json.Msg = "旧密码不正确";
                        return json;
                    }
                    else
                    {
                        member.LoginPwd = password2;
                        DB.Member_Info.Update(member);
                    }
                }
                else if (type == "支付密码")
                {
                    if (oldpwd2 != member.Pwd2)
                    {
                        json.Msg = "旧密码不正确";
                        return json;
                    }
                    else
                    {
                        member.Pwd2 = password2;
                        DB.Member_Info.Update(member);
                    }
                }
                json.Status = "y";
                json.Msg = "修改密码成功！";
                DB.SysLogs.setMemberLog(Enums.EventType.Edit, string.Format("会员：[{0}]修改{1}成功！", member.Code, type));
            }
            catch (Exception e)
            {
                LogHelper.Error("修改密码失败：" + e.Message);
            }
            return json;
        }
        #endregion

        #region 升级会员
        /// <summary>
        /// 升级会员
        /// </summary>
        /// <param name="memberId">会员id</param>
        /// <param name="pwd2">支付密码</param>
        /// <param name="levelId">会员级别id</param>
        /// <returns></returns>
        public JsonHelp Upgrade(string memberId, string pwd2, string levelId)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "升级失败" };

            var m = DB.Member_Info.FindEntity(memberId);
            var levels = DB.Sys_Level.getList();

            #region 检验参数
            if (string.IsNullOrEmpty(memberId))
            {
                json.Msg = "会员不存在，请刷新页面重试  ";
                return json;
            }
            if (string.IsNullOrEmpty(pwd2))
            {
                json.Msg = "支付密码不能为空";
                return json;
            }
            else if (m.Pwd2 != Common.CryptHelper.DESCrypt.Encrypt(pwd2))
            {
                json.Msg = "支付密码不正确";
                return json;
            }
            if (string.IsNullOrEmpty(levelId))
            {
                json.Msg = "请选择要升级的会员级别 ";
                return json;
            }
            #endregion
            #region 升级级别，并扣除相应的电子币           
            try
            {
                //db.BeginTran();
                int i_levelId = int.Parse(levelId);
                var level1 = levels.FirstOrDefault(a => a.LevelId == m.MemberLevelId);
                var level2 = levels.FirstOrDefault(a => a.LevelId == i_levelId);

                var coins = level2.Investment - level1.Investment;
                if (coins >= 0)
                {
                    m.Coins = m.Coins - coins;
                    if (m.Coins < 0)
                    {
                        json.Msg = "会员电子币不足，升级失败 ";
                        return json;
                    }

                    var upgrade = new Member_Upgrade();
                    upgrade.Code = m.Code;
                    upgrade.CreateTime = DateTime.Now;
                    upgrade.MemberId = m.MemberId;
                    upgrade.MemberLevelId = level1.LevelId;
                    upgrade.MemberLevelName = level1.LevelName;
                    upgrade.NewMemberLevelId = level2.LevelId;
                    upgrade.NewMemberLevelName = level2.LevelName;

                    m.MemberLevelId = level2.LevelId;
                    m.MemberLevelName = level2.LevelName;
                    m.ActiveAmount = level2.Investment;
                    var r = DB.Member_Info.Update(m);
                    if (r == true)
                    {
                        if (DB.Member_Upgrade.Insert(upgrade))
                        {
                            json.Status = "y";
                            json.Msg = "升级成功";
                            DB.SysLogs.setMemberLog("Upgrade", "升级会员成功，会员编号：[" + m.Code + "]");
                        }
                        else
                        {
                            json.Msg = "升级失败 ";
                            //DB.Rollback();
                            return json;
                        }
                    }
                }
                else
                {
                    json.Msg = "会员级别金额设置有问题，要升级的会员级别所需的金额小于当前会员级别 ";
                    return json;
                }
                #endregion
            }
            catch (Exception e)
            {
                //DB.Rollback();
                LogHelper.Error(string.Format("升级会员[{0}]失败：{1}", memberId, e.Message));
            }
            return json;
        }
        #endregion

        #region 增减电子币 收益
        /// <summary>
        /// 增减电子币 收益
        /// </summary>
        /// <param name="id">会员id</param>
        /// <param name="Commission">增减收益</param>
        /// <param name="Coins">增减电子币</param>
        /// <returns></returns>
        public JsonHelp ModifyCommissionCoins(string id, decimal Commission, decimal Coins)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            var m = DB.Member_Info.FindEntity(id);
            if (m == null)
            {
                json.Msg = "店主未找到，请刷新页面重试";
                return json;
            }
            m.Commission = (m.Commission ?? 0) + Commission;
            m.Coins = (m.Coins ?? 0) + Coins;
            var r = DB.Member_Info.Update(m);
            if (r)
            {
                json.Status = "y";
                json.Msg = "保存成功";
                DB.SysLogs.setAdminLog("Cancel", "会员：[" + m.Code + "]增减电子币：" + Coins + "，增减收益：" + Commission);
            }
            return json;
        }
        #endregion

        #region 增加一个子账号
        /// <summary>
        /// 增加一个子账号
        /// </summary>
        /// <param name="memberId">当前账号id（作为父账号）</param>
        /// <returns></returns>
        public JsonHelp AddSubMember(string memberId)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "生成子账号失败" };

            try
            {
                //db.BeginTran();
                var parent = DB.Member_Info.FindEntity(memberId);  //先取得子账号
                var levelInvestment = DB.Sys_Level.FindEntity(a => a.LevelId == parent.MemberLevelId).Investment; //主账号级别对应的金额
                var childCount = DB.Member_Info.Count(a => a.ParentId == memberId); //子账号个数

                #region 给子账号赋值
                var child = new Member_Info()
                {
                    MemberId = Guid.NewGuid().ToString(),
                    Code = parent.Code + (childCount + 1),
                    IsSub = true,
                    ParentId = memberId,
                    ActiveAmount = 0,
                    Alipay = parent.Alipay,
                    BankAccount = parent.BankAccount,
                    BankAddress = parent.BankAddress,
                    BankCode = parent.BankCode,
                    BankName = parent.BankName,
                    Coins = 0,
                    Commission = 0,
                    CommissionSum = 0,
                    CreateMemberId = parent.MemberId,
                    CreateMemberName = parent.NickName,
                    CreateTime = DateTime.Now,
                    Email = parent.Email,
                    FamousBrandFund = parent.FamousBrandFund,
                    IdCode = parent.IdCode,
                    InteractionAmount = 0,
                    IsLock = "否",
                    IsLockCommission = false,
                    IsLockDraw = false,
                    IsNullActive = false,
                    IsServiceCenter = "否",
                    LAmount = 0,
                    RAmount = 0,
                    Layered = string.Empty,
                    LoginPwd = parent.LoginPwd,
                    MemberLevelId = parent.MemberLevelId,
                    MemberLevelName = parent.MemberLevelName,
                    Mobile = parent.Mobile,
                    NickName = parent.NickName,
                    OpenBank = parent.OpenBank,
                    PostAddress = parent.PostAddress,
                    Pwd2 = parent.Pwd2,
                    Pwd3 = parent.Pwd3,
                    Position = parent.Position,
                    QQ = parent.QQ,
                    RecommendId = parent.RecommendId,
                    RecommendName = parent.RecommendName,
                    Scores = 0,
                    ServiceCenterCode = parent.ServiceCenterCode,
                    ServiceCenterId = parent.ServiceCenterId,
                    Sex = parent.Sex,
                    UpperId = parent.MemberId,
                    UpperName = parent.NickName,
                    WeiXin = parent.WeiXin
                };
                #endregion

                //默认激活，扣自己账号等值电子币，不足则不激活
                if (parent.Coins >= levelInvestment)
                {
                    child.IsActive = "已激活";
                    child.ActiveTime = DateTime.Now;
                    child.ActiveAmount = 0;
                    //扣除主账号等值电子币
                    parent.Coins -= levelInvestment;
                    DB.Member_Info.Update(parent);
                }
                else
                {
                    child.IsActive = "未激活";
                    json.Msg = "电子币不足";
                }
                var r = DB.Member_Info.Insert(child);
                if (Convert.ToBoolean(r))
                {
                    json.IsSuccess = true;
                    json.Msg = "生成子账号成功";
                    DB.SysLogs.setAdminLog("Add", "主账号：[" + parent.Code + "]，自动生成子账号:[" + child.Code + "]成功");
                }
            }
            catch (Exception e)
            {
                //DB.Rollback();
                LogHelper.Error(string.Format("会员：[{0}],生成子账号失败：" + e.Message));
            }
            return json;
        }
        #endregion

        #endregion
        private string RndNum(int VcodeNum)
        {
            //验证码可以显示的字符集合  
            string Vchar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,p" +
                ",q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,P,Q" +
                ",R,S,T,U,V,W,X,Y,Z";
            //string Vchar = "0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(new Char[] { ',' });//拆分成数组  
            string VNum = "";//产生的随机数  
            int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数  

            Random rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类  
                }
                int t = rand.Next(61);//获取随机数  
                //int t = rand.Next(10);//获取随机数  
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);//如果获取的随机数重复，则递归调用  
                }
                temp = t;//把本次产生的随机数记录起来  
                VNum += VcArray[t];//随机数的位数加一  
            }
            return VNum;
        }

        public string GetJiaMiCode(Member_Info member)
        {
            if (member == null)
            {
                return "";
            }
            else
            {
                return Common.CryptHelper.DESCrypt.Encrypt(member.MemberId);
            }
        }

        #region 查询

        #region 查询店主下面的会员列表（未激活 与 已激活）
        /// <summary>
        /// 查询店主下面的会员列表（未激活 与 已激活）
        /// </summary>
        /// <param name="id">店主MemberId,当id为空时查所有</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束时间</param>
        /// <param name="key">关键字</param>
        /// <param name="IsActive">是否激活, 为空时查所有</param>
        /// <param name="total">总条数</param>
        /// <param name="_start">开始条数</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public IQueryable<Member_Info> getSearchList(string id, DateTime? start, DateTime? end, string key, string IsActive, out int total, int _start, int pageSize,string Type="")
        {
            var query = DB.Member_Info.Where();
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if(Type=="普通")
            {
                query = query.Where(a => a.MemberLevelId==0);
            }
            if (Type == "正式")
            {
                query = query.Where(a => a.MemberLevelId > 0);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < end);
            }
            if (!string.IsNullOrEmpty(IsActive))
            {
                query = query.Where(a => a.IsActive == IsActive);
            }
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key) || a.Mobile.Contains(key));
            }
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.ServiceCenterId == id);
            }
            total = query.Count();

            query = query.OrderByDescending(p => p.CreateTime);

            return query.Skip(_start).Take(pageSize);
        }
        public IQueryable<Member_Info> getSearchListMianFei(string id, DateTime? start, DateTime? end, string key, string IsActive, out int total, int _start, int pageSize)
        {
            var query = DB.Member_Info.Where();
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < end);
            }
            if (!string.IsNullOrEmpty(IsActive))
            {
                query = query.Where(a => a.IsActive == IsActive);
            }
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.Code.Contains(key) || a.NickName.Contains(key) || a.Mobile.Contains(key));
            }
            if (!string.IsNullOrEmpty(id))
            {
                query = query.Where(a => a.RecommendId == id);
            }
            total = query.Count();
            if (IsActive == "已激活")
            {
                query = query.OrderByDescending(p => p.ActiveTime);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreateTime);
            }
            return query.Skip(_start).Take(pageSize);
        }
        /// <summary>
        /// 获取店主列表（不包含店主下面的会员）
        /// </summary>
        /// <param name="start">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        public IQueryable<Member_Info> getServiceList(DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Member_Info.Where(a => a.IsServiceCenter == "是" || a.IsServiceCenter == "审核中");
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < start);
            }
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.Code.IndexOf(key) > -1 || a.NickName.IndexOf(key) > -1 || a.Mobile.IndexOf(key) > -1);
            }
            var data = query.OrderByDescending(p => p.CreateTime).Skip(_start).Take(pageSize);
            total = query.Count();
            return data;
        }

        public IQueryable<Member_Info> getServiceListDL(DateTime? start, DateTime? end, string key, out int total, int _start, int pageSize)
        {
            var query = DB.Member_Info.Where(a => a.IsDL == "是" || a.IsDL == "审核中");
            if (start != null)
            {
                query = query.Where(a => a.CreateTime >= start);
            }
            if (end != null)
            {
                end = end.Value.AddDays(1);
                query = query.Where(a => a.CreateTime < start);
            }
            if (!string.IsNullOrEmpty(key))
            {
                query = query.Where(a => a.Code.IndexOf(key) > -1 || a.NickName.IndexOf(key) > -1 || a.Mobile.IndexOf(key) > -1);
            }
            var data = query.OrderBy(p => p.CreateTime).Skip(_start).Take(pageSize);
            total = query.Count();
            return data;
        }

        #endregion

        #region 加载推荐结构树
        /// <summary>
        /// 加载推荐结构树
        /// </summary>
        /// <param name="id">当前会员</param>
        /// <returns></returns>
        public string RecommendTreeData(string id)
        {
            StringBuilder sb2 = new StringBuilder();

            var m = DB.Member_Info.FindEntity(id);
            DateTime t1 = DateTime.Now;
            var list = DB.Member_Info.Where(p => p.IsActive == "已激活").Select(a => new
            {
                MemberId = a.MemberId,
                RecommendId = a.RecommendId,
                Code = a.Code,
                NickName = a.NickName,
                MemberLevelName = a.MemberLevelName
            }).ToList().Select(a => new Member_Info()
            {
                MemberId = a.MemberId,
                RecommendId = a.RecommendId,
                Code = a.Code,
                NickName = a.NickName,
                MemberLevelName = a.MemberLevelName
            }).ToList();
            sb2.Append("[{");
            sb2.AppendFormat("\"text\": \"{0}[{1}]\", \"id\": \"{2}\",\"icon\": \"glyphicon glyphicon-home\"", m.Code, m.NickName, m.MemberId);
            var r = BuildRecommendTreeData(m, list);
            sb2.Append(r);
            sb2.Append("}]");
            DateTime t2 = DateTime.Now;
            var xt = (t2 - t1);
            return sb2.ToString();
        }

        private string BuildRecommendTreeData(Member_Info info, List<Member_Info> list)
        {
            StringBuilder sb2 = new StringBuilder();
            var child = list.Where(a => a.RecommendId == info.MemberId && a.MemberId != info.MemberId);

            var count = child.Count();
            var i = 0;
            if (count > 0)
            {
                sb2.Append(",\"children\":[");
                foreach (var item in child)
                {
                    i++;
                    sb2.Append("{");
                    sb2.AppendFormat("\"text\": \"{0}[{1}]\", \"id\": \"{2}\",\"icon\": \"glyphicon glyphicon-user\"", item.Code, item.NickName, item.MemberId);
                    var x = BuildRecommendTreeData(item, list);
                    sb2.Append(x);
                    sb2.Append("}");
                    if (i < count)
                    {
                        sb2.Append(",");
                    }
                }
                sb2.Append("]");
            }
            return sb2.ToString();
        }
        #endregion

        #endregion

        #region 加载接点树

        public JsonHelp treeTableSearchNet(string memberCode, Member_Info currentMember, bool isAdmin)
        {
            JsonHelp json = new JsonHelp(false, "查询失败！");
            StringBuilder sb = new StringBuilder();
            var today = DateTime.Now.Date;
            var searchMember = Where(a => a.Code == memberCode).FirstOrDefault();
            int expandLevel = 1;
            if (searchMember == null)
            {
                json.Msg = "查询编号不正确！";
                return json;
            }
            if (searchMember.MemberId != currentMember.MemberId)
            {
                if (!searchMember.RPosition.StartsWith(currentMember.RPosition))
                {
                    json.Msg = "无权查看！";
                    return json;
                }
                expandLevel = searchMember.RPosition.Length - currentMember.RPosition.Length + 1;
            }
            json.IsSuccess = true;
            json.Msg = sb.ToString();
            if (isAdmin)
            {
                json.ReUrl = "/Admin_Member/Recommend/Index_Net?membercode=" + searchMember.Code + "&expandLevel=" + expandLevel;
            }
            else
            {
                json.ReUrl = "/Member_Market/Recommend/Index_Net?membercode=" + searchMember.Code + "&expandLevel=" + expandLevel;
            }
            return json;
        }
        public JsonHelp treeTableSearchNodeNet(string memberCode, Member_Info currentMember)
        {
            JsonHelp json = new JsonHelp(false, "查询失败！");
            StringBuilder sb = new StringBuilder();

            var today = DateTime.Now.Date;
            var dt = DateTime.Now;
            var searchMember = Where(a => a.Code == memberCode).FirstOrDefault();
            if (searchMember == null)
            {
                json.Msg = "查询编号不正确！";
                return json;
            }
            if (searchMember.MemberId == currentMember.MemberId)
            {
                sb.Append("<tr id=\"" + searchMember.MemberId + "\" " + (Any(a => a.RecommendId == searchMember.MemberId && a.IsActive == "已激活") ? "hasChild=\"true\"" : "") + ">");
                sb.Append("<td><span controller=\"true\">" + searchMember.Code + "</span></td>");
                sb.Append("<td>" + searchMember.NickName + "</td>");
                sb.Append("<td>" + searchMember.MemberLevelName + "</td>");
                sb.Append("<td>" + Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.IsActive == "已激活").Count() + "</td>");
                sb.Append("<td>" + Where(a => a.RPosition.StartsWith(searchMember.RPosition) && a.RecommendId != searchMember.MemberId && a.IsActive == "已激活").Sum(p => p.ActiveAmount) ?? 0 + "</td>");
                //sb.Append("<td>" + Where(a => a.RPosition.StartsWith(searchMember.RPosition) && a.RecommendId != searchMember.MemberId && a.RPosition.Length - searchMember.RPosition.Length > 4 && a.RPosition.Length - searchMember.RPosition.Length < 8 && a.IsActive == "已激活" && a.MemberId != searchMember.MemberId).Count() + "</td>");
                //sb.Append("<td>" + Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.RPosition.Length - searchMember.RPosition.Length > 2 && a.IsActive == "已激活").Count() + "</td>");
                //sb.Append("<td>" + Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.RPosition.Length - searchMember.RPosition.Length > 4 && a.IsActive == "已激活").Count() + "</td>");
                //sb.Append("<td>" + Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.RPosition.Length - searchMember.RPosition.Length > 6 && a.IsActive == "已激活").Count() + "</td>");
                //sb.Append("<td>" + (Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.IsActive == "已激活").Sum(a => a.ActiveAmount) ?? 0) + "</td>");
                //sb.Append("<td>" + Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.ActiveTime > today).Count() + "</td>");
                //sb.Append("<td>" + (Where(a => a.MemberId != searchMember.MemberId && a.RPosition.StartsWith(searchMember.RPosition) && a.ActiveTime > today).Sum(a => a.ActiveAmount) ?? 0) + "</td>");
                sb.Append("</tr>");
            }
            else
            {
                if (!searchMember.RPosition.StartsWith(currentMember.RPosition))
                {
                    json.Msg = "无权查看！";
                    return json;
                }

                var listAll = Where(a => a.RPosition.StartsWith(currentMember.RPosition) && a.IsActive == "已激活").Select(a => new { a.MemberId, a.Code, a.NickName, a.RecommendId, a.MemberLevelName, a.RPosition, a.ActiveTime }).ToList().Select(a => new Member_Info { MemberId = a.MemberId, Code = a.Code, NickName = a.NickName, RecommendId = a.RecommendId, MemberLevelName = a.MemberLevelName, RPosition = a.RPosition, ActiveTime = a.ActiveTime }).ToList();

                var listMember = listAll.Where(a => searchMember.RPosition.Contains(a.RPosition)).OrderBy(a => a.ActiveTime).ToList();

                if (listMember.Count > 0)
                {
                    for (int i = 0; i < listMember.Count; i++)
                    {
                        sb.Append("<tr id=\"" + listMember[i].MemberId + "\" " + (i == 0 ? "" : "pId=\"" + listMember[i].UpperId + "\"") + " " + (listAll.Any(a => a.UpperId == listMember[i].MemberId) ? "hasChild=\"true\"" : "") + ">");
                        sb.Append("<td><span controller=\"true\">" + listMember[i].Code + "</span></td>");
                        sb.Append("<td>" + listMember[i].NickName + "</td>");
                        sb.Append("<td>" + listMember[i].MemberLevelName + "</td>");
                        sb.Append("<td>" + listAll.Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition)).Count() + "</td>");
                        sb.Append("<td>" + Where(a => a.RPosition.StartsWith(searchMember.RPosition) && a.RecommendId != searchMember.MemberId && a.IsActive == "已激活").Sum(p => p.ActiveAmount) ?? 0 + "</td>");
                        //sb.Append("<td>" + Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition) && a.RPosition.Length - listMember[i].RPosition.Length > 2 && a.IsActive == "已激活").Count() + "</td>");
                        //sb.Append("<td>" + Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition) && a.RPosition.Length - listMember[i].RPosition.Length > 4 && a.IsActive == "已激活").Count() + "</td>");
                        //sb.Append("<td>" + Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition) && a.RPosition.Length - listMember[i].RPosition.Length > 6 && a.IsActive == "已激活").Count() + "</td>");
                        //sb.Append("<td>" + (listAll.Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition)).Sum(a => a.ActiveAmount) ?? 0) + "</td>");
                        //sb.Append("<td>" + listAll.Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition) && a.ActiveTime > today).Count() + "</td>");
                        //sb.Append("<td>" + (listAll.Where(a => a.MemberId != listMember[i].MemberId && a.RPosition.StartsWith(listMember[i].RPosition) && a.ActiveTime > today).Sum(a => a.ActiveAmount) ?? 0) + "</td>");
                        sb.Append("</tr>");
                    }
                }
            }
            json.IsSuccess = true;
            json.Msg = sb.ToString();
            return json;
        }

        public JsonHelp treeTableGetChildsNodeNet(string id)
        {
            var dt = DateTime.Now;
            JsonHelp json = new JsonHelp(false, "查询失败！");
            StringBuilder sb = new StringBuilder();
            var today = DateTime.Now.Date;
            if (string.IsNullOrEmpty(id))
            {
                return json;
            }
            var childs = Where(a => a.RecommendId == id && a.IsActive == "已激活").Select(a => new { a.MemberId, a.Code, a.NickName, a.RecommendId, a.MemberLevelName, a.RPosition, a.ActiveTime }).ToList().Select(a => new Member_Info { MemberId = a.MemberId, Code = a.Code, NickName = a.NickName, RecommendId = a.RecommendId, MemberLevelName = a.MemberLevelName, RPosition = a.RPosition, ActiveTime = a.ActiveTime }).ToList();
            foreach (var item in childs)
            {
                sb.Append("<tr id=\"" + item.MemberId + "\" pId=\"" + id + "\" " + (Any(a => a.RecommendId == item.MemberId && a.IsActive == "已激活") ? "hasChild=\"true\"" : "") + ">");
                sb.Append("<td><span controller=\"true\">" + item.Code + "</span></td>");
                sb.Append("<td>" + item.NickName + "</td>");
                sb.Append("<td>" + item.MemberLevelName + "</td>");
                sb.Append("<td>" + Where(a => a.MemberId != item.MemberId && a.RPosition.StartsWith(item.RPosition) && a.IsActive == "已激活").Count() + "</td>");
                sb.Append("<td>" + Where(a => a.RPosition.StartsWith(item.RPosition) && a.RecommendId != item.MemberId && a.IsActive == "已激活").Sum(p => p.ActiveAmount) ?? 0 + "</td>");
                //sb.Append("<td>" + (Where(a => a.MemberId != item.MemberId && a.RPosition.StartsWith(item.RPosition) && a.IsActive == "已激活").Sum(a => a.ActiveAmount) ?? 0) + "</td>");
                //sb.Append("<td>" + Where(a => a.MemberId != item.MemberId && a.RPosition.StartsWith(item.RPosition) && a.IsActive == "已激活" && a.ActiveTime > today).Count() + "</td>");
                //sb.Append("<td>" + (Where(a => a.MemberId != item.MemberId && a.RPosition.StartsWith(item.RPosition) && a.IsActive == "已激活" && a.ActiveTime > today).Sum(a => a.ActiveAmount) ?? 0) + "</td>");

                sb.Append("</tr>");
            }
            json.IsSuccess = true;
            json.Msg = sb.ToString();
            return json;
        }
        #endregion

        #region 辅助方法



        #region 获取下一个推荐关系编号
        /// <summary>
        /// 获取下一个推荐关系编号
        /// </summary>
        /// <param name="r">推荐人</param>
        /// <returns></returns>
        //public string getRPosition(Member_Info r)
        //{
        //    if (r == null)
        //    {
        //        LogHelper.Error("getRPosition推荐人为空");
        //        return null;
        //    }
        //    try
        //    {
        //        var childs = DB.Member_Info
        //            .Where(a => a.RecommendId == r.MemberId && !string.IsNullOrEmpty(a.Position))
        //            .Select(a => a.Position)
        //            .ToList();
        //        int id = 0;
        //        if (childs.Count > 0)
        //        {
        //            id = childs.Select(a => Convert.ToInt32(a.Substring(r.Position.Length).Replace("|", ""))).Max() + 1;
        //        }
        //        return r.Position + id + "|";
        //    }
        //    catch (Exception e)
        //    {
        //        LogHelper.Error($"getRPosition错误：{r.Code}-{r.Position}" + e.Message);
        //    }
        //    return "";
        //}
        #endregion
        #region 太阳线 位置处理
        /// <summary>
        /// 根据推荐人获取，当前人的位置
        /// </summary>
        /// <param name="recModel"></param>
        /// <returns></returns>
        public string GetPosition(Member_Info recModel)
        {
            string recPosition = recModel.RPosition;
            string firstPosition = DB.Member_Info
                .Where(q => q.RPosition.Contains(recPosition + "|") && q.RecommendId == recModel.MemberId)
                .OrderByDescending(q => q.CreateTime)
                .Select(q => q.RPosition)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(firstPosition) || firstPosition.Contains("|") == false)
                return recPosition + "|1";
            int last = Convert.ToInt32(firstPosition.Substring(firstPosition.LastIndexOf("|") + 1));
            return recPosition + "|" + (last + 1);
        }
        #endregion

        #region 递归获取推荐关系团队人数
        public int getRecommendCount(Member_Info member)
        {
            var list = DB.Member_Info.Where()
              .Select(a => new { a.MemberId, a.RecommendId })
              .ToList()
              .Select(a => new KeyValuePair<string, string>(a.MemberId, a.RecommendId))
              .ToArray();   //转换成数组，目测性能最好,上万条会员记录，从admin向下递归1秒左右

            Func<string, int> getCount = null;
            getCount = id =>
            {
                int count = 0;
                var childs = list.Where(a => a.Value == id);
                foreach (var it in childs)
                {
                    count++;
                    count += getCount(it.Key);
                }
                return count;
            };
            return getCount(member.MemberId);
        }
        #endregion

        #region 重新设置RPosition
        /// <summary>
        /// 从admin递归向下设置RPosition字段
        /// </summary>
        public void ResetRPosition()
        {
            using (var DB = new DbMallEntities())
            {
                var list = DB.Member_Info.ToList();
                var admin = list.FirstOrDefault(a => a.Code == "admin");
                setRP(admin, list);
                DB.SaveChanges();
            }
        }
        private void setRP(Member_Info m, List<Member_Info> list)
        {
            var childs = list.Where(a => a.RecommendId == m.MemberId);
            int i = 0;
            foreach (var item in childs)
            {
                item.RPosition = m.RPosition + i + "|";
                setRP(item, list);
                i++;
            }
        }
        #endregion
        #endregion

        #region 增减
        public JsonHelp SaveZeng(Enums.LoginType admin, string code, string Pwd2, decimal AddCommission, decimal AddCommissionSum, decimal AddCoins, decimal AddShopCoins, decimal AddScores, decimal AddTourScores)
        {
            JsonHelp json = new JsonHelp();
            using (var name = DB.Member_Info.BeginTransaction)
            {
                if (AddCommission > 0 || AddCommissionSum > 0 || AddCommissionSum < 0 || AddCoins > 0 || AddCoins < 0 || AddShopCoins > 0 || AddShopCoins < 0 || AddScores > 0 || AddScores < 0)
                {
                    var Pwd = Common.CryptHelper.DESCrypt.Encrypt(Pwd2);
                    var admins = DB.Member_Info.FindEntity(p => p.Code == "admin");
                    if (Pwd != admins.Pwd2)
                    {
                        json.Msg = "支付密码不正确,增减失败！";
                        return json;
                    }
                }
                var member = DB.Member_Info.FindEntity(p => p.Code == code);

                Fin_LiuShui _liushui = new Fin_LiuShui();
                if (AddCommission > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "收益(+)";
                    _liushui.Amount = AddCommission;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddCommission < 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "收益(-)";
                    _liushui.Amount = AddCommission;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);

                }

                if (AddCoins > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "报单积分(+)";
                    _liushui.Amount = AddCoins;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddCoins < 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "报单积分(-)";
                    _liushui.Amount = AddCoins;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }

                if (AddShopCoins > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "推广奖(+)";
                    _liushui.Amount = AddShopCoins;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddShopCoins < 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "推广奖(-)";
                    _liushui.Amount = AddShopCoins;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }

                if (AddCommissionSum > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "收益累计(+)";
                    _liushui.Amount = AddCommissionSum;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddCommissionSum < 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "收益累计(-)";
                    _liushui.Amount = AddCommissionSum;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                if (AddScores > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "配货价(+)";
                    _liushui.Amount = AddScores;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddScores != 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "配货价(-)";
                    _liushui.Amount = AddScores;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                if (AddTourScores > 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "商城积分(+)";
                    _liushui.Amount = AddTourScores;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                else if (AddTourScores != 0)
                {
                    //流水账单                    
                    _liushui.MemberId = member.MemberId;
                    _liushui.Code = member.Code;
                    _liushui.NickName = member.NickName;
                    _liushui.Type = "充值账单";
                    _liushui.Comment = "商城积分(-)";
                    _liushui.Amount = AddTourScores;
                    _liushui.CreateTime = DateTime.Now;
                    DB.Fin_LiuShui.Insert(_liushui);
                }
                member.Commission += AddCommission;
                member.CommissionSum += AddCommissionSum;
                member.Coins += AddCoins;
                member.ShopCoins += AddShopCoins;
                member.Scores += AddScores;
                member.TourScores += AddTourScores;
                if (member.Commission < 0 || member.CommissionSum < 0 || member.Coins < 0 || member.ShopCoins < 0 || member.Scores < 0 || member.TourScores < 0)
                {
                    json.Msg = "增减分后的最终值不能小于0";
                    return json;
                }
                if (DB.Member_Info.Update(member))
                {
                    json.Status = "y";
                    json.Msg = "修改成功";
                    name.Complete();
                }
            }
            return json;

        }
        #endregion
    }
}
