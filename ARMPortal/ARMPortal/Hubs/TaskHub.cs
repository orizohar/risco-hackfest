#region Using Statements

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using ARMPortal.Models;
using ARMPortal.Tasks;
using Microsoft.AspNet.SignalR;

#endregion

namespace ARMPortal.Hubs
{
    public class TaskManagerHub : Hub
    {
        private static ConcurrentDictionary<string, TaskDetails> _currentTasks;

        private ConcurrentDictionary<string, TaskDetails> CurrentTasks
        {
            get
            {
                if (_currentTasks == null)
                    _currentTasks = new ConcurrentDictionary<string, TaskDetails>();

                return _currentTasks;
            }
        }

        private void ReportProgress()
        {
            Clients.All.progressChanged(CurrentTasks.Select(t => t.Value));
        }


        public void RemoveTask(string taskId)
        {
            if (CurrentTasks.ContainsKey(taskId))
            {
                TaskDetails taskDetails;
                CurrentTasks.TryRemove(taskId, out taskDetails);
                ReportProgress();
            }
        }

        public async Task<string> StartDeploymentTask(string environmentName, string selectedSubscription,
             string selectedRegion)
        {
            var taskId = $"progress{Guid.NewGuid()}";
            //var taskId = $"{environmentName}{selectedSubscription}{selectedRegion}";

            if (!CurrentTasks.TryAdd(taskId, new TaskDetails
            {
                Percent = 100,
                Id = taskId,
                Name = $"Deployment Task {environmentName} #{CurrentTasks.Count} is Running...",
                BarCss = "progress-bar-primary",
                Status = Enums.TaskStatus.New,
                TaskRunning = true,
                TaskPage = "/"
            }))
            {
                return string.Empty;
            }

            ReportProgress();

            var taskExecution =
                await DeployArmTemplateTask.DeployTemplateAsync(environmentName, selectedSubscription, selectedRegion);

            TaskDetails newDetails = new TaskDetails
            {
                Id = taskId,
                Percent = 100,
                Status = taskExecution.TaskExecutionStatus,
                TaskRunning = false,
                TaskPage = "/"
            };

            switch (newDetails.Status)
            {
                case Enums.TaskStatus.New:
                    newDetails.Name = $"Deployment Task {environmentName} #{CurrentTasks.Count} is Starting...";
                    newDetails.BarCss = "progress-bar-info";
                    break;
                case Enums.TaskStatus.Running:
                    newDetails.Name = $"Deployment Task {environmentName} #{CurrentTasks.Count} is Running...";
                    newDetails.BarCss = "progress-bar-primary";
                    break;
                case Enums.TaskStatus.Succeeded:
                    newDetails.Name = $"Success {environmentName} #{CurrentTasks.Count}";
                    //"$"Deployment Task {environmentName} #{CurrentTasks.Count} Completed Successfully.";
                    newDetails.BarCss = "progress-bar-success";
                    break;
                case Enums.TaskStatus.Failed:
                    newDetails.Name = $"Deployment Task {environmentName} #{CurrentTasks.Count} Failed to Complete.";
                    newDetails.BarCss = "progress-bar-danger";
                    break;
                default:
                    newDetails.Name = $"Deployment Task {environmentName} #{CurrentTasks.Count} initialized.";
                    newDetails.BarCss = "progress-bar-info";
                    break;
            }

            if (!CurrentTasks.TryUpdate(taskId, newDetails, CurrentTasks[taskId]))
            {
                return string.Empty;
            }

            ReportProgress();
            return "Task result";
        }
    }
}