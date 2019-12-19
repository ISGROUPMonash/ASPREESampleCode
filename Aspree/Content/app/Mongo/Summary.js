var Summary = {};

Summary.AllCountries = {};
Summary.AllStates = {};
Summary.AllPostcodes = {};

Summary.project = null;
Summary.form = null;

Summary.FormList = [];
Summary.i = 0;
Summary.isValidFreeText = true;
Summary.ThisUserProjectList = {};

Summary.dbContextScheduledActivities = [];
Summary.StatusId = "";
Summary.TotalFormVariableCount = 0;

Summary.isSubmitClicked = false;
Summary.AllLinkedActivity = [];


Summary.IsLeftPanelVariablesContains = false;

Summary.LeftPanelAddressVariableList = [
    App.DefaultVariablesStringEnum.Email
    , App.DefaultVariablesStringEnum.Phone
    , App.DefaultVariablesStringEnum.Fax
    , App.DefaultVariablesStringEnum.State
    , App.DefaultVariablesStringEnum.StrtNum
    , App.DefaultVariablesStringEnum.StrtNme
    , App.DefaultVariablesStringEnum.Suburb
    , App.DefaultVariablesStringEnum.Postcode
    , App.DefaultVariablesStringEnum.StrtNum2
    , App.DefaultVariablesStringEnum.StrtNme2
    , App.DefaultVariablesStringEnum.Suburb2
    , App.DefaultVariablesStringEnum.State2
    , App.DefaultVariablesStringEnum.Postcode2
];


$(function () {
    $("#ActivityDate").on("click", function () {
        $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('');
        $('#ActivityDate').removeClass("input-validation-error");
        setTimeout(function () {
            $("#ActivityDate").parent(":first").find("form.editableform").on("submit", function () {

                var currentDate = new Date();
                let dd = currentDate.getDate();
                let mm = currentDate.getMonth() + 1;
                let yy = currentDate.getFullYear();
                currentDate = new Date(yy + "-" + mm + "-" + dd);

                var selectedDate = new Date($('#ActivityDate.inline-datepicker').editable('getValue').ActivityDate);
                if (selectedDate == "Invalid Date") {
                    $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('Invalid date.');
                    $('#ActivityDate').addClass("input-validation-error");
                }
                let dd1 = selectedDate.getDate();
                let mm1 = selectedDate.getMonth() + 1;
                let yy1 = selectedDate.getFullYear();
                selectedDate = new Date(yy1 + "-" + mm1 + "-" + dd1);

                if (selectedDate > currentDate) {
                    $("#ActivityDate").editable('setValue', null);
                    $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('Future date not allowed.');
                    $('#ActivityDate').addClass("input-validation-error");
                }
            });
        }, 300);
    });





});

Summary.validateFreeText = function (inputVal, id, validationName, regEx, min, max, errorMessage) {

    var a = App.checkValidation(inputVal, validationName, regEx, min, max, errorMessage);

    if (inputVal.length <= 0) {
        $('#' + id).addClass("input-validation-error");

        let messafge = $("#" + id).attr("data-variable-required-message");
        if (messafge == "null") {
            messafge = "Please enter missing data.";
        }
        $('span[data-valmsg-for="' + id + '"]').addClass("field-validation-error").text(messafge);
    } else {
        $('#' + id).attr('data-valid', a.result);
        $('#' + id).removeClass("input-validation-error");
        $('span[data-valmsg-for="' + id + '"]').addClass("field-validation-error").text(a.result1);
    }
}

// ============================================================== 
// summary page popup form validation   //17-Dec-2018  --PerformActivity page
// ==============================================================
Summary.validateActivityForm = function (e, isRequired) {
    let dataval = $(e).val();
    if ($(e).prop('type') == "checkbox" || $(e).prop('type') == "radio") {
        dataval = $('input[name="' + $(e).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
        isRequired = "false";
    }
    if ($(e).prop('nodeName').toLowerCase() === "a") {
        dataval = $(e).text();
    }
    if ($(e).prop('type') == "file") {
        dataval = $(e).attr("data-file-base");
    }
    if ($(e).attr("data-valid") != undefined) {

        if (!$.parseJSON($(e).attr("data-valid"))) {
            return true;
        }
    }

    if (isRequired == "true" && dataval == "" || isRequired == "true" && dataval == null) {
        if ($(e).attr('data-variable-required-message') != "null") {
            $('span[data-valmsg-for="' + $(e).prop('id') + '"]').addClass("field-validation-error").text($(e).attr('data-variable-required-message'));
        }
        return true;
    }
    return false;
}

// ============================================================== 
// getDatepickerFormat //18-Dec-2018
// ==============================================================
Summary.getDatepickerFormat = function (dateformate) {

    let format = "DD-MM-YYYY";
    switch (dateformate) {
        case "DDMMYYYY":
            format = "DD-MM-YYYY";
            break;

        case "MMDDYYYY":
            format = "MM-DD-YYYY";
            break;

        case "MMYYYY":
            format = "MM-YYYY";
            break;

        case "YYYY":
            format = "YYYY";
            break;

        case "DDMMMYYYY":
            format = "DD-MMM-YYYY";
            break;

        case "MMMYYYY":
            format = "MMM-YYYY";
            break;
        default:
            break;
    }
    return format;
}

// ============================================================== 
// Get all countries
// ==============================================================
Summary.GetAllCountries = function () {
    App.postData(App.ApiUrl + "/Country", {}, function (result) {
        Summary.AllCountries = result;
    }, "Get");
}

// ============================================================== 
// Get all states
// ==============================================================
Summary.GetAllStates = function () {
    App.postData(App.ApiUrl + "/State", {}, function (result) {
        Summary.AllStates = result;
        try {

        } catch (err) {  }
    }, "Get");
}

// ============================================================== 
// get all postcode
// ==============================================================
Summary.GetAllPostcodes = function () {
    App.postData(App.ApiUrl + "/PostCode", {}, function (result) {
        Summary.AllPostcodes = result;
    }, "Get");
}

//========================================================================================
//      Set Child Variables(dependent variables)
//========================================================================================
Summary.setChildVariables = function (e) {

    if ($(e).prop('type') == "checkbox") {
        if ($(e).prop('checked'))
            $(e).val("1");
        else
            $(e).val("0");
    }

    let childVarType = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-typename");
    if (childVarType == "Numeric (Integer)" || childVarType == "Numeric (Decimal)") {

        var thisVarType = $(e).attr("data-variable-typename");

        if (thisVarType == "Date") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().addClass("hide");
        }
        else if (thisVarType == "Numeric (Integer)") {



            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");


            let parentVariableResponse = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-parent-variable-response");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().addClass("hide");
            var pc = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-dependent-variable-value-isblank");
            var currentvalue = $(e).val();
            try {
                if (typeof pc === 'undefined') {
                    if ($(jj)[0].tagName == "H2" || $(jj)[0].tagName == "P") {
                        pc = "true";
                    }
                }
            } catch (e) {

            }
            if (pc) {
                if (currentvalue == "") {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    Summary.GetSubChield_Show($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']"));
                }
                else {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().addClass("hide");

                    Summary.GetSubChield_Hide($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']"));
                }

            }


        }
        else if (thisVarType == "Numeric (Decimal)") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            var pc = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-dependent-variable-value-isblank");
            var currentvalue = $(e).val();
            try {
                if (typeof pc === 'undefined') {
                    if ($(jj)[0].tagName == "H2" || $(jj)[0].tagName == "P") {
                        pc = "true";
                    }
                }
            } catch (e) {

            }
            if (pc) {
                if (currentvalue == "") {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    Summary.GetSubChield_Show($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']"));
                }
                else {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().addClass("hide");

                    Summary.GetSubChield_Hide($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']"));
                }

            }


        }
        else {

            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false"))
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().addClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {

                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").each(function (l, m) {

                    if ($(m).find("[data-variable-typename='Numeric (Decimal)']").length > 0) {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    } else if ($(m).find("[data-variable-typename='Numeric (Integer)']").length > 0) {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    } else if ($(m).prop("type") == "color") {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().removeClass("hide");
                    } else if ($(m).prop("type") == "file") {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().removeClass("hide");
                    } else {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    }
                });
            }

        }
    }
    else {
        var thisVarType = $(e).attr("data-variable-typename");
        if (thisVarType == "Date") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").each(function (l, m) {
                    let id = $(m).prop("id");
                    id = id.replace("inputField-", "");
                    $("#divDependent-" + id).addClass("hide");
                });
            }


            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").attr("data-variable-required", "true");
            //PC(10Sep2019)
            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {

                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").each(function (l, m) {

                    if ($(m).find("[data-variable-typename='Numeric (Decimal)']").length > 0) {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    } else if ($(m).find("[data-variable-typename='Numeric (Integer)']").length > 0) {
                        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").parent().parent().parent().parent().parent().removeClass("hide");
                    } else {

                        try {
                            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").each(function (ii, jj) {
                                let id = $(jj).prop("id");
                                id = id.replace("inputField-", "");
                                $("#divDependent-" + id).removeClass("hide");
                                Summary.GetSubChield_Show($(jj));
                            });
                        } catch (e) {
                            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).text() + "']").parent().parent().parent().parent().removeClass("hide");
                        }
                    }
                });
            }
        }
        else if (thisVarType == "Numeric (Integer)") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {
                if ($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").length > 0) {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").each(function (ii, jj) {

                        var pc = $(jj).attr("data-dependent-variable-value-isblank");
                        var currentvalue = $(e).val();
                        try {
                            if (typeof pc === 'undefined') {
                                if ($(jj)[0].tagName == "H2" || $(jj)[0].tagName == "P") {
                                    pc = "true";
                                }
                            }
                        } catch (e) {

                        }
                        if (pc) {
                            if (currentvalue == "") {
                                let id = $(jj).prop("id");
                                id = id.replace("inputField-", "");
                                $("#divDependent-" + id).removeClass("hide");
                                Summary.GetSubChield_Show($(jj));
                            }
                            else {
                                let id = $(jj).prop("id");
                                id = id.replace("inputField-", "");
                                $("#divDependent-" + id).addClass("hide");
                                Summary.GetSubChield_Hide($(jj));
                            }

                        } else {
                            let parentVariableResponse = $(jj).attr("data-parent-variable-response");
                            let direction = parentVariableResponse.charAt(0);
                            let cutPoint = parseInt(parentVariableResponse.substring(1, parentVariableResponse.length));
                            let currentVal = parseInt($(e).val());

                            //show current child variable and add required attribute of current child variable
                            if (direction == "<") {
                                if (currentVal < cutPoint) {
                                    $(jj).attr("data-variable-required", "true");

                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).removeClass("hide");
                                } else {
                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).addClass("hide");
                                }
                            }
                            if (direction == ">") {
                                if (currentVal > cutPoint) {
                                    $(jj).attr("data-variable-required", "true");

                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).removeClass("hide");
                                } else {
                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).addClass("hide");
                                }
                            }
                        }
                    });
                }
            }
        }
        else if (thisVarType == "Numeric (Decimal)") {
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {
                if ($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").length > 0) {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").each(function (ii, jj) {

                        var pc = $(jj).attr("data-dependent-variable-value-isblank");
                        var currentvalue = $(e).val();


                        try {
                            if (typeof pc === 'undefined') {
                                if ($(jj)[0].tagName == "H2" || $(jj)[0].tagName == "P") {
                                    pc = "true";
                                }
                            }
                        } catch (e) {

                        }


                        if (pc) {
                            if (currentvalue == "") {
                                let id = $(jj).prop("id");
                                id = id.replace("inputField-", "");
                                $("#divDependent-" + id).removeClass("hide");
                                Summary.GetSubChield_Show($(jj));
                            }
                            else {
                                let id = $(jj).prop("id");
                                id = id.replace("inputField-", "");
                                $("#divDependent-" + id).addClass("hide");
                                Summary.GetSubChield_Hide($(jj));
                            }
                        } else {
                            let parentVariableResponse = $(jj).attr("data-parent-variable-response");
                            let direction = parentVariableResponse.charAt(0);
                            let cutPoint = parseFloat(parentVariableResponse.substring(1, parentVariableResponse.length));
                            let currentVal = parseFloat($(e).val());
                            //show current child variable and add required attribute of current child variable
                            if (direction == "<") {
                                if (currentVal < cutPoint) {
                                    $(jj).attr("data-variable-required", "true");

                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).removeClass("hide");
                                } else {
                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).addClass("hide");
                                }
                            }
                            if (direction == ">") {
                                if (currentVal > cutPoint) {
                                    $(jj).attr("data-variable-required", "true");

                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).removeClass("hide");
                                } else {
                                    let id = $(jj).prop("id");
                                    id = id.replace("inputField-", "");
                                    $("#divDependent-" + id).addClass("hide");
                                }
                            }
                        }
                    });
                }
            }
        }
        else {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "false");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {

                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").each(function (ii, jj) {


                    if ($(e).prop('type') == "checkbox") {
                        if ($(e).prop('checked')) {

                            let id = $(jj).prop("id");
                            id = id.replace("inputField-", "");
                            $("#divDependent-" + id).addClass("hide");
                            Summary.GetSubChield_Show($(jj));

                        }
                        else {
                            let id = $(jj).prop("id");
                            id = id.replace("inputField-", "");
                            $("#divDependent-" + id).removeClass("hide");
                            Summary.GetSubChield_Hide($(jj));
                        }
                    } else {


                        let id = $(jj).prop("id");
                        id = id.replace("inputField-", "");
                        $("#divDependent-" + id).addClass("hide");
                        //$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");
                        Summary.GetSubChield_Show($(jj));
                    }
                });


            }
            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");

            if (!$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").hasClass("view-permissions-false")) {

                try {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + $(e).val() + "']").each(function (ii, jj) {

                        let id = $(jj).prop("id");
                        id = id.replace("inputField-", "");
                        $("#divDependent-" + id).removeClass("hide");
                        Summary.GetSubChield_Show($(jj));
                    });
                } catch (eKK) {
                    //cl  
                }

            }
        }
    }


}

//========================================================================================
//      Set State based on country change
//========================================================================================
Summary.StateListByCountry = function (e) {
    let countryId = $(e).val();

    let dropdown = $('.state-list');
    dropdown.empty();
    dropdown.append('<option value="">Select</option>');
    dropdown.prop('selectedIndex', 0);

    $.each(Summary.AllStates, function (key, entry) {
        if (entry.countryId == countryId) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        }
    });
}

//===============================================================
//  check username existence in userlogin table
//===============================================================
Summary.checkUserNameExistence = function (userName, authType, userid) {

    let isAsync = false;
    let response = null;
    App.postData(App.ApiUrl + "Account/CheckUsernameAvailability/" + userName + "/" + authType + "/" + userid, {}, function (result) {
        response = result;
    }, "Get", undefined, isAsync);

    return response;
}

Summary.ddmmyyyClassSort = function (a, b) {

    var A = new Date(a.getAttribute('data-activity-date')),
        B = new Date(b.getAttribute('data-activity-date'))
    return A < B ? 1 : -1
}

//====================================================================
// Get all project of current user for "Project Linkage"(14-May-2019)
//====================================================================
Summary.GetAllProjectByUserId = function () {
    App.postData(App.ApiUrl + 'Project/GetAllProjectByUserId/', {}, function (result) {

        Summary.ThisUserProjectList = result;
    }, 'GET');
}

// ============================================================== 
// summary page activity //13-May-2019
// ==============================================================
Summary.AddSummaryPageActivity = function () {
    $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('');
    $('#ActivityType').removeClass("input-validation-error");
    $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('');
    $('#CompletedByUser').removeClass("input-validation-error");
    $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('');
    $('#ActivityDate').removeClass("input-validation-error");

    var aDate = $("#ActivityDate").text().split("/");
    var dd = aDate[0];
    var mm = aDate[1];
    var yy = aDate[2];

    if (dd == "Empty") {
        $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('This field is required.');
        $('#ActivityDate').addClass("input-validation-error");
        return false;
    }

    if (dd > 31) { App.showError("Please enter a valid date."); return false; }
    if (mm > 12) { App.showError("Please enter a valid date."); return false; }

    var activityDate = mm + "-" + dd + "-" + yy;//new Date(mm + "-" + dd + "-" + yy);


    let entityCreatedDateArray = $("#EntityCreatedDate").val().split("-");
    if (new Date(yy + "-" + mm + "-" + dd) < new Date(entityCreatedDateArray[2] + "-" + entityCreatedDateArray[0] + "-" + entityCreatedDateArray[1])) {
        App.showError("Activity date should greater than entity registration date.");
        return false;
    }

    if ($("#ActivityType").val().length < 1) {
        $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('This field is required.');
        $('#ActivityType').addClass("input-validation-error");
        return false;
    }
    if ($("#CompletedByUser").val().length < 1) {
        $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('This field is required.');
        $('#CompletedByUser').addClass("input-validation-error");
        return false;
    }

    var sendData = {
        "ActivityGuid": $("#ActivityType").val(),
        "ActivityCompletedByGuid": $("#CompletedByUser").val(),
        "ActivityDate": activityDate,
        "IsActivityAdded": true,
        "ProjectGuid": $("#CurrentProjectId").val(),//$("#ProjectId").val(),  
        "PersonEntityId": $("#PersonEntityId").val(),
    };

    let url = "MongoDB_Summary/AddSummaryPageActivity";
    if (App.IsTestSite === true) {
        url = "Review/AddSummaryPageActivityMongo";
    }
    let methodeType = "POST";   
    App.postData(App.ApiUrl + url, sendData, function (result) {
      
        Summary.refreshSummaryPageActivities(result);

        try {
            Summary.ReloadSummaryPageDropdown($("#CurrentProjectId").val(), sendData.PersonEntityId);
        } catch (e) {           
        }

        App.showSuccess("Activity added successfully.");
    }, methodeType);
}

