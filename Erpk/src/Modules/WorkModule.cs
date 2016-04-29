using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erpk.Http;
using Erpk.Models.Activities;
using Erpk.Models.MyPlaces;

namespace Erpk.Modules
{
    public class WorkModule : Module
    {
        /// <summary>
        ///     Retrieves my companies page.
        /// </summary>
        public async Task<MyCompaniesPage> MyCompaniesPage()
        {
            var res = await Client.Get("economy/myCompanies").Send();
            return new MyCompaniesPage(res);
        }

        /// <summary>
        ///     Works as employee.
        /// </summary>
        public async Task<WorkResponseJson> WorkAsEmployee()
        {
            var req = await Work("work", "work");
            var res = await req.Send();
            return res.JSON<WorkResponseJson>();
        }

        /// <summary>
        ///     Works overtime.
        /// </summary>
        public async Task<WorkResponseJson> WorkOvertime()
        {
            var req = await Work("workOvertime", "workOvertime");
            var res = await req.Send();
            return res.JSON<WorkResponseJson>();
        }

        /// <summary>
        ///     Works as manager in selected companies.
        /// </summary>
        public async Task<WorkResponseJson> WorkAsManager(IEnumerable<ProductionTask> queue)
        {
            var req = await Work("production", "work");

            var i = 0;
            foreach (var company in queue.Where(company => company.OwnWork || company.EmployeeWorks > 0))
            {
                req.Form.Add($"companies[{i}][id]", company.CompanyId);
                req.Form.Add($"companies[{i}][employee_works]", company.EmployeeWorks);
                req.Form.Add($"companies[{i}][own_work]", company.OwnWork);
                i++;
            }

            var res = await req.Send();
            return res.JSON<WorkResponseJson>();
        }

        private async Task<Request> Work(string actionType, string urlAction)
        {
            var companies = await MyCompaniesPage();
            var url = "economy/" + (companies.HasCaptcha ? "captchaAjax" : urlAction);
            var req = Client.Post(url).CSRF().XHR();
            req.AddReferer("economy/myCompanies");
            if (companies.HasCaptcha)
            {
                req.Form.Add(await Client.SolveCaptcha());
            }
            req.Form.Add("action_type", actionType);
            return req;
        }
    }
}