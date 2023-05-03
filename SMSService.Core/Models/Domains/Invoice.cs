using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSService.Core.Models.Domains
{
    public class Invoice
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public decimal Value { get; set; }
        public string Vendee { get; set; }
        public DateTime DateOfCreation { get; set; }
        public bool IsPay { get; set; }
        public SendingInformation SendingInformation { get; set; }
    }
}
