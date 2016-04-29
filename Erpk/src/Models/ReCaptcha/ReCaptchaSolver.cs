using System.Threading.Tasks;

namespace Erpk.Models.ReCaptcha
{
    public delegate Task<ReCaptchaSolution> ReCaptchaSolver();
}