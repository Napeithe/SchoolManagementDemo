app.listOfParticipants = app.module(app.listOfParticipants);
app.listOfParticipants = (function() {

    function _init(options) {
        app.table.createTable("participantList")
            .withColumns([{ title: "Nazwa", data: "name" }])
            .withData(options.dataSet)
            .withTitle("Lista uczestników")
            .withClickRowAndGotoById(options.participantDetail)
            .build();
    }

    return {
        init: _init
    };
})();