using Common;
using DataBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Implementation
{
    public class Sys_EmployeeImp :EFBase<DataBase.Sys_Employee>
    {
        #region 删除
        public JsonHelp Delete(string idList)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "删除数据失败" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的数据"; return json; }
                var id = idList.TrimEnd(',').Split(',').ToList();
                var disableUser = DB.XmlConfig.XmlSite.sysuser.Split(',');
                for (int i = 0; i < id.Count; i++)
                {
                    for (int y = 0; y < disableUser.Length; y++)
                    {
                        if (id[i] == disableUser[y])
                        {
                            json.Msg = "包含不可删除的系统用户，如：管理员！";
                            return json;
                        }
                    }
                }

                try
                {
                    //开启事务，可以不使用事务,也可以使用多个事务                       
                    if (Delete(a => id.Contains(a.EmpId)) > 0)
                    {
                        json.Status = "y";
                        json.Msg = "删除数据成功";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                //添加操作日志
                DB.SysLogs.setAdminLog("Delete", "删除ID为[" + idList + "]的后台用户");

                return json;
            }
            catch (Exception e) { throw e.InnerException; }
        }
        #endregion

        #region 保存
        public JsonHelp Save(Sys_Employee entity)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存失败" };
            entity.Password = Common.CryptHelper.DESCrypt.Encrypt(entity.Password);

            if (string.IsNullOrEmpty(entity.EmpId))
            {
                entity.EmpId = Guid.NewGuid().ToString();
                entity.LastLogin = DateTime.Now;
                if (Any(a => a.LoginName == entity.LoginName))
                {
                    json.Msg = "用户名已经存在，请修改";
                    return json;
                }
                if (Insert(entity))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Add", "新建名称为[" + entity.RealName + "/" + entity.LoginName + "]的后台用户");
                }
            }
            else
            {
                var model = DB.Sys_Employee.FindEntity(entity.EmpId);
                //model.LoginName = entity.LoginName; //登录名不能修改
                model.RealName = entity.RealName;
                model.Password = entity.Password;
                model.Sex = entity.Sex;
                model.Mobile = entity.Mobile;
                model.DepartmentId = entity.DepartmentId;
                model.PositionId = entity.PositionId;
                model.RoleId = entity.RoleId;
                model.EmpState = entity.EmpState;
                model.Comment = entity.Comment;

                if (Update(model))
                {
                    json.Status = "y";
                    json.Msg = "保存成功";
                    //添加操作日志
                    DB.SysLogs.setAdminLog("Edit", "更新名称为[" + entity.RealName + "/" + entity.LoginName + "]的后台用户");
                }
            }
            return json;
        }
        #endregion

        #region 重置密码、修改密码
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonHelp ResetPwd(string id)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "密码重置失败" };
            var model = FindEntity(a => a.EmpId == id);
            var setPwd = "111111";
            model.Password = Common.CryptHelper.DESCrypt.Encrypt(setPwd);
            if (Update(model))
            {
                json.Status = "y";
                json.Msg = "密码已重置为：" + setPwd;
                //添加操作日志
                DB.SysLogs.setAdminLog("Edit", "重置密码,登录名为[" + model.LoginName + "]的后台用户");
            }
            return json;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public JsonHelp EditPwd(string id, string pwd)
        {
            JsonHelp json = new JsonHelp() { Status = "n", Msg = "保存成功" };
            var model = FindEntity(a => a.EmpId == id);
            model.Password = Common.CryptHelper.DESCrypt.Encrypt(pwd);
            if (Update(model))
            {
                json.Status = "y";
                json.Msg = "修改密码成功，请重新登录";
                json.ReUrl = "/admin";
                //添加操作日志
                DB.SysLogs.setAdminLog("Edit", "修改密码,登录名为[" + model.LoginName + "]的后台用户");
            }
            return json;
        }
        #endregion 
    }
}
