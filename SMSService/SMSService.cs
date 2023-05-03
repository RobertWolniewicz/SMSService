using Cipher;
using SMSSender;
using SMSService.Core.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace SMSService
{
    public partial class SMSService : ServiceBase
    {
        private readonly int _intervalInMinutes;
        private readonly Timer _timer;
        private InvoiceRepository _invoiceRepository = new InvoiceRepository();
        private SMS _sms;
        private string _phoneNumber = ConfigurationManager.AppSettings["PhonNumber"];
        private static readonly NLog.Logger Logger =
            NLog.LogManager.GetCurrentClassLogger();
        private StringCipher _stringCipher = new StringCipher("2B39665D-8181-4080-AB39-1B46AAE233AA");
        private const string NonEncryptedPasswordPrefix = "encrypt:";

        public SMSService()
        {
            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _timer = new Timer(_intervalInMinutes * 60000);
                _sms = new SMS(new SMSParams
                {
                    Sender = ConfigurationManager.AppSettings["Sender"],
                    UserName = ConfigurationManager.AppSettings["UserName"],
                    Password = DecryptSenderEmailPassword()
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }

            InitializeComponent();
        }

        private string DecryptSenderEmailPassword()
        {
            var encryptedPassword = ConfigurationManager.AppSettings
                ["Password"];

            if (encryptedPassword.StartsWith(NonEncryptedPasswordPrefix))
            {
                encryptedPassword = _stringCipher
                    .Encrypt(encryptedPassword.Replace(NonEncryptedPasswordPrefix, ""));
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["SenderEmailPasswor"].Value = encryptedPassword;

                configFile.Save();
            }

            return _stringCipher.Decrypt(encryptedPassword);
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started.");
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendInvoice();
                SendThanks();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private void SendInvoice()
        {
            var invoiceToSend = _invoiceRepository.GetUnpaidInvoices();

            if (invoiceToSend == null || !invoiceToSend.Any())
                return;


            foreach (var invoice in invoiceToSend)
            {
                var text = $@"Wystawiono nową fakturę o numerze {invoice.Number} dla {invoice.Vendee} na kwote {invoice.Value}. 
                    Termin płatności {invoice.DateOfCreation.AddMonths(1).ToShortDateString()}";

                _sms.Send(text, _phoneNumber);

                _invoiceRepository.UnpaidInvoiceSent(invoice);
            }

            Logger.Info("Invoices sent.");
        }

        private void SendThanks()
        {
            var invoiceToThanks = _invoiceRepository.GetInvoicesToThanks();

            if (invoiceToThanks == null || !invoiceToThanks.Any())
                return;


            foreach (var invoice in invoiceToThanks)
            {
                var text = $@"Dziękujemy za opłacenie faktury o numerze {invoice.Number}";

                _sms.Send(text, _phoneNumber);

                _invoiceRepository.ThanksSent(invoice);
            }

            Logger.Info("Thanks sent.");
        }

        protected override void OnStop()
        {
            Logger.Info("Service stoped...");
        }
    }
}
