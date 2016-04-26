namespace Erpk.Models.MyPlaces
{
    public class ProductionTask
    {
        public ProductionTask(int companyId, bool ownWork, int employeeWorks = 0)
        {
            CompanyId = companyId;
            EmployeeWorks = employeeWorks;
            OwnWork = ownWork;
        }

        public int CompanyId { get; }
        public bool OwnWork { get; }
        public int EmployeeWorks { get; }
    }
}