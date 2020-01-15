app.groupClass = app.module(app.groupClass);
app.groupClass.detail = (function () {
    let _options = {};

    function _setAsPaid(memberId) {

        app.modal.create()
            .withTitle("Zmiana statusu karnetu")
            .withContent("Czy chcesz oznaczyć karnet jako opłacony?")
            .withAccept({
                text: "Tak",
                onClick: function() {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json",
                        dataType: "json",
                        url: _options.changePassStatus,
                        data: JSON.stringify({ memberId }),
                        success: function() {
                            app.successMessage("Zmienieno status karnetu");
                            $('#participantsTable').DataTable().ajax.reload();
                        }
                    });
                }
            })
            .withCancel("Nie").show();
    }

    function _initMembersTable(options) {
        $("#participantsTable").DataTable({
            language: app.tableLanguage,
            ajax: {
                url: options.membersUrl,
                dataSrc: ''
            },
            responsive: {
                details: true
            },
            columns: [
                { title: "Imię i nazwisko", data: "userName" },
                { title: "Rola", data: "roleDescription", width: "15%" },
                {
                    title: "Karnet", "render": function(data, type, row) {
                        if (row.passWasPaid) {
                            return 'Opłacono';
                        } else {
                            return 'Nie opłacono';
                        }
                    }, width: "15%"
                },
                {
                    title: "", render: function(data, type, row) {
                        let buttons = `<div class='d-flex justify-content-center'><a role='button' title='Edytuj uczestnika' class='btn btn-primary btn-xs mr-2' href='${options.memberDetail}?memberId=${row.memberId}'><i class='far fa-edit'></i></a>`;

                        if (!row.passWasPaid && options.userHasPermissionToSetPassAsPaid) {
                            buttons +=
                                `<button type='button' title='Oznacz jako zapłacone' class='btn btn-success btn-xs mr-2' onclick='app.groupClass.detail.setAsPaid("${
                                row.memberId}")'><i class="fas fa-money-bill-wave-alt"></i></button>`;
                        }
                        buttons += "</div>";
                        return buttons;
                    }, width: "10%"
                }
            ],
            rowCallback: function(row, data) {
                if (!data.passWasPaid) {
                    $(row).addClass("notPayment");
                    $(row).removeClass("odd");
                }
            }
        });
    }

    function _drawTable(data) {
        var $thead = $("#participantPresenceTable > thead >tr");
        const th = $(`<th>Uczestnik</th>`);
        $thead.append(th);

        data.columns.forEach(function (el) {
            var th = $(`<th>${el}</th>`);
            $thead.append(th);
        });
        let participantsPresence = data.participantsPresence;
        var $tbody = $("#participantPresenceTable > tbody");
        for (let participantIndex in participantsPresence) {
            if (participantsPresence.hasOwnProperty(participantIndex)) {
                let participant = participantsPresence[participantIndex];
                let $participantRow = $(`<tr><td>${participant.participantName}</td></tr>`);

                let presenceValues = participant.presenceValues;
                for (let index in data.columns) {
                    if (data.columns.hasOwnProperty(index)) {
                        let colKey = data.columns[index];
                        let value = presenceValues.filter(function (el) {
                            return el.date === colKey;
                        });
                        if (value.length === 1) {
                            var checked = "";
                            if (value[0].value) {
                                checked = "checked";
                            }
                            let cell = $(
                                `<td><input type='checkbox' ${checked} data-toggle="toggle"  data-onstyle="success" data-offstyle="danger" data-participantId='${participant.participantId}' data-classTimeId='${value[0].id}'/></td>`);
                            $participantRow.append(cell);
                        } else {
                            let cell = $(`<td>X</td>`);
                            $participantRow.append(cell);
                        }
                    }
                }
                $tbody.append($participantRow);
            }
        }
        $("input[data-toggle*=toggle]").bootstrapToggle({
            size: 'xs',
            on: '',
            off: ''
        });
       
        $("#participantPresenceTable").DataTable({
            language: app.tableLanguage,
            scrollCollapse: true,
            scrollY: "1000px",
            scrollX: true,
            paging: false,
            fixedColumns: true
        });

        $('input[data-toggle*=toggle]').change(function () {
            let status = $(this).prop('checked');
            let participantId = $(this).data("participantid");
            let classTimeId = $(this).data("classtimeid");
            app.presence.show.changePresence(participantId, classTimeId, status,app.presence.show.presenceType.Member, _options.changePresenceUrl);
        });
    }

    function _initParticipantPresenceTable(options) {
        $.ajax({
            url: options.presenceUrl,
            type: "GET",
            contentType: "application/json",
            success: function (data) {
                _drawTable(data);
            }
        });
    }

    function _init(options) {
        _initMembersTable(options); 
        _initParticipantPresenceTable(options);
        _options = options;
    }


    return {
        init: _init,
        setAsPaid: _setAsPaid
    };
})();