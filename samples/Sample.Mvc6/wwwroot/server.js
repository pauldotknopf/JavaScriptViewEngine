
var RenderView = function (path, model, viewBag) {
    return {
        html: "<html><head></head><body><p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p></body>",
        status: 200,
        redirect: null
    };
};

var RenderPartialView = function (path, model, viewBag) {
    return "<div><p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p></div>";
};