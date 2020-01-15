app.presence = app.module(app.presence);
app.presence.show = (function () {
    let _option = {};
    let _presenceType = {
        None: 0,
        Member: 1,
        Help: 2,
        MakeUp: 3
    };

    function _initTable() {
        $("input[data-toggle*=toggle]").bootstrapToggle({
            size: 'small',
            on: '',
            off: ''
        });

        $("#participantPresenceTable").DataTable({
            language: app.tableLanguage,
            responsive: {
                details: true
            }
        });
         $("#additionalParticipantPresenceTable").DataTable({
            language: app.tableLanguage,
            responsive: {
                details: true
            }
        });
    }

    function _init(options) {
        _initTable(options);

        _option = option;

        $('input[data-toggle*=toggle]').change(function () {
            let status = $(this).prop('checked');
            let participantId = $(this).data("participantid");
            app.presence.show.changePresence(participantId, _option.classTimeId, status, _presenceType.Member, _option.changePresenceUrl);
        });

        $('button[class*=participant-remove]').click(function () {
            let participantId = $(this).data("participantid");
            let presenceType = $(this).data("presencetype");
            app.presence.show.changePresence(participantId, _option.classTimeId, false, presenceType, _option.changePresenceUrl, _addUserRow);
        });
    }

    function _addNewUser(userId, presenceType) {
        app.presence.show.changePresence(userId, _option.classTimeId, true, presenceType, _option.changePresenceUrl, _addUserRow);
    }

    function _addUser(userId) {
        $.ajax({
            type: "GET",
            data: { userId: userId },
            url: _option.getNewParticipantDetailUrl,
            contentType: "application/json",
            success: function (result) {
                if (result.status === app.resultStatus.success) {
                    let modal = app.modal.create()
                        .withTitle("Dodanie uczestnika z poza listy")
                        .withContent(
                            `Czy chesz dodać uczestnika <strong>${result.data.name
                            }</strong> jako dodatkowego uczestnika zajęć?<br/><br/> Oznacz zajęcia jako:`)
                        .withCancel("Nie")
                        .withAccept({
                            text: "Pomoc",
                            style: "success",
                            onClick: function () {
                                _addNewUser(result.data.id, _presenceType.Help);
                            }
                        });

                    if (result.data.wasAbsent) {
                        modal = modal
                            .withAccept({
                                text: "Odrabianie",
                                onClick: function () {
                                    _addNewUser(result.data.id, _presenceType.MakeUp);
                                }
                            });
                    }
                    modal.show();
                }
            }
        });
    }

    function _addUserRow() {
       window.location.reload();
    }

    function _changePresence(participantId, classTimeId, isPresence, presenceType, changePresenceUrl, successCallback) {
        let data = {
            participantId: participantId,
            classTimeId: classTimeId,
            isPresence: isPresence, 
            presenceType: presenceType
        };

        $.ajax({
            url: changePresenceUrl,
            type: "POST",
            data: JSON.stringify(data),
            dataType: "json",
            contentType: "application/json",
            success: function (data) {
                if (data.status === app.resultStatus.success) {
                    app.successMessage("Uaktualniono obecność użytkownika");
                    if (successCallback !== undefined) {
                        successCallback();
                    }
                } else {
                    app.errorMessage(data.message);
                }
            }
        });
    }

    return {
        init: _init,
        presenceType: _presenceType,
        changePresence: _changePresence,
        addUser: _addUser
    };
})();