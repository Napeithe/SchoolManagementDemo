app.modal = app.module(app.modal);
app.modal = (function () {

    function _renderModal(id) {
        let modalContainer = $(`<div class="modal fade" id="removeModal${id}" tabindex="-1" role="dialog" aria-labelledby="removeModal${id}Label" aria-hidden="true"></div>`);
        let modalDialog = $('<div class="modal-dialog" role="document"></div>');
        let modalContent = $('<div class="modal-content"></div>');
        let modalHeader = $('<div class="modal-header"></div>');

        let modalBody = $('<div class="modal-body"></div>');
        let projectCardContainer = $('<div class="mb-3"></div>');
        let modalFooter = $('<div class="modal-footer"></div>');

        modalContainer.append(modalDialog);
        modalDialog.append(modalContent);
        modalContent
            .append(modalHeader)
            .append(modalBody)
            .append(modalFooter);

        modalBody
            .append(projectCardContainer);


        $('body').append(modalContainer);
    }

    function _create(id) {
        let modal = {};
        $(`#removeModal${id}`).remove();
        modal.selector = `#removeModal${id}`;
        _renderModal(id);

        // methods
        modal.withTitle = _withTitle;
        modal.withContent = _withContent;
        modal.withInput = _withInput;
        modal.withCancel = _withCancel;
        modal.withAccept = _withAccept;
        modal.show = _show;

        return modal;
    }

    function _show() {
        let selector = this.selector;
        $(this.selector).modal('show');
        $(this.selector).on('hidden.bs.modal',
            function () {
                $(selector).remove();
            });
    }

    function _withTitle(title) {
        let modalTitle = $(`<h5 class="modal-title">${title}</h5>`);
        $(this.selector).find('.modal-header').append(modalTitle);
        return this;
    }

    function _withContent(content) {
        let text = $(`<div class="mb-3">${content}</div>`);
        $(this.selector).find('.modal-body').append(text);
        return this;
    }

    function _withInput(oldValue) {
        let input = $(`<input id="displayName" type=text class="form-control" name="DisplayName" value=${oldValue}>`);
        $(this.selector).find('.modal-body').append(input);
        return this;
    }

    function _withCancel(text) {
        let unwrappedText = text || "Anuluj";
        let cancelBtn = $(`<button type="button" class="btn btn-secondary" data-dismiss="modal">${unwrappedText}</button>`);
        $(this.selector).find('.modal-footer').append(cancelBtn);
        return this;
    }

    function _withAccept(opt) {
        let unwrappedText = opt.text || "Ok";
        let unwrappedStyle = opt.style || "primary";
        let onClick = opt.onClick || function () {
            console.log(`Button "${unwrappedText}"  clicked.`)
        };

        let acceptButton = $(`<button type="button" class="btn btn-${unwrappedStyle}" data-dismiss="modal">${unwrappedText}</button>`);
        acceptButton.on("click", (e) => {
            onClick(e);
        });


        $(this.selector).find('.modal-footer').append(acceptButton);
        return this;
    }

    return {
        create: _create
    };
}());