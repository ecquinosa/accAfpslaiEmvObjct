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
    
    public partial class Header_Request
    {
        public string MsgVerNo { get; set; }
        public string SourceId { get; set; }            
        public string TranCode { get; set; }
        public string ReferenceID { get; set; }
        public Nullable<System.DateTime> BusinessDate { get; set; }
        public Nullable<System.DateTime> TransactionTime { get; set; }
        public int TransactionStatus { get; set; }
        public int ProcessingMode { get; set; }       
        public string UserId { get; set; }
        public string SupervisorId { get; set; }
        public string BranchCode { get; set; }
        public int SequenceNo { get; set; }
        public string ClientIPAddress { get; set; }
        public string ClientWorkstationName { get; set; }

    }
}