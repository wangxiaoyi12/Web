using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class News_InfoImp :EFBase<DataBase.News_Info>
    {
        #region 查询      
        public List<News_Info> getDataSource(string key, out int total, int start, int pageSize)
        {
            var list = Where();
            if (!string.IsNullOrEmpty(key))
            {
                list = list.Where(a => a.Title.Contains(key) || a.CreateEmpName.Contains(key) || a.Comment.Contains(key));
            }

            var data = list.OrderByDescending(p => p.LastEditTime).Skip(start).Take(pageSize).ToList();
            total = data.Count;
            if (total >= pageSize)
            {
                total = list.Count();
            }
            return data;
        }
        #endregion

        #region 保存
        public JsonHelp Save(string type, string id, string title, string content, int ReadNum, string userid, string name)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            int intid = 0;
            int.TryParse(id, out intid);
            switch (type)
            {
                case "1"://编辑新闻
                    News_Info entity1 = DB.News_Info.FindEntity(intid);
                    if (entity1 != null)
                    {
                        entity1.Title = title;
                        entity1.Comment = content;
                        entity1.ReadNum = ReadNum;
                        entity1.LastEditTime = DateTime.Now;
                        if (Update(entity1))
                        {
                            json.Status = "y";
                            json.Msg = "保存成功";
                            json.ReUrl = "/Admin_Information/News/Index";
                            //添加操作日志
                            DB.SysLogs.setAdminLog("Edit", "更新标题为[" + entity1.Title + "]的新闻信息");
                        }
                    }
                    break;
                case "2"://添加新闻
                    News_Info entity2 = new News_Info();
                    entity2.Title = title;
                    entity2.Comment = content;
                    entity2.ReadNum = ReadNum;
                    entity2.LastEditTime = DateTime.Now;
                    entity2.CreateEmpId = userid;
                    entity2.CreateEmpName = name;
                    if (Insert(entity2))
                    {
                        json.Status = "y";
                        json.Msg = "保存成功";
                        json.ReUrl = "/Admin_Information/News/Index";
                        //添加操作日志
                        DB.SysLogs.setAdminLog("Add", "新增标题为[" + entity2.Title + "]的新闻信息");
                    }
                    break;
                case "3"://公司简介
                    Article_Info entity3 = DB.Article_Info.FindEntity(intid);
                    if (entity3 != null)
                    {
                        entity3.Title = title;
                        entity3.Comment = content;
                        entity3.ReadNum = ReadNum;
                        entity3.LastEditTime = DateTime.Now;
                        if (DB.Article_Info.Update(entity3))
                        {
                            json.Status = "y";
                            json.Msg = "保存成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog("Edit", "更新标题为[" + entity3.Title + "]的公司简介");
                        }
                    }
                    break;
                case "4"://公司账户
                    Article_Info entity4 = DB.Article_Info.FindEntity(intid);
                    if (entity4 != null)
                    {
                        entity4.Title = title;
                        entity4.Comment = content;
                        entity4.ReadNum = ReadNum;
                        entity4.LastEditTime = DateTime.Now;
                        if (DB.Article_Info.Update(entity4))
                        {
                            json.Status = "y";
                            json.Msg = "保存成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog("Edit", "更新标题为[" + entity4.Title + "]的公司简介");
                        }
                    }
                    break;
                case "5"://销售方案
                    Article_Info entity5 = DB.Article_Info.FindEntity(intid);
                    if (entity5 != null)
                    {
                        entity5.Title = title;
                        entity5.Comment = content;
                        entity5.ReadNum = ReadNum;
                        entity5.LastEditTime = DateTime.Now;
                        if (DB.Article_Info.Update(entity5))
                        {
                            json.Status = "y";
                            json.Msg = "保存成功";
                            //添加操作日志
                            DB.SysLogs.setAdminLog("Edit", "更新标题为[" + entity5.Title + "]的公司简介");
                        }
                    }
                    break;
                default:
                    break;
            }
            return json;

        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="idList">要删除的Id集合</param>
        /// <returns></returns>
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                var id = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();

                try
                {
                    if (Delete(a => id.Contains(a.NewsId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("删除News_info失败：" + ex.Message);
                }

                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除ID为[" + idList + "]的新闻信息");
                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 阅读次数+1
        public void ReadOne(DataBase.News_Info m)
        {
            if (m != null)
            {
                m.ReadNum = 1 + (m.ReadNum == null ? 0 : m.ReadNum.Value);
                Update(m);
            }
        }
        #endregion 
    }
}
