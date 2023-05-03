using SMSService.Core.Models.Domains;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SMSService.Core.Repositories
{
    public class InvoiceRepository
    {
        public List<Invoice> GetUnpaidInvoices()
        {
            using (var context = new AppDb())
            {
                return context.Invoices.Include(i => i.SendingInformation).Where(i => (i.IsPay == false) && (i.SendingInformation.IsSend == false)).ToList();
            }
        }

        public List<Invoice> GetInvoicesToThanks()
        {
            using (var context = new AppDb())
            {
                return context.Invoices.Where(i => (i.IsPay == true) && (i.SendingInformation.IsThanksSend == false)).ToList();
            }
        }

        public void UnpaidInvoiceSent(Invoice invoice)
        {
            using (var context = new AppDb())
            {
                var sentInvoice = context.Invoices.Include(i => i.SendingInformation).First(i => i.Id == invoice.Id);
                sentInvoice.SendingInformation.IsSend = true;
                context.SaveChanges();
            }
        }

        public void ThanksSent(Invoice invoice)
        {
            using (var context = new AppDb())
            {
                var invoiceThatWasThanked = context.Invoices.Include(i => i.SendingInformation).First(i => i.Id == invoice.Id);
                invoiceThatWasThanked.SendingInformation.IsThanksSend = true;
                context.SaveChanges();
            }
        }
    }
}
