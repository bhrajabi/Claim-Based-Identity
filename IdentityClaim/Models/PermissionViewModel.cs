using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityClaim.Models
{
    public class PermissionViewModel
    {
        public PermissionViewModel()
        {
            Values = new List<string>();
            Dic = new Dictionary<string, List<CheckPermision>>();
        }
        public List<string> Values { get; set; }
        public Dictionary<string, List<CheckPermision>> Dic { get; set; }
        public string  UserId { get; set; }
    }

    public class CheckPermision
    {
        public bool Allow { get; set; }
        public string Permission { get; set; }
        public string Type { get; set; }
    }
}