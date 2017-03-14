module.exports = {
    renderView: function (callback, path, model, viewBag, routeValues) {
        callback(null, {
            html: "<html><head></head><body>" +
                "<p><strong>Model:</strong> " + JSON.stringify(model) + "</p>" +
                "<p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p>" +
                "<p><strong>EnvironmentVariables:</strong> " + process.env.TEST_VAR + "</p>" +
                "</body>",
            status: 200,
            redirect: null
        });
    },
    renderPartialView: function (callback, path, model, viewBag, routeValues) {
        callback(null, {
            html: "<p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p>"
        });
    }
};