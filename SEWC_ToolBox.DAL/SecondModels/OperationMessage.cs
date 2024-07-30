using SEWC_ToolBox.DAL.EFs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SEWC_ToolBox.DAL.SecondModels
{
    /// <summary>
    /// 后台处理结果实体，可序列化后返回给前台
    /// </summary>
    public class OperationMessage
    {
        // 操作结果标识
        public bool OpResult { get; set; }
        // 结果消息
        public string OpMsg { get; set; }
        public string EXT01 { get; set; }
        public string EXT02 { get; set; }
        public string EXT03 { get; set; }
        public List<v_MenuList> MenuList { get; set; }
    }
}