using Common;
using DataBase;
using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_LevelImp : EFBase<DataBase.Sys_Level>
    {
        #region 查询
        public List<Sys_Level> getList()
        {
            var list = Where(p => true).OrderBy(a => a.LevelId).ToList();
            return list;
        }
        #endregion

        #region 保存
        public JsonHelp Save(string strList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };

            List<Sys_Level> LevList = new List<Sys_Level>();
            strList = strList.Substring(0, strList.Length - 1);
            var tr = strList.Split('&');
            for (int i = 0; i < tr.Length; i++)
            {
                var data = tr[i].Split(',');
                Sys_Level entity = DB.Sys_Level.FindEntity(Convert.ToInt32(data[0]));
                entity.LevelName = data[1];
                entity.RecAward1 = Convert.ToDecimal(data[2]);
                //entity.RecAward1 = Convert.ToInt32(data[3]);
                //entity.FindPointMoney = Convert.ToInt32(data[4]);
                //entity.FindPointLayer = Convert.ToInt32(data[5]);
                //entity.MinInvestment = Convert.ToInt32(data[6]);
                //entity.MinLevelId = Convert.ToInt32(data[7]);
                //entity.TeamAwardRange = Convert.ToDecimal(data[8]);
                entity.TeamAward = Convert.ToDecimal(data[3]);
                entity.LayerPeng = Convert.ToDecimal(data[4]);
              
                LevList.Add(entity);
            }
            var result = Update(LevList);
            if (result > 0)
            {
                json.Status = "y";
                json.Msg = "保存成功";
                //添加操作日志
                DB.SysLogs.setAdminLog("Edit", "修改了会员参数");
            }

            return json;
        }
        #endregion
    }
}
