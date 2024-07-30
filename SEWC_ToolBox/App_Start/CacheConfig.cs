using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.Utilities.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

namespace SEWC_ToolBox_Project
{
    public class CacheConfig
    {
        public enum CacheType
        {
            EntryList,
            UserList,
            MenuList,
            MenuCategoryList,
            ProcessConnectionList,
            ProcessNodeList,
            Report_CategoryList,
            Report_SubCategoryList,
            Report_FrequencyList,
            QuickLink_TypeList,
            LanguageProfileList,
            AccessList,
            UserRoleList
        }

        // 初始化缓存
        public static void Initial()
        {
            #region 入口列表缓存 (12小时刷新)

            object EntryList_Cache = GetCache("EntryList");

            if (EntryList_Cache == null)
            {
                List<t_Entry> EntryList = DBHelper_EntryMenu.Get_EntryList();
                SetCache("EntryList", EntryList, new TimeSpan(12, 0, 0));
            }

            #endregion

            #region 人员列表缓存 (12小时刷新)

            object UserList_Cache = GetCache("UserList");

            if (UserList_Cache == null)
            {
                List<v_User> UserList = DBHelper_User.Get_UserList_All();
                SetCache("UserList", UserList, new TimeSpan(12, 0, 0));
            }

            #endregion

            #region 菜单列表 (2小时刷新)

            object MenuList_Cache = GetCache("MenuList");

            if (MenuList_Cache == null)
            {
                List<v_MenuList> MenuList = DBHelper_EntryMenu.Get_MenuList();
                SetCache("MenuList", MenuList, new TimeSpan(2, 0, 0));
            }

            #endregion

            #region 报表类别缓存 (2小时刷新)

            object Report_SubCategoryList_Cache = GetCache("Report_SubCategoryList");

            if (Report_SubCategoryList_Cache == null)
            {
                List<v_MenuList> Report_CategoryList = DBHelper_EntryMenu.Get_Report_SubCategoryList();
                SetCache("Report_SubCategoryList", Report_CategoryList, new TimeSpan(2, 0, 0));
            }

            #endregion

            #region 报表类别缓存 (2小时刷新)

            object Report_CategoryList_Cache = GetCache("Report_CategoryList");

            if (Report_CategoryList_Cache == null)
            {
                List<v_MenuList> Report_CategoryList = DBHelper_EntryMenu.Get_Report_CategoryList();
                SetCache("Report_CategoryList", Report_CategoryList, new TimeSpan(2, 0, 0));
            }

            #endregion

            #region 报表频率缓存 (2小时刷新)

            object Report_FrequencyList_Cache = GetCache("Report_FrequencyList");

            if (Report_FrequencyList_Cache == null)
            {
                List<v_MenuList> Report_FrequencyList = DBHelper_EntryMenu.Get_Report_FrequencyList();
                SetCache("Report_FrequencyList", Report_FrequencyList, new TimeSpan(2, 0, 0));
            }

            #endregion

            #region 快速链接类型缓存 (2小时刷新)

            object QuickLink_TypeList_Cache = GetCache("QuickLink_TypeList");

            if (QuickLink_TypeList_Cache == null)
            {
                List<v_MenuList> QuickLink_TypeList = DBHelper_EntryMenu.Get_QuickLink_TypeList();
                SetCache("QuickLink_TypeList", QuickLink_TypeList, new TimeSpan(2, 0, 0));
            }

            #endregion

            #region 用户语言偏好缓存 (12小时刷新)

            object LanguageProfileList_Cache = GetCache("LanguageProfileList");

            if (LanguageProfileList_Cache == null)
            {
                List<t_LanguageProfile> LanguageProfileList = DBHelper_User.Get_LanguageProfile_All();
                SetCache("LanguageProfileList", LanguageProfileList, new TimeSpan(12, 0, 0));
            }

            #endregion

            #region 权限类别缓存 (24小时刷新)

            object AccessList_Cache = GetCache("AccessList");

            if (AccessList_Cache == null)
            {
                List<t_Access> AccessList = DBHelper_User.Get_Access_All();
                SetCache("AccessList", AccessList, new TimeSpan(24, 0, 0));
            }

            #endregion

            #region 人员角色缓存 （24小时刷新）

            object UserRoleList_Cache = GetCache("UserRoleList");

            if (UserRoleList_Cache == null)
            {
                List<v_UserRole> UserRoleList = DBHelper_User.Get_UserRole_All();
                SetCache("UserRoleList", UserRoleList, new TimeSpan(24, 0, 0));
            }

            #endregion

            #region ProcessNodeList

            object processNodeList_Cache = GetCache("ProcessNodeList");
            if (processNodeList_Cache == null)
            {
                List<v_ProcessNode> processNodeList = DBHelper_Content.Get_ProcessNodes();
                SetCache("ProcessNodeList", processNodeList, new TimeSpan(24, 0, 0));
            }

            #endregion

            #region ProcessConnectionList

            object processConnectionList_Cache = GetCache("ProcessConnectionList");
            if (processConnectionList_Cache == null)
            {
                List<t_ProcessConnection> processConnectionList = DBHelper_Content.Get_process_Connections();
                SetCache("ProcessConnectionList", processConnectionList, new TimeSpan(24, 0, 0));
            }

            #endregion
        }

