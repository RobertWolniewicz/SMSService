using SMSService.Core.Models.Domains;
using SMSService.Core.Properties;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace SMSService.Core
{
    public class AppDb : DbContext
    {
        public AppDb()
            : base($"Server={ConfigurationManager.AppSettings["Address"]}\\" +
                  $"{ConfigurationManager.AppSettings["ServerName"]};" +
                  $"Database={ConfigurationManager.AppSettings["DbName"]};" +
                  $"User Id={ConfigurationManager.AppSettings["UserName"]};" +
                  $"Password={ConfigurationManager.AppSettings["Password"]};")
        {
        }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<SendingInformation> SendingInformations { get; set; }
    }
}