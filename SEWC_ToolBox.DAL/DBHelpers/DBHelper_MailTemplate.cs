using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.EFs;
using SEWC_ToolBox.DAL.SecondModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.DBHelpers
{
    public static class DBHelper_MailTemplate
    {
        public static t_MailTemplate Get_MailTemplate_ByName(string TemplateName)
        {
            SEWC_ToolBoxEntities dbContext = new SEWC_ToolBoxEntities();

            try
            {
                var result = from mt in dbContext.t_MailTemplate
                             where mt.t_mt_Name == TemplateName
                             select mt;

                t_MailTemplate TargetTemplate = result.First<t_MailTemplate>();

                return TargetTemplate;
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
    }
}
