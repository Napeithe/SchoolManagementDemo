"use strict";

var app = function () {
  function _init() {}

  function _addUtcOffsetToForm(formName) {
    var utcOffset = moment().utcOffset();

    if (moment().isDST()) {
      utcOffset += 60;
    }

    var input = $("<input class='d-none' id='UtcOffsetInMinutes' value='".concat(utcOffset, "' name='utcOffsetInMinutes'/>\""));
    $("#".concat(formName)).append(input);
  }

  var _resultStatus = {
    success: 0,
    warning: 1,
    error: 2
  };
  var _calendarDefaultOptions = {
    locale: 'pl',
    icons: {
      time: "fas fa-clock",
      date: "fa fa-calendar"
    }
  };
  var _tableLanguage = {
    processing: "Przetwarzanie",
    search: "Szukaj:",
    lengthMenu: "Pokaż _MENU_ wpisów",
    info: "Pokazuje _START_ do _END_ z _TOTAL_ dostępnych wpisów",
    infoEmpty: "Pokazuje 0 do 0 z 0 wpisów ",
    infoFiltered: "(przefiltrowano z _MAX_ dostępnych wpisów)",
    infoPostFix: "",
    loadingRecords: "Ładowanie danych",
    zeroRecords: "Nie odnaleziono rekordów",
    emptyTable: "Brak danych",
    paginate: {
      first: "Pierwszy",
      previous: "Poprzedni",
      next: "Następny",
      last: "Ostatni"
    },
    aria: {
      sortAscending: ": aktywuj aby sortować rosnąco",
      sortDescending: ": aktywuj aby sortować malejąco"
    }
  };

  function _module(value) {
    if (value === undefined) return {};else return value;
  }

  function _contain(el) {
    return this.indexOf(el) !== -1;
  }

  function _notContain(el) {
    return this.indexOf(el) === -1;
  }

  function _any() {
    return this.length > 0;
  }

  function _sendForm($form, successCallback) {
    var formAction = $form.attr("action");
    var formData = $form.serializeArray();
    $.post(formAction, formData).done(function (data) {
      if (successCallback !== undefined) {
        successCallback();
      }
    });
  }

  function _rowClickAndGoToByIdEvent(table, goToUrl) {
    table.on("click", "tbody", function (evt) {
      var isHidding = window.getComputedStyle(evt.target, ':before').content;

      if (isHidding !== "none") {
        return;
      }

      var nearestTr = evt.target.closest("tr");

      if (nearestTr.className === "child") {
        var parent = $(nearestTr).parent();
        var children = parent.children();
        var indexChild = children.index(nearestTr);
        nearestTr = children[indexChild - 1];
      }

      var data = table.row(nearestTr).data();
      var id = data.id;
      app.goTo("".concat(goToUrl, "?id=").concat(id));
    });
  }

  function _formToJSON($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};
    $.map(unindexed_array, function (item, index) {
      if (indexed_array[item['name']]) {
        indexed_array[item['name']].push(item['value']);
      } else {
        if ($form.find('input[name="' + item['name'] + '"]').length > 1) {
          indexed_array[item['name']] = [item['value']];
        } else {
          indexed_array[item['name']] = item['value'];
        }
      }
    });
    return indexed_array;
  }

  function _goTo(url) {
    location.href = location.origin + url;
  }

  function _arrayRemove(arr, value) {
    return arr.filter(function (ele) {
      return ele !== value;
    });
  }

  function _getCalendarDefaultOptions() {
    return _calendarDefaultOptions;
  }

  function _gridRowClick(urlGoTo) {
    document.addEventListener('rowclick', function (e) {
      var id = e.detail.data.id;
      app.goTo("".concat(urlGoTo, "?id=").concat(id));
    });
  }

  function _successMessage(message) {
    new Noty({
      theme: 'bootstrap-v4',
      type: 'success',
      timeout: 1000,
      killer: true,
      text: message
    }).show();
  }

  function _errorMessage(message) {
    new Noty({
      theme: 'bootstrap-v4',
      type: 'error',
      timeout: 1000,
      killer: true,
      text: message
    }).show();
  }

  function _selectInit(selector) {
    $(selector).select2({
      language: "pl",
      width: "100%",
      theme: 'bootstrap4'
    });
  }

  function _selectInitRemote(selector, url, searchCallback, getExcludedDelegate) {
    $(selector).select2({
      language: "pl",
      width: "100%",
      theme: 'bootstrap4',
      ajax: {
        url: url,
        type: "POST",
        data: function data(params) {
          var query = {
            term: params.term
          };

          if (getExcludedDelegate !== undefined) {
            query.exclude = getExcludedDelegate();
          }

          return query;
        },
        delay: 250,
        dataType: 'json'
      }
    });
    $(selector).on('select2:selecting', function (e) {
      if (searchCallback !== undefined) {
        var id = e.params.args.data.id;
        searchCallback(id);
        $(selector).select2('close');
        return false;
      }
    });
  }

  return {
    gridRowClick: _gridRowClick,
    module: _module,
    formToJSON: _formToJSON,
    sendForm: _sendForm,
    tableLanguage: _tableLanguage,
    rowClickAndGoToByIdEvent: _rowClickAndGoToByIdEvent,
    goTo: _goTo,
    getCalendarDefaultOptions: _getCalendarDefaultOptions,
    arrayRemove: _arrayRemove,
    selectInit: _selectInit,
    selectInitRemote: _selectInitRemote,
    successMessage: _successMessage,
    addUtcToForm: _addUtcOffsetToForm,
    errorMessage: _errorMessage,
    resultStatus: _resultStatus
  };
}();
"use strict";

