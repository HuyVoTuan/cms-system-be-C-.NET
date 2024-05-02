using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dummy.Infrastructure.Services.EmailService
{
    public class EmailSetting
    {
        public String Mail { get; set; }
        public String DisplayName { get; set; }
        public String Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

    }
}
