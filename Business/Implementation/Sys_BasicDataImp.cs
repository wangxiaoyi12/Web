using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_BasicDataImp :EFBase<DataBase.Sys_BasicData>
    {
        #region 查询
        public List<Sys_BasicData> getBasicDataByType(string type)
        {
            using (var db = new DbMallEntities())
            {
                var list = from a in db.Sys_BasicData
                           join b in db.Sys_BasicType.Where(a => a.TypeName == type) on a.TypeId equals b.Id
                           orderby a.SortValue
                           select a;
                return list.ToList();
            }
        }
        #endregion

        #region 删除    
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };

            //是否为空
            if (string.IsNullOrEmpty(idList))
            {
                json.Msg = "未找到要删除的数据";
                return json;
            }
            var list = idList.Split(',').Select(a => Convert.ToInt32(a)).ToList();
            var r = DB.Sys_BasicData.Delete(a => list.Contains(a.Id));
            if (r > 0)
            {
                json.Status = "y";
                json.Msg = "删除数据成功";
            }

            //添加操作日志
            DB.SysLogs.setAdminLog("Delete", "删除ID为[" + idList + "]的基础数据");
            return json;
        }
        #endregion

        #region 保存
        public object Save(Sys_BasicData entity, int type)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            entity.TypeId = type;
            if (!Any(p => p.Id == entity.Id))
            {
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Add", "新建名称为[" + entity.BasicDataName + "]的部门");
                }
            }
            else
            {
                //禁止更新列
                var model = DB.Sys_BasicData.FindEntity(entity.Id);
                WebTools.CopyToObject(entity, model);
                if (Update(model))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Edit", "更新名称为[" + entity.BasicDataName + "]的部门");
                }
            }
            return json;
        }
        #endregion
    }
}
