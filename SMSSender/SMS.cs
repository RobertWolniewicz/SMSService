using serwersms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSSender
{
    public class SMS
    {
        private string _sender;
        private string _userName;
        private string _password;

        public SMS(SMSParams smsParams)
        {
            _sender = smsParams.Sender;
            _userName = smsParams.UserName;
            _password = smsParams.Password;
        }
        public void Send(string text, string phone)
        { 
            try
            {
                var serwerssms = new SerwerSMS(_userName, _password);
                if (_sender == null)
                    _sender = _userName;
  
                serwerssms.messages.sendSms(phone, text, _sender).ToString();
            }
            catch (Exception)
            {
                throw new Exception("Błąd przy wysyłaniu wiadomości SMS");
            }
        }
        
    }
}
