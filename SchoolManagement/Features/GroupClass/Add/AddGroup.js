app.groupClass = app.module(app.groupClass);
app.groupClass.add = (function () {
    var _options = {};
    var _participantTable;
    var _selectedUser = [];

    function _getRole() {
        var role = $("#participantRole input:checked").val();
        return role;
    }

    function _loadUser(id) {
        $.ajax({
            contentType: "application/json; charset=utf-8",
            type: "GET",
            data: { id: id, role: _getRole() },
            url: _options.getUserUrl,
            success: function (data) {
                _addToTable(data);
                _addToForm(data);
            }
        });
    }

    function _addToForm(user) {
        var numberOfData = $("#selectedUsers input").length / 3;
        var idInput = $(`<input name="participants[${numberOfData}].id" value="${user.id}" data-id="${user.id}" />`);
        var roleInput = $(`<input name="participants[${numberOfData}].role" value="${user.role}" data-id="${user.id}" />`);
        var nameInput = $(`<input name="participants[${numberOfData}].name" value="${user.userName}" data-id="${user.id}" />`);
        $("#selectedUsers").append(idInput, roleInput, nameInput);

    }
    function _addToTable(user) {
        _participantTable.row.add(user).draw();
        _selectedUser.push(user.id);
    }

    function _getSelectedUser() {
        return _selectedUser;
    }

    function _addUserToTable(id) {
        _loadUser(id);
    }

    function _removeParticipantFromForm(id) {
        _selectedUser = app.arrayRemove(_selectedUser, id);
        $(`#selectedUsers input[data-id=${id}]`).remove();
    }

    function _removeUserFromTable(el) {
        var row = _participantTable.row(el.closest("tr"));
        var id = row.data().id;
        _removeParticipantFromForm(id);
        row.remove().draw();
    }

    function _loadUserFromForm() {
        var ids = $("#selectedUsers input[name*='id'").map(function () {
            return $(this).val();
        }).get();
        var participants = [];
        ids.forEach(function (el) {
            var participant = {};
            participant.id = el;
            participant.roleDescription = $(`input[data-id='${el}'][name*='role']`).data("rolename");
            participant.userName = $(`input[data-id='${el}'][name*='name']`).val();
            participants.push(participant);
        });

        return participants;
    }

    function _initTable(data) {
        var options = {
            language: app.tableLanguage,
            searching: false,
            info: false,
            paging: false,
            responsive: {
                details: true
            },
            columns: [
                { title: "Imię i nazwisko", data: "userName" },
                { title: "Rola", data: "roleDescription", width: "15%" },
                {
                    data: null,
                    targets: -1,
                    defaultContent:
                        `<button class="btn btn-danger btn-sm" type="button" onclick="app.groupClass.add.remove(this)"><i class="fas fa-user-times"></i></button>`,
                    width: "5%"
                }
            ]
        };
        if (data !== undefined) {
            options.data = data;
        }
        _participantTable = $("#participantsTable").DataTable(options);
    }


    function _setVisibilityRolesRadio() {
        if ($("#IsSolo:checked").length) {
            document.getElementById("noneRadio").checked = true;
            $("#role-set").hide();
        } else {
            document.getElementById("leaderRadio").checked = true;
            $("#role-set").show();
        }
    }

    function _initStartDate() {
        var calendarOptions = app.getCalendarDefaultOptions();
        calendarOptions.format = "L";
        $("#startDate").datetimepicker(calendarOptions);
        $("#startDate").on("change.datetimepicker", function (e) {
            $('#Start').val(moment(e.date).toISOString(true).substring(0, 16));
        });
    }

    function _initTimeCalendar($selector) {
        var calendarOptions = app.getCalendarDefaultOptions();
        calendarOptions.format = "LT";

        $selector.datetimepicker(calendarOptions);
    }

    function _initSelectedDaysTime() {
        $("input[name*=BeginTime]").map(function () {
            _initTimeCalendar($(this).parent());
        });
    }

    function _initSelectDayOfWeek() {
        $("select[name*=DayOfWeek]").map(function() {
            app.selectInit(this);
        });
    }
    function _init(options) {
        _options = options;

        app.addUtcToForm("addGroupForm");

        _initStartDate();
        _initSelectDayOfWeek();
        _initSelectedDaysTime();

        var selectedParticipants = _loadUserFromForm();
        _initTable(selectedParticipants);

        selectedParticipants.forEach(function(el) {
            _selectedUser.push(el.id);
        });

        $("#IsSolo").on("change",
            function () {
                _setVisibilityRolesRadio();
            });

        _setVisibilityRolesRadio();
    }

    function _addDayOfWeekOption($selector) {

        $selector.select2({
            data: _options.daysOfWeek,
            theme: "bootstrap4",
            language: "pl",
            width: "100%"
        });
    }

    function _addTerm() {
        var terms = $("#terms");
        var nextTermNumber = terms.children().length;
        var mainFormRow = $(`<div class='form-row' id='term-${nextTermNumber}'></div>`);
        var dayOfWeekSelect = $(`<div class='form-group col-md-6'>
                                    <label for='Start'>Termin ${nextTermNumber + 1}</label>
                                    <select name='DayOfWeeks[${nextTermNumber}].DayOfWeek' class='form-control'></select>
                                </div>`);
        var timeOfClasses = $(`<div class='form-group col-md-5'>
                                    <label for='Start'>Godzina rozpoczęcia</label>
                                    <div class='input-group date' id='startTime-${nextTermNumber}' data-target-input='nearest'>
                                        <input type='text' class='form-control' data-target='#startTime-${nextTermNumber}' name='DayOfWeeks[${nextTermNumber}].BeginTime' id='BeginTime=${nextTermNumber}' />
                                        <div class='input-group-append' data-target='#startTime-${nextTermNumber}' data-toggle='datetimepicker'>
                                            <div class='input-group-text'><i class='fa fa-clock'></i></div>
                                        </div>
                                    </div>
                               </div>`);

        var removeButton = $(`<div class='form-group col-md-1 d-flex align-items-end'>
                                  <button type='button' onclick='app.groupClass.add.removeTerm("#term-${nextTermNumber}")' class='btn btn-danger'><span><i class='fa fa-recycle'></i></span></button>
                              </div>`);
        _addDayOfWeekOption(dayOfWeekSelect.find("select"));
        _initTimeCalendar(timeOfClasses.find(`#startTime-${nextTermNumber}`));
        mainFormRow.append(dayOfWeekSelect);
        mainFormRow.append(timeOfClasses);
        mainFormRow.append(removeButton);

        terms.append(mainFormRow);
    }

    function _removeTerm(termId) {
        $(termId).remove();
    }


    return {
        init: _init,
        addUserToTable: _addUserToTable,
        remove: _removeUserFromTable,
        getSelectedUser: _getSelectedUser,
        addTerm: _addTerm,
        removeTerm: _removeTerm

    };
})();