app.table = app.module(app.table);

app.table = function () {
  function _createTable(id) {
    var tableContainer = $("#".concat(id));
    tableContainer.addClass("card shadow mb-4");
    var titleElement = $("<div class=\"card-header py-3\">\n                                    <h6 class=\"m-0 font-weight-bold text-primary title\"></h6>\n                                </div>");
    var bodyElement = $("<div class=\"card-body\">\n                                <div class=\"row\">\n                                    <div class=\"col-sm-12\"> \n                                        <div class=\"table-responsive\">\n                                            <table id=\"table-".concat(id, "\" class=\"table table-bordered table-striped\" width=\"100%\"></table>\n                                        </div>\n                                    </div>        \n                                </div>\n                            </div>"));
    tableContainer.append(titleElement);
    tableContainer.append(bodyElement);
    var table = {};
    table.selector = tableContainer;
    table.withColumns = _withColumns;
    table.withData = _withData;
    table.withTitle = _withTitle;
    table.build = _build;
    table.tableOptions = {
      language: app.tableLanguage,
      responsive: {
        details: true
      }
    };
    table.withClickRowAndGotoById = _withClickRowAndGotoById;
    table.rowClick = {
      isSet: false
    };
    return table;
  }

  function _withColumns(columns) {
    this.tableOptions.columns = columns;
    return this;
  }

  function _withClickRowAndGotoById(goToUrl) {
    this.rowClick.isSet = true;
    this.rowClick.url = goToUrl;
    return this;
  }

  function _withData(dataSet) {
    this.tableOptions.data = dataSet;
    return this;
  }

  function _withTitle(title) {
    this.selector.find(".card-header > .title").text(title);
    return this;
  }

  function _build() {
    var table = this.selector.find("table");
    var renderedTable = table.DataTable(this.tableOptions);

    if (this.rowClick.isSet) {
      app.rowClickAndGoToByIdEvent(renderedTable, this.rowClick.url);
    }
  }

  return {
    createTable: _createTable
  };
}();
"use strict";

app.modal = app.module(app.modal);

