using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class JiangImp
    {
        #region 增加佣金 
        /// <summary>
        /// 增加佣金
        /// </summary>
        /// <param name="member">拿奖人</param>
        /// <param name="refMember">相关人</param>
        /// <param name="amount">金额</param>
        /// <param name="typeName">奖名</param>
        /// <param name="Comment">描述</param>
        /// <param name="awardType">发奖类型（佣金，股份，分红）</param>
        /// <returns></returns>
        public JsonHelp InsertFin(Member_Info member, Member_Info refMember, decimal amount, string typeName, string Comment = "")
        {
            JsonHelp json = new JsonHelp(true);
            #region 拓展奖(对碰奖)日封顶

            #endregion
            if (amount < 0)
            {
                json.Msg = "佣金不能小于0";
                return json;
            }
            else if (amount == 0)
            {
                //佣金为0，直接返回，不向数据库增加数据                
                json.Msg = "佣金为0";
                return json;
            }
            //检查是否锁定此会员不得奖
            if (member.IsLockCommission == true)
            {
                json.Msg = "此会员已锁定得奖，佣金不能分配";
                return json;
            }

            var Poundage = DB.XmlConfig.XmlSite.Poundage;
            var ChongXiao = DB.XmlConfig.XmlSite.ChongXiao;
            //插入费用明细                        
            var mFin = new Fin_Info();
            mFin.MemberId = member.MemberId;
            mFin.MemberCode = member.Code;
            mFin.NickName = member.NickName;
            mFin.Amount = amount;

            mFin.Poundage = amount * Poundage / 100m;
            mFin.CongXiao = 0;
            mFin.RealAmount = mFin.Amount - mFin.Poundage - mFin.CongXiao;


            mFin.TypeName = typeName;

            mFin.Comment = string.IsNullOrEmpty(Comment) ? typeName : Comment;

            mFin.RefMemberId = refMember.MemberId;
            mFin.RefMemberCode = refMember.Code;
            mFin.RefNickName = refMember.NickName;
            mFin.CreateTime = DateTime.Now;
            mFin.GroupDate = (int)(mFin.CreateTime.Value - new DateTime(1900, 1, 1)).TotalDays;
            if (mFin.Comment == "中级会员级差奖" || mFin.Comment == "高级会员级差奖" || mFin.TypeName == "代理推荐奖" || mFin.TypeName == "拼团奖")
            {
                mFin.IsSettlement = false;


            }
            else
            {
                mFin.IsSettlement = true;
                mFin.SettlementTime = mFin.CreateTime;
            }
            json.IsSuccess = DB.Fin_Info.Insert(mFin);
            if (json.IsSuccess == false)
            {
                return json;
            }
            else if (mFin.IsSettlement.Value)
            {

                member.Coins += mFin.RealAmount;
                member.CommissionSum += mFin.RealAmount;
                if (typeName == "分红奖")
                {
                    member.FHSum = 0;
                }
                else if (typeName == "突出贡献奖")
                {
                    member.LAmount = 0;
                }
                DB.Fin_LiuShui.AddLS(member.MemberId, mFin.RealAmount.Value, mFin.TypeName,"奖金");
                var r = DB.Member_Info.Where(a => a.MemberId == member.MemberId).Update(a => new Member_Info() { LAmount = member.LAmount, FHSum = member.FHSum, Coins = member.Coins, CommissionSum = member.CommissionSum, ShopCoins = member.ShopCoins });
                json.IsSuccess = r > 0;
                if (json.IsSuccess == false)
                {
                    return json;
                }
            }
            json.Msg = "插入余额成功";
            return json;
        }

        public void GiveJiang(Member_Info curUser, ShopOrder order)
        {
            var firstproductorder = DB.ShopOrderProduct.Where(a => a.OrderID == order.GUID).FirstOrDefault();
            var product = DB.ShopProduct.FindEntity(firstproductorder.ProductID);
            if (product.CategoryID1 != DB.XmlConfig.XmlSite.Scores)
            {
                ////产生推广奖
                //GouW(curUser, zhitui);
                ////产生推荐积分
                //GouScores(curUser, scores);
                ////超越奖
                //ZhiTui(curUser, dianpu2);

                ////平级奖
                //DianZhu(curUser, dianzhu);
                ////代理津贴
                //RecommendMember(curUser, dianpu1);
                ////三个代理
                //DaiLi(curUser, ysheng, yshi, yxian, order.Type);
                //推广 平级  超越
                JiCha(curUser, order.RealCongXiao);

                PingJi(curUser, order.RealCongXiao);

                ChaoYue(curUser, order.RealCongXiao);
                //加业绩
                UpdateAmount(curUser, order.RealCongXiao);
                //升级
                var recommendid = curUser.MemberId;
                var flag1 = true;
                while (flag1)
                {
                    Member_Info RemCem = DB.Member_Info.Where(a => a.MemberId == recommendid).FirstOrDefault();
                    if (RemCem != null)
                    {
                        bool istui = false;
                        if(curUser.RecommendCode==RemCem.Code)
                        {
                            istui = true;
                        }
                        //进行升级
                        GetShengJi(RemCem, istui);
                        recommendid = RemCem.RecommendId;
                    }
                    else
                    {
                        flag1 = false;
                        break;
                    }
                }


                //突出贡献级别升级
                recommendid = curUser.MemberId;
                flag1 = true;
                while (flag1)
                {
                    Member_Info RemCem = DB.Member_Info.Where(a => a.MemberId == recommendid).FirstOrDefault();
                    if (RemCem != null)
                    {
                        //突出贡献级别升级
                        GetShengJiTC(RemCem);
                        recommendid = RemCem.RecommendId;
                    }
                    else
                    {
                        flag1 = false;
                        break;
                    }
                }

            }
            else
            {   //产生拼团奖
                GouW(curUser, order.RealCongXiao, product.CanTuanBiLi);

            }
        }

        public void GetShengJi(Member_Info rModel,bool istui)
        {

            //满足条件升级
            //普通会员直接升级  初级看直推  高级看伞下部门
            int count = 3;
            while (count > 0)
            {
                var level = DB.Sys_Level.FindEntity(count);

                //伞下业绩
                var sanxia = rModel.ActiveAmount + rModel.RAmount;

                //直推
                var zhitui = 0;

                if (count == 2)
                {
                    zhitui = DB.Member_Info.Where(a => a.RecommendId == rModel.MemberId && a.MemberLevelId >= count - 1).Count();
                }
                else if (count == 3)
                {
                    var recommendlist = DB.Member_Info.Where(a => a.RecommendId == rModel.MemberId).OrderBy(a => a.CreateTime);


                    //高级会员数量
                    foreach (var item in recommendlist)
                    {
                        if (DB.Member_Info.Any(a => a.RPosition.StartsWith(item.RPosition) && a.MemberLevelId >= 2))
                        {
                            zhitui++;
                        }
                    }
                }

                //伞下业绩要大于每一层的   推荐人是一定会成为正式会员的
                if ((level.TeamAward <= sanxia && level.LayerPeng <= zhitui && rModel.MemberLevelId <= count) || (count == 1 && (istui || rModel.ActiveAmount > 0) && rModel.MemberLevelId <= count))
                {

                    rModel.MemberLevelId = count;
                    rModel.MemberLevelName = level.LevelName;
                    DB.Member_Info.Update(rModel);
                    break;
                }
                count--;
            }
        }


        public void GetShengJiTC(Member_Info rModel)
        {

            //满足条件升级
            var recommendlist = DB.Member_Info.Where(a => a.RecommendId == rModel.MemberId).OrderBy(a => a.CreateTime);


            //15个部门高级会员数量
            var zhitui = 0;
            foreach (var item in recommendlist)
            {
                if (DB.Member_Info.Any(a => a.RPosition.StartsWith(item.RPosition) && a.MemberLevelId == 3))
                {
                    zhitui++;
                }
            }
            if (zhitui >= DB.XmlConfig.XmlSite.ReCount)
            {

                rModel.IsLockFen = true;
                DB.Member_Info.Update(rModel);

            }


        }
        public string getService(int? serviceLevel)
        {
            if (serviceLevel == 0)
            {
                return "无级别";
            }
            else if (serviceLevel == 1)
            {
                return "一级";
            }
            else if (serviceLevel == 2)
            {
                return "二级";
            }
            else if (serviceLevel == 3)
            {
                return "三级";
            }
            else if (serviceLevel == 4)
            {
                return "四级";
            }
            else if (serviceLevel == 5)
            {
                return "五级";
            }

            return "";
        }
        internal JsonHelp JiCha(Member_Info member, decimal money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "级差奖分配出错" };
            Member_Info upper = DB.Member_Info.FindEntity(member.MemberId); //上级会员

            decimal Rote = 0m;

            decimal JCRote = 0m;
            //平级超越各一次机会
        
            while (upper != null)
            {


                if (upper.MemberLevelId == 0)
                {
                    upper.MemberLevelId = 1;
                }

                //真实的比例
                decimal activefee = 0;
                //当前的人比例
                decimal nowactivefee = 0m;
                //上个人的比例
                decimal roteactivefee = 0;

                nowactivefee = DB.Sys_Level.FindEntity(upper.MemberLevelId).RecAward1.Value;

                if (Rote > 0)
                {
                    roteactivefee = DB.Sys_Level.FindEntity(Rote).RecAward1.Value;
                }
                string comment = "推广奖";
                string typename = "推广奖";


                if(member.MemberId==upper.MemberId)
                {
                    comment = "复销奖";
                    typename = "复销奖";
                    nowactivefee = DB.Sys_Level.FindEntity(upper.MemberLevelId).RecAward2.Value;
                }


                //算级差
                if (nowactivefee - roteactivefee > 0)
                {
                    activefee = nowactivefee - roteactivefee;
                    comment = upper.MemberLevelName + "级差奖";
                }
                //else if ((nowactivefee - roteactivefee) == 0 && ispingji == true/*&& Rote == 7*/)
                //{

                //    if (Rote == 2 && upper.MemberLevelId == 2)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.ZZP;
                //    }
                //    else if (Rote == 3 && upper.MemberLevelId == 3)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.GGP;
                //    }
                //    else
                //    {
                //        activefee = 0;
                //    }
                //    if (activefee > 0)
                //    {
                //        ispingji = false;
                //    }
                //    typename = "平级奖";
                //    comment = upper.MemberLevelName + "平级奖";
                //}
                //else if ((nowactivefee - roteactivefee) < 0 && ischaoyue == true/*&& Rote == 7*/)
                //{

                //    if (Rote == 2 && upper.MemberLevelId == 1)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.CZC;
                //    }
                //    else if (Rote == 3 && upper.MemberLevelId == 2)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.ZGC;
                //    }
                //    else
                //    {
                //        activefee = 0;
                //    }
                //    if (activefee > 0)
                //    {
                //        ischaoyue = false;
                //    }
                //    typename = "超越奖";
                //    comment = upper.MemberLevelName + "超越奖";
                //}

                decimal amount = activefee / 100m * money;

                if (amount > 0)
                {
                    json = InsertFin(upper, member, amount, typename, comment);
                    Rote = Convert.ToInt32(upper.MemberLevelId);

                }





                upper = DB.Member_Info.FindEntity(upper.RecommendId);
            }



            return json;
        }


        internal JsonHelp PingJi(Member_Info member, decimal money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "级差奖分配出错" };
            Member_Info upper = DB.Member_Info.FindEntity(member.MemberId); //上级会员

            decimal Rote = 0m;

            decimal JCRote = 0m;
            //平级超越各一次机会
            bool ispingji = true;
            while (upper != null)
            {


                if (upper.MemberLevelId == 0)
                {
                    upper.MemberLevelId = 1;
                }
               
                //真实的比例
                decimal activefee = 0;
                //当前的人比例
                decimal nowactivefee = 0m;
                //上个人的比例
                decimal roteactivefee = 0;

                nowactivefee = DB.Sys_Level.FindEntity(upper.MemberLevelId).RecAward1.Value;

                if (Rote > 0)
                {
                    roteactivefee = DB.Sys_Level.FindEntity(Rote).RecAward1.Value;
                }
                string comment = "推广奖";
                string typename = "推广奖";
                //算级差
                if ((nowactivefee - roteactivefee) == 0 && ispingji == true/*&& Rote == 7*/)
                {

                    if (Rote == 2 && upper.MemberLevelId == 2)
                    {
                        activefee = DB.XmlConfig.XmlSite.ZZP;
                    }
                    else if (Rote == 3 && upper.MemberLevelId == 3)
                    {
                        activefee = DB.XmlConfig.XmlSite.GGP;
                    }
                    else
                    {
                        activefee = 0;
                    }
                    if (activefee > 0)
                    {
                        ispingji = false;
                    }
                    typename = "平级奖";
                    comment = upper.MemberLevelName + "平级奖";
                }
                //else if ((nowactivefee - roteactivefee) < 0 && ischaoyue == true/*&& Rote == 7*/)
                //{

                //    if (Rote == 2 && upper.MemberLevelId == 1)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.CZC;
                //    }
                //    else if (Rote == 3 && upper.MemberLevelId == 2)
                //    {
                //        activefee = DB.XmlConfig.XmlSite.ZGC;
                //    }
                //    else
                //    {
                //        activefee = 0;
                //    }
                //    if (activefee > 0)
                //    {
                //        ischaoyue = false;
                //    }
                //    typename = "超越奖";
                //    comment = upper.MemberLevelName + "超越奖";
                //}

                decimal amount = activefee / 100m * money;

                if (amount > 0)
                {
                    json = InsertFin(upper, member, amount, typename, comment);

                }

                Rote = Convert.ToInt32(upper.MemberLevelId);

                if(!ispingji)
                {
                    break;
                }

                upper = DB.Member_Info.FindEntity(upper.RecommendId);
            }



            return json;
        }

        internal JsonHelp ChaoYue(Member_Info member, decimal money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "级差奖分配出错" };
            Member_Info upper = DB.Member_Info.FindEntity(member.MemberId); //上级会员

            decimal Rote = 0m;

            decimal JCRote = 0m;
            //平级超越各一次机会
            bool ischaoyue = true;
            while (upper != null)
            {


                if (upper.MemberLevelId == 0)
                {
                    upper.MemberLevelId = 1;
                }
              
                //真实的比例
                decimal activefee = 0;
                //当前的人比例
                decimal nowactivefee = 0m;
                //上个人的比例
                decimal roteactivefee = 0;

                nowactivefee = DB.Sys_Level.FindEntity(upper.MemberLevelId).RecAward1.Value;

                if (Rote > 0)
                {
                    roteactivefee = DB.Sys_Level.FindEntity(Rote).RecAward1.Value;
                }
                string comment = "推广奖";
                string typename = "推广奖";
               if ((nowactivefee - roteactivefee) < 0 && ischaoyue == true/*&& Rote == 7*/)
                {

                    if (Rote == 2 && upper.MemberLevelId == 1)
                    {
                        activefee = DB.XmlConfig.XmlSite.CZC;
                    }
                    else if (Rote == 3 && upper.MemberLevelId == 2)
                    {
                        activefee = DB.XmlConfig.XmlSite.ZGC;
                    }
                    else
                    {
                        activefee = 0;
                    }
                    if (activefee > 0)
                    {
                        ischaoyue = false;
                    }
                    typename = "超越奖";
                    comment = upper.MemberLevelName + "超越奖";
                }

                decimal amount = activefee / 100m * money;

                if (amount > 0)
                {
                    json = InsertFin(upper, member, amount, typename, comment);

                }

                Rote = Convert.ToInt32(upper.MemberLevelId);

                if (!ischaoyue)
                {
                    break;
                }

                upper = DB.Member_Info.FindEntity(upper.RecommendId);
            }



            return json;
        }

        private decimal GetFuWuBiLi(ShopOrder shoporder, int rote, int type)
        {
            var prolist = DB.ShopOrderProduct.Where(a => a.OrderID == shoporder.GUID).ToList();
            var amount = 0m;
            for (int i = 0; i < prolist.Count; i++)
            {
                var productid = prolist[i].ProductID;
                var product = DB.ShopProduct.FindEntity(productid);

                if (type == 1 && rote == 1)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).ZC1 * prolist[i].Count).Value;
                }
                else if (type == 1 && rote == 2)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).ZC2 * prolist[i].Count).Value;
                }
                else if (type == 1 && rote == 3)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).ZC3 * prolist[i].Count).Value;
                }
                else if (type == 1 && rote == 4)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).ZC4 * prolist[i].Count).Value;
                }
                else if (type == 1 && rote == 5)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).ZC5 * prolist[i].Count).Value;
                }

                else if (type == 2 && rote == 1)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).TJZC1 * prolist[i].Count).Value;
                }
                else if (type == 2 && rote == 2)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).TJZC2 * prolist[i].Count).Value;
                }
                else if (type == 2 && rote == 3)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).TJZC3 * prolist[i].Count).Value;
                }
                else if (type == 2 && rote == 4)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).TJZC4 * prolist[i].Count).Value;
                }
                else if (type == 2 && rote == 5)
                {
                    amount += (DB.ShopProduct.GetSPrice(product, prolist[i].GuiGe).TJZC3 * prolist[i].Count).Value;
                }
            }
            return amount;
        }

        /// <summary>
        /// 增加佣金
        /// </summary>
        /// <param name="db">数据库实体</param>
        /// <param name="member">要增加佣金的会员,</param>
        /// <param name="refMember">相关的会员,</param>
        /// <param name="amount">金额</param>
        /// <param name="db">数据库连接实例</param>
        /// <param name="typeName">佣金类型名称</param>
        /// <param name="Comment">佣金描述</param>
        /// <returns></returns>DbMallEntities
        public JsonHelp InsertFin(DbMallEntities db, Member_Info member, Member_Info refMember, decimal amount, string typeName, string Comment = "")
        {
            JsonHelp json = new JsonHelp(true);
            if (amount < 0)
            {
                json.Msg = "佣金不能小于0";
                return json;
            }
            else if (amount == 0)
            {
                //佣金为0，直接返回，不向数据库增加数据                
                json.Msg = "佣金为0";
                return json;
            }
            //检查是否锁定此会员不得奖
            if (member.IsLockCommission == true)
            {
                json.Msg = "此会员已锁定得奖，佣金不能分配";
                return json;
            }

            var Poundage = DB.XmlConfig.XmlSite.Poundage;
            var ChongXiao = DB.XmlConfig.XmlSite.ChongXiao;
            //插入费用明细                        
            var mFin = new Fin_Info();
            mFin.MemberId = member.MemberId;
            mFin.MemberCode = member.Code;
            mFin.NickName = member.NickName;
            mFin.Amount = amount;
            mFin.Poundage = amount * Poundage / 100m;
            mFin.CongXiao = amount * ChongXiao / 100m;
            mFin.RealAmount = mFin.Amount - mFin.Poundage;
            mFin.TypeName = typeName;
            mFin.Comment = string.IsNullOrEmpty(Comment) ? typeName : Comment;
            mFin.RefMemberId = refMember.MemberId;
            mFin.RefMemberCode = refMember.Code;
            mFin.RefNickName = refMember.NickName;
            mFin.CreateTime = DateTime.Now;
            if (DB.XmlConfig.XmlSite.IsMiaoJie == true)
            {
                mFin.IsSettlement = true;
                mFin.SettlementTime = mFin.CreateTime;
            }
            else
            {
                mFin.IsSettlement = false;
            }
            db.Fin_Info.Add(mFin);
            if (DB.XmlConfig.XmlSite.IsMiaoJie == true)
            {
                var Comminssion = mFin.RealAmount * 50 / 100m;
                var Coins = mFin.RealAmount * 50 / 100m;
                member.Commission += Comminssion;
                member.CommissionSum += Comminssion;
                member.Coins += Coins;
                member.ShopCoins += Convert.ToDecimal(mFin.CongXiao);
            }
            json.Msg = "插入佣金成功";
            return json;
        }
        #endregion

        #region 报单补助
        public JsonHelp BaoDan(Member_Info serviceCenter, Member_Info refMember)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            decimal? amount = 0;
            if (serviceCenter.AppellationLeveId == 1)
            {
                amount = refMember.ActiveAmount * DB.XmlConfig.XmlSite.ZhiYingD / 100m;
            }
            else if (serviceCenter.AppellationLeveId == 2)
            {
                amount = refMember.ActiveAmount * DB.XmlConfig.XmlSite.ZhuanMaiD / 100m;
            }
            if (amount > 0)
            {
                json = InsertFin(serviceCenter, refMember, amount.Value, "报单补助");
            }
            return json;
        }
        #endregion

        #region 直推
        public JsonHelp Recommend(Member_Info member)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            if (Recommend != null)
            {
                decimal? amount = DB.XmlConfig.XmlSite.DaiLiAmount * DB.XmlConfig.XmlSite.DaiLiTuiJian / 100m;
                if (amount > 0)
                {
                    DB.Jiang.InsertFin(Recommend, member, amount.Value, "代理推荐奖", "代理推荐奖");
                }
            }
            return json;
        }
        public JsonHelp GouW(Member_Info member, decimal? money, string CanTuanBiLi)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            if (Recommend != null)
            {
                var cantuanbilistr = CanTuanBiLi.Split('|');
                var recommendlist = DB.Member_Info.Where(a => a.RecommendId == Recommend.MemberId).OrderBy(a => a.CreateTime);

                int i = 1;
                //判断是第几个人
                foreach (var item in recommendlist)
                {
                    if (item.MemberId == member.MemberId)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                var bili = 0m;
                if (i <= cantuanbilistr.Count())
                {
                    bili = Convert.ToDecimal(cantuanbilistr[i - 1]);
                }
                decimal? amount = money * bili / 100m;
                if (amount > 0)
                {
                    DB.Jiang.InsertFin(Recommend, member, amount.Value, "拼团奖", "拼团奖");
                }
            }
            return json;
        }
        public JsonHelp GouScores(Member_Info member, decimal? money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            if (Recommend != null)
            {
                decimal? amount = money;
                if (amount > 0 && DB.ShopOrder.Any(a => a.MemberID == Recommend.MemberId && a.State >= 2 && a.OrderType != "积分优惠价"))
                {
                    Recommend.Scores += amount;
                    DB.Member_Info.Update(Recommend);
                }
            }
            return json;
        }

        public JsonHelp DianZhu(Member_Info member, decimal? money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.ServiceCenterId);
            if (Recommend != null)
            {
                decimal? amount = money;
                if (amount > 0)
                {
                    DB.Jiang.InsertFin(Recommend, member, amount.Value, "平级奖", "平级奖");
                }
            }
            return json;
        }
        #endregion

        #region 更新业绩
        /// <summary>
        /// 更新业绩
        /// </summary>
        /// <param name="member">激活会员</param>
        /// <param name="db">数据库实例</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public JsonHelp UpdateAmount(Member_Info member, decimal amount)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var current = member;
            var upper = DB.Member_Info.FindEntity(current.MemberId);
            bool fh = true;
            bool tc = true;

            while (upper != null)
            {
                //给自己加业绩
                if (upper.MemberId == current.MemberId)
                {
                    upper.ActiveAmount += amount;
                }
                //团队业绩
                else if (upper.MemberLevelId > 0)
                {
                    upper.RAmount += amount;
                }
                //突出贡献业绩
                if (upper.IsLockFen == true && tc)
                {
                    tc = false;
                    upper.LAmount += amount;
                }
                //分红业绩
                if (upper.MemberLevelId == 3 && fh)
                {
                    fh = false;
                    upper.FHSum += amount;
                }
                //更新业绩结余
                var result = DB.Member_Info.Where(a => a.MemberId == upper.MemberId).Update(a => new Member_Info() { ActiveAmount = upper.ActiveAmount, LAmount = upper.LAmount, RAmount = upper.RAmount, FHSum = upper.FHSum }) > 0;
                if (result == false)
                {
                    json.IsSuccess = false;
                    return json; //如果 失败，则返回
                }

                current = upper;
                upper = DB.Member_Info.FindEntity(current.RecommendId);
            }
            return json;
        }
        #endregion

        #region 店主佣金(层奖)
        /// <summary>
        /// 分配层奖
        /// </summary>
        /// <param name="member">当前被激活的会员</param>
        /// <param name="db">数据库实例</param>
        /// <param name="levels">会员级别列表</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public JsonHelp FindPoint(Member_Info member, List<Sys_Level> levels = null)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            levels = getLevels(levels);
            Member_Info upper = DB.Member_Info.FindEntity(member.UpperId);

            int layer = 1;
            var maxLayer = DB.XmlConfig.XmlSite.RecAwardMaxRate34; //向上查找的最大层数
            while (upper != null)
            {
                if (layer % 2 == 0)
                {
                    //判断，如果 上级的级别对应的见点奖层数大于等于 当前分配到的层数，则分配见点奖
                    if (DB.XmlConfig.XmlSite.RecAwardMaxRate34 >= layer)
                    {
                        var level = DB.Sys_Level.FindEntity(p => p.LevelId == member.MemberLevelId);
                        var Amount = level.GlobalAward;
                        json = InsertFin(upper, member, Amount.Value, "店主佣金");
                        if (json.IsSuccess == false)
                        {
                            return json; //如果见点奖分配失败，则返回
                        }
                    }
                }
                upper = DB.Member_Info.FindEntity(upper.UpperId);
                layer++;
                if (layer > maxLayer)  //如果层数超过最大见点层数，则退出循环
                {
                    break;
                }
            }

            return json;
        }
        #endregion

        #region 拓展奖(对碰奖)
        /// <summary>
        /// 分配对碰奖
        /// </summary>
        /// <param name="member">当前被激活的会员</param>
        /// <param name="db">数据库实例</param>
        /// <param name="levels">会员级别列表</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public JsonHelp DuiPeng(Member_Info member, List<Sys_Level> levels = null)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "拓展奖分配出错" };
            levels = getLevels(levels);
            Member_Info upper = DB.Member_Info.FindEntity(member.UpperId); //上级会员

            while (upper != null)
            {
                //先把业绩加进上级会员的 左 或 右区业绩结余
                var LAmount = upper.LAmount;
                var RAmount = upper.RAmount;

                //进行1：1对碰  取左右区最小值
                var min = Math.Min(LAmount.Value, RAmount.Value);
                if (min > 0)
                {
                    var upperLevel = levels.FirstOrDefault(a => a.LevelId == upper.MemberLevelId);
                    var Amount = min * upperLevel.DuiPeng / 100M;
                    json = InsertFin(upper, member, Amount.Value, "拓展奖");
                    FuWuJiang(upper, Amount);
                    if (json.IsSuccess == false)
                    {
                        return json; //如果奖分配失败，则返回
                    }
                    else
                    {
                        //更新业绩结余
                        upper.LAmount -= min;
                        upper.RAmount -= min;
                        var result = DB.Member_Info.Where(a => a.MemberId == upper.MemberId).Update(a => new Member_Info() { LAmount = upper.LAmount, RAmount = upper.RAmount }) > 0;
                        if (result == false)
                        {
                            json.IsSuccess = false;
                            return json; //如果奖分配失败，则返回
                        }
                    }
                }
                upper = DB.Member_Info.FindEntity(upper.UpperId);
            }

            return json;
        }
        #endregion

        #region 服务奖
        public JsonHelp FuWuJiang(Member_Info member, decimal? Money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            for (int i = 1; i <= 3; i++)
            {
                if (Recommend == null)
                {
                    return json;
                }
                decimal? amount = 0;
                var count = DB.Member_Info.Count(p => p.RecommendId == Recommend.MemberId && p.IsActive == "已激活");
                if (i == 1 && count >= 1)
                {
                    amount = Money * DB.XmlConfig.XmlSite.FuWuJiang1 / 100m;
                }
                else if (i == 2 && count >= 2)
                {
                    amount = Money * DB.XmlConfig.XmlSite.FuWuJiang2 / 100m;
                }
                else if (i == 3 && count >= 3)
                {
                    amount = Money * DB.XmlConfig.XmlSite.FuWuJiang3 / 100m;
                }
                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, "服务奖");
                }
                Recommend = DB.Member_Info.FindEntity(Recommend.RecommendId);
            }
            return json;
        }
        #endregion

        #region 领导奖
        public JsonHelp LingDao(Member_Info member, decimal? dianpu1, decimal? dianpu2)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            int i = 1;
            while (Recommend != null)
            {
                if (Recommend.IsServiceCenter == "是")
                {
                    decimal? amount = 0;
                    string commen = "";
                    if (i == 1)
                    {
                        amount = dianpu1;
                        commen = "一代平级奖";
                    }
                    else if (i == 2)
                    {
                        amount = dianpu2;
                        commen = "二代平级奖";
                    }
                    if (amount > 0)
                    {
                        InsertFin(Recommend, member, amount.Value, commen);

                    }
                    i++;
                    if (i > 2)
                    {
                        break;
                    }
                }
                Recommend = DB.Member_Info.FindEntity(Recommend.RecommendId);
            }
            return json;
        }

        public JsonHelp ZhiTui(Member_Info member, decimal? zhitui)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            int i = 1;
            if (Recommend != null)
            {

                decimal? amount = zhitui;
                string commen = "超越奖";


                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, commen);

                }

            }
            return json;
        }
        public JsonHelp Recommend(Member_Info member, decimal? zhitui)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            int i = 1;
            if (Recommend != null)
            {

                decimal? amount = zhitui;
                string commen = "代理津贴";


                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, commen);

                }

            }
            return json;
        }
        public JsonHelp RecommendMember(Member_Info member, decimal? zhitui)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var ServiceCenter = DB.Member_Info.FindEntity(member.ServiceCenterId);
            int i = 1;
            if (ServiceCenter != null)
            {
                var Recommend = DB.Member_Info.FindEntity(ServiceCenter.RecommendId);
                if (Recommend != null)
                {
                    decimal? amount = zhitui;
                    string commen = "代理津贴";


                    if (amount > 0)
                    {
                        InsertFin(Recommend, member, amount.Value, commen);

                    }
                }
            }
            return json;
        }
        public JsonHelp DaiLi(Member_Info member, decimal sheng, decimal shi, decimal xian, string type)
        {

            string[] list = type.Split(',');
            int shengid = Convert.ToInt32(list[0]);
            int shiid = Convert.ToInt32(list[1]);
            int xianid = Convert.ToInt32(list[2]);
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.Where(a => a.ShengId == shengid && a.IsDL == "是" && a.DLName == "省级代理").FirstOrDefault();
            if (Recommend != null)
            {
                decimal? amount = sheng;
                string commen = "分红奖";


                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, commen);

                }

            }
            Recommend = DB.Member_Info.Where(a => a.ShiId == shiid && a.IsDL == "是" && a.DLName == "市级代理").FirstOrDefault();
            if (Recommend != null)
            {
                decimal? amount = shi;
                string commen = "突出贡献奖";


                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, commen);

                }

            }
            Recommend = DB.Member_Info.Where(a => a.XianId == xianid && a.IsDL == "是" && a.DLName == "县级代理").FirstOrDefault();
            if (Recommend != null)
            {
                decimal? amount = xian;
                string commen = "代理推荐奖";


                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, commen);

                }

            }
            return json;
        }

        #endregion

        #region 购物返利
        public JsonHelp ShangJia(Member_Info refMember, string id, decimal? Money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var shangjia = DB.Member_Info.FindEntity(p => p.MemberId == id);
            if (shangjia != null)
            {
                decimal? amount = Money * DB.XmlConfig.XmlSite.GouWuFanL / 100m;
                if (amount > 0)
                {
                    json = InsertFin(shangjia, refMember, amount.Value, "购物返利", "商家");
                }
            }
            return json;
        }
        #endregion

        #region 每日返利
        public JsonHelp FanLi()
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            //计算收益
            //var now = DateTime.Now.Date;//昨天的零点今天的零点
            //var tomorrow = DateTime.Now.AddDays(1).Date;//昨天的零点
            var list = DB.Member_Info.Where(p => p.IsActive == "已激活").ToList();
            //var staticList = DB.Fin_Info.Where(a => a.CreateTime >= now && a.CreateTime < tomorrow && a.TypeName == "每日返利").ToList();
            foreach (var item in list)
            {
                //if (staticList.Any(a => a.MemberCode == item.Code)) continue;
                var ListShop = DB.ShopOrder.Where(p => p.MemberID == item.MemberId && p.PayState == 1 && p.OrderType == "鑫鑫精品区").ToList();
                foreach (var items in ListShop)
                {
                    decimal? amount = items.RealAmount * DB.XmlConfig.XmlSite.MeiRiFanL / 100m;
                    if (items.YiDay >= items.ZongDay)
                    {
                        items.Type = "已分完";
                    }
                    else
                    {
                        items.YiDay += 1;
                    }
                    DB.ShopOrder.Update(items);
                    if (items.Type != "已分完")
                    {
                        if (amount > 0)
                        {
                            InsertFin(item, item, amount.Value, "每日返利", "每日返利");
                        }
                    }

                }

            }
            if (json.IsSuccess == true)
            {
                json.Status = "y";
                json.Msg = "发放成功";
            }
            else
            {
                json.Status = "n";
                json.Msg = "发放成功";
            }
            return json;
        }
        #endregion

        #region 分享消费奖
        public JsonHelp FenXiang(Member_Info member)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            for (int i = 1; i <= 2; i++)
            {
                if (Recommend == null)
                {
                    return json;
                }
                decimal? amount = 0;
                if (i == 1)
                {
                    amount = System.DB.XmlConfig.XmlSite.FenXiang1;
                }
                else if (i == 2)
                {
                    amount = System.DB.XmlConfig.XmlSite.FenXiang2;
                }
                if (amount > 0)
                {
                    InsertFin(Recommend, member, amount.Value, "分享消费奖", "分享消费奖");
                    FuDaoJ(Recommend, amount.Value);
                }
                Recommend = DB.Member_Info.FindEntity(Recommend.RecommendId);
            }
            return json;
        }
        #endregion

        #region 辅导奖
        public JsonHelp FuDaoJ(Member_Info member, decimal Money)
        {
            JsonHelp json = new JsonHelp() { IsSuccess = true, Msg = "" };
            var Recommend = DB.Member_Info.FindEntity(member.RecommendId);
            if (Recommend != null)
            {
                var count = DB.Member_Info.Count(p => p.RecommendId == Recommend.MemberId && p.IsActive == "已激活");
                if (count >= 3)
                {
                    decimal? amount = Money * System.DB.XmlConfig.XmlSite.FuDao / 100m;
                    if (amount > 0)
                    {
                        DB.Jiang.InsertFin(Recommend, member, amount.Value, "辅导奖", "辅导奖");
                    }
                }

            }
            return json;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 获取会员实例
        /// </summary>
        /// <param name="memberId">会员id</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public Member_Info getMember(string memberId, List<Member_Info> memberList)
        {
            Member_Info member = null;
            if (memberList != null)
            {
                member = memberList.FirstOrDefault(a => a.MemberId == memberId);
            }
            else
            {
                member = DB.Member_Info.FindEntity(memberId);
            }
            return member;
        }
        /// <summary>
        /// 获取会员级别列表
        /// </summary>
        /// <param name="levels">会员级别列表</param>
        /// <returns></returns>
        public List<Sys_Level> getLevels(List<Sys_Level> levels)
        {
            if (levels != null)
            {
                return levels;
            }
            else
            {
                return DB.Sys_Level.getList();
            }
        }
        /// <summary>
        /// 获取下级会员集合
        /// </summary>
        /// <param name="memberId">会员id</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public List<Member_Info> getChildMember(string memberId, List<Member_Info> memberList)
        {
            List<Member_Info> list = null;
            if (memberList != null)
            {
                list = memberList.Where(a => a.UpperId == memberId).ToList();
            }
            else
            {
                list = DB.Member_Info.Where(a => a.UpperId == memberId).ToList();
            }
            return list;
        }
        /// <summary>
        /// 获取Layer会员集合
        /// </summary>
        /// <param name="layerId">会员级别id</param>
        /// <param name="memberList">会员列表</param>
        /// <returns></returns>
        public List<Member_Info> getLayerMember(int? layerId, List<Member_Info> memberList)
        {
            List<Member_Info> list = null;
            if (memberList != null)
            {
                list = memberList.Where(a => a.MemberLevelId == layerId).ToList();
            }
            else
            {
                list = DB.Member_Info.Where(a => a.MemberLevelId == layerId).ToList();
            }
            return list;
        }
        #endregion
    }
}