// ============================================================== 
// referash summary page activity //13-May-2019
// ==============================================================
Summary.refreshSummaryPageActivities = function (result) {

    try {

        let dateArray = result.activityDate.split('-');
        let d = dateArray[2];
        let m = dateArray[1];
        let y = dateArray[0];

        d = d.slice(0, 2);

        result.activityDate = y + "-" + m + "-" + d;
    } catch (e) {

    }
    let activityDate = new Date(result.activityDate);
    let dd = activityDate.getDate();
    let mmm = activityDate.getMonth();

    var month_names_array = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    var container = $('#scheduledActivitiesContainer');
    var template = $('#scheduledActivitiesTemplate').children().clone();

    let replacedDD = $(template).html().replace('scheduled_activities_day', dd);
    $(template).html(replacedDD);

    let replacedMMM = $(template).html().replace('scheduled_activities_month', month_names_array[mmm]);
    $(template).html(replacedMMM);


    //let replacedCompletedBy = $(template).html().replace('scheduled_activities_completedby', $("#CompletedByUser option:selected").text());
    let replacedCompletedBy = $(template).html().replace('scheduled_activities_completedby', result.activityCompletedByName);
    $(template).html(replacedCompletedBy);


    template.find('.timeline-title').text(result.activityName);
    template.find('.timeline-title').attr("data-activity-display-name", result.activityName);


    let crntFrmStatus = 0;
    if (result.activityName == "Project Linkage") {
        let pOjb = result.summaryPageActivityFormsList.find(function (d) { return d; });
        if (pOjb) {
            crntFrmStatus = pOjb.formStatusId;
            if (pOjb.formStatusId != 6) {
                template.find('.btn-danger').remove();
            }
        }
    }

    var actNameLst = [
        App.DefaultFormNamesStringEnum.Person_Registration
        , App.DefaultFormNamesStringEnum.Participant_Registration
        , App.DefaultFormNamesStringEnum.Place__Group_Registration
        , App.DefaultFormNamesStringEnum.Project_Registration
    ];
    if (actNameLst.includes(result.activityName)) {
        template.find('.btn-danger').remove();
    }





    try {
        if (result.linkedProjectName != null) {
            template.find('.timeline-title').html(result.activityName + ' <small>(' + result.linkedProjectName + ')</small>');
        }
    } catch (e) {
      
    }


    template.attr('data-activity-date', result.activityDate);

    for (var i = 0; i < result.summaryPageActivityFormsList.length; i++) {
        let form = result.summaryPageActivityFormsList[i];



        let frmStatus = form.formStatusName;
        if (frmStatus)
            frmStatus = frmStatus.replace(/_/g, " ")
        if (frmStatus == "Published") {
            frmStatus = "Submitted";
        } else if (frmStatus == "Submitted for review") {
            frmStatus = "Submitted for Review";
        }
        var contentBody = `<div class="timeline-body">
            <div class="row">
                <div class ="col-sm-4"><small class ="font-16">${form.formTitle}</small></div>
                <div class="col-sm-4">
                    <small class ="ml-5 link font-16  ${frmStatus == 'Submitted' ? 'text-success' : frmStatus == 'Submitted for Review' ? 'text-submit-for-review' : 'text-warning'} "
                        id="${form.formGuid}-${result.id}"
                        data-summarypage-userentity="${result.id}"
                        onclick="Summary.OpenSummaryPageFormPopup('${result.personEntityId}','${form.formGuid}','${result.activityGuid}', this, '${result.projectVersion}')">${frmStatus}</small>
                </div>
            </div>
        </div>`


        template.find('.timeline-panel').append(contentBody);
    }
    template.attr("id", "activityContainer-" + result.id);
    template.find("#datemonth-new").attr("id", "datemonth-" + result.id);
    template.find("#completedbyname-new").attr("id", "completedbyname-" + result.id);

    let acM = mmm + 1;
    let acD = dd;
    let acY = activityDate.getFullYear();
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-date", acY + "-" + acM + "-" + acD);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-activityid", result.activityGuid);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-summarypage-activityid", result.id);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-completedby", result.activityCompletedByGuid);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-entity-number", result.personEntityId);


    template.find("#btnEditSummaryPageActivity-new").attr("id", "btnEditSummaryPageActivity-" + result.id);
    template.find(".btn-danger").attr("onclick", "Summary.DeleteSummaryPageActivity('" + result.id + "', '" + result.activityName + "')");
    container.append(template);
}

// ============================================================== 
// referash summary page activity //13-May-2019
// ==============================================================
$.fn.sortChildren = function (cmp) {

    var self = this,
        children = $.makeArray(this.children()).sort(cmp)
    $.each(children, function (i, child) {
        self.append(child)
    })
    return self
}

