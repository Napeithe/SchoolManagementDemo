app.room = app.module(app.room);
app.room.add = (function() {
    function _init() {
        $("#roomColor").colorpicker({
            useHashPrefix: false
        });
    }

    return {
        init: _init
    };
}());