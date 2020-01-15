
app.userList = app.module(app.userList);
app.userList = (function() {
    function _init(options) {
        app.table.createTable("userList")
            .withColumns([
                { title: "Imię", data: "firstName" },
                { title: "Nazwisko", data: "lastName" },
                { title: "E-mail", data: "email" },
                { title: "Rola", data: "roleName" } ])
            .withData(options.dataSet)
            .withTitle("Lista użytkowników")
            .withClickRowAndGotoById(options.editUserUrl)
            .build();
    }

    return {
        init: _init
    };
})();