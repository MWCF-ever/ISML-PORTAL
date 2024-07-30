using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.Utilities.Export
{
    public class ExcelHelper
    {
        /// <summary>
        /// 导出到execl
        /// </summary>
        /// <typeparam name="T">实体</typeparam>
        /// <param name="entitys">实体集合</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="addSerial">是否添加序号</param>
        /// <returns></returns>
        public static byte[] Export<T>(List<T> entitys, bool addSerial = true)
        {
            Type entityType = typeof(T);
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet");
            PropertyInfo[] entityProperties = entityType.GetProperties();

            //由于T 可能是Object，但它的原型是匿名类型，会导致无法获取其属性，因此需要用数据第一行原型进行判断
            if (entityProperties.Length == 0)
            {
                if (entitys.Count > 0)
                {
                    entityType = entitys[0].GetType();
                    entityProperties = entityType.GetProperties();
                }
            }
            //如果T是Object，但是无内容的情况
            if (entityProperties.Length == 0)
            {
                IRow noRow = sheet.CreateRow(0);
                noRow.CreateCell(0).SetCellValue("没有数据可以导出，请更改过滤条件。");
            }
            else
            {

                IRow titleRow = sheet.CreateRow(0);
                var start = 0;
                var offset = 0;
                if (addSerial)
                {
                    start = 1;
                    offset = 1;
                    titleRow.CreateCell(0).SetCellValue("序号");
                }
                for (int k = start; k < entityProperties.Length + offset; k++)
                {
                    var propertyInfo = entityProperties[k - offset];
                    string title = propertyInfo.Name;
                    var descAttribute = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
                    if (descAttribute != null)
                    {
                        title = descAttribute.Description;
                    }
                    titleRow.CreateCell(k).SetCellValue(title);
                }

                for (int i = 0; i < entitys.Count; i++)
                {
                    IRow rows = sheet.CreateRow(i + 1);
                    object entity = entitys[i];
                    if (addSerial)
                    {
                        rows.CreateCell(0).SetCellValue(i + 1);
                    }
                    for (int j = start; j < entityProperties.Length + offset; j++)
                    {
                        var properties = entityProperties[j - offset];
                        var val = entityProperties[j - offset].GetValue(entity);
                        if (val == null)
                        {
                            val = string.Empty;
                        }
                        var fildType = properties.PropertyType;
                        if (fildType.IsEnum
                            || fildType == typeof(string))
                        {
                            rows.CreateCell(j).SetCellType(CellType.String);
                        }
                        else if (fildType == typeof(bool)) rows.CreateCell(j).SetCellType(CellType.Boolean);
                        else if (fildType == typeof(int)
                             || fildType == typeof(long)
                             || fildType == typeof(short)
                             || fildType == typeof(byte)
                             || fildType == typeof(double)
                             || fildType == typeof(float))
                        {
                            rows.CreateCell(j).SetCellType(CellType.Numeric);
                        }
                        else rows.CreateCell(j).SetCellType(CellType.String);

                        rows.CreateCell(j).SetCellValue(val.ToString());
                    }
                }
            }

            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.ToArray();
                ms.Close();
            }

            return buffer;
        }

    }
}
