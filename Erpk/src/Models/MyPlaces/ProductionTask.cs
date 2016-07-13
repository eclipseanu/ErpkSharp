namespace Erpk.Models.MyPlaces
{
    public class ProductionTask
    {
        public ProductionTask(uint companyId, bool ownWork, int employeeWorks = 0)
        {
            CompanyId = companyId;
            EmployeeWorks = employeeWorks;
            OwnWork = ownWork;
        }

        public uint CompanyId { get; }
        public bool OwnWork { get; }
        public int EmployeeWorks { get; }
    }
}