using Common;
using DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_RoleImp :EFBase<DataBase.Sys_Role>
    {
        #region 获得角色权限
        public List<Sys_Role_Nav> getRoleNav(int? id)
        {
            return DB.Sys_Role_Nav.Where(p => p.role_id == id).ToList();
        }
        /// <summary>
        /// 获得角色权限(返回Json格式)
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns></returns>
        public string getRoleNavToJson(int id)
        {
            return DB.Sys_Role_Nav.Where(p => p.role_id == id).ToList().ToJsonString();
            //return JsonHelp.ConvertToTablesJson(DB.Sys_Role_Nav.Where(p => p.role_id == id).ToList());
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="role">角色对象</param>
        /// <param name="rolenav">权限集合</param>
        /// <param name="id">权限ID</param>
        /// <returns></returns>
        public JsonHelp Save(int id, string name, byte roletype, string rolenav)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            List<Sys_Role_Nav> role_nav = new List<Sys_Role_Nav>();

            using (var db = new DbMallEntities())
            {
                try
                {
                    if (!Any(p => p.id == id))
                    {
                        Sys_Role entity = new Sys_Role();
                        entity.role_name = name;
                        entity.role_type = roletype;
                        db.Sys_Role.Add(entity);
                        db.SaveChanges();
                        var str = rolenav.Trim(',').Split(',');
                        for (int i = 0; i < str.Length; i++)
                        {
                            var role_name = str[i].Split('&')[0];
                            var role_type = str[i].Split('&')[1];
                            Sys_Role_Nav role = new Sys_Role_Nav();
                            role.role_id = entity.id;
                            role.nav_name = role_name.ToString();
                            role.action_type = role_type.ToString();
                            role_nav.Add(role);
                            db.Sys_Role_Nav.Add(role);
                        }
                        db.SaveChanges();
                        json.Status = "y";
                        json.Msg = "保存成功";
                        DB.CacheRoleId++;
                        //添加操作日志
                        DB.SysLogs.setAdminLog("Add", "新建名称为[" + entity.role_name + "]的角色");

                    }
                    else
                    {
                        var model = db.Sys_Role.Find(id);
                        model.id = id;
                        model.role_name = name;
                        model.role_type = roletype;

                        var str = rolenav.Trim(',').Split(',');
                        for (int i = 0; i < str.Length; i++)
                        {
                            var role_name = str[i].Split('&')[0];
                            var role_type = str[i].Split('&')[1];

                            Sys_Role_Nav role = new Sys_Role_Nav();
                            role.role_id = id;
                            role.nav_name = role_name.ToString();
                            role.action_type = role_type.ToString();
                            role_nav.Add(role);
                        }
                        var oldNav = db.Sys_Role_Nav.Where(p => p.role_id == id);
                        //先删除原来多余的
                        foreach (var item in oldNav)
                        {
                            var exsit = role_nav.Any(a => a.nav_name == item.nav_name && a.action_type == item.action_type);
                            if (exsit == false)
                            {
                                db.Sys_Role_Nav.Remove(item);
                            }
                        }
                        //再添加原来没有的
                        foreach (var item in role_nav)
                        {
                            var exsit = oldNav.FirstOrDefault(a => a.nav_name == item.nav_name && a.action_type == item.action_type);
                            if (exsit == null)
                            {
                                db.Sys_Role_Nav.Add(item);
                            }
                        }

                        db.SaveChanges();
                        json.Status = "y";
                        json.Msg = "保存成功";
                        DB.CacheRoleId++;
                        //添加操作日志
                        DB.SysLogs.setAdminLog("Edit", "更新名称为[" + name + "]的角色");
                    }
                }
                catch (Exception e)
                {
                    json.IsSuccess = false;
                    LogHelper.Error("角色保存失败：" + e.Message);
                }
            }
            return json;
        }
        #endregion

        #region 删除
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            using (var tran=DB.Sys_Role.BeginTransaction)
            {
                try
                {
                    #region 执行逻辑处理
                    //是否为空
                    if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                    var id = idList.TrimEnd(',').Split(',').Select(a => Convert.ToInt32(a)).ToList();
                    var disableRole = DB.XmlConfig.XmlSite.sysrole.Split(',').ToList();
                    for (int i = 0; i < id.Count; i++)
                    {
                        for (int y = 0; y < disableRole.Count; y++)
                        {
                            if (id[i].ToString() == disableRole[y])
                            {
                                json.Msg = "包含不可删除的系统角色，如：超级管理员，会员角色！";
                                return json;
                            }
                        }
                    }
                    var id2 = new List<int?>();
                    foreach (var item in id)
                    {
                        id2.Add(item);
                    }
                    if (Convert.ToBoolean(Delete(a => id.Contains(a.id))))
                    {
                        var del = DB.Sys_Role_Nav.Delete(a => id2.Contains(a.role_id));
                        json.IsSuccess = true;
                        json.Msg = "删除数据成功";
                        DB.CacheRoleId++;
                    }

                    //添加操作日志
                    DB.SysLogs.setAdminLog("Delete", "删除ID为[" + idList + "]的角色");
                    #endregion
                    tran.Complete();
                }
                catch (Exception e)
                {
                    DB.Rollback();
                    LogHelper.Error("删除角色出错：" + e.Message);
                }
            }
            return json;
        }
        #endregion
    }
}
