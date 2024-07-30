using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEWC_ToolBox.DAL.EFs.EnumCollect
{
    /// <summary>
    /// Issue状态枚举
    /// </summary>
    public enum IssueState : short
    {
        [Description("待处理")]
        Panding = 1,

        [Description("已处理")]
        Solved = 2,

        [Description("撤销")]
        Revoke = 9
    }

    public enum IssueGroup : short
    {
        [Description("SCM")]
        SCM = 1,

        [Description("OTHER")]
        OTHER = 9,
    }
}
