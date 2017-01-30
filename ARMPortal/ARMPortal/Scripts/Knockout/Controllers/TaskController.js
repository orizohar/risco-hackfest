function TaskController() {
    var self = this;

    self.model = TaskModel;

    self.startTask = function () {
        $.connection.taskManagerHub.server.startDeploymentTask(self.model.environmentName(), self.model.selectedSubscription(), self.model.selectedRegion());
    }
}