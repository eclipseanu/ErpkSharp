using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using Erpk.Models.ReCaptcha;

namespace Erpk.Http
{
    public class Form
    {
        private readonly Dictionary<string, string> _fields = new Dictionary<string, string>();

        /// <summary>
        ///     Gets amount of POST fields added.
        /// </summary>
        public int FieldsCount => _fields.Count;

        public void Add(string key, string val) => _fields.Add(key, val);
        public void Add(string key, int val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, uint val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, long val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, ulong val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, float val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, double val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, decimal val) => _fields.Add(key, val.ToString(CultureInfo.InvariantCulture));
        public void Add(string key, bool val) => Add(key, val ? 1 : 0);

        /// <summary>
        ///     Adds captcha solution fields.
        /// </summary>
        public void Add(ReCaptchaSolution solution)
        {
            Add("recaptcha_response_field", solution.Response);
            Add("recaptcha_challenge_field", solution.Challenge);
        }

        /// <summary>
        ///     Encodes form fields to application/x-www-form-urlencoded string.
        /// </summary>
        public FormUrlEncodedContent Encode() => new FormUrlEncodedContent(_fields);
    }
}