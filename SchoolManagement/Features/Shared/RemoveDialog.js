app.removeDialog = app.module(app.removeDialog);
app.removeDialog = (function () {
    let _options = {};

    function _init(options) {
        _options = options;
    }

    function _remove(id) {
        $.ajax({
            url: _options.removeUrl,
            data: JSON.stringify({ id }),
            type: "POST",
            contentType: "application/json",
            success: function (data) {
                if (data.status === app.resultStatus.success) {
                    window.location.href = _options.backUrl;
                } else {
                    app.errorMessage(data.errorMessage);
                }
            }
        });
    }

    function _showModal(id) {
        app.modal.create("RemoveUser")
            .withTitle(_options.title)
            .withContent(_options.content)
            .withCancel()
            .withAccept({
                text: "Tak",
                onClick: (e) => {
                    app.removeDialog.remove(id);
                },
                style: "danger"
            })
            .show();
    }

    return {
        showModal: _showModal,
        remove: _remove,
        init: _init
    };
}());