app.listOfAnchors = app.module(app.listOfAnchors);
app.listOfAnchors = (function() {

    function _init(options) {
        app.table.createTable("anchorsList")
            .withColumns([{ title: "Nazwa", data: "name" }])
            .withData(options.dataSet)
            .withTitle("Lista prowadzących")
            .withClickRowAndGotoById(options.anchorDetail)
            .build();
    }

    return {
        init: _init
    };
})();