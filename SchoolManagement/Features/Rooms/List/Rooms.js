
app.rooms = app.module(app.rooms);
app.rooms = (function() {
    function _init(options) {
        app.table.createTable("roomList")
            .withColumns([{ title: "Nazwa pokoju", data: "name" }])
            .withData(options.dataSet)
            .withTitle("Lista pokoi")
            .withClickRowAndGotoById(options.editRoom)
            .build();
    }

    return {
        init: _init
    };
})();