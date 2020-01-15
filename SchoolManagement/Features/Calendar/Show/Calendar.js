app.calendar = app.module(app.calendar);
app.calendar.show = (function () {

    function _init(option) {
        var header = getHeader();
        var view = getView();
        var calendarEl = document.getElementById("calendar");
        var calendar = new FullCalendar.Calendar(calendarEl,
            {
                defaultView: view,
                minTime: "06:00:00",
                height: 'auto',
                eventSources: [
                    {
                        url: option.eventUrl
                    }],
                allDaySlot: false,
                plugins: ['interaction', 'dayGrid', 'timeGrid'],
                locale: "pl",
                header: header,
                windowResize: function () {
                    let view = getView();
                    //Update header during window resize
                    this.setOption('header', getHeader(), view);
                    this.render();
                },
                eventClick: function (info) {
                    if (option.userCanSeePresence) {
                        window.location.href = `${option.presenceAtDayUrl}?classTimeId=${info.event._def.publicId}`;
                    }
                },
                eventDrop: function (info) {
                    let newDate = {
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
                        success: function () {
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

        }
        else {
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
        }
        else {
            return "timeGridWeek";
        }
    }

    return {
        init: _init
    }
}());