using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.DBHelpers
{
    public static class DBHelper_User
    {
        // 获取已有的用户语言偏好
        public static List<t_LanguageProfile> Get_LanguageProfile_All()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from lp in dbContext.t_LanguageProfile
                             orderby lp.lp_u_GID
                             select lp;

                List<t_LanguageProfile> LanguageProfileList = result.ToList<t_LanguageProfile>();

                return LanguageProfileList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 新增用户语言偏好
        public static void Add_LanguageProfile(t_LanguageProfile NewLanguageProfile)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                dbContext.t_LanguageProfile.Add(NewLanguageProfile);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 更新用户语言偏好
        public static void Update_LanguageProfile(t_LanguageProfile TargetLanguageProfile)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                //将实体附加到对象管理器中
                dbContext.t_LanguageProfile.Attach(TargetLanguageProfile);
                //把当前实体的状态改为Modified
                dbContext.Entry(TargetLanguageProfile).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        
        // 获取所有用户
        public static List<v_User> Get_UserList_All()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.v_User
                             orderby e.User_Name_EN
                             select e;

                List<v_User> UserList = result.ToList<v_User>();

                return UserList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过GID获取用户
        public static v_User Get_User_ByGID(string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.v_User
                             where e.User_GID == User_GID
                             select e;
                
                v_User User = result.FirstOrDefault<v_User>();

                return User;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过GID获取用户
        public static v_User Get_User_ByUserID(int User_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.v_User
                             where e.User_ID == User_ID
                             select e;

                v_User User = result.First<v_User>();

                return User;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过上级ID获用户集合
        public static List<v_User> Get_UserList_ByDirectReportID(int LineManager_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from e in dbContext.v_User
                             where e.LineManager_ID == LineManager_ID
                             select e;
                
                List<v_User> UserList = result.ToList<v_User>();

                return UserList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取全部权限
        public static List<t_Access> Get_Access_All()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from a in dbContext.t_Access
                             orderby a.a_Sort
                             select a;

                List<t_Access> AccessList = result.ToList<t_Access>();

                return AccessList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获取未配置权限情况下的默认权限
        public static t_Access Get_DefaultAccess()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var access = from a in dbContext.t_Access
                                  where a.a_Type_R == 1
                                     && a.a_Type_C == 0
                                     && a.a_Type_U == 0
                                     && a.a_Type_D == 0
                                  select a;

                t_Access DefaultAccess = access.FirstOrDefault();

                return DefaultAccess;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 获超级管理员权限
        public static t_Access Get_SuperAdminAccess()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var access = from a in dbContext.t_Access
                             where a.a_Type_R == 99
                                && a.a_Type_C == 99
                                && a.a_Type_U == 99
                                && a.a_Type_D == 99
                             select a;

                t_Access DefaultAccess = access.FirstOrDefault();

                return DefaultAccess;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过人员角色ID获取对应信息
        public static t_UserRole Get_UserRole_ByID(int UserRole_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ur in dbContext.t_UserRole
                             where ur.ur_ID == UserRole_ID
                             select ur;

                t_UserRole Result = result.FirstOrDefault();

                return Result;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        
        // 获取所有人员角色
        public static List<v_UserRole> Get_UserRole_All()
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ur in dbContext.v_UserRole
                             orderby ur.User_GID,ur.ur_e_Dept_ID
                             select ur;

                List<v_UserRole> UserRoleList = result.ToList();

                return UserRoleList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过用户GID获取人员角色（若无配置的任何角色，默认为公共元素只读）
        public static List<v_UserRole> Get_UserRole_ByGID(string User_GID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ur in dbContext.v_UserRole
                             where ur.User_GID == User_GID
                             select ur;

                List<v_UserRole> UserRoleList = result.ToList();

                t_Access DefaultAccess = Get_DefaultAccess();
                t_Access SuperAdminAccess = Get_SuperAdminAccess();

                // 若无配置的任何角色，默认为公共元素只读
                if (UserRoleList.Count == 0)
                {
                    UserRoleList.Add(new v_UserRole() {
                        RowNo = 0,
                        ur_e_Dept_ID = 0,
                        e_Dept_AbbreviatedName = string.Empty,
                        User_GID = User_GID,
                        User_DisplayText = string.Empty,
                        a_Name = DefaultAccess.a_Name,
                        a_Type_R = DefaultAccess.a_Type_R,
                        a_Type_C = DefaultAccess.a_Type_C,
                        a_Type_U = DefaultAccess.a_Type_U,
                        a_Type_D = DefaultAccess.a_Type_D
                    });
                }
                // 若有配置权限，但没有超级管理员权限，则也添加只读权限
                else if (!UserRoleList.Exists(o => o.a_ID == SuperAdminAccess.a_ID))
                {
                    UserRoleList.Add(new v_UserRole()
                    {
                        RowNo = 0,
                        ur_e_Dept_ID = 0,
                        e_Dept_AbbreviatedName = string.Empty,
                        User_GID = User_GID,
                        User_DisplayText = string.Empty,
                        a_Name = DefaultAccess.a_Name,
                        a_Type_R = DefaultAccess.a_Type_R,
                        a_Type_C = DefaultAccess.a_Type_C,
                        a_Type_U = DefaultAccess.a_Type_U,
                        a_Type_D = DefaultAccess.a_Type_D
                    });
                }

                return UserRoleList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过用户GID获取人员角色（若无配置的任何角色，默认为公共元素只读）
        public static List<t_UserRole> Get_UserRole_Table_ByGID(string User_GID, int ExceptionID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ur in dbContext.t_UserRole
                             where ur.ur_User_GID == User_GID
                                && ur.ur_ID != ExceptionID
                             select ur;

                List<t_UserRole> UserRoleList = result.ToList();
                
                return UserRoleList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 通过部门ID获取部门内已有的人员角色
        public static List<v_UserRole> Get_UserRole_ByDeptID(int DeptID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from ur in dbContext.v_UserRole
                             where ur.ur_e_Dept_ID == DeptID
                             orderby ur.User_DisplayText
                             select ur;

                List<v_UserRole> UserRoleList = result.ToList();
                
                return UserRoleList;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 新增人员角色
        public static OperationMessage Add_UserRole(t_UserRole NewUserRole)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                var result = from ur in dbContext.t_UserRole
                             where ur.ur_e_Dept_ID == NewUserRole.ur_e_Dept_ID
                                && ur.ur_User_GID == NewUserRole.ur_User_GID
                             select ur;

                // 该人员在该部门中无角色配置，则进行新增
                if (result.ToList().Count == 0)
                {
                    t_Access SuperAdminAccess = Get_SuperAdminAccess();

                    // 如果新增的是超级管理员权限，则删除该用户其余权限，并去除部门限制
                    if (SuperAdminAccess.a_ID == NewUserRole.ur_a_ID)
                    {
                        Delete_UserRole_ByGID(NewUserRole.ur_User_GID);
                        NewUserRole.ur_e_Dept_ID = 0;
                    }
                    
                    dbContext.t_UserRole.Add(NewUserRole);
                    dbContext.SaveChanges();
                    
                    ReturnMsg.OpResult = true;
                }
                else
                {
                    ReturnMsg.OpResult = false;
                    ReturnMsg.EXT01 = "Existed";
                }

                return ReturnMsg;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                
                ReturnMsg.OpResult = false;

                return ReturnMsg;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 更新人员角色
        public static bool Update_UserRole(t_UserRole TargetUserRole)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();
            OperationMessage ReturnMsg = new OperationMessage();

            try
            {
                t_Access SuperAdminAccess = Get_SuperAdminAccess();
                
                // 如果新增的是超级管理员权限，则删除该用户其余权限，并去除部门限制
                if (SuperAdminAccess.a_ID == TargetUserRole.ur_a_ID)
                {
                    Delete_UserRole_ByGID(TargetUserRole.ur_User_GID, TargetUserRole.ur_ID);
                    TargetUserRole.ur_e_Dept_ID = 0;
                }
                
                //将实体附加到对象管理器中
                dbContext.t_UserRole.Attach(TargetUserRole);
                //把当前实体的状态改为Modified
                dbContext.Entry(TargetUserRole).State = EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        // 删除人员角色
        public static bool Delete_UserRole_ByID(int ur_ID)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                t_UserRole TargetUserRole = new t_UserRole() { ur_ID = ur_ID };

                // 将实体附加到对象管理器中
                dbContext.t_UserRole.Attach(TargetUserRole);
                // 进行删除
                dbContext.t_UserRole.Remove(TargetUserRole);
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        
        // 删除人员角色
        public static bool Delete_UserRole_ByGID(string u_GID, int ExceptionID = 0)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                List<t_UserRole> UserRoleList = Get_UserRole_Table_ByGID(u_GID, ExceptionID);

                foreach (t_UserRole TargetUserRole in UserRoleList)
                {
                    // 将实体附加到对象管理器中
                    dbContext.t_UserRole.Attach(TargetUserRole);
                    // 进行删除
                    dbContext.t_UserRole.Remove(TargetUserRole);
                }
                
                dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}
