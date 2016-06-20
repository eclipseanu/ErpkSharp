using Erpk.Json;
using Newtonsoft.Json;

namespace Erpk.Models.Activities
{
    public enum WorkResultMessage
    {
        [JsonEnum(true)] Successful,
        [JsonEnum("already_worked")] AlreadyWorked,
        [JsonEnum("not_worked")] NotWorked,
        [JsonEnum("employee")] NotEmployed,
        [JsonEnum("no_rest_points")] NotEnoughOvertimePoints,
        [JsonEnum("money")] EmployerHasNoMoney,
        [JsonEnum("not_enough_storage")] NotEnoughStorage,
        [JsonEnum("not_enough_works")] NotEnoughWorkTickets,
        [JsonEnum("redirect")] NoCompaniesSelected
    }

    public class WorkResponseJson : CommonResponseJson
    {
        [JsonConverter(typeof(EnumConverter))]
        public WorkResultMessage message { get; set; }
    }

    public abstract class CommonWorkResultJson
    {
        public string to_achievment { get; set; }
        public string to_achievment_text { get; set; }

        public int xp { get; set; }
        public int health { get; set; }
    }

    public class WorkSuccessfulResultJson : CommonWorkResultJson
    {
        public float netSalary { get; set; }
        public float grossSalary { get; set; }
        public float tax { get; set; }
        public string currency { get; set; }
        public int days_in_a_row { get; set; }
        public bool first_work { get; set; }
        public bool daily_tasks_done { get; set; }
    }

    public class WorkNotEnoughWorkTicketsJson : CommonWorkResultJson
    {
        public int need { get; set; }
        public int limit { get; set; }
    }
}