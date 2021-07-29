using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accAfpslaiEmvObjct
{
    public class user
    {
        
        public int userId { get; set; }
        public string userName { get; set; }
        public string userPass { get; set; }

        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string suffixName { get; set; }

        public string status { get; set; }

        public string fullName { get; set; }
        public int roleId { get; set; }
        public string roleDesc { get; set; }

        public DateTime dateCreated { get; set; }

        public bool is_deleted { get; set; }

        public bool isChangePassword { get; set; }

    }


}