app.groupMember = app.module(app.groupMember);

app.groupMember.edit = (function () {
    var _options = {};

    function _init(options) {
        _options = options;
    }

    function _passRemoveDialog(passId) {
        app.removeDialog.init(_options.removeDialog);

        app.removeDialog.showModal(passId);
    }

    return {
        passRemoveDialog: _passRemoveDialog,
        init: _init
    };
})();