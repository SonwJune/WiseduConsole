using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiseduConsole.Models;
using NSoup;
using NSoup.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Net.Http;

namespace WiseduConsole.Models
{
    class LoginProcess
    {
        private IapLoginEntity loginEntity;
        private Dictionary<string,string> pairs;



        public LoginProcess(string loginUrl, Dictionary<string, string> pairs)
        {
            this.loginEntity = new IapLoginEntityBuilder().LoginUrl(loginUrl).Build();
            this.pairs = pairs;
        }

        public async Task<IDictionary<string,string>> Login()
        {
            //请求 URL: https://aust.campusphere.net/portal/login
            //请求方法: GET
            //状态代码: 302 Moved Temporarily


            //Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
            //Accept-Encoding: gzip, deflate, br
            //Accept-Language: zh-CN,zh;q=0.9
            //Connection: keep-alive
            //Cookie: HWWAFSESID=504cf73e5175e21690; HWWAFSESTIME=1615078159150
            //Host: aust.campusphere.net
            //Referer: https://aust.campusphere.net/portal/index.html
            //Sec-Fetch-Dest: document
            //Sec-Fetch-Mode: navigate
            //Sec-Fetch-Site: same-origin
            //Sec-Fetch-User: ?1
            //Upgrade-Insecure-Requests: 1
            //User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.72 Safari/537.36 Edg/89.0.774.45

            //忽略证书错误
            //...

            //请求登陆页
            IConnection con = NSoupClient.Connect(loginEntity.LoginUrl)
                .FollowRedirects(true);
            IResponse res = con.Execute();

            




            //构造请求头
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string reffer = res.Url().ToString();
            string host = res.Url().Host;
            string protocol = res.Url().Scheme;
            string origin = protocol + "://" + res.Url().Host;

            headers.Add("Host", host);
            headers.Add("Connection", "keep-alive");
            headers.Add("Accept", "application/json, text/plain, */*");
            headers.Add("X-Requested-With", "XMLHttpRequest");
            headers.Add("User-Agent", "Mozilla/5.0 (Linux; Android 4.4.4; OPPO R11 Plus Build/KTU84P) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/33.0.0.0 Safari/537.36");
            headers.Add("Content-Type", "application/x-www-form-urlencoded"); 
            headers.Add("Origin", origin);
            headers.Add("Referer", reffer);
            headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Accept-Language", "zh-CN,zh;q=0.9");

            //全局cookie
            IDictionary<string, string> cookies = res.Cookies();

            string username = this.pairs["username"];
            string password = this.pairs["password"];

            //构造请求参数
            Dictionary<String, String> pairs = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "rememberMe", false.ToString()},
                { "mobile", "" },
                { "dllt", "" }
            };

            //申请It
            string itUrl = loginEntity.ItUrl;
            Dictionary<string, string> itData = new();
            itData.Add("It", res.Url().Query.Substring("_2lBepC=".Length));
            Document doc = NSoupClient.Connect(itUrl)
                .IgnoreContentType(true)
                .Header("Content-Type", "application/x-www-form-urlencoded")
                .Cookies(cookies)
                .Data(itData)
                .Post();
            WiseduResponse message = JsonSerializer
                .Deserialize<WiseduResponse>(doc.Body.Text());
            if (message.Code != 200)
            {
                Console.WriteLine("申请It失败");
            }
            if (message.Results.Lt == null)
            {
                Console.WriteLine("申请It失败");
            }
            pairs.Add("lt", message.Results.Lt);

            //拿到encryptSalt
            string encryptSalt = message.Results.EncryptSalt;
            // 密码暂时不需要加密，不排除后面需要加密的可能
            //pairs.Add("password", AESHelper.encryptAES(password, encryptSalt));

            //登陆地址处理
            string login_url = loginEntity.DoLoginUrl;

            //是否需要验证码地址处理
            string needcaptcha_url = loginEntity.NeedcaptchaUrl
                + "?username="
                + username;

            //模拟登陆之前首先请求是否需要验证码接口
            doc = NSoupClient.Connect(needcaptcha_url)
                .Cookies(cookies)
                .IgnoreContentType(true)
                .Get();

            return await IapSendLoginData(login_url, headers, cookies, pairs);

        }

        private async Task<IDictionary<string,string>> IapSendLoginData(
            string login_url,
            Dictionary<string,string> headers,
            IDictionary<string,string> cookies,
            Dictionary<string,string> pairs)
        {
            Uri uri = new Uri(login_url);

            HttpClientHandler handler = new();
            handler.AllowAutoRedirect = false;
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //添加cookies
            CookieContainer cookieContainer = new();
            foreach (var item in cookies)
            {
                Cookie cookie = new Cookie(item.Key, item.Value,"/iap",uri.Host);
                cookieContainer.Add(cookie);
            }


            handler.CookieContainer = cookieContainer;
            HttpClient client = new(handler);

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key,header.Value);
            }

            Dictionary<string, string> resCookies = new();

            HttpContent content = new FormUrlEncodedContent(pairs);
            try
            {
                HttpResponseMessage res = await client.PostAsync(login_url, content);
                var cookiesEum = res.Headers.GetValues("Set-Cookie");

                //Console.WriteLine(res.Content);

                if (res.StatusCode == HttpStatusCode.OK)
                {
                    //"CASTGC=iap-1020372549012747-TGT-76dff06b-ef09-47a6-8eb3-aee21eb42708; Expires=Sat, 08-Mar-2031 12:14:18 GMT; Path=/iap; HttpOnly"
                    foreach (string cookiesStrs in cookiesEum)
                    {
                        string[] lines = cookiesStrs.Split("; ");
                        foreach (string line in lines)
                        {
                            if (line.IndexOf('=')!= -1)
                            {
                                string[] twostr = line.Split('=');
                                resCookies.Add(twostr[0], twostr[1]);


                            }

                        }

                    }

                    cookies = MergeDict(cookies, resCookies);
                    Console.WriteLine(res.Content);
                }
            }
            catch (Exception)
            {

                throw;
            }


            return cookies;
        }

        private IDictionary<string,string> MergeDict(
            IDictionary<string,string> first,
            Dictionary<string,string> second)
        {
            if (first == null) first = new Dictionary<string, string>();
            if (second == null) return first;
            foreach (var item in second)
            {
                if (!first.ContainsKey(item.Key))
                {
                    first.Add(item.Key, item.Value);
                }
                else
                {
                    first[item.Key] = item.Value;
                }
            }
            return first;

        }


    }
}
