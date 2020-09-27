using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_NavigationImp :EFBase<DataBase.Sys_Navigation>
    {
        #region 查询
        /// <summary>
        /// 获取1级菜单 不带权限
        /// </summary>
        /// <returns></returns>
        public List<Sys_Navigation> getFirstMenu()
        {
            var FirstMenu = Where(p => p.class_layer == 1).OrderBy(p => p.sort_id).ToList();
            return FirstMenu;
        }
        /// <summary>
        /// 根据父级Id获取子级菜单 不带权限
        /// </summary>
        /// <param name="id">父级Id</param>
        /// <returns></returns>
        public List<Sys_Navigation> getMenuByParentId(int id)
        {
            var Menu = Where(p => p.parent_id == id).OrderBy(p => p.sort_id).ToList();
            return Menu;
        }

        /// <summary>
        /// 获取1级菜单 带权限
        /// </summary>
        /// <returns></returns>
        public List<Sys_Navigation> getFirstMenuByRole(string[] Role_Nav)
        {
            var FirstMenu = Where(p => p.class_layer == 1 && p.is_lock == 0 && Role_Nav.Contains(p.name)).OrderBy(p => p.sort_id).ToList();
            return FirstMenu;
        }

        /// <summary>
        /// 根据父级Id获取子级菜单 带权限
        /// </summary>
        /// <param name="id">父级Id</param>
        /// <returns></returns>
        public List<Sys_Navigation> getMenuByParentIdOrRole(int id, string[] Role_Nav)
        {
            var Menu = Where(p => p.parent_id == id && p.is_lock == 0 && Role_Nav.Contains(p.name)).OrderBy(p => p.sort_id).ToList();
            return Menu;
        }
        #endregion

        #region 获得菜单对象,加载权限树jsTree
        /// <summary>
        /// 获得菜单对象
        /// </summary>
        /// <param name="url">当前页面的URL</param>
        /// <returns></returns>
        public Sys_Navigation getMenuByLinkUrl(string url)
        {
            return FindEntity(p => p.link_url.Contains(url));
        }

        /// <summary>
        /// 加载权限树jsTree
        /// </summary>
        /// <returns></returns>
        public string ConvertjsTreeData()
        {
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            var firstMenu = getFirstMenu();
            sb2.Append("[");
            foreach (var item in firstMenu)
            {
                sb2.Append("{");
                sb2.Append("\"text\": \"" + item.title + "\", \"id\": \"" + item.id + "\",");
                sb2.Append("\"children\":[");
                foreach (var item1 in getMenuByParentId(item.id))
                {
                    sb2.Append("{");
                    sb2.Append("\"text\": \"" + item1.title + "\", \"id\": \"" + item1.id + "\"");
                    //sb2.Append("\"children\":[");

                    //sb2.Append("]");
                    sb2.Append("},");
                }
                sb2 = new StringBuilder(sb2.ToString().Trim(','));
                sb2.Append("]");
                sb2.Append("},");
            }
            sb2 = new StringBuilder(sb2.ToString().Trim(','));
            sb2.Append("]");
            return sb2.ToString();
        }
        /// <summary>
        /// 获取菜单对象
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        public Sys_Navigation getMenuById(int id)
        {
            return FindEntity(p => p.id == id);
        }
        #endregion

        #region 保存       
        public JsonHelp Save(Sys_Navigation entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            if (!string.IsNullOrEmpty(entity.action_type))
            {
                entity.action_type = entity.action_type.Trim(',');
            }

            if (!Any(p => p.id == entity.id))
            {
                entity.is_sys = 0;
                entity.nav_type = "System";
                if (string.IsNullOrEmpty(entity.parent_id.ToString()))
                {
                    entity.class_layer = 1;
                    entity.icon = "fa-th-list";
                }
                else
                {
                    entity.class_layer = 2;
                }
                entity.channel_id = 0;
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    DB.CacheRoleId++;
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Add", "新建名称为[" + entity.title + "]的菜单");
                }
            }
            else
            {
                //禁止更新列
                //db.AddDisableUpdateColumn("id", "is_sys", "nav_type", "class_layer", "channel_id", "class_list", "icon");//添加禁止更新列
                var model = DB.Sys_Navigation.FindEntity(entity.id);
                WebTools.CopyToObject(entity, model);
                if (Update(model))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    DB.CacheRoleId++;
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Edit", "更新名称为[" + entity.title + "]的菜单");
                }
            }
            return json;
        }
        #endregion

        #region 删除
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
                    if (Convert.ToBoolean(Delete(a => id.Contains(a.id))))
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                        DB.CacheRoleId++;
                    }
                }
                catch (Exception ex)
                {
                    //回滚事务
                    //db.RollbackTran();
                    throw ex;
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除ID为[" + idList + "]的菜单");
                return json;
            }
            catch (Exception e)
            {
                throw e.InnerException;
            }
        }
        #endregion 
    }
}
