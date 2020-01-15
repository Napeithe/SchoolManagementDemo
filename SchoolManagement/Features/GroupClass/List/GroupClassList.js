app.groupClass = app.module(app.groupClass);
app.groupClass.list = (function() {
    function _init(options) {
        app.table.createTable("groupClassList")
            .withColumns([
                { title: "Nazwa", data: "name" },
                { title: "Limit miejsc", data: "limitParticipants" },
                { title: "Obecna liczba uczestników", data: "currentParticipants" }])
            .withData(options.dataSet)
            .withTitle("Zajęcia grupowe")
            .withClickRowAndGotoById(options.editRoom)
            .build();
    }

    return {
        init: _init
    };
})();