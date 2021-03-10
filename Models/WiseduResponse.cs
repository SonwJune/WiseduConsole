using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WiseduConsole.Models
{
    class WiseduResponse
    {

        [JsonPropertyName("code")]
        public int Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("result")]
        public Result Results { get; set; }

        
        public class Result
        {
            [JsonPropertyName("_encryptSalt")]
            public string EncryptSalt { get; set; }
            [JsonPropertyName("_lt")]
            public string Lt { get; set; }
            [JsonPropertyName("forgetPwdUrl")]
            public string ForgetPwdUrl { get; set; }
            [JsonPropertyName("needCaptcha")]
            public bool NeedCaptcha { get; set; }
        }

    }
}