//===============================================================
//  open form in model  13-May-2019
//===============================================================
Summary.OpenSummaryPageFormPopup = function (entId, formId, activityId, e, p_Version, fname) {
    Summary.IsLeftPanelVariablesContains = false;
    Summary.StatusId = "";
    let summarypageactivity = "";
    if (jQuery.type(e) === "object") {
        Summary.StatusId = $(e).prop('id');
        summarypageactivity = $(e).attr("data-summarypage-userentity");

    } else {
        Summary.StatusId = formId + "-" + e;
        summarypageactivity = e;
    }


    let url = "MongoDB_Summary/GetSummaryPageForm/" + entId + "/" + formId + "/" + activityId + "/" + $("#ProjectId").val() + "/" + p_Version + "/" + summarypageactivity + "/" + $("#CurrentProjectId").val();
    if (App.IsTestSite === true) {
        url = "Review/GetSummaryPageForm/" + entId + "/" + formId + "/" + activityId + "/" + $("#ProjectId").val() + "/" + p_Version + "/" + summarypageactivity + "/" + $("#CurrentProjectId").val();
    }
    let methodeType = "GET";
    let isFormDisabledCount = 0;

    App.postData(App.ApiUrl + url, {}, function (result) {
      
        if (result === null) {
            App.showError('This form "' + fname + '" is not available in current project');

            try {
                Summary.UpdateURL_AfterDataEntry();
            } catch (errURL) {
          
            }

            return false;
        }
        Summary.TotalFormVariableCount = result.totalFormVariableCount;

        $('#frm-1').attr('data-summarypageactivity', summarypageactivity);

        $('#frm-1').attr('data-form-guid', result.formGuid);

        $('#activityformVariableContainer').empty();
        $('#search-variable-result-model-footer').empty();
        $('#result-model-footer-for-modifiedby').empty();

        var container = $('#activityformVariableContainer');
        let resultFooter = $('#search-variable-result-model-footer');
        let resultFooterModifiedby = $('#result-model-footer-for-modifiedby');

        $('#search-variable-result-model-title').text(result.formTitle);
        $("#ActivityId").attr('data-activity-guid', activityId);

        //make single column form(participant registration)
        if (result.formTitle == "Participant Registration") {
            $('#activityformVariableContainer').removeClass("row");
        } else if (result.formTitle == "Person Registration") {
            $('#activityformVariableContainer').removeClass("row");
        } else {

        }

        if (result.formDataEntryId != null)
            $("#hdnFormDataEntryGuid").val(result.formDataEntryId);
        else
            $("#hdnFormDataEntryGuid").val("");
        let varIndex = 0;

        let SubmitButtonText = "Submit";

        let alwaysAllowedRoles = ["System Admin", "Project Admin"];
        let isAdminRoleCount = alwaysAllowedRoles.findIndex(function (obj) { return obj === $("#UserProjectRole").val(); });
        let isAdminRole = isAdminRoleCount < 0 ? false : true;

        if (result.isDefaultForm === 0) isAdminRole = false;        
        result.variablesListMongo.forEach(function (element) {

            //=========================================
            if (Summary.LeftPanelAddressVariableList.includes(element.variableName)) {
                Summary.IsLeftPanelVariablesContains = true;
            }
            //=========================================

            varIndex++;
            //=========================================================================================
            //============== can view permission start
            //=========================================================================================
            let roleIndexCanView = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
            if (roleIndexCanView >= 0) {
                if (element.variableRoleListMongo[roleIndexCanView].canView == false) {

                    let canViewVar = `<div class="view-permissions-false"><input id="inputField-${varIndex}"
                                class ="form-control form-control-line dynamic-variables view-permissions-false"
                                type="text"
                                name="inputField-${varIndex}"
                                title ="${element.helpText}"
                                value="${element.selectedValues}"
                                placeholder="${element.question}"
                                data-variable-guid="${element.variableId}"
                                data-variable-username-guid="${element.userLoginGuid}" /></div>`;
                    container.append(canViewVar);

                    return;
                }
            }
            else if (roleIndexCanView != 0) {
                let canViewVar = `<div class="view-permissions-false"><input id="inputField-${varIndex}"
                                class ="form-control form-control-line dynamic-variables view-permissions-false"
                                type="text"
                                name="inputField-${varIndex}"
                                title ="${element.helpText}"
                                value="${element.selectedValues}"
                                placeholder="${element.question}"
                                data-variable-guid="${element.variableId}"
                                data-variable-username-guid="${element.userLoginGuid}" /></div>`;
                container.append(canViewVar);
                return;
            }
            //=========================================================================================
            //============== can view permission end
            //=========================================================================================
            let helpTextSpan = "";

            if (element.helpText) {
                helpTextSpan = '<span class="help-block"><small>' + element.helpText + '</small></span><br>';
            }
            element.helpText = (!element.helpText ? element.question : element.helpText);
            if (element.question != null) {
                element.question.trim("");
            }
            if (element.selectedValues == "") {
                if (element.variableName != "MiddleName") {
                    if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
                        SubmitButtonText = "Submit for Review";
                }
            }
            if (element.variableName == "Email" && (result.formTitle == "Person Registration" || result.formTitle == "Place/Group Registration")) {
                element.isRequired = true;
            }
            let containerBody = "";
            let canEdit = "true";

            if (element.variableTypeName == "Text Box") {
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableValidationRuleListMongo) {
                    PreviewPane_ValidationName.push(element.variableValidationRuleListMongo[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableValidationRuleListMongo[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableValidationRuleListMongo[key].min);
                    PreviewPane_ValidationMax.push(element.variableValidationRuleListMongo[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableValidationRuleListMongo[key].validationMessage);
                }

                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" ' + requiredMessage;
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="text"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    //' title ="' + element.helpText + '"' +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' value="' + element.selectedValues + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-typename="' + element.variableTypeName + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-username-guid="' + element.userLoginGuid + '"' + //onchange + --13052019
                    jqueryValidator +

                    "onblur='Summary.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableTypeName == "Free Text") {

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableValidationRuleListMongo) {
                    PreviewPane_ValidationName.push(element.variableValidationRuleListMongo[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableValidationRuleListMongo[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableValidationRuleListMongo[key].min);
                    PreviewPane_ValidationMax.push(element.variableValidationRuleListMongo[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableValidationRuleListMongo[key].validationMessage);
                }
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }


                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" ' + requiredMessage;
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<textarea id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables" rows="2" cols="50"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-typename="' + element.variableTypeName + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    jqueryValidator +
                    "onblur='Summary.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  >" + element.selectedValues + "</textarea>" +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';

            }

            else if (element.variableTypeName == "Dropdown") {
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        if (element.variableName != "EntGrp")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    if (element.variableName != "EntGrp")
                        isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }
                //EntGroup default settings
                if (result.formTitle == "Person Registration" && element.variableName === "EntGrp") {
                    element.selectedValues = 2;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Participant Registration" && element.variableName === "EntGrp") {
                    element.selectedValues = 1;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Place/Group Registration" && element.variableName === "EntGrp") {
                    element.selectedValues = 3;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Project Registration" && element.variableName === "EntGrp") {
                    element.selectedValues = 4;
                    isDisabled = "disabled";
                }

                var options = "";
                for (var i = 0; i < element.values.length; i++) {
                    if (element.values[i] == element.selectedValues) {

                        options = options + "<option selected value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                    } else {
                        options = options + "<option value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                    }
                }

                //---------for country/state dropdown-----------
                let variableTypeClass = "";
                let countryChange = "";
                if (element.variableName == "Country" || element.variableName == "State") {
                    options = "";
                    if (element.variableName == "Country") {
                        variableTypeClass = "country-list";
                        countryChange = "onchange=Summary.StateListByCountry(this)";
                        $.each(Summary.AllCountries, function (key, entry) {
                            if (entry.guid == element.selectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    } else if (element.variableName == "State") {
                        variableTypeClass = "state-list";
                        $.each(Summary.AllStates, function (key, entry) {
                            if (entry.guid == element.selectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    }
                }
                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];


                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + '"' +
                    'id="inputField-' + varIndex + '"' + isDisabled +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-guid-id="' + element.variableGuid + '"' + countryChange +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-typename = "' + element.variableTypeName + '"' +

                    ' onblur="Summary.validateFreeText(this.value, this.id, [], [], [], [], [])"' +
                    ' name="inputField-' + varIndex + '">' +
                    '<option value="">Select</option>' +
                    options +
                    '</select>' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }
            else if (element.variableTypeName == "LKUP") {
                let variableTypeClass = "";

                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        if (element.variableName != "EntGrp")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    if (element.variableName != "EntGrp")
                        isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                if (result.formTitle == "Project Linkage" && element.variableName == "LnkPro") {
                    if (element.selectedValues != "") {
                        isDisabled = "disabled";
                    }
                }
                var options = "";

                if (result.formTitle == "Project Linkage" && element.variableName == "LnkPro") {
                    var temp1 = element.linkedProjectListWithGroupList;
                    var arrReportType = temp1.map(x => x.groupName);
                    var arrUniqueReportType = arrReportType.filter(function (itm, i) {
                        return i == arrReportType.indexOf(itm);
                    });

                    arrUniqueReportType.forEach(function (item) {
                        var ddlOG = $("<optgroup></optgroup>").attr("label", item);
                        temp1.filter(function (fItem) {
                            if (item == fItem.groupName) {

                                try {
                                    var txtSelected = fItem.projectId == element.selectedValues ? "selected" : "";
                                    if (txtSelected != "") {
                                        let isExest = Summary.AllLinkedActivity.includes(fItem.projectId);
                                        if (isExest) {
                                            if (fItem.projectId != element.selectedValues) {
                                                return;
                                            }
                                        }
                                    } else {
                                        let isExest = Summary.AllLinkedActivity.includes(fItem.projectId);
                                        if (isExest) {
                                            return;
                                        }
                                    }
                                } catch (eLinkedProj) {
                                   
                                }
                                ddlOG.append($("<option " + txtSelected + ">" + fItem.projectName + "</option>").attr("value", fItem.projectId))
                            }
                        });
                        options += ddlOG[0].outerHTML;
                    });
                  
                } else {
                    for (var i = 0; i < element.values.length; i++) {
                        if (result.formTitle == "Project Linkage") {
                            var thisProj = Summary.ThisUserProjectList.find(function (d) { return d.guid == element.values[i] });
                            if (typeof thisProj !== 'undefined') {
                                if (Summary.ISSpaceOnly(element.valueDescription[i])) {
                                    if (element.values[i] == element.selectedValues) {
                                        options = options + "<option selected value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                                    } else {
                                        options = options + "<option value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                                    }
                                }
                            }
                        } else {
                            if (Summary.ISSpaceOnly(element.valueDescription[i])) {
                                if (element.values[i] == element.selectedValues) {
                                    options = options + "<option selected value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                                } else {
                                    options = options + "<option value='" + element.values[i] + "'>" + element.valueDescription[i] + "</option>";
                                }
                            }
                        }
                    }
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6"> ' +
                    '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + '"' +
                    'id="inputField-' + varIndex + '"' + isDisabled +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-guid-id="' + element.variableGuid + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-typename="' + element.variableTypeName + '"' +
                    ' onblur="Summary.validateFreeText(this.value, this.id, [], [], [], [], [])"' +
                    jqueryValidator +
                    ' name="inputField-' + varIndex + '">' +
                    '<option value="">Select</option>' +
                    options +
                    '</select>' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }
            else if (element.variableTypeName == "Checkbox") {

                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        if (element.variableName != "EntID")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    if (element.variableName != "EntID")
                        isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                let options = "";
                for (var i = 0; i < element.values.length; i++) {
                    if (element.values[i] == element.selectedValues) {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" ' + isDisabled + ' name="inputField-' + varIndex + '" data-can-edit="' + canEdit + '" data-variable-guid-id="' + element.variableGuid + '" id="inputField-' + varIndex + '" value="' + element.values[i] + '"  data-dependent-variable-value-isblank="' + element.isBlank + '" data-variable-guid="' + element.variableId + '" data-variable-required="' + element.isRequired + '" data-variable-name="' + element.variableName + '" data-variable-typename="' + element.variableTypeName + '" class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.valueDescription[i] + '</span></label>';
                    } else {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" ' + isDisabled + ' name="inputField-' + varIndex + '" data-can-edit="' + canEdit + '" data-variable-guid-id="' + element.variableGuid + '" id="inputField-' + varIndex + '" value="' + element.values[i] + '"  data-dependent-variable-value-isblank="' + element.isBlank + '" data-variable-guid="' + element.variableId + '" data-variable-required="' + element.isRequired + '" data-variable-name="' + element.variableName + '" data-variable-typename="' + element.variableTypeName + '" class="custom-control-input dynamic-variables" /><span class="custom-control-label">' + element.valueDescription[i] + '</span></label>';
                    }
                }

                let isRequired = "";

                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }

                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '</div><div class="col-sm-6">' +
                    options +

                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div>';
            }

            else if (element.variableTypeName == "Radio") {
                let options = "";
                for (var i = 0; i < element.values.length; i++) {
                    options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.values[i] + '"  data-variable-guid="' + element.variableId + '" data-variable-required="' + element.isRequired + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.valueDescription[i] + '</span></label>';
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    options +

                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableTypeName == "Numeric (Integer)") {
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        if (element.variableName != "EntID")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    if (element.variableName != "EntID")
                        isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableValidationRuleListMongo) {
                    PreviewPane_ValidationName.push(element.variableValidationRuleListMongo[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableValidationRuleListMongo[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableValidationRuleListMongo[key].min);
                    PreviewPane_ValidationMax.push(element.variableValidationRuleListMongo[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableValidationRuleListMongo[key].validationMessage);
                }
                var Newid = element.selectedValues;
                if (element.variableName === "EntID") {
                    isDisabled = "disabled";
                    Newid = App.ConvertInto7Digit(element.selectedValues);
                    element.question = result.formTitle == "Project Registration" ? "Project ID" : result.formTitle == "Participant Registration" ? "Participant ID" : "ID";
                }

                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }


                let numberFieldClassStart = '<div class="col-md-4 pl-0">';
                let numberFieldClassEnd = '</div>';
                let dataVarTypeName = ' data-variable-typename="Numeric (Integer)"';

                let lnt = Math.max(PreviewPane_ValidationMax);
                if (isNaN(lnt) || lnt == 0 || lnt == null || lnt == undefined)
                    lnt = '999999999999999999999999999999999999999999999';
                lnt = lnt + "";

                if (element.variableName === "Phone") {
                    numberFieldClassStart = '';
                    numberFieldClassEnd = '';
                    dataVarTypeName = '';
                    lnt = "999999999999999";
                }


                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    numberFieldClassStart +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="text"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    //' title ="' + element.helpText + '"' +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' value="' + Newid + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-guid-id="' + element.variableGuid + '"' +
                    ' data-parent-variable-id="' + element.dependentVariableId + '"' +
                    ' data-parent-variable-response="' + element.responseOption + '"' +
                    ' maxlength="' + lnt.length + '"' +
                    dataVarTypeName +
                    jqueryValidator +
                    "onblur='Summary.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    numberFieldClassEnd +

                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableTypeName == "Numeric (Decimal)") {
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {

                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {

                    isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableValidationRuleListMongo) {
                    PreviewPane_ValidationName.push(element.variableValidationRuleListMongo[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableValidationRuleListMongo[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableValidationRuleListMongo[key].min);
                    PreviewPane_ValidationMax.push(element.variableValidationRuleListMongo[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableValidationRuleListMongo[key].validationMessage);
                }
                if (element.variableName === "EntID") {
                    isDisabled = "disabled";
                }
                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<div class="col-md-4 pl-0">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="text"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' value="' + element.selectedValues + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-guid-id="' + element.variableGuid + '"' +
                    ' data-variable-typename="Numeric (Decimal)"' +
                    jqueryValidator +
                    "onblur='Summary.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    "</div>" +

                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableTypeName == "Date") {
                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "readonly='readonly' style='pointer-events: none;'";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    isFormDisabledCount++;
                    isDisabled = "readonly='readonly' style='pointer-events: none;'";
                    canEdit = "false";
                }
                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableValidationRuleListMongo) {
                    PreviewPane_ValidationName.push(element.variableValidationRuleListMongo[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableValidationRuleListMongo[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableValidationRuleListMongo[key].min);
                    PreviewPane_ValidationMax.push(element.variableValidationRuleListMongo[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableValidationRuleListMongo[key].validationMessage);
                }
                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableRequiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableRequiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }

                let dateformat = "DDMMYYYY";
                $.each(element.variableValidationRuleListMongo, function (key, value) {
                    if (value.validationName == "Date") {
                        dateformat = "DDMMYYYY";
                    } else {
                        dateformat = value.validationName.replace("Date_", "");
                    }
                });


                if (result.formTitle == App.DefaultFormNamesStringEnum.Project_Linkage && (element.variableName == App.DefaultVariablesStringEnum.Join || element.variableName == App.DefaultVariablesStringEnum.End)) {
                    dateformat = "DDMMMYYYY"
                }
                let datepickerformat = Summary.getDatepickerFormat(dateformat);
                if (element.selectedValues.trim() == "") {
                    var fullDate = new Date();
                    var twoDigitMonth = fullDate.getMonth() + 1;
                    var twoDigitDate = fullDate.getDate();

                    var currentDate = twoDigitDate + "-" + twoDigitMonth + "-" + fullDate.getFullYear();
                    switch (dateformat) {
                        case "DDMMYYYY":
                            currentDate = twoDigitDate + "-" + twoDigitMonth + "-" + fullDate.getFullYear();
                            break;
                        case "MMDDYYYY":
                            currentDate = twoDigitMonth + "-" + twoDigitDate + "-" + fullDate.getFullYear();
                            break;
                        default:
                            currentDate = twoDigitDate + "-" + twoDigitMonth + "-" + fullDate.getFullYear();
                            break;
                    }

                    element.selectedValues = "";
                }
                if (element.selectedValues == "Empty") {
                    element.selectedValues = "";
                }
                containerBody = `<div class ="form-group mb-2 date-input">
                      <div class ="row align-items-center lable-tl"><div class ="col-sm-6 text-right">
                                        <label class ="control-label">${element.question}${isRequired}</label>
                                          </div><div class="col-sm-6">

                                        <a href="#"
                                        id="inputField-${varIndex}"

                                        class ="singledate form-control form-control-line dynamic-variables customdatepicker"
                                        data-type="combodate"
                                        data-value="${element.selectedValues}"
                                        value="${element.selectedValues}"
                                        data-format="${datepickerformat}"
                                        data-viewformat="${datepickerformat}"
                                        data-variable-name="${element.variableName}"
                                        data-template="${datepickerformat}"
                                        data-pk="1"
                                        name="inputField-${varIndex}"  ${isDisabled}
                                        data-title="${element.question}"
                                        data-variable-guid="${element.variableId}"
                                        data-variable-required-message="${element.variableRequiredMessage}"
                                        data-variable-required="${element.isRequired}"
                                        data-variable-guid-id="${element.variableGuid}"
                                        data-can-edit="${canEdit}"
                                        data-max-date="0d"

                                        data-min-year="2000"
                                        data-max-year="2020"

                                        data-can-future-date = "${element.canFutureDate}"
                                        data-dependent-variable-value-isblank="${element.isBlank}"
                                        data-variable-typename="${element.variableTypeName}">${element.selectedValues}</a>

                                        ${helpTextSpan}

                                        <span class ="field-validation-valid form-control-feedback" data-valmsg-for="inputField-${varIndex}" data-valmsg-replace="true"></span>
                                        </div></div></div>`;

            }
            else if (element.variableTypeName == "Heading") {
                try {
                    let startStr = element.question.substring(0, 3);

                    if (element.question.charAt(0) == "<") {
                        let endStr = element.question.substring(3, element.question.length);

                        startStr = startStr + " id='inputField-" + varIndex + "' data-variable-guid=" + element.formPageVariableId + " data-variable-guid-id=" + element.variableGuid + "  data-variable-name=" + element.variableName;

                        element.question = startStr + endStr;
                    } else {
                        element.question = "";
                    }
                } catch (e) {
                   
                }
                containerBody = '<div class="form-group mb-3"> <div class="row"><div class="col-sm-12">' +
                    element.question +
                    '</div>';
            }

            else if (element.variableTypeName == "Other Text") {

                if (element.question == "Insert other text")
                    element.question = "";

                containerBody = '<div class="form-group mb-3">    <div class="row"><div class="col-sm-12">' +
                    '<p id="inputField-' + varIndex + '" data-variable-guid="' + element.formPageVariableId + '"         data-variable-guid-id="' + element.variableGuid + '"     data-variable-name="' + element.variableName + '">' +
                    element.question +
                    '</p>' +
                    '</div></div></div>';
            }
            else if (element.variableTypeName == "FileType") {

                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                let thisFieldId = "#inputField-" + varIndex;
                let fileName = "";
                let fileDownloadPath = "";
                let thisFieldChangEvent = `onchange="App.setImage(this, '${thisFieldId}')"`;
                let fileTypeValidation = '';
                if (result.formTitle == App.DefaultFormNamesStringEnum.Person_Registration
                    || result.formTitle == App.DefaultFormNamesStringEnum.Participant_Registration
                    || result.formTitle == App.DefaultFormNamesStringEnum.Place__Group_Registration
                    || result.formTitle == App.DefaultFormNamesStringEnum.Project_Registration
                    || result.formTitle == App.DefaultFormNamesStringEnum.Project_Linkage
                ) {
                    fileTypeValidation = 'accept="image/gif,image/jpeg,image/png, image/jpg"';
                    thisFieldChangEvent = `onchange="App.setImage(this, '${thisFieldId}')"`;
                } else {
                    thisFieldChangEvent = `onchange="App.setFormVariableFile(this, '${thisFieldId}')"`;

                    fileName = App.getFileName(element.fileName);
                    fileDownloadPath = "";
                    if (element.selectedValues != "" && element.selectedValues != null) {
                        var d = App.remove_first_occurrence(element.selectedValues, "~")
                        fileDownloadPath = App.UploadsPathAPI + d;
                    }
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="file"' +
                    thisFieldChangEvent +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-name="' + element.variableName + '"' +
                    fileTypeValidation +
                    ' value="' + element.selectedValues + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-variable-username-guid="' + element.userLoginGuid + '"' +
                    ' data-variable-typename="' + element.variableTypeName + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-file-base="' + element.selectedValues + '"' +
                    '  />' +
                    '<a class="externalfilelink" href="' + fileDownloadPath + '" target="_blank">' + fileName + '</a> ' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableTypeName == "ColorPicker") {

                let roleIndex = element.variableRoleListMongo.findIndex(function (obj) { return obj.roleName === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex >= 0) {
                    if (element.variableRoleListMongo[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                } else if (roleIndex != 0) {
                    isFormDisabledCount++;
                    isDisabled = "disabled";
                    canEdit = "false";
                }

                if (element.selectedValues == "") {
                    element.selectedValues = "#1f88e5";
                    if (element.variableName == "ProjectDisplayNameTextColour") { element.selectedValues = "#ffffff" }
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.question + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="color"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-name="' + element.variableName + '"' +
                    ' value="' + element.selectedValues + '"' +
                    ' placeholder="' + element.question + '" ' +
                    ' data-variable-guid="' + element.variableId + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableRequiredMessage + '"' +
                    ' data-variable-username-guid="' + element.userLoginGuid + '"' +
                    ' data-dependent-variable-value-isblank="' + element.isBlank + '"' +
                    ' data-variable-typename="' + element.variableTypeName + '"' +
                    ' />' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }
            let bodyTemplate = '<div class="row pll-5 pr-5"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
            if (result.formTitle == "Participant Registration") {
                if (element.variableName == "MiddleName") {
                    bodyTemplate = '<div class="row pll-5 pr-5" id="divMiddName"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
                }
                else if (element.variableName == "NoMidNm") {
                    bodyTemplate = '<div class="col-md-12 col-sm-6 pt-2" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div>';
                }
                else {
                    bodyTemplate = '<div class="row pll-5 pr-5"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
                }
            }
            else if (result.formTitle == "Person Registration") {
                bodyTemplate = '<div class="row pll-5 pr-5"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
                try {
                    if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() != "1") {
                        $("#activityformVariableContainer").find("[data-variable-guid='42']").attr("disabled", "disabled").val("");
                        $("#activityformVariableContainer").find("[data-variable-guid='43']").attr("disabled", "disabled").val("");
                        $("#activityformVariableContainer").find("[data-variable-guid='51']").attr("disabled", "disabled").val("");
                    }
                } catch (e) {
                    
                }

                try {

                    $("#activityformVariableContainer").find("[data-variable-guid='41']").change(function () {
                        if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() == "1") {
                            $("#activityformVariableContainer").find("[data-variable-guid='42']").removeAttr("disabled");
                            $("#activityformVariableContainer").find("[data-variable-guid='43']").removeAttr("disabled");
                            $("#activityformVariableContainer").find("[data-variable-guid='51']").removeAttr("disabled");

                        } else {
                            $("#activityformVariableContainer").find("[data-variable-guid='42']").attr("disabled", "disabled").val("");
                            $("#activityformVariableContainer").find("[data-variable-guid='43']").attr("disabled", "disabled").val("");
                            $("#activityformVariableContainer").find("[data-variable-guid='51']").attr("disabled", "disabled").val("");

                        }
                    });
                } catch (e) {
                    
                }
            }
            else {
                bodyTemplate = '<div class="row pll-5 pr-5"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
            }

            container.append(bodyTemplate);
            container.append("<div class='dv-loading'><div>");


            if (result.formTitle == "Participant Registration" && element.variableName == "NoMidNm") { $("#divDependent-" + varIndex).appendTo("#divMiddName"); }

            if (element.variableTypeName == "Date") {
                let maxRange = 2119;
                let minRange = 1919;

                if (element.minRange != null && element.minRange != 0) {
                    minRange = element.minRange;
                }
                if (element.maxRange != null && element.maxRange != 0) {
                    maxRange = element.maxRange;
                }

                $('#inputField-' + varIndex).editable({
                    //mode: 'popup'
                    mode: 'inline',
                    maxDate: 0,// new Date(),
                    combodate: {
                        minYear: minRange,
                        maxYear: maxRange,
                    },
                });
            }
        });
        //parent variable display logic
        varIndex = 0;
        setTimeout(function () {
            result.variablesListMongo.forEach(function (element) {
                varIndex++;

                if (element.dependentVariableId != null && element.dependentVariableId != "") {
                    if (element.responseOption) {
                        try {
                            element.responseOption = element.responseOption.toLowerCase();
                        } catch (e) {

                        }
                    }
                    if (element.isBlank === true) {
                        element.responseOption = "";
                    }
                    if ($("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").prop('type') == "checkbox") {

                        if ($("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").prop('checked'))
                            $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val("1");
                        else
                            $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val("0")
                    }
                    $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").attr('onchange', 'Summary.setChildVariables(this)');
                    if (element.variableTypeName == "Other Text" || element.variableTypeName == "Heading") {
                        $("#frm-1 ").find("[data-variable-guid='" + element.formPageVariableId + "']").attr('data-parent-variable-id', element.dependentVariableId);
                        $("#frm-1 ").find("[data-variable-guid='" + element.formPageVariableId + "']").attr('data-parent-variable-response', element.responseOption);
                    } else {
                        $("#frm-1 ").find("[data-variable-guid='" + element.variableId + "']").attr('data-parent-variable-id', element.dependentVariableId);
                        $("#frm-1 ").find("[data-variable-guid='" + element.variableId + "']").attr('data-parent-variable-response', element.responseOption);
                    }
                    var elementdatatype = $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").attr("data-variable-typename");


                    var elementRespOtion = "";
                    if (elementdatatype == "Date") {
                        elementRespOtion = $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").text();
                    } else {
                        elementRespOtion = $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val();
                    }
                    var setDefault = true;
                    if (elementdatatype == "Numeric (Decimal)") {
                        elementRespOtion = element.responseOption;
                        let direction = elementRespOtion.charAt(0);
                        let cutPoint = parseFloat(elementRespOtion.substring(1, elementRespOtion.length));
                        let currentVal = parseFloat($("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val());
                        if (direction == "<") {
                            if (currentVal < cutPoint) {
                                $("#divDependent-" + varIndex).removeClass("hide");
                                $("#inputField-" + varIndex).attr("data-variable-required", "true");
                                setDefault = false;
                            }
                        }
                        else if (direction == ">") {
                            if (currentVal > cutPoint) {
                                $("#divDependent-" + varIndex).removeClass("hide");
                                $("#inputField-" + varIndex).attr("data-variable-required", "true");
                                setDefault = false;
                            }
                        } else if (direction == "" && element.isBlank && !isNaN(currentVal)) {
                            $("#divDependent-" + varIndex).addClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "true");
                            setDefault = false;
                        }
                    }
                    else if (elementdatatype == "Numeric (Integer)") {

                        elementRespOtion = element.responseOption;
                        let direction = elementRespOtion.charAt(0);
                        let cutPoint = parseInt(elementRespOtion.substring(1, elementRespOtion.length));
                        let currentVal = parseInt($("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val());
                        if (direction == "<") {
                            if (currentVal < cutPoint) {
                                $("#divDependent-" + varIndex).removeClass("hide");
                                $("#inputField-" + varIndex).attr("data-variable-required", "true");
                                setDefault = false;
                            }
                        }
                        else if (direction == ">") {
                            if (currentVal > cutPoint) {
                                $("#divDependent-" + varIndex).removeClass("hide");
                                $("#inputField-" + varIndex).attr("data-variable-required", "true");
                                setDefault = false;
                            }
                        }
                        else if (direction == "" && element.isBlank && !isNaN(currentVal)) {
                            $("#divDependent-" + varIndex).addClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "true");
                            setDefault = false;
                        }
                    }
                    else if (elementdatatype == "Dropdown") {
                        elementRespOtion = $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val();
                        if (elementRespOtion == element.responseOption) {
                            $("#divDependent-" + varIndex).removeClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "true");
                            setDefault = false;
                        }
                    }
                    else if (elementdatatype == "Checkbox") {
                        elementRespOtion = $("#frm-1 ").find("[data-variable-guid='" + element.dependentVariableId + "']").val();
                        if (elementRespOtion == element.responseOption) {
                            $("#divDependent-" + varIndex).removeClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "true");
                            setDefault = false;
                        }
                        else if (elementRespOtion != '0' && element.isBlank) {
                            $("#divDependent-" + varIndex).addClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "true");
                            setDefault = false;
                        }
                    }

                    else if (elementRespOtion == element.responseOption) {
                        $("#divDependent-" + varIndex).removeClass("hide");
                        $("#inputField-" + varIndex).attr("data-variable-required", "true");
                        setDefault = false;
                    }
                    if (setDefault) {

                        if (element.isBlank === true && (elementRespOtion == "" || elementRespOtion == 'Empty' || elementRespOtion == '0')) {
                            $("#divDependent-" + varIndex).removeClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "false");
                            element.responseOption = "";
                        } else {
                            $("#divDependent-" + varIndex).addClass("hide");
                            $("#inputField-" + varIndex).attr("data-variable-required", "false");
                        }
                        try {

                            if (element.isBlank) {

                                let isParent = false;
                                if (element.selectedValues == "") {
                                    let idd = element.variableId;
                                    do {
                                        isParent = false;
                                        let currentField = $("#frm-1").find("[data-variable-guid='" + idd + "']");
                                        let currentFieldChieldVariable = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
                                        if (currentFieldChieldVariable.length > 0) {

                                            $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");
                                            currentField = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
                                            if (currentField.length > 0) {
                                                isParent = true;
                                                idd = $(currentField).attr("data-variable-guid");
                                            }

                                        }
                                    } while (isParent) {
                                        isParent = false;
                                    }
                                } else {
                                    do {
                                        let currentField = $("#frm-1").find("[data-variable-guid='" + element.variableId + "']");
                                        let currentFieldChieldVariable = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
                                        if (currentFieldChieldVariable.length > 0) {
                                            isParent = false;
                                            $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().addClass("hide");
                                            currentField = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
                                        }
                                    } while (isParent) {
                                        isParent = true;
                                    }
                                }
                            }
                        } catch (eEEE) {
                            
                        }
                    }
                }
            });
        }, 200);
        let isFormDisabledButton = "";
        if (isFormDisabledCount > 0) {
            isFormDisabledButton = "data-fields-for-review-count=" + isFormDisabledCount;
        }

        if (SubmitButtonText == "Submit") {
            resultFooter.append($("<button type='button' id='btnUndoSubmit_ActivityformVariableContainer' class='hide btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.UndoSubmitActivityForm(this)\">Undo Submit</button>"));
            resultFooter.append($("<button type='button' id='btnSubmit_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.saveSummaryPageFormPopup(this)\">" + SubmitButtonText + "</button>"));
            resultFooter.append($("<button type='button' id='btnSave_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.saveSummaryPageFormPopup(this)\">Save</button>"));

            if (result.isDefaultForm === 1)
                resultFooter.append($("<button type='button' id='btnReset_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.ResetSummaryPageForm('" + result.formDataEntryId + "', this)\">Clear All</button>"));
        }
        else {
            resultFooter.append($("<button type='button' id='btnUndoSubmit_ActivityformVariableContainer' class='hide btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.UndoSubmitActivityForm(this)\">Undo Submit</button>"));
            resultFooter.append($("<button type='button' id='btnSubmit_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.saveSummaryPageFormPopup(this)\">" + SubmitButtonText + "</button>"));
            resultFooter.append($("<button type='button' id='btnSave_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.saveSummaryPageFormPopup(this)\">Save</button>"));

            if (result.isDefaultForm === 1)
                resultFooter.append($("<button type='button' id='btnReset_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Summary.ResetSummaryPageForm('" + result.formDataEntryId + "', this)\">Clear All</button>"));
        }

        resultFooter.append($("<button type=\"button\" class=\"btn btn-danger waves-effect waves-light\" data-dismiss=\"modal\">Cancel</button>"));



        let footerModifiedBy = "<input type='text' disabled class='form-control' value='" + result.modifiedBy + "'>";
        let footerModifiedDate = "<input type='text' disabled class='form-control' value='" + result.modifiedDate + "'>";
        var dotcolon = 'dotcolon';
        resultFooterModifiedby.append("Modified By <span class=" + dotcolon + ">:</span>");
        resultFooterModifiedby.append($(footerModifiedBy));
        resultFooterModifiedby.append("Modified Date: ");
        resultFooterModifiedby.append($(footerModifiedDate));
        $("#hdnFormDataEntryTypeIsNew").val(result.isNewForm);
        $('#activity-form-variables-model').modal({ show: true });
        $("input:radio[name=RadioButtonValue]:first").attr('checked', true);
        $('#search-for-tabular-data').css('max-width', 900);
        Summary.FormVariableContainer_onBlurEvent();
        Summary.executeEventFirst();
        $(".customdatepicker").on("click", function (dateElement) {
            let e1 = $(dateElement).prop("target");
            $('span[data-valmsg-for="' + $(e1).prop('id') + '"]').addClass("field-validation-error").text('');
            setTimeout(function () {
                $("form .editableform").on("submit", function () {
                    let canfuturedateElement = $(dateElement).prop("target");
                    let canfuturedate = $(canfuturedateElement).attr("data-can-future-date")
                    if (canfuturedate === "false") {
                        var currentDate = new Date();
                        var tempnewDate = $(canfuturedateElement).text();
                        let dateFormat = $(canfuturedateElement).attr("data-template");
                        let enteredDate = App.getDateBasedOnFormat(tempnewDate, dateFormat);
                        if (enteredDate == 'undefined') {
                            var from = tempnewDate.split("-")
                            enteredDate = new Date(from[2], from[1] - 1, from[0])
                        }
                        if (enteredDate > currentDate) {
                            $(canfuturedateElement).text("");

                            $('span[data-valmsg-for="' + $(canfuturedateElement).prop('id') + '"]').addClass("field-validation-error").text('Future dates are not allowed.');

                            $(canfuturedateElement).editable('setValue', null);
                        }
                        else {

                        }
                    }
                    if ($(canfuturedateElement).attr("onchange") != undefined) {
                        Summary.setChildVariables(canfuturedateElement);
                    }

                    Summary.FormVariableContainer_onBlurEvent_Each();
                });
            }, 300);
        });
        $("#activity-form-variables-model").find("[data-dismiss]").on("click", function () {
            try {
                Summary.UpdateURL_AfterDataEntry();
            } catch (errURL) {

            }
        });

    }, methodeType);
}

//===============================================================
//  open form in model  13-May-2019
//===============================================================
Summary.FormVariableContainer_onBlurEvent = function () {
    

    $('#activityformVariableContainer select, #activityformVariableContainer input:text, #activityformVariableContainer input:checkbox, #activityformVariableContainer a, #activityformVariableContainer textarea, #activityformVariableContainer input:file').on('blur change keyup', function () {

        if ($(this).prop('tagName').toLowerCase() == "a") {
            setTimeout(function () {
                Summary.FormVariableContainer_onBlurEvent_Each();
            }, 200);
        } else {
            Summary.FormVariableContainer_onBlurEvent_Each();
        }
    });
}


Summary.FormVariableContainer_onBlurEvent_Each = function () {
    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-info").removeClass("btn-warning").text("Submit");
    let frmTitle = $("#search-variable-result-model-title").text();

    $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer a:visible , #activityformVariableContainer textarea:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer input[type=color]:visible').each(function (index, element) {
        let _thisElementValue = $(element).val();
        if (_thisElementValue === null) { _thisElementValue = ""; }
        if ($(element).prop('type') == "checkbox") {
            _thisElementValue = $('input[name="' + $(element).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
        }
        if ($(element).prop('tagName').toLowerCase() == "a") {
          
            _thisElementValue = $(element).text();
            if (_thisElementValue == "Empty" || _thisElementValue == "Invalid date") {
                _thisElementValue = "";
            }
        }

        if ($(element).prop('type') == "select-one") {
            if (_thisElementValue != "")
                $(element).removeClass('summary-page-missing-data-error');
            else
                $(element).addClass('summary-page-missing-data-error');
        }

        if ($(element).prop('type') == "file") {
            _thisElementValue = $(element).attr("data-file-base");
        }

        if (frmTitle == "Participant Registration") {
            if ($(element).attr('data-variable-guid') == 17 || $(element).attr('data-variable-guid') == 18) {

                let chkBox = $("#activityformVariableContainer").find("[data-variable-guid='18']");
                let chkVal = $('input[name="' + $(chkBox).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
                let midName = $("#activityformVariableContainer").find("[data-variable-guid='17']").val();

                if (midName.length > 0 || chkVal.length > 0) {
                    _thisElementValue = "ok";
                }
            }
        }


        try {
            if (frmTitle == "Person Registration") {

                if ($(element).attr('data-variable-guid') == 42 || $(element).attr('data-variable-guid') == 43) {
                    if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() != "1") {
                        return;
                    }
                }
            }
        } catch (e) {
          
        }


        try {
            if ($(element).attr('data-variable-guid') == 38 && $("#search-variable-result-model-title").text() == "Participant Registration") {
                let isActive = $("#activityformVariableContainer").find("[data-variable-guid='42']");
                if ($(isActive).val() != "1") {
                    _thisElementValue = "email";
                }
            }
        } catch (e) {
         
        }

        if ($(element).prop('type') == "checkbox" && frmTitle != "Participant Registration") {
            return;
        }
        if (_thisElementValue == "") {
            if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
                $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
        }
    });
    if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin) {
        $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
    }
}

//===============================================================
//  manage form status
//===============================================================
Summary.executeEventFirst = function () {

    $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSave_ActivityformVariableContainer").addClass("hide");
    $("#btnReset_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").text("Submit");
    let isSubmitted = false;
    if (Summary.StatusId != "")
        if ($("#" + Summary.StatusId).text().trim().toLowerCase() == "submitted" || $("#" + Summary.StatusId).text().trim().toLowerCase() == "submitted for review")
            isSubmitted = true;
    setTimeout(function () {
        let frmTitle = $("#search-variable-result-model-title").text();
        let cntVisibleVar = 0;
        $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer input[type=color]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer textarea:visible, #activityformVariableContainer h2:visible, #activityformVariableContainer h3:visible, #activityformVariableContainer h4:visible, #activityformVariableContainer h5:visible, #activityformVariableContainer h6:visible, #activityformVariableContainer p:visible').each(function (index, element) {
           
            cntVisibleVar++;
            let _thisElementValue = $(element).val();
            let fieldtype = $(element).prop('type');
            let fieldTag = $(element).prop('tagName').toLowerCase();
            if ($(element).prop('type') == "checkbox") {
                _thisElementValue = $('input[name="' + $(element).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
            }
            if ($(element).prop('tagName').toLowerCase() == "a") {
                _thisElementValue = $(element).text();
                if (_thisElementValue == "Empty" || _thisElementValue == "Invalid date") {
                    _thisElementValue = "";
                }
            }
            if (fieldTag == "h2"
                || fieldTag == "h3"
                || fieldTag == "h4"
                || fieldTag == "h5"
                || fieldTag == "h6"
                || fieldTag == "p"
            ) {
                _thisElementValue = fieldTag;
            }
            if ($(element).prop('type') == "select-one" && _thisElementValue == "") {
                $(element).addClass('summary-page-missing-data-error');
            }
            if (frmTitle == "Participant Registration") {
                if ($(element).attr('data-variable-guid') == 17 || $(element).attr('data-variable-guid') == 18) {

                    let chkBox = $("#activityformVariableContainer").find("[data-variable-guid='18']");
                    let chkVal = $('input[name="' + $(chkBox).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
                    let midName = $("#activityformVariableContainer").find("[data-variable-guid='17']").val();

                    if (midName.length > 0 || chkVal.length > 0) {
                        _thisElementValue = "ok";
                    }
                }
            }
            if ($(element).prop('type') == "checkbox" && frmTitle != "Participant Registration") {
                if (isSubmitted) {
                    $(element).addClass("disabled-button-cursor").attr("disabled", true);
                }
                return;
            }
            try {
                if (frmTitle == "Person Registration") {

                    if ($(element).attr('data-variable-guid') == 42 || $(element).attr('data-variable-guid') == 43) {
                        if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() != "1") {
                            return;
                        }
                    }
                }
            } catch (e) {
               
            }
            try {
                if ($(element).attr('data-variable-guid') == 38 && $("#search-variable-result-model-title").text() == "Participant Registration") {
                    let isActive = $("#activityformVariableContainer").find("[data-variable-guid='42']");
                    if ($(isActive).val() != "1") {
                        _thisElementValue = "email";
                    }
                }
            } catch (e) {
            
            }
            if (Summary.TotalFormVariableCount > $("#activityformVariableContainer").find(".dynamic-variables").length) {
                _thisElementValue = "";
            }
            if (_thisElementValue == "") {
                if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
                    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
            }
            if (isSubmitted) {

                $(element).attr("disabled", true);
                $(element).addClass("disabled-button-cursor");

                try {
                    if (fieldTag.toLowerCase() == "a") {
                        if (!$(element).hasClass("externalfilelink")) {
                            $(element).css("pointer-events", "none");
                            $(element).attr("readonly", "readonly");
                        } else {
                            $(element).removeAttr("disabled");
                            $(element).removeClass("disabled-button-cursor");
                        }
                    }
                } catch (e) {
               
                }

                
                if (fieldtype == "file") {

                }
                if (fieldtype == "color") {
                }
            }
        });

        if (cntVisibleVar === 0) {
            $("#activityformVariableContainer").append("");
            $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSave_ActivityformVariableContainer").addClass("hide");
            $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
        }
        if (isSubmitted) {
            $("#btnUndoSubmit_ActivityformVariableContainer").removeClass("hide");
            $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSave_ActivityformVariableContainer").addClass("hide");
            $("#btnReset_ActivityformVariableContainer").addClass("hide");
        }
        else {

            $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSubmit_ActivityformVariableContainer").removeClass("hide");
            $("#btnSave_ActivityformVariableContainer").removeClass("hide");
            if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin) {
                $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
            }
        }
    }, 300);
}

//===============================================================
//  manage form status
//===============================================================
Summary.UndoSubmitActivityForm = function () {
    $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").removeClass("hide");

    $("#btnReset_ActivityformVariableContainer").removeClass("hide");

    $("#btnSave_ActivityformVariableContainer").removeClass("hide");
    let frmTitle = $("#search-variable-result-model-title").text();
    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-info").removeClass("btn-warning").text("Submit");
    if (($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
        && (frmTitle == App.DefaultFormNamesStringEnum.Person_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Participant_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Place__Group_Registration)
    ) {
        try {
            if (typeof $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.SysAppr + "']") !== "undefined") {
                let sysApprVariable = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.SysAppr + "']").val();
                if (sysApprVariable == "1") {
                    $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.SysAppr + "']").val("0");
                    $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.AuthenticationMethod + "']").val("").attr("disabled", true);
                    $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Active + "']").val("");
                    $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.SysRole + "']").val("");
                }
            }
        } catch (eUndoSubmit) {
           
        }
    }


    $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer input[type=color]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer textarea:visible').each(function (index, element) {
        let _thisElementValue = "";

        _thisElementValue = $(element).val();
        if ($(element).prop('type') == "checkbox") {

        }

        if ($(element).prop('type') == "select-one") {
            _thisElementValue = $(element).val();
            if (_thisElementValue != "")
                $(element).removeClass('summary-page-missing-data-error');
            else
                $(element).addClass('summary-page-missing-data-error');
        }
        if ($(element).prop('type') == "file") {
            _thisElementValue = $(element).attr("data-file-base");
        }
        if ($(element).attr("data-variable-guid") == "3" || $(element).attr("data-variable-guid") == "4" || $(element).attr("data-variable-guid") == "52") {
            return;
        }
        if ($(element).attr("data-can-edit") == "false") {
            return;
        }

        if ($(element).prop('tagName').toLowerCase() == "a") {
            try {
                $(element).css("pointer-events", "auto");
                $(element).removeAttr("readonly");
            } catch (e) {
              
            }            
            _thisElementValue = $(element).text();
            if (_thisElementValue == "Empty" || _thisElementValue == "Invalid date") {
                _thisElementValue = "";
            }
        }

        try {
            if (frmTitle == "Person Registration") {

                if ($(element).attr('data-variable-guid') == 42 || $(element).attr('data-variable-guid') == 43) {

                    if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() != "1") {
                        return;
                    }
                }
            } else if (frmTitle == "Participant Registration") {

                if ($(element).attr('data-variable-guid') == 17 && $(element).val() == "") {
                    let noMidName = $("#activityformVariableContainer").find("[data-variable-guid='18']");

                    _thisElementValue = $('input[name="' + $(noMidName).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
                }
                try {
                    if ($(element).attr('data-variable-guid') == 38 && $("#search-variable-result-model-title").text() == "Participant Registration") {
                        let isActive = $("#activityformVariableContainer").find("[data-variable-guid='42']");
                        if ($(isActive).val() != "1") {
                            _thisElementValue = "email";
                        }
                    }
                } catch (e) {
                    
                }

            }
        } catch (e) {
          
        }
        if (($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
            && ($(element).attr("data-variable-name") == App.DefaultVariablesStringEnum.AuthenticationMethod)
            && (frmTitle == App.DefaultFormNamesStringEnum.Person_Registration
                || frmTitle == App.DefaultFormNamesStringEnum.Participant_Registration
                || frmTitle == App.DefaultFormNamesStringEnum.Place__Group_Registration)
        ) {
            try {
                $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.AuthenticationMethod + "']").attr("disabled", true);
                return;
            }
            catch (eUndoSubmit) {
              
            }
        }

        try {

            if ($(element).attr("data-variable-name") == App.DefaultVariablesStringEnum.AuthenticationMethod) {
                if ($("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.SysAppr + "']").val() != 1) {
                    return;
                }
            }
        } catch (eAtu) {
          
        }


        if (_thisElementValue == "") {
            if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin)
                $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
        }
        $(element).removeAttr("disabled");
        $(element).removeClass("disabled-button-cursor");
    });


    if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin) {
        $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
    }
}

// ============================================================== 
// Save summary page form //13-May-2019
// ==============================================================
Summary.saveSummaryPageFormPopup = function (e, status, isSubmitClicked) {

    if (e == "eConfirmBtn") {
        e = $("#btnSubmit_ActivityformVariableContainer");
    }
    let frmTitle = $("#search-variable-result-model-title").text();

    if ($(e).text() == "Submit") {
        status = 4;
    }
    if ($(e).text() == "Submit for Review") {
        status = 5;
    }
    if (status == 4) {

        let disabledFieldCount = $(e).attr("data-fields-for-review-count");
        try {
            let disabledFieldCount_int = parseInt(disabledFieldCount);
            if (disabledFieldCount_int > 0) { status = 5; }
        } catch (e) {  }


        //====================[ASPMONASH-312]=======================
        if ($("#UserProjectRole").val() == App.DefaultRoleNameStringEnum.System_Admin) {

            if (!isSubmitClicked) {
                if (!Summary.isValidDataEntryForm()) {
                    $("#confirm-submit-form-data-entry").modal("show");
                    $("#butConfirmSubmitFormDataEntry").attr("onclick", "Summary.saveSummaryPageFormPopup('eConfirmBtn', '" + status + "', 'true' )");
                    return false;
                }
            }
        } else {
            if (!Summary.isValidDataEntryForm()) {
                return false;
            }
        }

        $("#butConfirmSubmitFormDataEntry").removeAttr("onclick");
        Summary.isSubmitClicked = false;
        //====================[ASPMONASH-312]=======================




        //====================[ASPMONASH-429]=======================

        if (frmTitle == App.DefaultFormNamesStringEnum.Person_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Participant_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Place__Group_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Project_Registration
        ) {
            if ($("#UserProjectRole").val() != App.DefaultRoleNameStringEnum.System_Admin) {
                status = 5;
            }
        }
        //====================[ASPMONASH-429]=======================


    }


    let currentForm = $(e).parents('form:first');
    let currentFormId = currentForm.prop('id');
    if (frmTitle === "Project Linkage") {
        if (!Summary.validateProjectLinkageForm(e)) {
            return false;
        }
    }
    var success = true;
    let isEdit = true;
    if (!$("#hdnFormDataEntryGuid").val()) {
        isEdit = false;
    }

    let url = isEdit == false ? "Search/SaveEntities" : "Search/EditEntities/" + $("#hdnFormDataEntryGuid").val();
    if (App.IsTestSite === true) {
        url = isEdit == false ? "Review/SaveEntities" : "Review/EditEntities/" + $("#hdnFormDataEntryGuid").val();
    }
    let methodeType = isEdit ? "PUT" : "POST"

    let sendData = {};
    sendData.FormDataEntryVariable = [];

    sendData.ParticipantId = $("#PersonEntityId").val();
    if (!isEdit) {
        sendData.ParticipantId = $("#PersonEntityId").val();
        sendData.ParentEntityNumber = $("#PersonEntityId").val();
    }

    let previousChk = "";

    let isMiddleName = false;

    $('#' + currentFormId + ' .dynamic-variables').each(function () {

        if ($("#search-variable-result-model-title").text() == "Project Linkage" && $(this).attr("data-variable-name") === App.DefaultVariablesStringEnum.LnkPro) {//Linked Project
            let linkedProjectId = $(this).val();
            if (!linkedProjectId) {
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text($(this).attr("data-variable-required-message"));
                success = false;
                return false;
            }
            else {

                if (!isEdit) {
                    let checkEntityLinkedProject = Summary.checkEntityLinkedProject(linkedProjectId, $("#PersonEntityId").val());
                    if (checkEntityLinkedProject != null) {
                        $(this).addClass("input-validation-error");
                        $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text("This project is already linked with entity number: " + $("#PersonEntityId").val() + ".");
                        success = false;
                        return false;
                    }
                }
            }
        }
        let isRequired = $(this).attr('data-variable-required');

        let fieldtype = $(this).prop('type');
        let fieldTag = $(this).prop('tagName').toLowerCase();
        let currentElementValue = "";

        if (fieldtype == "checkbox") {
            currentElementValue = $('input[name="' + $(this).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
        }
        else if (fieldtype == "radio") {
            currentElementValue = $('input[name="' + $(this).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
        }
        else if (fieldtype == "" && fieldTag == "a") {
            currentElementValue = $(this).text();
            if (currentElementValue == "Empty") {
                currentElementValue = "";
            }
        }
        else if (fieldtype == "file") {
            currentElementValue = $(this).attr("data-file-base");
        }
        else {
            currentElementValue = $(this).val();
        }
        $(this).removeClass("input-validation-error");
        $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');

        if ($(this).attr('data-variable-guid') == 17 && currentElementValue.length > 0) {
            isMiddleName = true;
        }
        try {
            if (frmTitle == "Person Registration") {
                if ($(this).attr('data-variable-guid') == 42 || $(this).attr('data-variable-guid') == 43) {
                    if ($("#activityformVariableContainer").find("[data-variable-guid='41']").val() != "1") {
                        return;
                    }
                }
            }
            else if (frmTitle == "Participant Registration") {
                if ($(this).attr('data-variable-guid') == 38) {
                    if ($("#activityformVariableContainer").find("[data-variable-guid='42']").val() != "1") {
                        if ($(this).val() == "") {
                            return;
                        }
                    }
                }
            }
        } catch (e) {            
        }
        let chck = false;
        if (status == 4) {

        }


        if ($(this).attr("data-valid") == "false" && currentElementValue.length > 0) {

            $(this).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');
            chck = true;
            try {
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text($(this).attr('onBlur').split(",")[$(this).attr('onBlur').split(",").length - 1].replace('[', '').replace(']', '').replace(')', '').replace(/"/g, ''));
            } catch (err) {
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text("Invalid data");
               
            }
        }


        if ($(this).attr('data-variable-guid') == 18 && isMiddleName == true) {
            chck = false;
        }
        let isDuplicate = false;
        if ($(this).attr('data-variable-guid') == 40 && chck == false) {

            var userguid = $(this).attr('data-variable-username-guid');

            var authType = $('[data-variable-guid="51"]').val();
            var email = $('[data-variable-guid="38"]').val();

            if (authType.length > 0) {
            }
        }
        if (chck == true && $(this).is(":visible")) {
            success = false;
            $(this).addClass("input-validation-error");
            if ($('span[data-valmsg-for="' + $(this).prop('id') + '"]').text() == "") {
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('This field is required');
            }

            if (isDuplicate) {
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('This username and authentication method already exists');
            }
            return false;
        }


        let chkValue = "";

        let variableTypename = $(this).attr("data-variable-typename");

        if (fieldtype == "checkbox") {
            if (previousChk != $(this).prop('id')) {
                previousChk = $(this).prop('id');
                chkValue = $('input[name="' + $(this).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-guid'),
                    "selectedValues": chkValue,
                    "VariableTypeName": variableTypename,
                });
            }
        }
        else if (fieldtype == "radio") {
            if (previousChk != $(this).prop('id')) {
                previousChk = $(this).prop('id');
                chkValue = $('input[name="' + $(this).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');

                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-guid'),
                    "selectedValues": chkValue,
                    "VariableTypeName": variableTypename,
                });
            }
        }
        else if (fieldtype == "" && fieldTag == "a") {

            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).text(),
                "VariableTypeName": variableTypename,
            });
        }
        else if (fieldtype == "file") {

            let fileName = $(this).val();
            try {
                fileName = $(this).val().split('\\').pop();

                if (fileName == "") {
                    fileName = $(this).parent().find(".externalfilelink").text();
                }

            } catch (efname) {

            }

            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).attr("data-file-base"),
                "VariableTypeName": variableTypename,
                "FileName": fileName,
            });
        }
        else {
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).val(),
                "VariableTypeName": variableTypename,
            });
        }
    });

    if (frmTitle == "Project Registration") {

        try {
            let nmVal = $("#activityformVariableContainer").find("[data-variable-guid='15']").val();
            if (typeof nmVal === 'undefined') {
                nmVal = "";
            } else if (nmVal == null) {
                nmVal = "";
            }
            if (nmVal.trim() == "") {
                let nm = $("#activityformVariableContainer").find("[data-variable-guid='15']");
                $(nm).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(nm).prop('id') + '"]').addClass("field-validation-error").text('Please enter missing data.');
                return false;
            }
        } catch (e) {
           
        }


        let projStartDate = "";
        let projEndDate = "";
        try {
            projStartDate = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.RecruitStart + "']").text();
            projEndDate = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.RecruitEnd + "']").text();
        } catch (e) { }
        try {
            projStartDate = projStartDate != "" ? App.convertStringToDate(projStartDate) : "";
        } catch (e) {
            
        }
        try {
            projEndDate = projEndDate != "" ? App.convertStringToDate(projEndDate) : "";
        } catch (e) {
       
        }
        if (projStartDate != "" && projEndDate != "") {
            if (projStartDate > projEndDate) {

                let sdate = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.RecruitStart + "']");
                $(sdate).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(sdate).prop('id') + '"]').addClass("field-validation-error").text('Recruitment Start Date must be before the Recruitment End Date.');
                return false;
            }
        }
    }

    const found = sendData.FormDataEntryVariable.some(function (el) {
        return el.variableId == 3;
    });
    if (!found) sendData.FormDataEntryVariable.push({ "variableId": 3, "selectedValues": "" });
   


    if (success) {
        if (frmTitle == "Place/Group Registration") {
            var EntityType = $('[data-variable-guid=5]').find(":selected").text();
            if (EntityType == "API") {
                var email = $('[data-variable-guid="38"]').val();
                var authType = $('[data-variable-guid="51"]').val();
                var userName = $('[data-variable-guid="40"]').val();
                var userid = $('[data-variable-guid="40"]').attr('data-variable-username-guid');
                let chkUsername = false;
                if (userName != "" && authType != "" && userid != "") {
                    chkUsername = Summary.checkUserNameExistence(userName, authType, userid);

                    if (chkUsername == false) {
                        success = false;
                        let txtbx = $('span[data-username-msg-for="spn-Username"]').attr("data-valmsg-for");
                        $("#" + txtbx).addClass("input-validation-error");
                        $('span[data-username-msg-for="spn-Username"]').addClass("field-validation-error").text('Username and auth type already exist');
                    } else {
                        let txtbx = $('span[data-username-msg-for="spn-Username"]').attr("data-valmsg-for");
                        $("#" + txtbx).removeClass("input-validation-error");
                        $('span[data-username-msg-for="spn-Username"]').addClass("field-validation-error").text('');
                    }
                }
            }
        }

        if (frmTitle == "Person Registration" || frmTitle == "Participant Registration") {
            var email = $('[data-variable-guid="38"]').val();
            var authType = $('[data-variable-guid="51"]').val();
            var userName = $('[data-variable-guid="40"]').val();
            var userid = $('[data-variable-guid="40"]').attr('data-variable-username-guid');
            let chkUsername = false;
            if (userName != "" && authType != "" && userid != "") {
                chkUsername = Summary.checkUserNameExistence(userName, authType, userid);

                if (chkUsername == false) {
                    success = false;
                    let txtbx = $('span[data-username-msg-for="spn-Username"]').attr("data-valmsg-for");
                    $("#" + txtbx).addClass("input-validation-error");
                    $('span[data-username-msg-for="spn-Username"]').addClass("field-validation-error").text('Username and auth type already exist');
                } else {
                    let txtbx = $('span[data-username-msg-for="spn-Username"]').attr("data-valmsg-for");
                    $("#" + txtbx).removeClass("input-validation-error");
                    $('span[data-username-msg-for="spn-Username"]').addClass("field-validation-error").text('');
                }
            }
        }
    }
    if (success) {
        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = $("#" + currentFormId).attr('data-form-guid');
        sendData.ActivityId = $("#ActivityId").attr('data-activity-guid');
        sendData.Status = status;
        sendData.FormTitle = $("#search-variable-result-model-title").text();

        sendData.SummaryPageActivityObjId = $("#" + currentFormId).attr('data-summarypageactivity');

        App.postData(App.ApiUrl + url, sendData, function (result) {
            try {
                Summary.UpdateURL_AfterDataEntry();
            } catch (errURL) {
                
            }
            Summary.UpdateLeftPanel(sendData.FormTitle);
            if (sendData.FormTitle == "Project Registration") {
                if (sendData.ProjectId == result.projectId) {
                    let clr = $("#" + currentFormId + " :input[type=color]").val();
                    $(".topbar.theme-bgcolor.fixed-top").css({ "background": clr });

                    let clrFont = $("#" + currentFormId).find("[data-variable-name='" + App.DefaultVariablesStringEnum.ProjectDisplayNameTextColour + "']").val();
                    let clrDisplayName = $("#" + currentFormId).find("[data-variable-name='" + App.DefaultVariablesStringEnum.ProjectDisplayName + "']").val();

                    if (clrDisplayName != "") {
                        $(".pjt-name").html(clrDisplayName);
                    }
                    if (clrFont != "") {
                        $(".pjt-name").css({ "color": clrFont });
                    }
                    let fil = $("#" + currentFormId + " :input[type=file]").attr("data-file-base");

                    if (fil != "") {
                        $(".header-top-left").find(".headerLogo").remove();
                        $(".header-top-left").append('<div class="headerLogo"><a class="navbar-brand" href="/Home/ListOfProject"><img src="' + fil + '" style="height:65px;" /></a></div>');
                    }
                }

                let fil = $("#" + currentFormId + " :input[type=file]").attr("data-file-base");

                if (fil != "") {
                    let cstmStyl = "";
                    var i = new Image();
                    i.src = fil;
                    i.onload = function () {
                        if (i.width > i.height) {
                            cstmStyl = "height: 170px!important; width:auto";
                        }
                        else if (i.width < i.height) {
                            cstmStyl = "width:170px!important;height:auto;";
                        } else {
                            cstmStyl = "height: 170px!important; width:auto";
                        }
                        let imgTag = "<img src='" + fil + "' style='" + cstmStyl + "'/>";
                        let spnTag = "<span class='span_name_full' id='lftCrdspnEntityUserName'>" + $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']").val() + "</span>";

                        $(".content-circle").empty();
                        if ($(".content-circle").parent(":first.card-body").find("#lftCrdspnEntityUserName").length > 0) {
                            $(".content-circle").parent(":first.card-body").find("#lftCrdspnEntityUserName").remove();
                        }
                        $(".content-circle").after(spnTag);
                        $(".content-circle").append(imgTag);
                    };
                } else {

                    let spnTag = "<span class='span_name_full' id='lftCrdspnEntityUserName'>" + $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']").val() + "</span>";

                    $(".content-circle").empty();
                    if ($(".content-circle").parent(":last.card-body").find("#lftCrdspnEntityUserName").length > 0) {
                        $(".content-circle").parent(":last.card-body").find("#lftCrdspnEntityUserName").remove();
                    }
                    $(".content-circle").append(spnTag);
                }
            }

            if (sendData.FormTitle == "Project Linkage") {
                let frmLinked = $("#frm-1").find("[data-variable-guid='52']").find('option:selected').text();

                if ($("#" + Summary.StatusId).parent().parent().parent().parent().find(".timeline-title").find("small").length > 0) {
                    $("#" + Summary.StatusId).parent().parent().parent().parent().find(".timeline-title").find("small").text("(" + frmLinked + ")");
                } else {
                    $("#" + Summary.StatusId).parent().parent().parent().parent().find(".timeline-title").append(" <small>(" + frmLinked + ")</small>");
                }
            }

            if (sendData.FormTitle == "Person Registration" || sendData.FormTitle == "Project Linkage" || sendData.FormTitle == "Place/Group Registration") {
                Summary.ReloadSummaryPageDropdown($("#CurrentProjectId").val(), sendData.ParticipantId);
            } else {
                if (Summary.IsLeftPanelVariablesContains) {
                    let updatePanelUrl = "MongoDB_Summary/UpdateLeftPanelSummaryPage/" + $("#CurrentProjectId").val() + "/" + sendData.ParticipantId;
                    if (App.IsTestSite === true) {
                        updatePanelUrl = "Test/MongoDB_Summary/UpdateLeftPanelSummaryPage/" + $("#CurrentProjectId").val() + "/" + sendData.ParticipantId;
                    }
                    App.postData(App.ApiUrl + updatePanelUrl, {}, function (leftPanelResult) {

                        if (leftPanelResult != null)
                            Summary.UpdateLeftPanelDefaultVariables(leftPanelResult);
                    }, "Get");
                }
            }

            if (sendData.Status === 4) {
                $('#activity-form-variables-model').modal('hide');
                $('#' + Summary.StatusId).text('Submitted');
                $('#' + Summary.StatusId).removeClass('text-warning text-submit-for-review').addClass('text-success');
                Summary.StatusId = "";
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " submitted successfully.");
            }
            else if (sendData.Status === 5) {//(int)Core.Enum.FormStatusType.Submit_for_Review
                $('#activity-form-variables-model').modal('hide');
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " submitted for review successfully.");
                $('#' + Summary.StatusId).text('Submitted for Review');
                $('#' + Summary.StatusId).removeClass('text-success text-warning').addClass('text-submit-for-review');
                Summary.StatusId = "";
            }
            else {
                $('#activity-form-variables-model').modal('hide');
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " saved successfully.");
                $('#' + Summary.StatusId).text('Draft');
                $('#' + Summary.StatusId).removeClass('text-success text-submit-for-review').addClass('text-warning');
                Summary.StatusId = "";
            }
        }, methodeType);
    }
    else {
        App.showError("Please fill all mandatory fields.", '.role-model-error');
    }
}

