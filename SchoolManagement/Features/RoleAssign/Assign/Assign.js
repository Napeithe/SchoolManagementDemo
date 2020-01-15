app.anchors = app.module(app.anchors)
app.anchors.assign = app.module(app.anchors.assign);
app.anchors.assign = (function() {
    function _init() {
        $("#userList").select2({
            language: "pl",
            width: "100%"
        });
    }

    return {
        init: _init
    }

})();