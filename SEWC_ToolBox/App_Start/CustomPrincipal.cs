using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SEWC_ToolBox_Project
{
    public class CustomPrincipal : IPrincipal
    {
        public string  Gid { get; set; }

        public string Email { get; set; }

        public IIdentity Identity
        {
            get; private set;
        }

        public bool IsInRole(string role)
        {
            return true;
        }

        public CustomPrincipal(string gid)
        {
            Identity = new GenericIdentity(gid);
        }

    }
}