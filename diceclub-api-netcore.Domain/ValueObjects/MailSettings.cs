using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diceclub_api_netcore.Domain.ValueObjects
{
    public class MailSettings
    {
        public static string Server { get; set; }
        public static int Port { get; set; }
        public static string SenderName { get; set; }
        public static string SenderEmail { get; set; }
        public static string UserName { get; set; }
        public static string Password { get; set; }
    }
}
    