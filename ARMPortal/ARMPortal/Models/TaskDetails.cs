#region Using Statements

using System.Threading;
using Newtonsoft.Json;

#endregion

namespace ARMPortal.Models
{
    public class TaskDetails
    {
        [JsonProperty("taskId")]
        public string Id { get; set; }

        [JsonProperty("taskName")]
        public string Name { get; set; }

        [JsonProperty("taskPercent")]
        public int Percent { get; set; }

        [JsonProperty("barCss")]
        public string BarCss { get; set; }

        [JsonProperty("taskStatus")]
        public Enums.TaskStatus Status { get; set; }

        [JsonProperty("taskMessage")]
        public string TaskMessage { get; set; }

        [JsonProperty("taskPage")]
        public string TaskPage { get; set; }

        [JsonProperty("taskRunning")]
        public bool TaskRunning { get; set; }
    }
}