app.groupClass = app.module(app.groupClass);
app.groupClass.participantImport = (function () {
    let _options = {};

    function _init(options) {
        _options = options;
        $('#btnUpload').on('click', _upload);
    }

    function _upload() {
        let fileExtension = ['xls', 'xlsx'];
        let filename = $('#fUpload').val();
        if (filename.length === 0) {
            alert("Please select a file.");
            return false;
        } else {
            let extension = filename.replace(/^.*\./, '');
            if ($.inArray(extension, fileExtension) == -1) {
                alert("Please select only excel files.");
                return false;
            }
        }
        var fdata = new FormData(); 
        var fileUpload = $("#fUpload").get(0);
        var files = fileUpload.files;
        fdata.append(files[0].name, files[0]);
        let groupId = $("input[name='GroupId']").val();
        fdata.append("id", groupId);
        $.ajax({
            type: "POST",
            url: _options.url,
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            data: fdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.length === 0)
                    alert('Some error occured while uploading');
                else {
                    app.successMessage("Dodano użytkowników do grupy");
                }
            },
            error: function (e) {
                $('#dvData').html(e.responseText);
            }
        });
    }

    return {
        init: _init
    }
}());