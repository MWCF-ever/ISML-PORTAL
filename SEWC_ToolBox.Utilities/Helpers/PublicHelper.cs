﻿using SEWC_NetDevLib.SEWC_NetLibExtend;
using SEWC_ToolBox.DAL.DBHelpers;
using SEWC_ToolBox.DAL.EFs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Helpers
{
    // 公用方法，主要用于通用的数据、结构处理
    public static class PublicHelper
    {
        /// <summary>
        /// 通过反射将类对象的所有列值初始化
        /// </summary>
        /// <typeparam name="S">实体类型</typeparam>
        /// <param name="source">实体对象</param>
        public static void ObjectReset<S>(S source)
        {
            PropertyInfo[] PropertyList = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo CurrentProperty in PropertyList)
            {
                var CurrentValue = CurrentProperty.GetValue(source);
                var CurrentType = CurrentProperty.PropertyType;

                if (CurrentType == typeof(int))
                    CurrentProperty.SetValue(source, 0);
                else if (CurrentType == typeof(string))
                    CurrentProperty.SetValue(source, string.Empty);
                else if (CurrentType == typeof(bool))
                    CurrentProperty.SetValue(source, false);
            }
        }

        /// <summary>
        /// 邮件模板参数替换 
        /// 通过对象属性替换模板内容
        /// </summary>
        /// <param name="MailTemplate">模板</param>
        /// <param name="Parameters">参数对象，不支持自定义对象</param>
        /// <returns></returns>
        public static string GenerateMailTemplateContent(string MailTemplate, object Parameters)
        {
            var res = string.Empty;
            if (string.IsNullOrEmpty(MailTemplate))
            {
                EventlogHelper.AddLog("模板内容为空");
                return res;
            }
            if (Parameters == null)
            {
                return res;
            }

            try
            {
                var type = Parameters.GetType();
                var Properties = type.GetProperties();
                foreach (var Propertie in Properties)
                {
                    var Val = Propertie.GetValue(Parameters);
                    //如果当前属性还有嵌套对象，递归获取内容
                    if (!Propertie.PropertyType.IsPrimitive && Propertie.PropertyType != typeof(string))
                    {
                        MailTemplate = GenerateMailTemplateContent(MailTemplate, Val);
                    }
                    else
                    {
                        var Key = $"$$.{Propertie.Name}";
                        if (Val != null)
                        {
                            MailTemplate = MailTemplate.Replace(Key, Val.ToString());
                        }
                    }
                }
                res = MailTemplate;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
            }

            return res;
        }

        // 替换邮件参数
        public static string GenerateMailContent(string MailTemplate, Dictionary<string, string> Parameters)
        {
            try
            {
                if (Parameters != null)
                {
                    foreach (KeyValuePair<string, string> CurKVP in Parameters)
                    {
                        MailTemplate = MailTemplate.Replace("$$." + CurKVP.Key, CurKVP.Value);
                    }
                }

                return MailTemplate;
            }
            catch (Exception ex)
            {
                EventlogHelper.AddLog(ex.Message);
                return MailTemplate;
            }
        }

        // 获取选中的三级菜单ActionName
        public static v_MenuList Get_ThirdMenu(object TempData_Third_ActionName, string ControllerName, string Main_ActionName, string Sub_ActionName)
        {
            v_MenuList ThirdMenu = new v_MenuList();

            // 若已选二级菜单，则继续
            if (!string.IsNullOrWhiteSpace(Sub_ActionName))
            {
                // 若有已选三级菜单(或无三级菜单)，则返回
                if (TempData_Third_ActionName != null)
                    ThirdMenu.ml_Third_ActionName = TempData_Third_ActionName.ToString();
                // 若未加载过三级菜单，则加载初始化被选中的三级菜单
                else
                    ThirdMenu = DBHelper_EntryMenu.Get_DefaultThirdMenu(ControllerName, Main_ActionName, Sub_ActionName);
            }

            return ThirdMenu;
        }

        // 获取选中的si级菜单ActionName
        public static v_MenuList Get_FourthMenu(object TempData_Fourth_ActionName, string ControllerName, string Main_ActionName, string Sub_ActionName, string third_ActionName)
        {
            v_MenuList FourthMenu = new v_MenuList();

            // 若已选二级菜单，则继续
            if (!string.IsNullOrWhiteSpace(third_ActionName))
            {
                // 若有已选三级菜单(或无三级菜单)，则返回
                if (TempData_Fourth_ActionName != null)
                    FourthMenu.ml_Fourth_ActionName = TempData_Fourth_ActionName.ToString();
                // 若未加载过三级菜单，则加载初始化被选中的三级菜单
                else
                    FourthMenu = DBHelper_EntryMenu.Get_DefaultFourthMenu(ControllerName, Main_ActionName, Sub_ActionName, third_ActionName);
            }

            return FourthMenu;
        }
    }
}
