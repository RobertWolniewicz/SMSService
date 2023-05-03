using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService.Core.Models.Domains
{
    public class SendingInformation
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public bool IsSend { get; set; }
        public bool IsThanksSend { get; set; }
    }
}