        public static T Get<T>(CacheType type)
        {

            // default(T)返回泛型的默认类型
            T result = default(T);
            string CacheKey = type.ToString();
            object CacheObject = GetCache(CacheKey);

            if (CacheObject == null)
            {
                if (type == CacheType.EntryList)
                {
                    List<t_Entry> EntryList = DBHelper_EntryMenu.Get_EntryList();
                    SetCache(CacheKey, EntryList, new TimeSpan(12, 0, 0));

                    result = (T)(object)EntryList;
                }
                else if (type == CacheType.UserList)
                {
                    List<v_User> UserList = DBHelper_User.Get_UserList_All();
                    SetCache(CacheKey, UserList, new TimeSpan(12, 0, 0));

                    result = (T)(object)UserList;
                }
                else if (type == CacheType.MenuList)
                {
                    List<v_MenuList> MenuList = DBHelper_EntryMenu.Get_MenuList();
                    SetCache(CacheKey, MenuList, new TimeSpan(2, 0, 0));

                    result = (T)(object)MenuList;
                }
                else if (type == CacheType.MenuCategoryList)
                {
                    List<v_SideMenuList> MenuList = DBHelper_EntryMenu.MenuCategory();
                    SetCache(CacheKey, MenuList, new TimeSpan(2, 0, 0));

                    result = (T)(object)MenuList;
                }
                else if (type == CacheType.Report_CategoryList)
                {
                    List<v_MenuList> Report_CategoryList = DBHelper_EntryMenu.Get_Report_CategoryList();
                    SetCache(CacheKey, Report_CategoryList, new TimeSpan(2, 0, 0));

                    result = (T)(object)Report_CategoryList;
                }
                else if (type == CacheType.Report_SubCategoryList)
                {
                    List<v_MenuList> Report_SubCategoryList = DBHelper_EntryMenu.Get_Report_SubCategoryList();
                    SetCache(CacheKey, Report_SubCategoryList, new TimeSpan(2, 0, 0));

                    result = (T)(object)Report_SubCategoryList;
                }
                else if (type == CacheType.Report_FrequencyList)
                {
                    List<v_MenuList> Report_FrequencyList = DBHelper_EntryMenu.Get_Report_FrequencyList();
                    SetCache(CacheKey, Report_FrequencyList, new TimeSpan(2, 0, 0));

                    result = (T)(object)Report_FrequencyList;
                }
                else if (type == CacheType.QuickLink_TypeList)
                {
                    List<v_MenuList> QuickLink_TypeList = DBHelper_EntryMenu.Get_QuickLink_TypeList();
                    SetCache(CacheKey, QuickLink_TypeList, new TimeSpan(2, 0, 0));

                    result = (T)(object)QuickLink_TypeList;
                }
                else if (type == CacheType.LanguageProfileList)
                {
                    List<t_LanguageProfile> LanguageProfileList = DBHelper_User.Get_LanguageProfile_All();
                    SetCache(CacheKey, LanguageProfileList, new TimeSpan(12, 0, 0));

                    result = (T)(object)LanguageProfileList;
                }
                else if (type == CacheType.AccessList)
                {
                    List<t_Access> AccessList = DBHelper_User.Get_Access_All();
                    SetCache(CacheKey, AccessList, new TimeSpan(24, 0, 0));

                    result = (T)(object)AccessList;
                }
                else if (type == CacheType.UserRoleList)
                {
                    List<v_UserRole> UserRoleList = DBHelper_User.Get_UserRole_All();
                    SetCache(CacheKey, UserRoleList, new TimeSpan(24, 0, 0));

                    result = (T)(object)UserRoleList;
                }
            }
            else
                result = (T)(object)CacheObject;

            return result;
        }

