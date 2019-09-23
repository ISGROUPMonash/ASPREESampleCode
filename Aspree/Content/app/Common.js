var App = {};
App.alertHideTimeout = 20000;
App.masterCallback = function (result) {

};

App.postData = function (url, data, callback, methodType, doNotClearError, isAsync) {
    if (typeof isAsync === 'undefined')
        isAsync = true;

    methodType = methodType || "POST";
    App.clearValidationErrors();
    $.support.cors = true;
    if (!doNotClearError) {
        App.clearError();
    }
    callback = callback || App.masterCallback
    App.showLoader();
    $.ajax({
        url: url,
        headers: { "Authorization": "Bearer " + App.AccessToken },
        contentType: 'application/json',
        data: JSON.stringify(data),
        type: methodType,
        dataType: 'json',
        crossDomain: true,
        success: function (result, textStatus, xhr) {

            App.hideLoader()
            callback(result);
        },
        error: function (xhr, status, error) {

            App.hideLoader();
            console.log(xhr, status, error);
            if (xhr.status == 400) {
                if (xhr.responseText.indexOf('"key"') > -1)
                    App.showValidationErrors('body', xhr.responseJSON);
                else
                    App.showError(xhr.responseJSON.message, selector);
            } else if (xhr.status == 300) {
                var selector = $('.role-model-error').is(":visible") ? '.role-model-error' : '.alerts';
                if (xhr.responseText.indexOf('"key"') > -1)
                    App.showValidationErrors('body', xhr.responseJSON);
                else
                    App.showError(xhr.responseJSON.message, selector);
            }
            else {
                var selector = $('.role-model-error').is(":visible") ? '.role-model-error' : '.alerts';
                try {
                    App.showError(xhr.responseJSON.message, selector);
                } catch (ex) {
                    App.showError(xhr.responseText, selector);
                }
            }
        },
        async: isAsync
    });
};

App.showSuccess = function (message, selector, timeout) {
    timeout = timeout || App.alertHideTimeout;
    selector = selector || '.alerts';
    $(selector).empty();
    var message = $('<div class="alert alert-success alert-dismissible m-t-10 m-l-10 m-r-10 m-b-10"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Success!</strong> ' + message + '</div>');
    $(selector).append(message)
    setTimeout(function () { message.remove(); }, timeout)

    $('html, body').animate({
        scrollTop: $(selector).offset().top
    }, 500);
}

App.showError = function (message, selector, timeout) {
    timeout = timeout || App.alertHideTimeout;
    selector = selector || '.alerts';
    $(selector).empty();
    var message = $('<div class="alert alert-danger alert-dismissible m-t-10 m-l-10 m-r-10 m-b-10"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Error!</strong> ' + message + '</div>');
    $(selector).append(message)
    setTimeout(function () { message.remove(); }, timeout)

    $('html, body').animate({
        scrollTop: $(selector).offset().top
    }, 500);
}
App.clearValidationErrors = function () {
    $('div.input-validation-error').removeClass("input-validation-error");
    $('.form-control-feedback').removeClass('field-validation-error').html("");
}
App.showValidationErrors = function (selector, errors) {
    selector = selector;
    App.clearValidationErrors();
    var parent = $(selector);
    for (var i = 0; i < errors.length; i++) {
        let error = errors[i];
        let control = $('#' + error.key, parent);
        control.parents('.form-group:first').find('.form-control-feedback:first').addClass('field-validation-error').html(error.message);
        control.addClass('input-validation-error');
        control.off("click").on("click", function () {
            control.removeClass('input-validation-error');
            control.parents('.form-group:first').find('.form-control-feedback').removeClass('field-validation-error').html("");
        });
    }

}
App.showLoader = function () {
    $('#processLoader').show();
    $('#blocker').show();
}
App.hideLoader = function () {
    $('#processLoader').hide();
    $('#blocker').hide();
}
App.refresh = function (timeout) {
    timeout = timeout || 2000;
    setTimeout(function () { window.location.reload(); }, timeout)
}
App.clearError = function () {
    $("div.alert").remove();
}
App.setupTable = function (id, url, columns, serverSide) {
    serverSide = serverSide || false;
    var dataTable = $('#' + id).DataTable({
        dom: 'Bfrtip',
        "order": [],
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
        }],
        serverSide: false,
        columns: columns,
        select: true,
    });
    $.support.cors = true;
    dataTable.refresh = function () {
        App.showLoader();
        $.ajax({
            url: App.ApiUrl + url,
            type: 'GET',
            headers: { "Authorization": "Bearer " + App.AccessToken },
            dataType: 'json',
            contentType: 'application/json',
            crossDomain: true,
            success: function (data, textStatus, xhr) {
                dataTable.clear().draw();
                for (var i = 0; i < data.length; i++) {
                    dataTable.row.add(data[i])
                }
                dataTable.draw();
                App.hideLoader();
            },
            error: function (xhr, textStatus, errorThrown) {
                App.hideLoader();
            }
        });
    }
    dataTable.getById = function (guid) {
        return dataTable.rows().data().filter(function (row) { return row.guid == guid })[0];
    }
    dataTable.refresh();
    return dataTable;
}

