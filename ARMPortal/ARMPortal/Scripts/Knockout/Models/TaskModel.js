var TaskModel = {
    taskDelay: ko.observable(30000),
    environmentName: ko.observable(''),
    availableSubscriptions: ko.observableArray(['Dev', 'QA', 'Production']),
    selectedSubscription: ko.observable(''),
    availableRegions: ko.observableArray(['West Europe', 'North Europe', 'West US']),
    selectedRegion: ko.observable(''),
    availableCss: ko.observableArray(['progress-bar-primary', 'progress-bar-success', 'progress-bar-info', 'progress-bar-warning', 'progress-bar-danger']),
    selectedCss: ko.observable('progress-bar-primary'),
    taskRunning: ko.observable(true),
    taskMessage: ko.observable(''),
    taskStatus: ko.observable(0)
};