        public static object Set(CacheType type)
        {
            string CacheKey = type.ToString();

            if (type == CacheType.EntryList)
            {
                List<t_Entry> EntryList = DBHelper_EntryMenu.Get_EntryList();
                SetCache(CacheKey, EntryList, new TimeSpan(12, 0, 0));

                return EntryList;
            }
            else if (type == CacheType.UserList)
            {
                List<v_User> UserList = DBHelper_User.Get_UserList_All();
                SetCache(CacheKey, UserList, new TimeSpan(12, 0, 0));

                return UserList;
            }
            else if (type == CacheType.MenuList)
            {
                List<v_MenuList> MenuList = DBHelper_EntryMenu.Get_MenuList();
                SetCache(CacheKey, MenuList, new TimeSpan(2, 0, 0));

                return MenuList;
            }
            else if (type == CacheType.Report_SubCategoryList)
            {
                List<v_MenuList> Report_SubCategoryList = DBHelper_EntryMenu.Get_Report_SubCategoryList();
                SetCache(CacheKey, Report_SubCategoryList, new TimeSpan(2, 0, 0));

                return Report_SubCategoryList;
            }
            else if (type == CacheType.Report_CategoryList)
            {
                List<v_MenuList> Report_CategoryList = DBHelper_EntryMenu.Get_Report_CategoryList();
                SetCache(CacheKey, Report_CategoryList, new TimeSpan(2, 0, 0));

                return Report_CategoryList;
            }
            else if (type == CacheType.Report_FrequencyList)
            {
                List<v_MenuList> Report_FrequencyList = DBHelper_EntryMenu.Get_Report_FrequencyList();
                SetCache(CacheKey, Report_FrequencyList, new TimeSpan(2, 0, 0));

                return Report_FrequencyList;
            }
            else if (type == CacheType.QuickLink_TypeList)
            {
                List<v_MenuList> QuickLink_TypeList = DBHelper_EntryMenu.Get_QuickLink_TypeList();
                SetCache(CacheKey, QuickLink_TypeList, new TimeSpan(2, 0, 0));

                return QuickLink_TypeList;
            }
            else if (type == CacheType.LanguageProfileList)
            {
                List<t_LanguageProfile> LanguageProfileList = DBHelper_User.Get_LanguageProfile_All();
                SetCache(CacheKey, LanguageProfileList, new TimeSpan(12, 0, 0));

                return LanguageProfileList;
            }
            else if (type == CacheType.AccessList)
            {
                List<t_Access> AccessList = DBHelper_User.Get_Access_All();
                SetCache(CacheKey, AccessList, new TimeSpan(24, 0, 0));

                return AccessList;
            }
            else if (type == CacheType.UserRoleList)
            {
                List<v_UserRole> UserRoleList = DBHelper_User.Get_UserRole_All();
                SetCache(CacheKey, UserRoleList, new TimeSpan(24, 0, 0));

                return UserRoleList;
            }
            else
            {
                return null;
            }
        }

        #region Cache操作方法

        /// <summary>  
        /// 获取数据缓存  
        /// </summary>  
        /// <param name="CacheKey">键</param>  
        private static object GetCache(string CacheKey)
        {
            // 获取当前应用程序的Cache
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        private static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (objObject != null)
                objCache.Insert(CacheKey, objObject);
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        private static void SetCache(string CacheKey, object objObject, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (objObject != null)
                objCache.Insert(CacheKey, objObject, null, DateTime.MaxValue, Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary>  
        private static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (objObject != null)
                objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>  
        /// 移除指定数据缓存  
        /// </summary>  
        private static void RemoveCache(string CacheKey)
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            _cache.Remove(CacheKey);
        }

        /// <summary>  
        /// 移除全部缓存  
        /// </summary>  
        private static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                _cache.Remove(CacheEnum.Key.ToString());
            }
        }

        #endregion
    }
}