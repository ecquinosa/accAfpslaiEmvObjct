//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace accAfpslaiEmvObjct
{
    using System;
    using System.Collections.Generic;
    
    public partial class api_request_log
    {
        public int id { get; set; }
        public Nullable<int> member_id { get; set; }
        public Nullable<int> card_id { get; set; }
        public string request { get; set; }
        public string response { get; set; }
        public Nullable<bool> is_success { get; set; }
        public Nullable<System.DateTime> date_post { get; set; }
        public Nullable<System.TimeSpan> time_post { get; set; }
        public string api_owner { get; set; }
        public string api_name { get; set; }
    }
}
