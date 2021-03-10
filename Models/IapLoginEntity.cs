using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseduConsole.Models
{
    class IapLoginEntity
    {
        public string LoginUrl { get; set; }
        public string DoLoginUrl { get; set; }
        public string ItUrl { get; set; }
        public string NeedcaptchaUrl { get; set; }
        public string CaptchaUrl { get; set; }

        public IapLoginEntity(string loginUrl,
            string doLoginUrl,
            string itUrl,
            string needcpatchaUrl,
            string captchaUrl)
        {
            this.LoginUrl = loginUrl;
            this.DoLoginUrl = doLoginUrl;
            this.ItUrl = itUrl;
            this.NeedcaptchaUrl = needcpatchaUrl;
            this.CaptchaUrl = captchaUrl;
        }

        public override string ToString()
        {
            return "IapLoginEntity{" +
                "loginUrl='" + LoginUrl + '\'' +
                ", doLoginUrl='" + DoLoginUrl + '\'' +
                ", itUrl='" + ItUrl + '\'' +
                ", needcaptchaUrl='" + NeedcaptchaUrl + '\'' +
                ", captchaUrl='" + CaptchaUrl + '\'' +
                '}';
        }
    }
}
