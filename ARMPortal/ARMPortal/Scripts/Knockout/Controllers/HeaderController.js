function HeaderController() {
    var self = this;

    self.tasks = ko.observableArray([]);

    self.removeTask = function (task, e) {
        $.connection.taskManagerHub.server.removeTask(task.taskId());
        e.stopPropagation();
    }

    self.tasksNumber = ko.computed(function () {
        return self.tasks().length;
    }, self);

    $.connection.taskManagerHub.client.progressChanged = function (taskList) {
        if (taskList.length == 0)
            self.tasks.removeAll();
        else {
            // Removed
            var diff = self.tasks().filter(function (oldTask) {
                return taskList.every(function (newTask) {
                    return oldTask.taskId() !== newTask.taskId;
                });
            });
            
            $.each(diff, function (index, task) {
                self.tasks.remove(task);
            });
            

            // Added or Updated
            $.each(taskList, function (index, item) {
                var foundTask = taskList.filter(function (t) { return t.taskId === item.taskId });
                var existingTask = self.tasks().filter(function (t) { return t.taskId() === item.taskId });
                if (foundTask.length > 0 && existingTask.length > 0) {
                    var temp = new TaskProgressModel(foundTask[0]);
                    self.tasks.remove(existingTask[0]);
                    self.tasks.push(temp);
                } else
                    self.tasks.push(new TaskProgressModel(item));
            });
        }
    };    
}