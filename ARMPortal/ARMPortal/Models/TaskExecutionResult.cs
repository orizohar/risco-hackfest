namespace ARMPortal.Models
{
    public class TaskExecutionResult
    {
        public Enums.TaskStatus TaskExecutionStatus { get; set; }

        public string TaskExecutionMessage { get; set; }
    }
}