Summary.EditSummaryPageActivity = function (_this) {

    $("#formType").removeClass("form-badge").addClass("form-badge-edit");
    $("#formType").find(".text-uppercase").text("Edit");

    $("#ActivityType").val($(_this).attr("data-editactivity-activityid"));
    $("#EditedActivityType").val($(_this).attr("data-editactivity-activityid"));

    let actDisplayName = $(_this).parents('.row:first').find("h4.timeline-title").attr("data-activity-display-name");
    $("#ActivityTypeNameDisplayOnly").val(actDisplayName);

    $("#CompletedByUser").val($(_this).attr("data-editactivity-completedby"));


    let dd = $(_this).attr("data-editactivity-date").split("-");
    let d = dd[2];
    let m = dd[1];
    let y = dd[0];
    try {
        $("#ActivityDate").editable('setValue', null);
        $("#ActivityDate").editable('setValue', y + "-" + m + "-" + d);

    } catch (e) {

    }

    $("#ActivityDate").attr("data-value", $(_this).attr("data-editactivity-date"));
    $("#ActivityDate").text(d + "/" + m + "/" + y);
    $("#btnAddEditSummaryPageActivity").addClass("rounded-circle");
    $("#btnAddEditSummaryPageActivity").find(".fa-plus").removeClass("fa-plus").addClass("fa-check");
    $("#ActivityType").addClass("hide").attr("disabled", "disabled");
    $("#ActivityTypeNameDisplayOnly").removeClass("hide").attr("disabled", "disabled");
    let summarypageActId = $(_this).attr("data-editactivity-summarypage-activityid");
    $("#btnAddEditSummaryPageActivity").attr("onclick", "Summary.EditSummaryPageActivityPost('" + summarypageActId + "' )");
}

