using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Erpk.Modules
{
    public class ReCaptchaModule : Module
    {
        private const string BaseUrl = "https://www.google.com/recaptcha/api/";
        private const string PublicKey = "6LeWK7wSAAAAAA_QFoHVnY5HwVCb_CETsvrayFhu";

        public static string NoscriptUrl => string.Format(BaseUrl + "noscript?k={0}", PublicKey);

        /// <summary>
        ///     Parses URL of following format: "http://www.google.com/recaptcha/api/image?c={challenge}" to "{challenge}
        /// </summary>
        public static string ParseChallengeFromImageUrl(string url)
        {
            return Regex.Match(url, @"image\?c=(.+)").Groups[1].Value;
        }

        /// <summary>
        ///     Returns ReCaptcha challenge code.
        /// </summary>
        public async Task<string> GetNewChallenge()
        {
            try
            {
                var req = Client.Get(string.Format(BaseUrl + "challenge?k={0}", PublicKey)).DisableAutologin();
                var res = await req.Send();

                var html = await res.Content.ReadAsStringAsync();
                var match = Regex.Match(html, "challenge : '(.+?)'");

                return match.Captures.Count == 0 ? null : match.Groups[1].Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Returns ReCaptcha challenge image.
        /// </summary>
        public async Task<Stream> GetCaptchaImageStream(string challenge)
        {
            var req = Client.Get(string.Format(BaseUrl + "image?c={0}", challenge)).DisableAutologin();
            var res = await req.Send();
            return await res.Content.ReadAsStreamAsync();
        }
    }
}