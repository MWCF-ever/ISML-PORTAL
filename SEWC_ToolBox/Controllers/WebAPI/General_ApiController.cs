using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.Utilities.Helpers;
using SEWC_ToolBox_Project;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SEWC_ToolBox.Controllers.WebAPI
{
    public class General_ApiController : ApiController
    {
        /// <summary>
        /// 切换语言
        /// </summary>
        [HttpPost]
        // POST: api/General_Api/LanguageSwitch?languageType=0
        public void Language_Switch([FromBody]int languageType)
        {
            // 保证值在可取范围
            if (languageType != 0 && languageType != 1)
                languageType = 1;
            
            // 从缓存读取用户语言偏好列表
            List<t_LanguageProfile> LanguageProfileList = CacheConfig.Get<List<t_LanguageProfile>>(CacheConfig.CacheType.LanguageProfileList);
            v_User CurrentUser = HttpContext.Current.Session["CurrentUser"] as v_User;

            // 找到当前用户偏好
            t_LanguageProfile CurUserLanguage = LanguageProfileList.Where(o => o.lp_u_GID == CurrentUser.User_GID).FirstOrDefault();

            // 更新偏好表
            if (CurUserLanguage != null)
            {
                CurUserLanguage.lp_LanguageType = languageType;
                DBHelper_User.Update_LanguageProfile(CurUserLanguage);
            }

            HttpContext.Current.Session["CurrentLanguage"] = languageType;
        }
    }
}