Summary.EditSummaryPageActivityPost = function (_id) {

    $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('');
    $('#ActivityType').removeClass("input-validation-error");
    $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('');
    $('#CompletedByUser').removeClass("input-validation-error");
    $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('');
    $('#ActivityDate').removeClass("input-validation-error");

    var aDate = $("#ActivityDate").text().split("/");
    var dd = aDate[0];
    var mm = aDate[1];
    var yy = aDate[2];

    if (dd == "Empty") {
        $('span[data-valmsg-for="ActivityDate"]').addClass("field-validation-error").text('This field is required.');
        $('#ActivityDate').addClass("input-validation-error");
        return false;
    }

    if (dd > 31) { App.showError("Please enter a valid date."); return false; }
    if (mm > 12) { App.showError("Please enter a valid date."); return false; }

    var activityDate = mm + "-" + dd + "-" + yy;//new Date(mm + "-" + dd + "-" + yy);

    let entityCreatedDateArray = $("#EntityCreatedDate").val().split("-");
    var crntActivityName = $("#ActivityTypeNameDisplayOnly").val();
    var actNameLst = [
        App.DefaultFormNamesStringEnum.Person_Registration
        , App.DefaultFormNamesStringEnum.Participant_Registration
        , App.DefaultFormNamesStringEnum.Place__Group_Registration
        , App.DefaultFormNamesStringEnum.Project_Registration
    ];
    var isDefaultAct = actNameLst.includes(crntActivityName);
    if (isDefaultAct === false) {
        if (new Date(yy + "-" + mm + "-" + dd) < new Date(entityCreatedDateArray[2] + "-" + entityCreatedDateArray[0] + "-" + entityCreatedDateArray[1])) {
            App.showError("Activity date should greater than entity registration date.");
            return false;
        }
    }
    if ($("#EditedActivityType").val().length < 1) {
        $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('This field is required.');
        $('#ActivityType').addClass("input-validation-error");
        return false;
    }
    if ($("#CompletedByUser").val().length < 1) {
        $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('This field is required.');
        $('#CompletedByUser').addClass("input-validation-error");
        return false;
    }

    var sendData = {
        "Id": _id,
        "ActivityGuid": $("#EditedActivityType").val(),
        "ActivityCompletedByGuid": $("#CompletedByUser").val(),
        "ActivityDate": activityDate,
        "IsActivityAdded": true,
        "ProjectGuid": $("#CurrentProjectId").val(),//$("#ProjectId").val(),
        "PersonEntityId": $("#PersonEntityId").val(),
    };

    let url = "MongoDB_Summary/EditSummaryPageActivity/" + _id;
    if (App.IsTestSite === true) {
        url = "Review/EditSummaryPageActivity/" + _id;
    }
    let methodeType = "PUT";
  

    App.postData(App.ApiUrl + url, sendData, function (result) {

        try {
            Summary.ReloadSummaryPageDropdown($("#CurrentProjectId").val(), sendData.PersonEntityId);
        } catch (e) {            
        }

        App.showSuccess("Activity updated successfully.");
        let activityDate = new Date(result.activityDate);
        let dd = activityDate.getDate();
        let mmm = activityDate.getMonth();
        var month_names_array = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

        $("#datemonth-" + _id).html(dd + "<br>" + month_names_array[mmm]);
        $("#completedbyname-" + _id).text("Staff: " + result.activityCompletedByName);
        $("#formType").removeClass("form-badge-edit").addClass("form-badge");
        $("#formType").find(".text-uppercase").text("NEW");

        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-activityid", result.activityGuid);
        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-completedby", result.activityCompletedByGuid);
        let mnth = mmm + 1;
        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-date", activityDate.getFullYear() + "-" + mnth + "-" + dd);
        $("#btnAddEditSummaryPageActivity").removeClass("rounded-circle");
        $("#btnAddEditSummaryPageActivity").find(".fa-check").removeClass("fa-check").addClass("fa-plus");
        $("#btnAddEditSummaryPageActivity").attr("onclick", "Summary.AddSummaryPageActivity()");
        $('#ActivityType').prop('selectedIndex', 0);
        $('#CompletedByUser').prop('selectedIndex', 0);
        $('#EditedActivityType').val("");
        $("#ActivityType").removeClass("hide").removeAttr("disabled");
        $("#ActivityTypeNameDisplayOnly").addClass("hide").attr("disabled", "disabled");

    }, methodeType);
}

