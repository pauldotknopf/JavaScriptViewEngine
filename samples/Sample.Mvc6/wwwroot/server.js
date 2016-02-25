
var RenderView = function (path, model) {
    return {
        html: "<html><head></head><body><strong>" + model.Greeting + "</strong></body>",
        status: 200,
        redirect: null
    };
};

var RenderPartialView = function (path, model) {
    return "<div><strong>" + model.Greeting + "</strong></div>";
};