app.modal = function () {
  function _renderModal(id) {
    var modalContainer = $("<div class=\"modal fade\" id=\"removeModal".concat(id, "\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"removeModal").concat(id, "Label\" aria-hidden=\"true\"></div>"));
    var modalDialog = $('<div class="modal-dialog" role="document"></div>');
    var modalContent = $('<div class="modal-content"></div>');
    var modalHeader = $('<div class="modal-header"></div>');
    var modalBody = $('<div class="modal-body"></div>');
    var projectCardContainer = $('<div class="mb-3"></div>');
    var modalFooter = $('<div class="modal-footer"></div>');
    modalContainer.append(modalDialog);
    modalDialog.append(modalContent);
    modalContent.append(modalHeader).append(modalBody).append(modalFooter);
    modalBody.append(projectCardContainer);
    $('body').append(modalContainer);
  }

  function _create(id) {
    var modal = {};
    $("#removeModal".concat(id)).remove();
    modal.selector = "#removeModal".concat(id);

    _renderModal(id); // methods


    modal.withTitle = _withTitle;
    modal.withContent = _withContent;
    modal.withInput = _withInput;
    modal.withCancel = _withCancel;
    modal.withAccept = _withAccept;
    modal.show = _show;
    return modal;
  }

  function _show() {
    var selector = this.selector;
    $(this.selector).modal('show');
    $(this.selector).on('hidden.bs.modal', function () {
      $(selector).remove();
    });
  }

  function _withTitle(title) {
    var modalTitle = $("<h5 class=\"modal-title\">".concat(title, "</h5>"));
    $(this.selector).find('.modal-header').append(modalTitle);
    return this;
  }

  function _withContent(content) {
    var text = $("<div class=\"mb-3\">".concat(content, "</div>"));
    $(this.selector).find('.modal-body').append(text);
    return this;
  }

  function _withInput(oldValue) {
    var input = $("<input id=\"displayName\" type=text class=\"form-control\" name=\"DisplayName\" value=".concat(oldValue, ">"));
    $(this.selector).find('.modal-body').append(input);
    return this;
  }

  function _withCancel(text) {
    var unwrappedText = text || "Anuluj";
    var cancelBtn = $("<button type=\"button\" class=\"btn btn-secondary\" data-dismiss=\"modal\">".concat(unwrappedText, "</button>"));
    $(this.selector).find('.modal-footer').append(cancelBtn);
    return this;
  }

  function _withAccept(opt) {
    var unwrappedText = opt.text || "Ok";
    var unwrappedStyle = opt.style || "primary";

    var onClick = opt.onClick || function () {
      console.log("Button \"".concat(unwrappedText, "\"  clicked."));
    };

    var acceptButton = $("<button type=\"button\" class=\"btn btn-".concat(unwrappedStyle, "\" data-dismiss=\"modal\">").concat(unwrappedText, "</button>"));
    acceptButton.on("click", function (e) {
      onClick(e);
    });
    $(this.selector).find('.modal-footer').append(acceptButton);
    return this;
  }

  return {
    create: _create
  };
}();
"use strict";

app.removeDialog = app.module(app.removeDialog);

app.removeDialog = function () {
  var _options = {};

  function _init(options) {
    _options = options;
  }

  function _remove(id) {
    $.ajax({
      url: _options.removeUrl,
      data: JSON.stringify({
        id: id
      }),
      type: "POST",
      contentType: "application/json",
      success: function success(data) {
        if (data.status === app.resultStatus.success) {
          window.location.href = _options.backUrl;
        } else {
          app.errorMessage(data.errorMessage);
        }
      }
    });
  }

  function _showModal(id) {
    app.modal.create("RemoveUser").withTitle(_options.title).withContent(_options.content).withCancel().withAccept({
      text: "Tak",
      onClick: function onClick(e) {
        app.removeDialog.remove(id);
      },
      style: "danger"
    }).show();
  }

  return {
    showModal: _showModal,
    remove: _remove,
    init: _init
  };
}();
"use strict";

app.listOfAnchors = app.module(app.listOfAnchors);