Summary.DeleteSummaryPageActivity = function (id, name) {

    $(".activity-delete-confirm-message").html("Are you sure want to remove " + name + "  from summary page?");
    $("#summaryactivity-delete-confirm").modal("show");

    $("#butDeleteConfirmed").attr("onclick", "Summary.DeleteSummaryPageActivityPost('" + id + "' )");
}
Summary.DeleteSummaryPageActivityPost = function (_id) {

    let url = "MongoDB_Summary/DeleteSummaryPageActivity/" + _id;
    if (App.IsTestSite === true) {
        url = "Review/DeleteSummaryPageActivity/" + _id;
    }
    let methodeType = "Delete";
    App.postData(App.ApiUrl + url, {}, function (result) {       
        $("#butDeleteConfirmed").removeAttr("onclick");
        $("#activityContainer-" + _id).remove();
        try {
            Summary.ReloadSummaryPageDropdown($("#CurrentProjectId").val(), result.personEntityId);
        } catch (e) {            
        }

        App.showSuccess("Activity removed form summary page successfully.");
    }, methodeType);
}

//===============================================================
//  check project linkage
//===============================================================
Summary.checkEntityLinkedProject = function (projectId, entityId) {

    let isAsync = false;
    let response = null;

    let url = "ProjectDeploy/CheckEntityLinkedProject/" + projectId + "/" + entityId;
    if (App.IsTestSite === true) {
        url = "Review/CheckEntityLinkedProject/" + projectId + "/" + entityId;
    }
    App.postData(App.ApiUrl + url, {}, function (result) {

        response = result;
    }, "Get", undefined, isAsync);

    return response;
}

// ============================================================== 
// summary page popup form validation   //06-Aug-2019
// ==============================================================
Summary.isValidDataEntryForm = function (e, isRequired) {
    let isValid = true;
    $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer textarea:visible').each(function (index, element) {
        if (!$(element).prop('disabled')) {
            //remove old alert
            $(element).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(element).prop('id') + '"]').addClass("field-validation-error").text('');

            let _thisElementValue = $(element).val();

            let fieldtype = $(element).prop('type');
            let fieldTag = $(element).prop('tagName').toLowerCase();

            if (fieldtype == "file") {
                _thisElementValue = $(element).attr("data-file-base");
            }
            if ($(element).prop('tagName').toLowerCase() == "a") {

                _thisElementValue = $(element).text();
                if (_thisElementValue == "Empty" || _thisElementValue == "Invalid date") {
                    _thisElementValue = "";
                }

                if ($(element).hasClass("externalfilelink")) {
                    _thisElementValue = "externalfilelink";
                }
            }

            if ($(element).prop('tagName').toLowerCase() == "a" && $("#search-variable-result-model-title").text() == "Project Registration") {
                if ($(element).attr('data-variable-guid') != 50 && $("#search-variable-result-model-title").text() == "Project Registration") {
                    _thisElementValue = "Empty";
                }
            }

            if ($(element).attr('data-variable-guid') == 17 && $("#search-variable-result-model-title").text() == "Participant Registration") {
                let noMiddleNameCheckbox = $("#activityformVariableContainer").find("[data-variable-guid='18']");
                if ($(noMiddleNameCheckbox).prop('checked')) {
                    _thisElementValue = "MiddleNameExist";
                }
            }

            if ($(element).attr('data-variable-guid') == 38 && $("#search-variable-result-model-title").text() == "Participant Registration") {
                let isActive = $("#activityformVariableContainer").find("[data-variable-guid='42']");
                if ($(isActive).val() != "1") {
                    _thisElementValue = "MiddleNameExist";
                }
            }

            if (_thisElementValue == "") {
                $(element).addClass("input-validation-error");
                let errorText = $(element).attr("data-variable-required-message");
                if (errorText == "null")
                    errorText = "Please enter missing data.";

                $('span[data-valmsg-for="' + $(element).prop('id') + '"]').addClass("field-validation-error").text(errorText);
                isValid = false;
            }
        }
    });
    return isValid;
}


Summary.ReloadSummaryPageDropdown = function (projectId, entityNumber) {

    $("#ActivityType").removeClass("hide").removeAttr("disabled");
    $("#ActivityTypeNameDisplayOnly").addClass("hide").attr("disabled", "disabled");
    try {
        let aDate = new Date();
        let d = aDate.getDate();
        let m = aDate.getMonth() + 1;
        let y = aDate.getFullYear();
        try {
            m = m.toString();
            m = m.length > 1 ? m : '0' + m;

            d = d.toString();
            d = d.length > 1 ? d : '0' + d;

            $("#ActivityDate").editable('setValue', null);
            $("#ActivityDate").editable('setValue', y + "-" + m + "-" + d);

        } catch (e) {
          
        }
        $("#ActivityDate").attr("data-value", y + "-" + m + "-" + d);
        $("#ActivityDate").text(d + "/" + m + "/" + y);
    } catch (e1) {       
    }
    let getSummaryPageDetails = "MongoDB_Summary/GetSummaryDetails/" + projectId + "/" + entityNumber;
    if (App.IsTestSite === true) {
        getSummaryPageDetails = "Review/GetSummaryDetails/" + projectId + "/" + entityNumber;
    }

    App.postData(App.ApiUrl + getSummaryPageDetails, {}, function (summaryPageData) {
       
        let activityTypeDropdown = $('#ActivityType');
        activityTypeDropdown.empty();
        activityTypeDropdown.append('<option value="">Select Activity</option>');
        activityTypeDropdown.prop('selectedIndex', 0);
        $.each(summaryPageData.summaryPageActivityTypeList, function (key, entry) {
            if (entry.isDefaultActivity == 0 && entry.activityName != "Project Linkage") { return; }
            activityTypeDropdown.append($('<option></option>').attr('value', entry.activityGuid).text(entry.activityName));
        });

        let completedByUserDropdown = $('#CompletedByUser');
        completedByUserDropdown.empty();
        completedByUserDropdown.append('<option value="">Select User</option>');
        completedByUserDropdown.prop('selectedIndex', 0);
        $.each(summaryPageData.summaryPageProjectUsersList, function (key, entry) {
            completedByUserDropdown.append($('<option></option>').attr('value', entry.userGuid).text(entry.userName));
        });

        $("#scheduledActivitiesContainer li").remove();
        Summary.AllLinkedActivity = [];
        $.each(summaryPageData.summaryPageActivitiesList, function (key, entry) {

            try {
                if (entry.activityName == App.DefaultFormNamesStringEnum.Project_Linkage && entry.linkedProjectGuid != null) {
                    Summary.AllLinkedActivity.push(entry.linkedProjectGuid);
                }
            } catch (eE) {              
            }
            Summary.refreshSummaryPageActivities(entry);
        });

        $("#ActivityType").removeAttr("disabled");
        try {

            let sumDateNew = summaryPageData.createdDate;
            try {
                sumDateNew = summaryPageData.createdDate.substring(0, 10);
            } catch (eDate) {
            }

            let ed = new Date(sumDateNew).getDate();
            let em = new Date(sumDateNew).getMonth() + 1;
            let ey = new Date(sumDateNew).getFullYear();
            $("#EntityCreatedDate").val(em + "-" + ed + "-" + ey);
        } catch (eEntityDateUpdate) {

        }

        let leftPanelResult = {
            email: summaryPageData.email,
            phone: summaryPageData.phone,
            state: summaryPageData.state,
            suburb: summaryPageData.suburb,
            postcode: summaryPageData.postcode,
            fax: summaryPageData.fax,
            strtNum: summaryPageData.strtNum,
            strtNum2: summaryPageData.strtNum2,
            strtNme: summaryPageData.strtNme,
            strtNme2: summaryPageData.strtNme2,
        }
        if (summaryPageData.entityType == App.CommonEntityType.Participant) {
            leftPanelResult.defaultFormType = App.DefaultFormNamesStringEnum.Participant_Registration;
        } else if (summaryPageData.entityType == App.CommonEntityType.Person) {
            leftPanelResult.defaultFormType = App.DefaultFormNamesStringEnum.Person_Registration;
        } else if (summaryPageData.entityType == App.CommonEntityType.Place_Group) {
            leftPanelResult.defaultFormType = App.DefaultFormNamesStringEnum.Place__Group_Registration;
        } else if (summaryPageData.entityType == App.CommonEntityType.Project) {
            leftPanelResult.defaultFormType = App.DefaultFormNamesStringEnum.Project_Registration;
        }

        Summary.UpdateLeftPanelDefaultVariables(leftPanelResult);
    }, "GET");
}
Summary.checkActivityType = function (ac) {
}

