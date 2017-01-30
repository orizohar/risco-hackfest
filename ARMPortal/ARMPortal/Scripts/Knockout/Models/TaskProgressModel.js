function TaskProgressModel(data) {
    ko.mapping.fromJS(data, {}, this);

    ////////from mapping
    //taskId
    //taskName
    //taskPercent
    //barCss
    //taskPage
    //taskRunning   
    this.taskPercentCss = ko.computed(function () {
        return this.taskPercent() + '%';
    }, this);
}