app.listOfAnchors = function () {
  function _init(options) {
    app.table.createTable("anchorsList").withColumns([{
      title: "Nazwa",
      data: "name"
    }]).withData(options.dataSet).withTitle("Lista prowadzących").withClickRowAndGotoById(options.anchorDetail).build();
  }

  return {
    init: _init
  };
}();
"use strict";

app.login = app.module(app.login);

app.login = function () {
  function _init() {
    app.addUtcToForm("loginForm");
  }

  return {
    init: _init
  };
}();
"use strict";

app.calendar = app.module(app.calendar);

app.calendar.show = function () {
  function _init(option) {
    var header = getHeader();
    var view = getView();
    var calendarEl = document.getElementById("calendar");
    var calendar = new FullCalendar.Calendar(calendarEl, {
      defaultView: view,
      minTime: "06:00:00",
      height: 'auto',
      eventSources: [{
        url: option.eventUrl
      }],
      allDaySlot: false,
      plugins: ['interaction', 'dayGrid', 'timeGrid'],
      locale: "pl",
      header: header,
      windowResize: function windowResize() {
        var view = getView(); //Update header during window resize

        this.setOption('header', getHeader(), view);
        this.render();
      },
      eventClick: function eventClick(info) {
        if (option.userCanSeePresence) {
          window.location.href = "".concat(option.presenceAtDayUrl, "?classTimeId=").concat(info.event._def.publicId);
        }
      },
      eventDrop: function eventDrop(info) {
        var newDate = {
          id: info.event.id,
          start: moment(info.event.start).toISOString(),
          end: moment(info.event.end).toISOString()
        };
        $.ajax({
          url: option.updateClassTimeUrl,
          contentType: "application/json",
          dataType: "json",
          type: "POST",
          data: JSON.stringify(newDate),
          success: function success() {
            app.successMessage("Przeniesiono zajęcia");
          }
        });
      }
    });
    calendar.render();
  }

  function getHeader() {
    if (window.innerWidth < 768) {
      return {
        left: 'prev ',
        center: 'title',
        right: 'next'
      };
    } else {
      return {
        left: 'prev,next today myCustomButton',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      };
    }
  }

  function getView() {
    if (window.innerWidth < 768) {
      return "timeGridDay";
    } else {
      return "timeGridWeek";
    }
  }

  return {
    init: _init
  };
}();
"use strict";

app.groupClass = app.module(app.groupClass);

