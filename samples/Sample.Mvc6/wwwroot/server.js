
var RenderView = function (path, model) {
    return "<html><head></head><body><strong>" + model.Greeting + "</strong></body>";
};

var RenderPartialView = function (path, model) {
    return "<div><strong>" + model.Greeting + "</strong></div>";
};