Summary.ResetSummaryPageForm = function (e, _this) {
    
    if (e == "null") {
        let currentForm = $(_this).parents('form:first');
        let currentFormId = currentForm.prop('id');

        $("#butResetConfirmed").attr("onclick", "Summary.ResetSummaryPageFormStatic('" + currentFormId + "')");
        $("#reset-summarypage-form").modal("show");
    }
    else {
        $("#butResetConfirmed").attr("onclick", "Summary.ResetSummaryPageFormPost('" + e + "')");
        $("#reset-summarypage-form").modal("show");
    }
}

Summary.ResetSummaryPageFormPost = function (_id) {    
    let url = "Search/DeleteSummaryPageFormData/" + _id;
    if (App.IsTestSite === true) {
        url = "Review/DeleteSummaryPageFormData/" + _id;
    }
    let methodeType = "Delete";
    App.postData(App.ApiUrl + url, {}, function (result) {       
        if (result != null) {
            try {
                $("#reset-summarypage-form").modal("hide");
                $('#activity-form-variables-model').modal('hide');

                Summary.ReloadSummaryPageDropdown($("#CurrentProjectId").val(), result.parentEntityNumber);
                App.showSuccess("All the data entered into the form cleared successfully.");
            } catch (e) {
               
            }
        }
    }, methodeType);
}
Summary.ResetSummaryPageFormStatic = function (_formId) {
    $("#reset-summarypage-form").modal("hide");
    $('#activity-form-variables-model').modal('hide');
}


Summary.validateProjectLinkageForm = function (e) {
    let success = true;

    let linkedProject = $("#activityformVariableContainer").find("[data-variable-guid='52']");
    let dateJoinedProject = $("#activityformVariableContainer").find("[data-variable-guid='53']");
    let isActiveProjectUser = $("#activityformVariableContainer").find("[data-variable-guid='54']");
    let dateLeftProject = $("#activityformVariableContainer").find("[data-variable-guid='55']");

    $(linkedProject).removeClass("input-validation-error");
    $('span[data-valmsg-for="' + $(linkedProject).prop('id') + '"]').addClass("field-validation-error").text('');

    $(dateJoinedProject).removeClass("input-validation-error");
    $('span[data-valmsg-for="' + $(dateJoinedProject).prop('id') + '"]').addClass("field-validation-error").text('');

    $(isActiveProjectUser).removeClass("input-validation-error");
    $('span[data-valmsg-for="' + $(isActiveProjectUser).prop('id') + '"]').addClass("field-validation-error").text('');

    $(dateLeftProject).removeClass("input-validation-error");
    $('span[data-valmsg-for="' + $(dateLeftProject).prop('id') + '"]').addClass("field-validation-error").text('');



    if ($(linkedProject).val() == "") {
        $(linkedProject).addClass("input-validation-error");
        $('span[data-valmsg-for="' + $(linkedProject).prop('id') + '"]').addClass("field-validation-error").text($(linkedProject).attr("data-variable-required-message"));
        success = false;
    }
    if ($(isActiveProjectUser).val() == "1") {
        let joinDateValue = $(dateJoinedProject).text();
        if (joinDateValue == "" || joinDateValue == "Empty" || joinDateValue == "Invalid date") {
            $(dateJoinedProject).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(dateJoinedProject).prop('id') + '"]').addClass("field-validation-error").text($(dateJoinedProject).attr("data-variable-required-message"));
            success = false;
        }
    }
    else if ($(isActiveProjectUser).val() == "0") {
        let joinDateValue = $(dateJoinedProject).text();
        if (joinDateValue == "" || joinDateValue == "Empty" || joinDateValue == "Invalid date") {
            $(dateJoinedProject).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(dateJoinedProject).prop('id') + '"]').addClass("field-validation-error").text($(dateJoinedProject).attr("data-variable-required-message"));
            success = false;
        }

        let leftDateValue = $(dateLeftProject).text();
        if (leftDateValue == "" || leftDateValue == "Empty" || leftDateValue == "Invalid date") {
            $(dateLeftProject).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(dateLeftProject).prop('id') + '"]').addClass("field-validation-error").text($(dateLeftProject).attr("data-variable-required-message"));
            success = false;
        } else {


            try {
                var month_names_array = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                if (month_names_array.includes(joinDateValue.slice(3, -5))) {
                    let sdate = new Date(joinDateValue);
                    let edate = new Date(leftDateValue);

                    var twoDigitMonth1 = sdate.getMonth() + 1;
                    var twoDigitDate1 = sdate.getDate();
                    joinDateValue = twoDigitDate1 + "-" + twoDigitMonth1 + "-" + sdate.getFullYear();

                    var twoDigitMonth2 = edate.getMonth() + 1;
                    var twoDigitDate2 = edate.getDate();
                    leftDateValue = twoDigitDate2 + "-" + twoDigitMonth2 + "-" + edate.getFullYear();
                }
            } catch (e) {  }

            if (!App.checkStartDateAndEndDate(joinDateValue, leftDateValue)) {
                $(dateLeftProject).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(dateLeftProject).prop('id') + '"]').addClass("field-validation-error").text("project left date should be greater than project join  date");
                success = false;
            }
        }
    }

    return success;
}

Summary.UpdateLeftPanel = function (formTitle) {
    let sname, fname, mname, dob, gender, email, phone, personsubtype, state;
    switch (formTitle) {
        case "Participant Registration":
            sname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']");
            fname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.FirstName + "']");
            mname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.MiddleName + "']");
            dob = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.DOB + "']");
            gender = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Gender + "']");
            email = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Email + "']");
            phone = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Phone + "']");
            let mn = "";
            try {
                if ($(mname).val().trim() != "") {
                    mn = " " + $(mname).val().trim() + " ";
                }
            } catch (e) {

            }

            let g = $(gender).val();
            g = g == "1" ? "Male" : g == "2" ? "Female" : g == "3" ? "Other" : "";
            $(".content-circle").text($(fname).val() + mn + " " + $(sname).val());
            $("#lftCrdspnDOB").text($(dob).text());
            $("#lftCrdspnGender").text(g);
            $("#lftCrdspnEmail").text($(email).val());
            if ($("#lftCrdspnEmail").text().length > 0) {
                $("#lftCrdspnEmail").parent(":first.card-Text").removeClass("hide");
            } else {
                $("#lftCrdspnEmail").parent(":first.card-Text").addClass("hide");
            }
            break;
        case "Person Registration":
            sname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']");
            fname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.FirstName + "']");
            email = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Email + "']");
            phone = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Phone + "']");
            personsubtype = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.PerSType + "']"); //Person Sub-Type
            let pst = $(personsubtype).val();
            pst = pst == "1" ? "Medical Practitioner/Allied Health" : pst == "2" ? "Non-Medical Practitioner" : "";
            $("#lftCrdspnEntityUserName").text($(fname).val() + " " + $(sname).val());
            $("#lftCrdspnProfession").text(pst);
            $("#lftCrdspnPhone").text($(phone).val());
            $("#lftCrdspnEmail").text($(email).val());
            break;
        case "Place/Group Registration":
            state = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.State + "']"); //State
            sname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']");
            email = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Email + "']");
            try {
                let st = Summary.AllStates.find(function (x) { return x.guid === $(state).val(); }).name;
                $('#spnState').text(st);
            } catch (e) {

            }
            $("#lftCrdspnEntityUserName").text($(sname).val());
            $("#lftCrdspnEmail").text($(email).val());
            let fil = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.PlaceProfilePicture + "']").attr("data-file-base");
            if (fil != "") {
                let cstmStyl = "";
                var i = new Image();
                i.src = fil;
                i.onload = function () {
                    if (i.width > i.height) {
                        cstmStyl = "height: 170px!important; width:auto";
                    }
                    else if (i.width < i.height) {
                        cstmStyl = "width:170px!important;height:auto;";
                    } else {
                        cstmStyl = "height: 170px!important; width:auto";
                    }
                    let imgTag = "<img src='" + fil + "' style='" + cstmStyl + "'/>";
                    let spnTag = "<span class='span_name_full' id='lftCrdspnEntityUserName'>" + $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']").val() + "</span>";

                    $(".content-circle").empty();
                    if ($(".content-circle").parent(":first.card-body").find("#lftCrdspnEntityUserName").length > 0) {
                        $(".content-circle").parent(":first.card-body").find("#lftCrdspnEntityUserName").remove();
                    }
                    $(".content-circle").after(spnTag);
                    $(".content-circle").append(imgTag);
                };
            } else {

                let spnTag = "<span class='span_name_full' id='lftCrdspnEntityUserName'>" + $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']").val() + "</span>";

                $(".content-circle").empty();
                if ($(".content-circle").parent(":last.card-body").find("#lftCrdspnEntityUserName").length > 0) {
                    $(".content-circle").parent(":last.card-body").find("#lftCrdspnEntityUserName").remove();
                }
                $(".content-circle").append(spnTag);
            }
            break;
        case "Project Registration":
            sname = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Name + "']");
            $("#lftCrdspnEntityUserName").text($(sname).val());
            break;
        default:
            break;
    }
}

Summary.CallEntityFormDataEntryModel = function () {
    let query_string = window.location.search.toLowerCase();
    let search_params = new URLSearchParams(query_string);
    let entityId = search_params.get('participant');
    let formId = search_params.get('formid');
    let projectId = search_params.get('guid');
    let activityId = search_params.get('entityactivity');
    let summaryPageActivityId = search_params.get('entitysummarypageactivityid');
    let projectVersion = search_params.get('entityprojectversion');
    let fname = search_params.get('fname');
    if (activityId === null && summaryPageActivityId === null) {
        return false;
    }
    Summary.OpenSummaryPageFormPopup(entityId, formId, activityId, summaryPageActivityId, projectVersion, fname);
}

