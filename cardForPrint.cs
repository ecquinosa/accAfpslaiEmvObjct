using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace accAfpslaiEmvObjct
{
    public class cardForPrint
    {

        public Nullable<int> cardId { get; set; }
        public int memberId { get; set; }
        public string cif { get; set; }
        public string cardNo { get; set; }
        public string cardName { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public string suffix { get; set; }
        public Nullable<DateTime> membership_date { get; set; }
        public string card_valid_thru { get; set; }
        public string gender { get; set; }
        public string branch_issued { get; set; }
        public string mobileNo { get; set; }
        public string terminalId { get; set; }
        public Nullable<DateTime> dateCaptured { get; set; }
        public Nullable<DateTime> datePrinted { get; set; }
        public Nullable<TimeSpan> timePrinted { get; set; }
        public string base64Photo { get; set; }

    }


}