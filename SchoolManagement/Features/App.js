var app = (function () {
    function _init() {

    }

    function _addUtcOffsetToForm(formName) {
        let utcOffset = moment().utcOffset();
        if (moment().isDST()) {
            utcOffset += 60;
        }
        let input = $(`<input class='d-none' id='UtcOffsetInMinutes' value='${utcOffset}' name='utcOffsetInMinutes'/>"`);
        $(`#${formName}`).append(input);
    }

    let _resultStatus = {
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
        if (value === undefined) return {};
        else return value;
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
        table.on("click", "tbody",
            function (evt) {
                let isHidding = window.getComputedStyle(evt.target, ':before').content;
                if (isHidding !== "none") {
                    return;
                }
                var nearestTr = evt.target.closest("tr");
                if (nearestTr.className === "child") {
                    let parent = $(nearestTr).parent();
                    let children = parent.children();
                    let indexChild = children.index(nearestTr);
                    nearestTr = children[indexChild - 1];
                }
                var data = table.row(nearestTr).data();
                var id = data.id;
                app.goTo(`${goToUrl}?id=${id}`);

            });
    }

    function _formToJSON($form) {
        var unindexed_array = $form.serializeArray();
        var indexed_array = {};
        $.map(unindexed_array, function (item, index) {
            if (indexed_array[item['name']]) {
                indexed_array[item['name']].push(item['value']);
            }
            else {
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

            app.goTo(`${urlGoTo}?id=${id}`);
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
                data: function (params) {
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
}());