module.exports = {
    renderView: function (callback, path, model, viewBag) {
        console.time("renderView");
        callback(null, {
            html: "<html><head></head><body><p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p></body>",
            status: 200,
            redirect: null
        });
        console.timeEnd("renderView");
    },
    renderPartialView: function (callback, path, model, viewBag) {
        console.time("renderPartialView");
        callback(null, {
            html: "<p><strong>Model:</strong> " + JSON.stringify(model) + "</p><p><strong>ViewBag:</strong> " + JSON.stringify(viewBag) + "</p>"
        });
        console.timeEnd("renderPartialView");
    }
};