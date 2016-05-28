using BSE365.Results;
using BSE365.ViewModels;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Linq;

namespace BSE365.Helper
{
    public static class CatpchaValidator
    {
        public static CaptchaResponse Validate(CaptchaViewModel captcha)
        {
            string myParameters = string.Format("privatekey={0}&response={1}&challenge={2}&remoteip={3}", 
                ConfigurationManager.AppSettings["captchaSecret"], captcha.Response, captcha.Challenge,
                "127.0.0.1");

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                var reply = wc.UploadString(ConfigurationManager.AppSettings["captchaUrl"], myParameters);

                var results = reply.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var captchaResponse = new CaptchaResponse
                {
                    Success = Convert.ToBoolean(results[0]),
                    ErrorCodes = results.Skip(1).ToList(),                    
                };

                return captchaResponse;
            }            
        }
    }
}