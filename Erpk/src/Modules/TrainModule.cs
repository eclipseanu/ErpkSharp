using System.Collections.Generic;
using System.Threading.Tasks;
using Erpk.Models.Activities;
using Erpk.Models.MyPlaces;

namespace Erpk.Modules
{
    public class TrainModule : Module
    {
        /// <summary>
        ///     Retrieves Training Grounds page.
        /// </summary>
        public async Task<TrainingGroundsPage> MyTrainingGroundsPage()
        {
            var req = Client.Get("economy/training-grounds");
            var res = await req.Send();
            return new TrainingGroundsPage(res);
        }

        /// <summary>
        ///     Trains in selected training grounds.
        /// </summary>
        public async Task<TrainResponseJson> Train(IEnumerable<uint> groundIds)
        {
            var trainPage = await MyTrainingGroundsPage();
            var solution = trainPage.HasCaptcha ? await Client.SolveCaptcha() : null;

            var url = "economy/" + (solution == null ? "train" : "captchaAjax");
            var req = Client.Post(url).CSRF().XHR();
            req.AddReferer("economy/training-grounds");

            if (solution != null)
            {
                req.Form.Add(solution);
                req.Form.Add("action_type", "train");
            }

            var i = 0;
            foreach (var groundId in groundIds)
            {
                req.Form.Add($"grounds[{i}][id]", groundId);
                req.Form.Add($"grounds[{i}][train]", 1);
                i++;
            }

            var res = await req.Send();
            return res.JSON<TrainResponseJson>();
        }
    }
}