App.setupSimpleTable = function (id, url, columns, serverSide) {
    serverSide = serverSide || false;
    var dataTable = $('#' + id).DataTable({
        "order": [],
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false

        }],
        "fnDrawCallback": function (oSettings) {
            $('.dataTables_paginate').hide();
            if (oSettings.aoData.length > oSettings._iDisplayLength) {
                $('.dataTables_paginate').show();
            }
        },
        serverSide: false,
        columns: columns,
    });
    dataTable.refresh = function () {
        App.showLoader();
        $.ajax({
            url: App.ApiUrl + url,
            type: 'GET',
            headers: { "Authorization": "Bearer " + App.AccessToken },
            dataType: 'json',
            contentType: 'application/json',
            crossDomain: true,
            success: function (data, textStatus, xhr) {
                dataTable.clear().draw();
                for (var i = 0; i < data.length; i++) {
                    dataTable.row.add(data[i])
                }
                dataTable.draw();
                App.hideLoader();
            },
            error: function (xhr, textStatus, errorThrown) {
                App.hideLoader();
            }
        });
    }
    dataTable.getById = function (guid) {
        return dataTable.rows().data().filter(function (row) { return row.guid == guid })[0];
    }
    dataTable.refresh();
    return dataTable;
}
App.applyRemoteValidation = function () {
    $('input[data-remote-prop]').off("blur").on("blur", function () {
        let ctrl = $(this);
        ctrl.removeClass('input-validation-error');
        ctrl.next('.form-control-feedback').removeClass('field-validation-error').html("");
        if (ctrl.val().length == 0) return;

        let formValidator = ctrl.parents('form:first').validate();
        if (!formValidator.element('#' + ctrl.attr('id'))) return;

        let sendData = {
            "Property": ctrl.attr('data-remote-prop'),
            "Value": ctrl.val(),
            "Guid": ctrl.attr('data-remote-guid'),
            "Message": ctrl.attr('data-remote-message')
        };

        App.postData(App.ApiUrl + "validator", sendData, function (result) {
            if (result) {
                ctrl.addClass('input-validation-error');
                ctrl.next('.form-control-feedback').addClass('field-validation-error').html(sendData.Message);
            }
        });
    });
}
App.sessionExpire = function () {
    if (location.pathname == '/' || location.pathname.toLowerCase().indexOf('account/login') > -1) return;
    setTimeout(function () { location.href = '/account/logout?expired=true' }, 18 * 60 * 1000)
}
App.sessionExpire();

App.redirectSuccess = function (message, url) {
    var sendData = {
        "message": message
    };
    App.postData("/home/RedirectSuccess", sendData, function (result) {
        if (result) {
            window.location.href = url;
        }
    });
}
App.goto = function (path) {
    window.location.href = path;
}
App.resetForm = function (selector) {
    $(selector)[0].reset();
};