app.groupClass.detail = function () {
  var _options = {};

  function _setAsPaid(memberId) {
    app.modal.create().withTitle("Zmiana statusu karnetu").withContent("Czy chcesz oznaczyć karnet jako opłacony?").withAccept({
      text: "Tak",
      onClick: function onClick() {
        $.ajax({
          type: "POST",
          contentType: "application/json",
          dataType: "json",
          url: _options.changePassStatus,
          data: JSON.stringify({
            memberId: memberId
          }),
          success: function success() {
            app.successMessage("Zmienieno status karnetu");
            $('#participantsTable').DataTable().ajax.reload();
          }
        });
      }
    }).withCancel("Nie").show();
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
      columns: [{
        title: "Imię i nazwisko",
        data: "userName"
      }, {
        title: "Rola",
        data: "roleDescription",
        width: "15%"
      }, {
        title: "Karnet",
        "render": function render(data, type, row) {
          if (row.passWasPaid) {
            return 'Opłacono';
          } else {
            return 'Nie opłacono';
          }
        },
        width: "15%"
      }, {
        title: "",
        render: function render(data, type, row) {
          var buttons = "<div class='d-flex justify-content-center'><a role='button' title='Edytuj uczestnika' class='btn btn-primary btn-xs mr-2' href='".concat(options.memberDetail, "?memberId=").concat(row.memberId, "'><i class='far fa-edit'></i></a>");

          if (!row.passWasPaid && options.userHasPermissionToSetPassAsPaid) {
            buttons += "<button type='button' title='Oznacz jako zap\u0142acone' class='btn btn-success btn-xs mr-2' onclick='app.groupClass.detail.setAsPaid(\"".concat(row.memberId, "\")'><i class=\"fas fa-money-bill-wave-alt\"></i></button>");
          }

          buttons += "</div>";
          return buttons;
        },
        width: "10%"
      }],
      rowCallback: function rowCallback(row, data) {
        if (!data.passWasPaid) {
          $(row).addClass("notPayment");
          $(row).removeClass("odd");
        }
      }
    });
  }

  function _drawTable(data) {
    var $thead = $("#participantPresenceTable > thead >tr");
    var th = $("<th>Uczestnik</th>");
    $thead.append(th);
    data.columns.forEach(function (el) {
      var th = $("<th>".concat(el, "</th>"));
      $thead.append(th);
    });
    var participantsPresence = data.participantsPresence;
    var $tbody = $("#participantPresenceTable > tbody");

    for (var participantIndex in participantsPresence) {
      if (participantsPresence.hasOwnProperty(participantIndex)) {
        var participant = participantsPresence[participantIndex];
        var $participantRow = $("<tr><td>".concat(participant.participantName, "</td></tr>"));
        var presenceValues = participant.presenceValues;

        for (var index in data.columns) {
          if (data.columns.hasOwnProperty(index)) {
            var checked;

            (function () {
              var colKey = data.columns[index];
              var value = presenceValues.filter(function (el) {
                return el.date === colKey;
              });

              if (value.length === 1) {
                checked = "";

                if (value[0].value) {
                  checked = "checked";
                }

                var cell = $("<td><input type='checkbox' ".concat(checked, " data-toggle=\"toggle\"  data-onstyle=\"success\" data-offstyle=\"danger\" data-participantId='").concat(participant.participantId, "' data-classTimeId='").concat(value[0].id, "'/></td>"));
                $participantRow.append(cell);
              } else {
                var _cell = $("<td>X</td>");

                $participantRow.append(_cell);
              }
            })();
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
      var status = $(this).prop('checked');
      var participantId = $(this).data("participantid");
      var classTimeId = $(this).data("classtimeid");
      app.presence.show.changePresence(participantId, classTimeId, status, app.presence.show.presenceType.Member, _options.changePresenceUrl);
    });
  }

  function _initParticipantPresenceTable(options) {
    $.ajax({
      url: options.presenceUrl,
      type: "GET",
      contentType: "application/json",
      success: function success(data) {
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
}();
"use strict";

app.groupClass = app.module(app.groupClass);

app.groupClass.list = function () {
  function _init(options) {
    app.table.createTable("groupClassList").withColumns([{
      title: "Nazwa",
      data: "name"
    }, {
      title: "Limit miejsc",
      data: "limitParticipants"
    }, {
      title: "Obecna liczba uczestników",
      data: "currentParticipants"
    }]).withData(options.dataSet).withTitle("Zajęcia grupowe").withClickRowAndGotoById(options.editRoom).build();
  }

  return {
    init: _init
  };
}();
"use strict";

app.groupClass = app.module(app.groupClass);

app.groupClass.participantImport = function () {
  var _options = {};

  function _init(options) {
    _options = options;
    $('#btnUpload').on('click', _upload);
  }

  function _upload() {
    var fileExtension = ['xls', 'xlsx'];
    var filename = $('#fUpload').val();

    if (filename.length === 0) {
      alert("Please select a file.");
      return false;
    } else {
      var extension = filename.replace(/^.*\./, '');

      if ($.inArray(extension, fileExtension) == -1) {
        alert("Please select only excel files.");
        return false;
      }
    }

    var fdata = new FormData();
    var fileUpload = $("#fUpload").get(0);
    var files = fileUpload.files;
    fdata.append(files[0].name, files[0]);
    var groupId = $("input[name='GroupId']").val();
    fdata.append("id", groupId);
    $.ajax({
      type: "POST",
      url: _options.url,
      beforeSend: function beforeSend(xhr) {
        xhr.setRequestHeader("XSRF-TOKEN", $('input:hidden[name="__RequestVerificationToken"]').val());
      },
      data: fdata,
      contentType: false,
      processData: false,
      success: function success(response) {
        if (response.length === 0) alert('Some error occured while uploading');else {
          app.successMessage("Dodano użytkowników do grupy");
        }
      },
      error: function error(e) {
        $('#dvData').html(e.responseText);
      }
    });
  }

  return {
    init: _init
  };
}();
"use strict";

app.groupMember = app.module(app.groupMember);

app.groupMember.edit = function () {
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
}();
"use strict";

app.listOfParticipants = app.module(app.listOfParticipants);

app.listOfParticipants = function () {
  function _init(options) {
    app.table.createTable("participantList").withColumns([{
      title: "Nazwa",
      data: "name"
    }]).withData(options.dataSet).withTitle("Lista uczestników").withClickRowAndGotoById(options.participantDetail).build();
  }

  return {
    init: _init
  };
}();
"use strict";

app.presence = app.module(app.presence);

app.presence.show = function () {
  var _option = {};
  var _presenceType = {
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
      var status = $(this).prop('checked');
      var participantId = $(this).data("participantid");
      app.presence.show.changePresence(participantId, _option.classTimeId, status, _presenceType.Member, _option.changePresenceUrl);
    });
    $('button[class*=participant-remove]').click(function () {
      var participantId = $(this).data("participantid");
      var presenceType = $(this).data("presencetype");
      app.presence.show.changePresence(participantId, _option.classTimeId, false, presenceType, _option.changePresenceUrl, _addUserRow);
    });
  }

  function _addNewUser(userId, presenceType) {
    app.presence.show.changePresence(userId, _option.classTimeId, true, presenceType, _option.changePresenceUrl, _addUserRow);
  }

  function _addUser(userId) {
    $.ajax({
      type: "GET",
      data: {
        userId: userId
      },
      url: _option.getNewParticipantDetailUrl,
      contentType: "application/json",
      success: function success(result) {
        if (result.status === app.resultStatus.success) {
          var modal = app.modal.create().withTitle("Dodanie uczestnika z poza listy").withContent("Czy chesz doda\u0107 uczestnika <strong>".concat(result.data.name, "</strong> jako dodatkowego uczestnika zaj\u0119\u0107?<br/><br/> Oznacz zaj\u0119cia jako:")).withCancel("Nie").withAccept({
            text: "Pomoc",
            style: "success",
            onClick: function onClick() {
              _addNewUser(result.data.id, _presenceType.Help);
            }
          });

          if (result.data.wasAbsent) {
            modal = modal.withAccept({
              text: "Odrabianie",
              onClick: function onClick() {
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
    var data = {
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
      success: function success(data) {
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
}();
"use strict";

app.anchors = app.module(app.anchors);
app.anchors.assign = app.module(app.anchors.assign);

app.anchors.assign = function () {
  function _init() {
    $("#userList").select2({
      language: "pl",
      width: "100%"
    });
  }

  return {
    init: _init
  };
}();
"use strict";

app.groupClass = app.module(app.groupClass);

app.groupClass.add = function () {
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
      data: {
        id: id,
        role: _getRole()
      },
      url: _options.getUserUrl,
      success: function success(data) {
        _addToTable(data);

        _addToForm(data);
      }
    });
  }

  function _addToForm(user) {
    var numberOfData = $("#selectedUsers input").length / 3;
    var idInput = $("<input name=\"participants[".concat(numberOfData, "].id\" value=\"").concat(user.id, "\" data-id=\"").concat(user.id, "\" />"));
    var roleInput = $("<input name=\"participants[".concat(numberOfData, "].role\" value=\"").concat(user.role, "\" data-id=\"").concat(user.id, "\" />"));
    var nameInput = $("<input name=\"participants[".concat(numberOfData, "].name\" value=\"").concat(user.userName, "\" data-id=\"").concat(user.id, "\" />"));
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
    $("#selectedUsers input[data-id=".concat(id, "]")).remove();
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
      participant.roleDescription = $("input[data-id='".concat(el, "'][name*='role']")).data("rolename");
      participant.userName = $("input[data-id='".concat(el, "'][name*='name']")).val();
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
      columns: [{
        title: "Imię i nazwisko",
        data: "userName"
      }, {
        title: "Rola",
        data: "roleDescription",
        width: "15%"
      }, {
        data: null,
        targets: -1,
        defaultContent: "<button class=\"btn btn-danger btn-sm\" type=\"button\" onclick=\"app.groupClass.add.remove(this)\"><i class=\"fas fa-user-times\"></i></button>",
        width: "5%"
      }]
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
    $("select[name*=DayOfWeek]").map(function () {
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

    selectedParticipants.forEach(function (el) {
      _selectedUser.push(el.id);
    });
    $("#IsSolo").on("change", function () {
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
    var mainFormRow = $("<div class='form-row' id='term-".concat(nextTermNumber, "'></div>"));
    var dayOfWeekSelect = $("<div class='form-group col-md-6'>\n                                    <label for='Start'>Termin ".concat(nextTermNumber + 1, "</label>\n                                    <select name='DayOfWeeks[").concat(nextTermNumber, "].DayOfWeek' class='form-control'></select>\n                                </div>"));
    var timeOfClasses = $("<div class='form-group col-md-5'>\n                                    <label for='Start'>Godzina rozpocz\u0119cia</label>\n                                    <div class='input-group date' id='startTime-".concat(nextTermNumber, "' data-target-input='nearest'>\n                                        <input type='text' class='form-control' data-target='#startTime-").concat(nextTermNumber, "' name='DayOfWeeks[").concat(nextTermNumber, "].BeginTime' id='BeginTime=").concat(nextTermNumber, "' />\n                                        <div class='input-group-append' data-target='#startTime-").concat(nextTermNumber, "' data-toggle='datetimepicker'>\n                                            <div class='input-group-text'><i class='fa fa-clock'></i></div>\n                                        </div>\n                                    </div>\n                               </div>"));
    var removeButton = $("<div class='form-group col-md-1 d-flex align-items-end'>\n                                  <button type='button' onclick='app.groupClass.add.removeTerm(\"#term-".concat(nextTermNumber, "\")' class='btn btn-danger'><span><i class='fa fa-recycle'></i></span></button>\n                              </div>"));

    _addDayOfWeekOption(dayOfWeekSelect.find("select"));

    _initTimeCalendar(timeOfClasses.find("#startTime-".concat(nextTermNumber)));

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
}();
"use strict";

app.room = app.module(app.room);

app.room.add = function () {
  function _init() {
    $("#roomColor").colorpicker({
      useHashPrefix: false
    });
  }

  return {
    init: _init
  };
}();
"use strict";

app.rooms = app.module(app.rooms);

app.rooms = function () {
  function _init(options) {
    app.table.createTable("roomList").withColumns([{
      title: "Nazwa pokoju",
      data: "name"
    }]).withData(options.dataSet).withTitle("Lista pokoi").withClickRowAndGotoById(options.editRoom).build();
  }

  return {
    init: _init
  };
}();
"use strict";

app.userList = app.module(app.userList);

app.userList = function () {
  function _init(options) {
    app.table.createTable("userList").withColumns([{
      title: "Imię",
      data: "firstName"
    }, {
      title: "Nazwisko",
      data: "lastName"
    }, {
      title: "E-mail",
      data: "email"
    }, {
      title: "Rola",
      data: "roleName"
    }]).withData(options.dataSet).withTitle("Lista użytkowników").withClickRowAndGotoById(options.editUserUrl).build();
  }

  return {
    init: _init
  };
}();