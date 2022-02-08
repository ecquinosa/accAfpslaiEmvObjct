using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace accAfpslaiEmvObjct.CBS.Core.Message
{
    public class TransactionServiceMessageBase : MessageBase
    {
        [JsonRequired]
        public TransactionMessageHeader Header { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TranAmount { get; set; }
        public string AccountNo { get; set; }
        public long ReversalSequenceNo { get; set; }
        public string ReversalReason { get; set; }
    }

    public class TransactionMessageHeader
    {
        public string MsgVerNo { get; set; }
        [JsonRequired]
        public string SourceId { get; set; }
        [JsonRequired]
        public string TranCode { get; set; }
        [JsonRequired]
        public string ReferenceID { get; set; }
        [JsonRequired]
        public DateTime BusinessDate { get; set; }
        public DateTime TransactionTime { get; set; }
        [JsonRequired]
        public TransactionStatus TransactionStatus { get; set; }
        [JsonRequired]
        public ProcessingMode ProcessingMode { get; set; }
        [JsonRequired]
        public string UserId { get; set; }
        public string SupervisorId { get; set; }
        [JsonRequired]
        public string BranchCode { get; set; }
        [JsonRequired]
        public long SequenceNo { get; set; }
        public string ClientIPAddress { get; set; }
        public string ClientWorkstationName { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}