App.checkValidation = function (inputField, ValidationType, RegExPattern, minLength, maxLength, errorMessage) {
    let result = false;
    let result1 = "";
    for (var i = 0; i < ValidationType.length; i++) {
        if (ValidationType[i].indexOf("Date") != -1) {
            ValidationType[i] = "Date";
        }

        switch (ValidationType[i]) {
            case "Email": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Mobile": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Url": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Numeric": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Letter Only": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Decimal": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Date": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Length": {

                if (inputField.length >= minLength[i] && inputField.length <= maxLength[i])
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Range": {
                if (minLength[i] != null && maxLength[i] != null) {
                    if (parseFloat(inputField) >= minLength[i] && parseFloat(inputField) <= maxLength[i])
                        result = true;
                    else {
                        result1 = errorMessage[i]; result = false;
                    }
                } else if (minLength[i] != null) {
                    if (parseFloat(inputField) >= minLength[i]) {
                        result = true;
                    }
                } else if (maxLength[i] != null) {
                    if (parseFloat(inputField) <= maxLength[i]) {
                        result = true;
                    }
                }

                if (minLength[i] != null && maxLength[i] != null) {
                    if (parseFloat(inputField) >= minLength[i] && parseFloat(inputField) <= maxLength[i])
                        result = true;
                    else {
                        result1 = errorMessage[i];
                        result = false;
                    }
                }
                else if (minLength[i] == null && maxLength[i] != null) {
                    if (parseFloat(inputField) <= maxLength[i])
                        result = true;
                    else {
                        result1 = errorMessage[i]; result = false;
                    }
                }
                else if (minLength[i] != null && maxLength[i] == null) {
                    if (parseFloat(inputField) >= minLength[i])
                        result = true;
                    else {
                        result1 = errorMessage[i]; result = false;
                    }
                }
                else if (minLength[i] == null && maxLength[i] == null) {
                    result = true;
                }

                break;
            }

            case "Required": {

                if (inputField.trim() != "")
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Custom": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            default: {
                console.log("default");
                result = true;
                break;
            }
        }
        if (!result) {
            break;
        }
    }
    if (ValidationType.length == 0 && result == false) {
        console.log("ValidationType.length result == false");
        result = true;
    }
    return { 'result1': result1, 'result': result };
}

App.checkEmailValidation = function (emailid) {
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (filter.test(emailid))
        return true;
    else
        return false;
};



//====================================================================
//    compare date(start date < end date)
//====================================================================
App.checkStartDateAndEndDate = function (startDate, endDate) {
    //let's suppose parameter date format is DD-MM-YYYY
    let s = startDate.split("-");
    let e = endDate.split("-");

    let dd_S = s[0]; let mm_S = s[1]; let yy_S = s[2];
    let dd_E = e[0]; let mm_E = e[1]; let yy_E = e[2];

    let sDate = new Date(yy_S + "-" + mm_S + "-" + dd_S);
    let eDate = new Date(yy_E + "-" + mm_E + "-" + dd_E);

    if (eDate < sDate) {
        return false;
    } else {
        return true;
    }
}


//====================================================================
//    convert string to date
//====================================================================
App.convertStringToDate = function (dateString) {
    let dateArray = dateString.split("-");
    let dd = dateArray[0]; let mm = dateArray[1]; let yy = dateArray[2];
    let date = new Date(yy + "-" + mm + "-" + dd);
    return date;
}


//====================================================================
//    convert string in sentence case   //02-Jan-2019
//====================================================================
App.convertSentenceCase = function (str) {
    if ((str === null) || (str === ''))
        return false;
    else
        str = str.toString();
    return str.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
}


App.setImage = function (file, update) {
    App.clearValidationErrors();
    var reader = new FileReader;

    reader.onload = function () {

        var selectedFile = file.files[0];
        var fileType = selectedFile["type"];
        var validImageTypes = ["image/gif", "image/jpeg", "image/png", "image/jpg", ];
        if ($.inArray(fileType, validImageTypes) < 0) {
            App.showValidationErrors('body', [{ "key": "fileProfile", "message": "Allowed file type extensions are jpeg jpg png gif." }])
            $(file).val("");

            $(update).attr('data-file-base', '');
            $('span[data-valmsg-for="' + $(update).prop('id') + '"]').addClass("field-validation-error").text('Allowed file type extensions are jpeg jpg png gif.');
            return;
        }


        var img = new Image;

        img.onload = function () {
            console.log(img.width, img.height, file.files[0].size);



            if (file.files[0].size > 2000000) {

                App.showValidationErrors('body', [{ "key": "fileProfile", "message": "Image size can not be more than 2MB." }])
                $(file).val("");

                $(update).attr('data-file-base', '');
                $('span[data-valmsg-for="' + $(update).prop('id') + '"]').addClass("field-validation-error").text('Image size can not be more than 2MB.');
                return;
            }

            $(update).attr('data-file-base', reader.result);
        };

        img.src = reader.result;
    };
    try {
        reader.readAsDataURL(file.files[0]);
    } catch (e) {
        $(update).attr('data-file-base', "");
        console.log(e);
    }

}