using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public struct sOnlineRoleInfo
    {
        public Dictionary<int, cAccessRole> lstonlineroles;
        public List<int> lstroleids;
    }
}