Summary.UpdateURL_AfterDataEntry = function () {

    let urlWithParam = window.location.search.toLowerCase();
    let urlActionMethod = window.location.pathname;
    //Sample URL_1: http://localhost:55330/Summary/Index?participant=5005&formId=504787a1-2925-4418-a492-c314f8e2daeb&guid=e1c3d556-9db8-4daf-a22e-ac09e9788e4e
    //Sample URL_2: http://localhost:55330/Summary/Index?participant=4170&formId=bf8e31bc-3fb0-48c5-9b02-a402ede8e6d8&guid=e1c3d556-9db8-4daf-a22e-ac09e9788e4e&entityActivity=50575929-de91-44dc-b3d6-77e14b59d9e7&entitySummaryPageActivityId=141&entityProjectVersion=0

    if (typeof URLSearchParams !== 'undefined') {
        const params = new URLSearchParams(urlWithParam);
        params.delete('entityactivity');
        params.delete('entitysummarypageactivityid');
        params.delete('entityprojectversion');
        params.delete('fname');

        try {
            if (new URLSearchParams(urlWithParam).get('entitysummarypageactivityid') != null) {
                window.history.pushState({}, document.title, urlActionMethod + "?" + params.toString());
            }
        } catch (eFilterURL) {          
            window.history.pushState({}, document.title, urlActionMethod + "?" + params.toString());
        }
    } else {
      
    }
}
Summary.UpdateLeftPanelDefaultVariables = function (leftPanelResult) {

    let email, phone, state;

    switch (leftPanelResult.defaultFormType) {

        case "Participant Registration":

            if (typeof leftPanelResult !== "undefined") {
                try {
                    $("#displayPhoneList").find(".lftCrdspnPhoneCls").remove();
                    $.each(leftPanelResult.phone.split(":"), function (i, ph) {
                        $("#displayPhoneList").append('<span class="lftCrdspnPhoneCls row ml-0 pt-05" id="lftCrdspnPhone">' + ph + '</span>');
                    });
                } catch (eLPanel) {
                  
                }

                try {
                    $("#displayEmailList").find(".lftCrdspnEmailCls").remove();
                    $.each(leftPanelResult.email.split(":"), function (i, em) {
                        $("#displayEmailList").append('<span class="lftCrdspnEmailCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.email != "")
                        leftPanelResult.email.split(":").length > 0 ? $("#displayEmailList").removeClass("hide") : $("#displayEmailList").addClass("hide");
                    else
                        $("#displayEmailList").addClass("hide");
                } catch (eLPanel) {
                  
                }


                try {
                    $("#displayFaxList").find(".lftCrdspnFaxCls").remove();
                    $.each(leftPanelResult.fax.split(":"), function (i, em) {
                        $("#displayFaxList").append('<span class="lftCrdspnFaxCls row ml-0 pt-05" id="lftCrdspnFax">' + em + '</span>');
                    });

                    if (leftPanelResult.fax != "")
                        leftPanelResult.fax.split(":").length > 0 ? $("#displayFaxList").removeClass("hide") : $("#displayFaxList").addClass("hide");
                    else
                        $("#displayFaxList").addClass("hide");
                } catch (eLPanel) {
                  
                }


                try {
                    $("#displayStreetNameList").find(".lftCrdspnStreetNameCls").remove();
                    $.each(leftPanelResult.strtNme.split(":"), function (i, em) {
                        $("#displayStreetNameList").append('<span class="lftCrdspnStreetNameCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    if (leftPanelResult.strtNme != "")
                        leftPanelResult.strtNme.split(":").length > 0 ? $("#displayStreetNameList").removeClass("hide") : $("#displayStreetNameList").addClass("hide");
                    else
                        $("#displayStreetNameList").addClass("hide");
                } catch (eLPanel) {
                    
                }

                try {
                    $("#displayStreetNumberList").find(".lftCrdspnStreetNumberCls").remove();
                    $.each(leftPanelResult.strtNum.split(":"), function (i, em) {
                        $("#displayStreetNumberList").append('<span class="lftCrdspnStreetNumberCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.strtNum != "")
                        leftPanelResult.strtNum.split(":").length > 0 ? $("#displayStreetNumberList").removeClass("hide") : $("#displayStreetNumberList").addClass("hide");
                    else
                        $("#displayStreetNumberList").addClass("hide");


                } catch (eLPanel) {
                  
                }

                try {
                    $("#displaySuburbList").find(".lftCrdspnSuburbCls").remove();
                    $.each(leftPanelResult.suburb.split(":"), function (i, em) {
                        $("#displaySuburbList").append('<span class="lftCrdspnSuburbCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    if (leftPanelResult.strtNum != "")
                        leftPanelResult.suburb.split(":").length > 0 ? $("#displaySuburbList").removeClass("hide") : $("#displaySuburbList").addClass("hide");
                    else
                        $("#displaySuburbList").addClass("hide");

                } catch (eLPanel) {
                }


                try {
                    $("#displayStateList").find(".lftCrdspnStateCls").remove();
                    $.each(leftPanelResult.state.split(":"), function (i, em) {
                        $("#displayStateList").append('<span class="lftCrdspnStateCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.state != "")
                        leftPanelResult.state.split(":").length > 0 ? $("#displayStateList").removeClass("hide") : $("#displayStateList").addClass("hide");
                    else
                        $("#displayStateList").addClass("hide");

                } catch (eLPanel) {
                }

                try {
                    $("#displayPostcodeList").find(".lftCrdspnPostcodeCls").remove();
                    $.each(leftPanelResult.postcode.split(":"), function (i, em) {
                        $("#displayPostcodeList").append('<span class="lftCrdspnPostcodeCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.postcode != "")
                        leftPanelResult.postcode.split(":").length > 0 ? $("#displayPostcodeList").removeClass("hide") : $("#displayPostcodeList").addClass("hide");
                    else
                        $("#displayPostcodeList").addClass("hide");

                } catch (eLPanel) {
                
                }
            }

            break;
        case "Person Registration":
            //email = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Email + "']");
            //phone = $("#activityformVariableContainer").find("[data-variable-name='" + App.DefaultVariablesStringEnum.Phone + "']");           
            if (typeof leftPanelResult !== "undefined") {
                try {
                    $("#displayPhoneList").find(".lftCrdspnPhoneCls").remove();
                    $.each(leftPanelResult.phone.split(":"), function (i, ph) {
                        $("#displayPhoneList").append('<span class="lftCrdspnPhoneCls row ml-0 pt-05" id="lftCrdspnPhone">' + ph + '</span>');
                    });
                } catch (eLPanel) {
                  
                }
                try {
                    $("#displayEmailList").find(".lftCrdspnEmailCls").remove();
                    $.each(leftPanelResult.email.split(":"), function (i, em) {
                        $("#displayEmailList").append('<span class="lftCrdspnEmailCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                } catch (eLPanel) {
               
                }
                try {
                    $("#displayFaxList").find(".lftCrdspnFaxCls").remove();
                    $.each(leftPanelResult.fax.split(":"), function (i, em) {
                        $("#displayFaxList").append('<span class="lftCrdspnFaxCls row ml-0 pt-05" id="lftCrdspnFax">' + em + '</span>');
                    });
                    if (leftPanelResult.fax != "")
                        leftPanelResult.fax.split(":").length > 0 ? $("#displayFaxList").removeClass("hide") : $("#displayFaxList").addClass("hide");
                    else
                        $("#displayFaxList").addClass("hide");
                } catch (eLPanel) {
             
                }
                try {
                    $("#displayStreetNameList").find(".lftCrdspnStreetNameCls").remove();
                    $.each(leftPanelResult.strtNme.split(":"), function (i, em) {
                        $("#displayStreetNameList").append('<span class="lftCrdspnStreetNameCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    if (leftPanelResult.strtNme != "")
                        leftPanelResult.strtNme.split(":").length > 0 ? $("#displayStreetNameList").removeClass("hide") : $("#displayStreetNameList").addClass("hide");
                    else
                        $("#displayStreetNameList").addClass("hide");
                } catch (eLPanel) {
                   
                }
                try {
                    $("#displayStreetNumberList").find(".lftCrdspnStreetNumberCls").remove();
                    $.each(leftPanelResult.strtNum.split(":"), function (i, em) {
                        $("#displayStreetNumberList").append('<span class="lftCrdspnStreetNumberCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.strtNum != "")
                        leftPanelResult.strtNum.split(":").length > 0 ? $("#displayStreetNumberList").removeClass("hide") : $("#displayStreetNumberList").addClass("hide");
                    else
                        $("#displayStreetNumberList").addClass("hide");
                } catch (eLPanel) {
                    
                }
                try {
                    $("#displaySuburbList").find(".lftCrdspnSuburbCls").remove();
                    $.each(leftPanelResult.suburb.split(":"), function (i, em) {
                        $("#displaySuburbList").append('<span class="lftCrdspnSuburbCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    if (leftPanelResult.suburb != "")
                        leftPanelResult.suburb.split(":").length > 0 ? $("#displaySuburbList").removeClass("hide") : $("#displaySuburbList").addClass("hide");
                    else
                        $("#displaySuburbList").addClass("hide");
                } catch (eLPanel) {
               
                }
                try {
                    $("#displayStateList").find(".lftCrdspnStateCls").remove();
                    $.each(leftPanelResult.state.split(":"), function (i, em) {
                        $("#displayStateList").append('<span class="lftCrdspnStateCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.state != "")
                        leftPanelResult.state.split(":").length > 0 ? $("#displayStateList").removeClass("hide") : $("#displayStateList").addClass("hide");
                    else
                        $("#displayStateList").addClass("hide");
                } catch (eLPanel) {
                  
                }
                try {
                    $("#displayPostcodeList").find(".lftCrdspnPostcodeCls").remove();
                    $.each(leftPanelResult.postcode.split(":"), function (i, em) {
                        $("#displayPostcodeList").append('<span class="lftCrdspnPostcodeCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    if (leftPanelResult.postcode != "")
                        leftPanelResult.postcode.split(":").length > 0 ? $("#displayPostcodeList").removeClass("hide") : $("#displayPostcodeList").addClass("hide");
                    else
                        $("#displayPostcodeList").addClass("hide");

                } catch (eLPanel) {
                   
                }

            }

            break;
        case "Place/Group Registration":
            if (typeof leftPanelResult !== "undefined") {
                try {
                    $("#displayPhoneList").find(".lftCrdspnPhoneCls").remove();
                    $.each(leftPanelResult.phone.split(":"), function (i, ph) {
                        $("#displayPhoneList").append('<span class="lftCrdspnPhoneCls row ml-0 pt-05" id="lftCrdspnPhone">' + ph + '</span>');
                    });

                    if (leftPanelResult.phone != "")
                        leftPanelResult.phone.split(":").length > 0 ? $("#displayPhoneList").removeClass("hide") : $("#displayPhoneList").addClass("hide");
                    else
                        $("#displayPhoneList").addClass("hide");
                } catch (eLPanel) {                    
                }

                try {
                    $("#displayEmailList").find(".lftCrdspnEmailCls").remove();
                    $.each(leftPanelResult.email.split(":"), function (i, em) {
                        $("#displayEmailList").append('<span class="lftCrdspnEmailCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.email.split(":").length > 0 ? $("#displayEmailList").removeClass("hide") : $("#displayEmailList").addClass("hide");
                } catch (eLPanel) {
                  
                }


                try {
                    $("#displayFaxList").find(".lftCrdspnFaxCls").remove();
                    $.each(leftPanelResult.fax.split(":"), function (i, em) {
                        $("#displayFaxList").append('<span class="lftCrdspnFaxCls row ml-0 pt-05" id="lftCrdspnFax">' + em + '</span>');
                    });
                    leftPanelResult.fax.split(":").length > 0 ? $("#displayFaxList").removeClass("hide") : $("#displayFaxList").addClass("hide");
                    if (leftPanelResult.fax != "")
                        leftPanelResult.fax.split(":").length > 0 ? $("#displayFaxList").removeClass("hide") : $("#displayFaxList").addClass("hide");
                    else
                        $("#displayFaxList").addClass("hide");
                } catch (eLPanel) {                    
                }


                try {
                    $("#displayStreetNameList").find(".lftCrdspnStreetNameCls").remove();
                    $.each(leftPanelResult.strtNme.split(":"), function (i, em) {
                        $("#displayStreetNameList").append('<span class="lftCrdspnStreetNameCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    leftPanelResult.strtNme.split(":").length > 0 ? $("#displayStreetNameList").removeClass("hide") : $("#displayStreetNameList").addClass("hide");
                    if (leftPanelResult.strtNme != "")
                        leftPanelResult.strtNme.split(":").length > 0 ? $("#displayStreetNameList").removeClass("hide") : $("#displayStreetNameList").addClass("hide");
                    else
                        $("#displayStreetNameList").addClass("hide");
                } catch (eLPanel) {
                }

                try {
                    $("#displayStreetNumberList").find(".lftCrdspnStreetNumberCls").remove();
                    $.each(leftPanelResult.strtNum.split(":"), function (i, em) {
                        $("#displayStreetNumberList").append('<span class="lftCrdspnStreetNumberCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.strtNum.split(":").length > 0 ? $("#displayStreetNumberList").removeClass("hide") : $("#displayStreetNumberList").addClass("hide");

                    if (leftPanelResult.strtNum != "")
                        leftPanelResult.strtNum.split(":").length > 0 ? $("#displayStreetNumberList").removeClass("hide") : $("#displayStreetNumberList").addClass("hide");
                    else
                        $("#displayStreetNumberList").addClass("hide");
                } catch (eLPanel) {
                }


                try {
                    $("#displaySuburbList").find(".lftCrdspnSuburbCls").remove();
                    $.each(leftPanelResult.suburb.split(":"), function (i, em) {
                        $("#displaySuburbList").append('<span class="lftCrdspnSuburbCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 ? $("#displaySuburbList").removeClass("hide") : $("#displaySuburbList").addClass("hide");

                    if (leftPanelResult.suburb != "")
                        leftPanelResult.suburb.split(":").length > 0 ? $("#displaySuburbList").removeClass("hide") : $("#displaySuburbList").addClass("hide");
                    else
                        $("#displaySuburbList").addClass("hide");

                } catch (eLPanel) {
                  
                }


                try {
                    $("#displayStateList").find(".lftCrdspnStateCls").remove();
                    $.each(leftPanelResult.state.split(":"), function (i, em) {
                        //try { em = Summary.AllStates.find(function (x) { return x.guid === em; }).name; } catch (e) { }

                        $("#displayStateList").append('<span class="lftCrdspnStateCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.state.split(":").length > 0 ? $("#displayStateList").removeClass("hide") : $("#displayStateList").addClass("hide");

                } catch (eLPanel) {
                 
                }


                try {
                    $("#displayPostcodeList").find(".lftCrdspnPostcodeCls").remove();
                    $.each(leftPanelResult.postcode.split(":"), function (i, em) {
                        $("#displayPostcodeList").append('<span class="lftCrdspnPostcodeCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.postcode.split(":").length > 0 ? $("#displayPostcodeList").removeClass("hide") : $("#displayPostcodeList").addClass("hide");

                    if (leftPanelResult.postcode != "")
                        leftPanelResult.postcode.split(":").length > 0 ? $("#displayPostcodeList").removeClass("hide") : $("#displayPostcodeList").addClass("hide");
                    else
                        $("#displayPostcodeList").addClass("hide");
                } catch (eLPanel) {
                   
                }

            }
            break;
        case "Project Registration":
            if (typeof leftPanelResult !== "undefined") {
                try {
                    $("#displayPhoneList").find(".lftCrdspnPhoneCls").remove();
                    $.each(leftPanelResult.phone.split(":"), function (i, ph) {
                        $("#displayPhoneList").append('<span class="lftCrdspnPhoneCls row ml-0 pt-05" id="lftCrdspnPhone">' + ph + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":") != "" ? $("#displayPhoneList").removeClass("hide") : $("#displayPhoneList").addClass("hide");

                } catch (eLPanel) {
                    
                }

                try {
                    $("#displayEmailList").find(".lftCrdspnEmailCls").remove();
                    $.each(leftPanelResult.email.split(":"), function (i, em) {
                        $("#displayEmailList").append('<span class="lftCrdspnEmailCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":") != "" ? $("#displayEmailList").removeClass("hide") : $("#displayEmailList").addClass("hide");
                } catch (eLPanel) {                  
                }


                try {
                    $("#displayFaxList").find(".lftCrdspnFaxCls").remove();
                    $.each(leftPanelResult.fax.split(":"), function (i, em) {
                        $("#displayFaxList").append('<span class="lftCrdspnFaxCls row ml-0 pt-05" id="lftCrdspnFax">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":") != "" ? $("#displayFaxList").removeClass("hide") : $("#displayFaxList").addClass("hide");
                } catch (eLPanel) {
                 
                }


                try {
                    $("#displayStreetNameList").find(".lftCrdspnStreetNameCls").remove();
                    $.each(leftPanelResult.strtNme.split(":"), function (i, em) {
                        $("#displayStreetNameList").append('<span class="lftCrdspnStreetNameCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":") != "" ? $("#displayStreetNameList").removeClass("hide") : $("#displayStreetNameList").addClass("hide");
                } catch (eLPanel) {                    
                }

                try {
                    $("#displayStreetNumberList").find(".lftCrdspnStreetNumberCls").remove();
                    $.each(leftPanelResult.strtNum.split(":"), function (i, em) {
                        $("#displayStreetNumberList").append('<span class="lftCrdspnStreetNumberCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":")!="" ? $("#displayStreetNumberList").removeClass("hide") : $("#displayStreetNumberList").addClass("hide");
                } catch (eLPanel) {                  
                }

                try {
                    $("#displaySuburbList").find(".lftCrdspnSuburbCls").remove();
                    $.each(leftPanelResult.suburb.split(":"), function (i, em) {
                        $("#displaySuburbList").append('<span class="lftCrdspnSuburbCls row ml-0 pt-05" id="lftCrdspnEmail">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":")!="" ? $("#displaySuburbList").removeClass("hide") : $("#displaySuburbList").addClass("hide");
                    if (em == "")
                        $("#displaySuburbList").addClass("hide");
                } catch (eLPanel) {                    
                }
                try {
                    $("#displayStateList").find(".lftCrdspnStateCls").remove();
                    $.each(leftPanelResult.state.split(":"), function (i, em) {
                        $("#displayStateList").append('<span class="lftCrdspnStateCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":")!="" ? $("#displayStateList").removeClass("hide") : $("#displayStateList").addClass("hide");

                } catch (eLPanel) {                    
                }
                try {
                    $("#displayPostcodeList").find(".lftCrdspnPostcodeCls").remove();
                    $.each(leftPanelResult.postcode.split(":"), function (i, em) {
                        $("#displayPostcodeList").append('<span class="lftCrdspnPostcodeCls row ml-0 pt-05" id="lftCrdspnEmail" style="width: 80%;">' + em + '</span>');
                    });
                    leftPanelResult.suburb.split(":").length > 0 && leftPanelResult.suburb.split(":") != "" ? $("#displayPostcodeList").removeClass("hide") : $("#displayPostcodeList").addClass("hide");

                } catch (eLPanel) {              
                }
            }
            break;
        default:
            break;
    }
}


Summary.GetSubChield_Hide = function (currentField) {

    let isParent = false;
    do {
        isParent = false;
        let currentFieldChieldVariable = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
        if (currentFieldChieldVariable.length > 0) {

            $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().addClass("hide");
            currentField = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");

            if (currentField.length > 0) {
                isParent = true;
            }
        }
    } while (isParent) {
        isParent = true;
    }
}

Summary.GetSubChield_Show = function (currentField) {

    let isParent = false;
    do {
        isParent = false;
        let currentFieldChieldVariable = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
        if (currentFieldChieldVariable.length > 0) {

            $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']").parent().parent().parent().parent().parent().removeClass("hide");
            currentField = $("#frm-1").find("[data-parent-variable-id='" + $(currentField).attr("data-variable-guid") + "']");
            if (currentField.length > 0) {
                isParent = true;
            }
        }
    } while (isParent) {
        isParent = true;
    }
}



Summary.setChildVariables1 = function (e) {

    let newId = $(e).prop("id").replace("inputField-", "");
    let parentDiv = $("#divDependent-" + newId);
    let currentSelectedVal = $(e).val();
    var thisVarType = $(e).attr("data-variable-typename");
    if (thisVarType == "Date") { currentSelectedVal = $(e).text() }
    if ($(e).prop('type') == "checkbox") {
        if ($(e).prop('checked'))
            currentSelectedVal = "1";
        else
            currentSelectedVal = "0";
    }

    if (thisVarType == "Numeric (Integer)" || thisVarType == "Numeric (Decimal)") {

        let parentVariableResponse = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-parent-variable-response");
        let parentVariableResponse1 = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']");//.attr("data-parent-variable-response");
        if (parentVariableResponse1.attr("data-dependent-variable-value-isblank") == "true" && currentSelectedVal == "") {

            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "true");

            let cid = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").prop("id");

            cid = cid.replace("inputField-", "");

            $("#divDependent-" + cid).removeClass("hide");


            Summary.GetSubChield_Show($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']"));
        } else {
            let direction = parentVariableResponse.charAt(0);
            let cutPoint;
            let currentVal;
            if (thisVarType == "Numeric (Integer)") {
                cutPoint = parseInt(parentVariableResponse.substring(1, parentVariableResponse.length));
                currentVal = parseInt($(e).val());

            } else if (thisVarType == "Numeric (Decimal)") {
                cutPoint = parseFloat(parentVariableResponse.substring(1, parentVariableResponse.length));
                currentVal = parseFloat($(e).val());
            }

            //show current child variable and add required attribute of current child variable
            if (direction == "<") {
                if (currentVal < cutPoint) {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").attr("data-variable-required", "true");
                    let cid = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").prop("id");
                    cid = cid.replace("inputField-", "");
                    $("#divDependent-" + cid).removeClass("hide");
                }
            }
            if (direction == ">") {
                if (currentVal > cutPoint) {
                    let cid = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").prop("id");
                    cid = cid.replace("inputField-", "");
                    $("#divDependent-" + cid).removeClass("hide");
                }
            }
        }
    }
    else {
        $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + currentSelectedVal + "']").attr("data-variable-required", "true");
        let cid = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "']").prop("id");
        cid = cid.replace("inputField-", "");
        $("#divDependent-" + cid).removeClass("hide");

        let cid1 = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid") + "'][data-parent-variable-response='" + currentSelectedVal + "']").prop("id");
        cid1 = cid1.replace("inputField-", "");
        $("#divDependent-" + cid1).addClass("hide");
    }
}

Summary.ISSpaceOnly = function (ff) {
    var str = ff;
    if (!str.replace(/\s/g, '').length) {
        return false;
    } else {
        return true;
    }
}


