using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiseduConsole.Models
{
    class IapLoginEntityBuilder
    {
        private String loginUrl;
        private String doLoginUrl;
        private String itUrl;
        private String needcaptchaUrl;
        private String captchaUrl;
        private String host;
        private String protocol;

        public IapLoginEntityBuilder LoginUrl(string loginUrl)
        {
            Uri url = null;
            try
            {
                url = new Uri(loginUrl);
            }
            catch (System.UriFormatException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            this.host = url.Host;
            this.protocol = url.Scheme;
            this.loginUrl = loginUrl;
            return this;
        }

        public IapLoginEntity Build()
        {
            this.doLoginUrl = protocol + "://" + host + "/iap/doLogin";
            this.itUrl = protocol + "://" + host + "/iap/security/lt";
            this.needcaptchaUrl = protocol + "://" + host + "/iap/checkNeedCaptcha";
            this.captchaUrl = protocol + "://" + host + "/iap/generateCaptcha";

            return new IapLoginEntity(loginUrl, doLoginUrl, itUrl, needcaptchaUrl, captchaUrl);
        }
    }
}
