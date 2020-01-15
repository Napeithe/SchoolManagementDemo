app.login = app.module(app.login);
app.login = (function() {
    function _init() {
        app.addUtcToForm("loginForm");
    }

    return {
        init: _init
    }
}());