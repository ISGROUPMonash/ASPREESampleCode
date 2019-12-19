
var Deployment = {};
Deployment.TestSiteWebUrl;
Deployment.AllCountries = {};
Deployment.AllStates = {};
Deployment.AllPostcodes = {};

Deployment.project = null;
Deployment.form = null;

Deployment.FormList = [];
Deployment.i = 0;
Deployment.isValidFreeText = true;
Deployment.DateFormat = "";

Deployment.dbContextScheduledActivities = [];

Deployment.ScheduledActivitiesNameList = [];

$(function () {
    Deployment.EnableDisableDeploymentChecklist();

    $('#search-var-text').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            Deployment.searchExistingActivities(true);
            return false;
        }
    });

    $('#activitiesContainer').on("drop", function (e) {
        e.preventDefault();

        if (!Deployment.currentActivity.category)
            Deployment.addActivityInContainer(Deployment.currentActivity.guid, Deployment.currentActivity.name, Deployment.currentActivity.repeatationCount, Deployment.currentActivity.categoryName, Deployment.currentActivity.guid, Deployment.currentActivity.isAllVariableApprovedOfActivity, Deployment.currentActivity.isDefaultVariable);
        else {

            $('a[data-id="' + Deployment.currentActivity.guid + '"]').parent().find('ul li a').each(function (i, d) {


                Deployment.addActivityInContainer($(d).attr('data-id'), $(d).text(), $(d).attr('data-repeatation-count'), Deployment.currentActivity.categoryName, Deployment.currentActivity.categoryGuid, $(d).attr('data-activity-variables-approved'), Deployment.currentActivity.isDefaultVariable);
            });
        }

        Deployment.EnableDisableDeploymentChecklist();
        return false;
    }).on('dragover', function (e) { e.preventDefault(); });

    $('#activitiesContainer').sortable({ 'handle': 'i.mdi-menu' });


    $('#activityformVariableContainer select').on('change', function () {

    });


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

Deployment.validateDeploymentChecklist = function () {
    let isValid = true;
    let NotApproved = true;

    App.clearError();

    if ($('#activitiesContainer div.row').length === 0) {
        App.showError("Please drag and drop activities to deploy");
        return false;
    }

    let acList = [];
    $('#activitiesContainer div.row').each(function (i, row) {

        let variablesApproved = $(row).attr('data-activity-variables-approved').toLowerCase();

        let isTrueSet = (variablesApproved == 'true');

        if (!isTrueSet) {

            acList.push($(row).text());
            NotApproved = false;
        }
    });
    if (!NotApproved) {
        App.showError("All variables used in '" + acList.join(", ") + "' activities are not approved, please approve variables to deploy the activities.");
        return false;
    }
    $("input:checkbox").each(function () {
        var droppedActivityCount = $('#activitiesContainer div.row').length;
        if ($(this).attr("disabled") == "disabled") {
            if (droppedActivityCount > 1)
                App.showError("These activities have already been deployed.", ".role-model-error");
            else
                App.showError("This activity has already been deployed.", ".role-model-error");
            isValid = false;
            return false;
        }
        if (!$(this).is(':checked')) {
            App.showError("Please complete the checklist.", ".role-model-error");
            isValid = false;
            return false;
        }
    });
    if (isValid) {

        $("#manage-confirm").find(".sf-submit-info").html("Are you sure you want to deploy this activity?");
        $("#manage-confirm").find(".sf-submit-button").attr("onclick", "Deployment.deployActivities()");
        var droppedActivityCount = $('#activitiesContainer div.row').length;
        var pc = $('#activitiesContainer div.row');

        $('#manage-confirm').modal();
        if (droppedActivityCount > 1)
            $('.modal-body .sf-submit-info').html("Are you sure you want to deploy these activities?");
        else {
            $('.modal-body .sf-submit-info').html("Are you sure you want to deploy this activity?");
        }
    }
}

Deployment.GenerateDeploymentPreview = function (guid) {
    var DeploymentStatus = 3;
    let url = "Scheduling/PushScheduledActivities/" + DeploymentStatus;
    let methodeType = "POST";
    var sendData = [];

    $('#activitiesContainer div.row').each(function (i, row) {

        let guid = $(row).attr('data-id');

        var scheduling = Deployment.dbContextScheduledActivities.find(function (d) { return d.activityId == guid });
        sendData.push(scheduling.guid);
    });

    if (sendData.length === 0) {
        App.showError("Please drag and drop activities to push.");
        return false;
    }

    App.postData(App.ApiUrl + url, sendData, function (result) {

        window.open(
            Deployment.TestSiteWebUrl,
            //"http://uds-test.cloud.monash.edu/",
            '_blank' // <- This is what makes it open in a new window.
        );
    }, methodeType);
}

Deployment.GetProjectByGuid = function (guid) {
    App.postData(App.ApiUrl + 'project/' + guid, {}, function (result) {
        Deployment.project = result;
    }, 'GET', true);
}

Deployment.GetProjectUserByRole = function (guid) {

    if (guid.value.length < 1) {
        $('#projectRoles').addClass('input-validation-error');
        $('#projectUsers').addClass('input-validation-error');
    } else {
        $('#projectRoles').removeClass('input-validation-error');
        $('#projectUsers').removeClass('input-validation-error');
    }

    let dropdown = $('#projectUsers');
    dropdown.empty();
    dropdown.append('<option selected="true" disabled>Select User</option>');
    dropdown.prop('selectedIndex', 0);
    $.each(Deployment.project.projectStaffMembersRoles, function (key, entry) {
        if (entry.roleGuid == guid.value) {
            dropdown.append($('<option></option>').attr('value', entry.userGuid).text(entry.projectUserName));
        }
    });
}

Deployment.SubmitRoleUser = function (guid) {

    $('#projectRoles').removeClass('input-validation-error');
    $('#projectUsers').removeClass('input-validation-error');

    roleGuid = $('#projectRoles').val();
    userGuid = $('#projectUsers').val();

    if (roleGuid.length < 1) {
        $('#projectRoles').addClass('input-validation-error');
        $('#projectUsers').addClass('input-validation-error');
        return false;
    }

    if (userGuid == null) {
        $('#projectUsers').addClass('input-validation-error');
        return false;
    }
    let data = guid + "," + roleGuid + "," + userGuid;
    location.href = "/Test/ProjectBuilder/DeploymentPreviewDashboard/" + data;
}

Deployment.GetActivityform = function () {
    $('.alert-dismissible').remove();
    if (Deployment.i != 0) {
        let valTest = Deployment.customRequiredValidator();
        if (!valTest) {
            App.showError("All fields are mandatory.", '.role-model-error');
            return false;
        }
    }

    if (!Deployment.isValidFreeText) {
        App.showError("validatin regex.");
        return false;
    }

    let guid = Deployment.FormList[Deployment.i];
    Deployment.i++;

    if (Deployment.FormList.length <= Deployment.i) {
        $('#btnNext').text('Finish');
    }

    if (Deployment.FormList.length < Deployment.i) {
        App.showSuccess("Forms submitted successfully.");
        return false;
    }
    App.postData(App.ApiUrl + 'form/' + guid, {}, function (result) {
        Deployment.dynamicallyRenderFormVariable_DeploymentPreview(result.guid, result.variables);
        $("#formTitle").text(result.formTitle);
    }, 'GET', true);
}

Deployment.DrawFormVariables = function (formguid, variables) {
    $('#activityformVariableContainer').empty();
    var container = $('#activityformVariableContainer');

    let varIndex = 0;
    variables.forEach(function (element) {
        varIndex++;

        var middelcontent = "</br>";
        if (element.variableType == "Text Box") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let dataAttr = "data-validationname='" + JSON.stringify(PreviewPane_ValidationName) + "'" +
                "data-validation-regex='" + JSON.stringify(PreviewPane_ValidationRegEx) + "'" +
                "data-validation-min='" + JSON.stringify(PreviewPane_ValidationMin) + "'" +
                "data-validation-max='" + JSON.stringify(PreviewPane_ValidationMax) + "'" +
                "data-validation-error-message='" + JSON.stringify(PreviewPane_ValidationErrorMessage) + "'";

            middelcontent = "<input  id='inputField-" + varIndex + "'  " + dataAttr + " class='form-control form-control-sm dynamic-variables' name='inputField-" + varIndex + "' value=''  " +
                "onblur='Deployment.validateFreeText(this.value, this.id," + JSON.stringify(PreviewPane_ValidationName) + "," + JSON.stringify(PreviewPane_ValidationRegEx) + "," + JSON.stringify(PreviewPane_ValidationMin) + "," + JSON.stringify(PreviewPane_ValidationMax) + "," + JSON.stringify(PreviewPane_ValidationErrorMessage) + ") ' />";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";
        }
        else if (element.variableType == "Free Text") {

            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let dataAttr = "data-validationname='" + JSON.stringify(PreviewPane_ValidationName) + "'" +
                "data-validation-regex='" + JSON.stringify(PreviewPane_ValidationRegEx) + "'" +
                "data-validation-min='" + JSON.stringify(PreviewPane_ValidationMin) + "'" +
                "data-validation-max='" + JSON.stringify(PreviewPane_ValidationMax) + "'" +
                "data-validation-error-message='" + JSON.stringify(PreviewPane_ValidationErrorMessage) + "'";

            middelcontent = "<textarea class='form-control dynamic-variables' rows='3' cols='50' id='inputField-" + varIndex + "'  " + dataAttr +
                "onblur='Deployment.validateFreeText(this.value, this.id," + JSON.stringify(PreviewPane_ValidationName) + "," + JSON.stringify(PreviewPane_ValidationRegEx) + "," + JSON.stringify(PreviewPane_ValidationMin) + "," + JSON.stringify(PreviewPane_ValidationMax) + "," + JSON.stringify(PreviewPane_ValidationErrorMessage) + ") ' ></textarea>";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";
        }

        else if (element.variableType == "Dropdown") {
            var options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
            }
            middelcontent = "<div class='form-group' id='selectdropdownvalue'>" +
                "<select class='form-control dynamic-variables' id='inputField-" + varIndex + "' name='inputField-" + varIndex + "'>" +
                "<option value=''>Select</option>" +
                options +
                "</select>" +
                "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>" +
                "</div>";
        }

        else if (element.variableType == "LKUP") {
            var options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
            }
            middelcontent = "<div class='form-group' id='selectdropdownvalue'>" +
                "<select class='form-control dynamic-variables' id='inputField-" + varIndex + "' name='inputField-" + varIndex + "'>" +
                "<option value=''>Select</option>" +
                options +

                "</select>" +
                "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>" +
                "</div>";
        }
        else if (element.variableType == "Checkbox") {
            var options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"  class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
            }

            middelcontent = "<div class='form-group' id='selectcheckboxvalue'>" +
                options +
                "</div>";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";

        }
        else if (element.variableType == "Radio") {
            var options = "";

            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
            }

            middelcontent = "<div class='form-group' id='selectradiobuttonvalue'>" + options + "</div>";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";
        }
        else if (element.variableType == "Numeric") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let dataAttr = "data-validationname='" + JSON.stringify(PreviewPane_ValidationName) + "'" +
                "data-validation-regex='" + JSON.stringify(PreviewPane_ValidationRegEx) + "'" +
                "data-validation-min='" + JSON.stringify(PreviewPane_ValidationMin) + "'" +
                "data-validation-max='" + JSON.stringify(PreviewPane_ValidationMax) + "'" +
                "data-validation-error-message='" + JSON.stringify(PreviewPane_ValidationErrorMessage) + "'";
            middelcontent = "<input  id='inputField-" + varIndex + "'  " + dataAttr + " class='form-control form-control-sm dynamic-variables' name='inputField-" + varIndex + "' value=''  " +
                "onblur='Deployment.validateFreeText(this.value, this.id," + JSON.stringify(PreviewPane_ValidationName) + "," + JSON.stringify(PreviewPane_ValidationRegEx) + "," + JSON.stringify(PreviewPane_ValidationMin) + "," + JSON.stringify(PreviewPane_ValidationMax) + "," + JSON.stringify(PreviewPane_ValidationErrorMessage) + ") ' />";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";
        }

        else if (element.variableType == "Formula") {
            middelcontent = "<input class='form-control form-control-sm dynamic-variables' name='formulavalue' id='formulavalue-" + varIndex + "' value='' />";
        }
        else if (element.variableType == "Date") {

            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let dataAttr = "data-validationname='" + JSON.stringify(PreviewPane_ValidationName) + "'" +
                "data-validation-regex='" + JSON.stringify(PreviewPane_ValidationRegEx) + "'" +
                "data-validation-min='" + JSON.stringify(PreviewPane_ValidationMin) + "'" +
                "data-validation-max='" + JSON.stringify(PreviewPane_ValidationMax) + "'" +
                "data-validation-error-message='" + JSON.stringify(PreviewPane_ValidationErrorMessage) + "'";

            middelcontent = "<input  id='inputField-" + varIndex + "'  " + dataAttr + " class='singledate form-control form-control-sm dynamic-variables' name='inputField-" + varIndex + "' value=''  " +
                "onblur='Deployment.validateFreeText(this.value, this.id," + JSON.stringify(PreviewPane_ValidationName) + "," + JSON.stringify(PreviewPane_ValidationRegEx) + "," + JSON.stringify(PreviewPane_ValidationMin) + "," + JSON.stringify(PreviewPane_ValidationMax) + "," + JSON.stringify(PreviewPane_ValidationErrorMessage) + ") ' />";
            middelcontent += "<span class='field-validation-valid form-control-feedback' data-valmsg-for='inputField-" + varIndex + "' data-valmsg-replace='true'></span>";
        }

        var isRequired = "";
        if (element.isRequired) {
            isRequired = "<span class='text-danger'>*</span>";
        }

        var dropableDiv = "<div class='col-md-12'><div class='form-group'><div class='card'>" +
            "<div class='card-body variable-card'>" + //text-center                                
            "<input type='hidden' value='" + element.variableViewModel.guid + "' class='variable-guid' />" +
            "<input type='hidden' value='" + formguid + "' class='form-guid' />" +
            element.variableName + isRequired +
            "</br>" +
            middelcontent +
            "</div>" +
            "</div></div></div>";
        container.append(dropableDiv);
    });

    $("input:radio[name=RadioButtonValue]:first").attr('checked', true);

    $('.singledate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: Deployment.DateFormat,
        }
    });
}

Deployment.validateFreeText = function (inputVal, id, validationName, regEx, min, max, errorMessage) {

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

Deployment.customRequiredValidator = function () {
    let valcount = 0;

    $(".dynamic-variables").each(function () {

        let fieldtype = $(this).prop('type');

        if (fieldtype == "text") {

            $(this).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');
            let val = $(this).val();
            if (val.length < 1) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
                return false;
            }
            if (typeof $(this).attr("data-valid") !== 'undefined') {
                let isValid = $.parseJSON($(this).attr("data-valid"));
                if (!isValid) {
                    valcount++;
                    $(this).addClass("input-validation-error");
                    $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('invalid input');
                    return false;
                }
            } else {
                let isValid = $(this).attr("data-valid");
                if (isValid == false) {
                    valcount++;
                    return false;
                }
            }
        }
        else if (fieldtype == "checkbox") {
            $(this).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');
            let name = $(this).attr("name");
            let chk = 0;
            $('input[name="' + name + '"]:checked').each(function () {
                chk++;
            });
            if (chk == 0) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
                return false;
            }
        }
        else if (fieldtype == "radio") {
            $(this).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');
            let chk = 0;
            let name = $(this).attr("name");
            $('input[name="' + name + '"]:checked').each(function () {
                chk++;
            });
            if (chk == 0) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
                return false;
            }
        }
        else if (fieldtype == "select-one") {
            $(this).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');
            let val = $(this).val();
            if (val.length < 1) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
                return false;
            }
        }
        else if (fieldtype == "textarea") {
            $(this).removeClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('');

            let val = $(this).val();
            if (val.length < 1) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
                return false;
            }

            let isValid = $.parseJSON($(this).attr("data-valid"));

            if (!isValid) {
                valcount++;
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('invalid input');
                return false;
            }
        }
        else {

            let val = $(this).val();
            if (val.length < 1) {
                valcount++;
                return false;
            }
        }
    });
    return valcount > 0 ? false : true;
}

//==============================================================
// old code of project deployment(removed 29-03-2019)
//==============================================================
Deployment.deployProject = function (projectId) {

    let url = "Project/PublishProject/" + projectId;
    let methodeType = "PUT";

    var sendData = {};
    sendData.ProjectUrl = $('#ProjectUrl').val();
    sendData.ProjectName = $('#ProjectName').val();

    App.postData(App.ApiUrl + url, sendData, function (result) {     
        App.showSuccess("Project published successfully.");
    }, methodeType);
}

// ============================================================== 
// Save activity form //14-Dec-2018  --PerformActivity page
// ==============================================================
Deployment.saveActivityform = function (e, status, isSubmitClicked) {

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
            if (disabledFieldCount_int > 0) { status = 5; }//(int)Core.Enum.FormStatusType.Submit_for_Review
        } catch (e) {  }

        //====================[ASPMONASH-312]=======================
        if ($("#projectStaffMemberRoleName").val() == App.DefaultRoleNameStringEnum.System_Admin) {
            if (!isSubmitClicked) {
                if (!Deployment.isValidDataEntryForm()) {
                    $("#confirm-submit-form-data-entry").modal("show");
                    $("#butConfirmSubmitFormDataEntry").attr("onclick", "Deployment.saveActivityform('eConfirmBtn', '" + status + "', 'true' )");
                    return false;
                }
            }
        } else {
            if (!Deployment.isValidDataEntryForm()) {
                return false;
            }
        }
        $("#butConfirmSubmitFormDataEntry").removeAttr("onclick");
        Deployment.isSubmitClicked = false;
        //====================[ASPMONASH-312]=======================


        //====================[ASPMONASH-429]=======================

        if (frmTitle == App.DefaultFormNamesStringEnum.Person_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Participant_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Place__Group_Registration
            || frmTitle == App.DefaultFormNamesStringEnum.Project_Registration
        ) {
            if ($("#projectStaffMemberRoleName").val() != App.DefaultRoleNameStringEnum.System_Admin) {
                status = 5;
            }
        }
        //====================[ASPMONASH-429]=======================

    }

    let currentForm = $(e).parents('form:first');
    let currentFormId = currentForm.prop('id');
    if (frmTitle === "Project Linkage") {
        if (!Deployment.validateProjectLinkageForm(e)) {
            return false;
        }
    }
    var success = true;
    let isEdit = true;
    if (!$("#hdnFormDataEntryGuid").val()) {
        isEdit = false;
    }
    if ($("#hdnFormDataEntryTypeIsNew").val() == "true") {
        isEdit = false;
    }

    //--------------------------------------////////////////////--------------------------------------
    //isEdit = false;
    //--------------------------------------////////////////////--------------------------------------

    let url = isEdit == false ? "FormDataEntry/Save" : "FormDataEntry/Edit/" + $("#hdnFormDataEntryGuid").val();

    if (App.IsTestSite === true) {
        url = isEdit == false ? "Review/FormDataEntrySave" : "Review/FormDataEntryEdit/" + $("#hdnFormDataEntryGuid").val();
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

        if ($("#search-variable-result-model-title").text() == "Project Linkage" && $(this).attr("data-variable-name") === App.DefaultVariablesStringEnum.LnkPro) { //$(this).prop("title") === "Linked Project" 
            let linkedProjectId = $(this).val();
            if (!linkedProjectId) {
                $(this).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text($(this).attr("data-variable-required-message"));
                success = false;
                return false;
            }
            else {

                if (!isEdit) {
                    let checkEntityLinkedProject = Deployment.checkEntityLinkedProject(linkedProjectId, $("#PersonEntityId").val());
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
            //data-variable-guid="51"
            var authType = $('[data-variable-guid="51"]').val();
            var email = $('[data-variable-guid="38"]').val();

            if (authType.length > 0) {
            }
        }
        if (chck == true) {
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
        if (fieldtype == "checkbox") {
            if (previousChk != $(this).prop('id')) {
                previousChk = $(this).prop('id');
                chkValue = $('input[name="' + $(this).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');

                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-guid'),
                    "selectedValues": chkValue,
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
                });
            }
        }
        else if (fieldtype == "" && fieldTag == "a") {
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).text(),
            });
        }
        else if (fieldtype == "file") {
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).attr("data-file-base"),
            });
        }
        else {
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-guid'),
                "selectedValues": $(this).val(),
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
            projStartDate = $("#activityformVariableContainer").find("[data-title='Recruitment Start Date']").text();
            projEndDate = $("#activityformVariableContainer").find("[data-title='Recruitment End Date']").text();
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

                let sdate = $("#activityformVariableContainer").find("[data-title='Recruitment Start Date']");
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
                    chkUsername = Deployment.checkUserNameExistence(userName, authType, userid);

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
                chkUsername = Deployment.checkUserNameExistence(userName, authType, userid);

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
        sendData.SubjectId = $("#" + currentFormId).attr('data-summarypageactivity');
        App.postData(App.ApiUrl + url, sendData, function (result) {
            try {
                Deployment.UpdateURL_AfterDataEntry();
            } catch (errURL) {               
            }

            if (sendData.FormTitle == "Project Registration") {
                if (sendData.ProjectId == $("#hdnFormDataEntryGuid").val()) {
                    let clr = $("#" + currentFormId + " :input[type=color]").val();
                    $(".topbar.theme-bgcolor.fixed-top").css({ "background": clr });
                    let clrFont = $("#" + currentFormId).find("[title='Project Display Name Text Colour']").val();
                    let clrDisplayName = $("#" + currentFormId).find("[title='Project Display Name']").val();
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
            }
            if (sendData.Status === 4) {
                $('#activity-form-variables-model').modal('hide');
                $('#' + Deployment.StatusId).text('Submitted');
                $('#' + Deployment.StatusId).removeClass('text-warning text-submit-for-review').addClass('text-success');
                Deployment.StatusId = "";
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " submitted successfully.");
                location.reload(true);
            }
            else if (sendData.Status === 5) { //(int)Core.Enum.FormStatusType.Submit_for_Review
                $('#activity-form-variables-model').modal('hide');
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " submitted for review successfully.");
                location.reload(true);
                $('#' + Deployment.StatusId).text('Submitted for Review');
                $('#' + Deployment.StatusId).removeClass('text-success text-warning').addClass('text-submit-for-review');
                Deployment.StatusId = "";
            }
            else {
                $('#activity-form-variables-model').modal('hide');
                App.showSuccess("Forms- " + $('#search-variable-result-model-title').text() + " saved successfully.");
                location.reload(true);
                $('#' + Deployment.StatusId).text('Draft');
                $('#' + Deployment.StatusId).removeClass('text-success text-submit-for-review').addClass('text-warning');
                Deployment.StatusId = "";
            }
        }, methodeType);
    }
    else {
        App.showError("Please fill all mandatory fields.", '.role-model-error');
    }
}

// ============================================================== 
// Deployment form validation   //17-Dec-2018  --PerformActivity page
// ==============================================================
Deployment.validateActivityForm = function (e, isRequired) {
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
    //requiredMessage
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
// Open Activity Form //18-Dec-2018  --Summary-->Index page
// ==============================================================
Deployment.StatusId = "";
Deployment.OLdOpenActivityForm = function (entId, formId, activityId, e) {
    Deployment.StatusId = "";

    Deployment.StatusId = $(e).prop('id')

    let url = "Form/GetActivityFormBySearchedEntity/" + entId + "/" + formId + "/" + activityId;
    let methodeType = "GET";
    let isFormDisabledCount = 0;
    App.postData(App.ApiUrl + url, {}, function (result) {
        $('#frm-1').attr('data-form-guid', result.guid);

        $('#activityformVariableContainer').empty();
        $('#search-variable-result-model-footer').empty();

        var container = $('#activityformVariableContainer');
        let resultFooter = $('#search-variable-result-model-footer');

        $('#search-variable-result-model-title').text(result.formTitle);
        $("#ActivityId").attr('data-activity-guid', activityId);
        try {
            $("#hdnFormDataEntryGuid").val(result.variables[0].variableViewModel.formDataEntryGuid);
        } catch (ex) {        
        }
        let varIndex = 0;
        result.variables.forEach(function (element) {
            varIndex++;

            if (element.variableName == "Email") {
                element.isRequired = true;
            }

            var middelcontent = "</br>";
            if (element.variableType == "Text Box") {

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                    }
                }
                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }
                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"> <div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +

                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +

                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Free Text") {

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }

                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });

                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                    }
                }

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +

                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<textarea id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables" rows="2" cols="50"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-username-guid="' + element.variableViewModel.userNameVariableGuid + '"' + //onchange +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  >" + element.variableViewModel.variableSelectedValues + "</textarea>" +

                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Dropdown") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        if (element.variableViewModel.variableName != "EntGrp")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                    }
                }

                //EntGroup default settings                
                if (result.formTitle == "Person Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 2;
                    isDisabled = "disabled";
                }
                if (result.formTitle == "Participant Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 1;
                    isDisabled = "disabled";
                }
                if (result.formTitle == "Place/Group Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 3;
                    isDisabled = "disabled";
                }
                if (result.formTitle == "Project Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 4;
                    isDisabled = "disabled";
                }
                var options = "";
                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                        options = options + "<option selected value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                    } else {
                        options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                    }
                }
                //---------for country/state dropdown-----------
                let variableTypeClass = "";

                let countryChange = "";

                if (element.variableViewModel.variableName == "Country" || element.variableViewModel.variableName == "State") {
                    options = "";
                    if (element.variableViewModel.variableName == "Country") {
                        variableTypeClass = "country-list";
                        countryChange = "onchange=Deployment.StateListByCountry(this)";
                        $.each(Deployment.AllCountries, function (key, entry) {
                            if (entry.guid == element.variableViewModel.variableSelectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    } else if (element.variableViewModel.variableName == "State") {
                        variableTypeClass = "state-list";
                        $.each(Deployment.AllStates, function (key, entry) {
                            if (entry.guid == element.variableViewModel.variableSelectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    }
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + '"' +
                    'id="inputField-' + varIndex + '"' + isDisabled +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-guid-id="' + element.variableId + '"' + countryChange +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' name="inputField-' + varIndex + '">' +
                    '<option value="">Select</option>' +
                    options +
                    '</select>' +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "LKUP") {

                let options = "";

                let variableTypeClass = "";
                if (element.variableViewModel.id == 23) {
                    variableTypeClass = "country-list";
                    $.each(Deployment.AllCountries, function (key, entry) {
                        options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                    });
                } else if (element.variableViewModel.id == 24) {
                    variableTypeClass = "state-list";

                    $.each(Deployment.AllStates, function (key, entry) {
                        options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                    });
                } else if (element.variableViewModel.id == 25) {
                    variableTypeClass = "postcode-list";
                    $.each(Deployment.AllPostcodes, function (key, entry) {
                        options = options + "<option value='" + entry.guid + "'>" + entry.postalCode + "</option>";
                    });
                }

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }

                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<div class="input-group">' +
                    '<input type="text" disabled class="form-control" placeholder="Search for ' + (element.variableViewModel.lookupEntitySubtypeName ? element.variableViewModel.lookupEntitySubtypeName : '') + '"' +
                    ' id="inputField-' + varIndex + '"' +
                    ' name="inputField-' + varIndex + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-lookup-entity-type="' + element.variableViewModel.lookupEntityTypeName + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' onchange="Deployment.ValidateLKUP(this.value, this.id, this)"' +
                    ' >' +
                    '<div class="input-group-append">' +
                    '<button class="btn btn-info" type="button"><i class="fas fa-search" aria-hidden="true"></i></button>' +

                    '</div>' +
                    '</div>' +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);

                $('#inputField-' + varIndex).val(element.variableViewModel.variableSelectedValues);
            }

            else if (element.variableType == "Checkbox") {
                let options = "";

                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"     data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
                    } else {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"     data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" checked="false"/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
                    }
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '</br>' +
                    //'<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    options +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Radio") {
                let options = "";
                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"  data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    options +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Numeric") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled1 = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled1 = "disabled";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }
                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let isDisabled = "";
                if (element.variableViewModel.variableName === "EntID") {
                    isDisabled = "disabled";
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled + " " + isDisabled1 +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Numeric (Integer)") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        if (element.variableViewModel.variableName != "EntID")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }
                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                if (element.variableViewModel.variableName === "EntID") {
                    isDisabled = "disabled";
                    element.variableViewModel.question = result.formTitle == "Project Registration" ? "Project Id" : result.formTitle == "Participant Registration" ? "Participant Id" : "Id";
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Numeric (Decimal)") {

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled1 = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled1 = "disabled";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }
                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let isDisabled = "";
                if (element.variableViewModel.variableName === "EntID") {
                    isDisabled = "disabled";
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled + isDisabled1 +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Formula") {
                middelcontent = "<input class='form-control form-control-sm dynamic-variables' name='formulavalue' id='formulavalue-" + varIndex + "' value='' />";
            }

            else if (element.variableType == "Date") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }

                $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
                $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
                $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
                $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
                $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                let formField = '<div class="row"><div class="col-md-6 col-sm-6" id = "divDependent-' + varIndex + '">' +
                    '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '<input id="inputField-' + varIndex + '" class="singledate form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
                container.append(formField);
                let dateformat = "DDMMYYYY";
                $.each(element.variableViewModel.variableValidationRuleViewModel, function (key, value) {
                    if (value.validationName == "Date") {
                        dateformat = "DDMMYYYY";
                    } else {
                        dateformat = value.validationName.replace("Date_", "");
                    }
                });
                let datepickerformat = Deployment.getDatepickerFormat(dateformat);

                if (element.variableViewModel.canFutureDate == false) {
                    $('#inputField-' + varIndex).daterangepicker({
                        singleDatePicker: true,
                        showDropdowns: true,
                        maxDate: new Date(),
                        locale: {
                            format: datepickerformat
                        }
                    });
                } else {
                    $('#inputField-' + varIndex).daterangepicker({
                        singleDatePicker: true,
                        showDropdowns: true,
                        locale: {
                            format: datepickerformat
                        }
                    });
                }


                $('#inputField-' + varIndex).data('daterangepicker').setStartDate(element.variableViewModel.variableSelectedValues);
                $('#inputField-' + varIndex).data('daterangepicker').setEndDate(element.variableViewModel.variableSelectedValues);
            }

            else if (element.variableType == "Heading") {
                let formField = '<div class="col-md-12 col-sm-12">' +
                    '<div class="form-group mb-3">' +
                    //'<h2 class="control-label">' + element.questionText + '</h2>' +
                    element.questionText +
                    '</div>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Other Text") {
                let formField = '<div class="col-md-12 col-sm-12">' +
                    '<div class="form-group mb-3"><p>' +
                    element.questionText +
                    '</p></div>' +
                    '</div>';
                container.append(formField);

            }
        });

        //parent variable display logic
        varIndex = 0;
        result.variables.forEach(function (element) {
            varIndex++;

            if (element.dependentVariableId != null && element.dependentVariableId != "") {
                $("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").attr('onchange', 'Deployment.setChildVariables(this)');

                $("#frm-1 ").find("[data-variable-guid='" + element.variableViewModel.id + "']").attr('data-parent-variable-id', element.dependentVariableId);
                $("#frm-1 ").find("[data-variable-guid='" + element.variableViewModel.id + "']").attr('data-parent-variable-response', element.responseOption);

                if ($("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").val() == element.responseOption) {
                    $("#divDependent-" + varIndex).removeClass("hide");
                    $("#inputField-" + varIndex).attr("data-variable-required", "true");
                } else {
                    $("#divDependent-" + varIndex).addClass("hide");
                    $("#inputField-" + varIndex).attr("data-variable-required", "false");
                }
            }
        });


        let isFormDisabledButton = "";
        if (isFormDisabledCount > 0) {
            isFormDisabledButton = "disabled";
            resultFooter.append($("<span for=\"EmailTEST\" class=\"form -control-feedback error-red field-validation-error\">You do not have permission to modify this form.</span>"));
        }

        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this, 4)\">Submit</button>"));
        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this)\">Save</button>"));
        resultFooter.append($("<button type=\"button\" class=\"btn btn-danger waves-effect waves-light\" data-dismiss=\"modal\">Cancel</button>"));



        $('#activity-form-variables-model').modal('show');
        $("input:radio[name=RadioButtonValue]:first").attr('checked', true);
        $('#search-for-tabular-data').css('max-width', 900);
    }, methodeType);
}

Deployment.getDatepickerFormat = function (dateformate) {

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
Deployment.GetAllCountries = function () {
    App.postData(App.ApiUrl + "/Country", {}, function (result) {
        Deployment.AllCountries = result;
    }, "Get");
}

// ============================================================== 
// Get all states
// ==============================================================
Deployment.GetAllStates = function () {
    App.postData(App.ApiUrl + "/State", {}, function (result) {
        Deployment.AllStates = result;
        try {
            //let sname = Deployment.AllStates.find(x => x.guid === $('#spnState').text()).name;
            let sname = Deployment.AllStates.find(function (x) { return x.guid === $('#spnState').text(); }).name;
            $('#spnState').text(sname);
        } catch (err) {}
    }, "Get");
}

// ============================================================== 
// get all postcode
// ==============================================================
Deployment.GetAllPostcodes = function () {
    App.postData(App.ApiUrl + "/PostCode", {}, function (result) {
        Deployment.AllPostcodes = result;
    }, "Get");
}

Deployment.ValidateLKUP = function (value, id, e) {

    var isCountry = $(e).is('.country-list');
    var isState = $(e).is('.state-list');

    if (isCountry) {
        let dropdown = $('.state-list');
        dropdown.empty();
        dropdown.append('<option selected="true" disabled value="">Select State</option>');
        dropdown.prop('selectedIndex', 0);
        $.each(Deployment.AllStates, function (key, entry) {
            if (entry.countryId == value) {
                dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
            }
        })
    }
    if (isState) {
        let dropdown = $('.postcode-list');
        dropdown.empty();
        dropdown.append('<option selected="true" disabled value="">Select Postcode</option>');
        dropdown.prop('selectedIndex', 0);
        $.each(Deployment.AllPostcodes, function (key, entry) {
            if (entry.stateId == value) {
                dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.postalCode));
            }
        })
    }
}

// ============================================================== 
// set current activity --------- Drag and Drop Activities
// ==============================================================
Deployment.setCurrentActivity = function (guid, name, category, categoryName, repeatationCount, categoryGuid, isAllVariableApprovedOfActivity, isDefaultVariable) {
    //
    Deployment.currentActivity = {};
    Deployment.currentActivity.guid = guid;
    Deployment.currentActivity.name = name;
    Deployment.currentActivity.category = category;
    Deployment.currentActivity.categoryName = categoryName;
    Deployment.currentActivity.repeatationCount = repeatationCount;
    Deployment.currentActivity.categoryGuid = categoryGuid;
    Deployment.currentActivity.isDefaultVariable = isDefaultVariable;
    Deployment.currentActivity.isAllVariableApprovedOfActivity = isAllVariableApprovedOfActivity;


}

Deployment.clearScheduleActivity = function () {
    $('#ActivityName').val('');
    $('#ActivityCategory').val('');
    $('#CurrentRepeat').val('');
    $('#ActivityGuid').val('');
    $('#ActivityCategoryGuid').val('');
}

Deployment.addActivityInContainer = function (guid, name, repeatationCount, categoryName, categoryGuid, isAllVariableApprovedOfActivity, isDefaultVariable) {



    var container = $('#activitiesContainer');

    if ($('div.row[data-id="' + guid + '"]', container).length > 0) return; // prevent duplicate variable entry

    var template = $('#activityTemplate').children().clone();
    template.find('.var-label').text(name);
    template.attr('data-id', guid);

    template.attr('data-category', categoryName);
    template.attr('data-name', name);
    template.attr('data-repeatation-count', repeatationCount);
    template.attr('data-category-guid', categoryGuid);
    template.attr('data-repeatation-offset', 1);
    template.attr('data-activity-schedule-type', 1);

    template.attr('data-isdefault-activity-type', isDefaultVariable);

    template.attr('data-activity-variables-approved', isAllVariableApprovedOfActivity);

    $('#drag-message').remove();
    container.append(template);
    container.sortable('refresh');

    Deployment.EnableDisableDeploymentChecklist();
}

Deployment.addActivityCategory = function (category) {

    $('a[data-id="' + Deployment.currentActivity.guid + '"]').parent().find('ul li a').each(function (i, d) {
        Deployment.addActivityInContainer($(d).attr('data-id'), $(d).text());
    });
}

//===============================================================
//"Testing/Deployment Preview" >>>> dynamically render form-variable 
//===============================================================
Deployment.dynamicallyRenderFormVariable_DeploymentPreview = function (formguid, variables) {

    $('#activityformVariableContainer').empty();
    var container = $('#activityformVariableContainer');

    let varIndex = 0;
    variables.forEach(function (element) {
        varIndex++;

        var middelcontent = "</br>";

        if (element.variableType == "Text Box") {

            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                ' name="inputField-' + varIndex + '" ' +
                //' value="' + element.variableViewModel.variableSelectedValues + '"' +
                ' placeholder="' + element.variableViewModel.valueDescription + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +

                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Free Text") {

            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +

                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<textarea id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables" rows="2" cols="50"' +
                ' name="inputField-' + varIndex + '" ' +
                ' placeholder="' + element.variableViewModel.valueDescription + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  ></textarea>" +

                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Dropdown") {
            var options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
            }

            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<select class="form-control form-control-line dynamic-variables"' +
                'id="inputField-' + varIndex + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                ' name="inputField-' + varIndex + '">' +
                '<option value="">Select</option>' +
                options +
                '</select>' +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "LKUP") {
            let options = "";

            let variableTypeClass = "";
            if (element.variableViewModel.id == 23) {
                variableTypeClass = "country-list";
                $.each(Deployment.AllCountries, function (key, entry) {
                    options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                });
            } else if (element.variableViewModel.id == 24) {
                variableTypeClass = "state-list";

                $.each(Deployment.AllStates, function (key, entry) {
                    options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                });
            } else if (element.variableViewModel.id == 25) {
                variableTypeClass = "postcode-list";
                $.each(Deployment.AllPostcodes, function (key, entry) {
                    options = options + "<option value='" + entry.guid + "'>" + entry.postalCode + "</option>";
                });
            }

            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + ' "' +
                'id="inputField-' + varIndex + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                ' onchange="Deployment.ValidateLKUP(this.value, this.id, this)"' +
                ' name="inputField-' + varIndex + '" >' +
                '<option value="">Select</option>' +
                options +
                '</select>' +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);

            $('#inputField-' + varIndex).val(element.variableViewModel.variableSelectedValues);
        }

        else if (element.variableType == "Checkbox") {
            let options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"     data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                options +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Radio") {
            let options = "";
            for (var i = 0; i < element.variableViewModel.values.length; i++) {
                options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"  data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                options +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Numeric") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }
            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isDisabled = "";
            if (element.variableViewModel.id === 1) {

            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                ' name="inputField-' + varIndex + '" ' + isDisabled +
                ' placeholder="' + element.variableViewModel.valueDescription + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Numeric (Integer)") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }
            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isDisabled = "";
            if (element.variableViewModel.id === 1) {

            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                ' name="inputField-' + varIndex + '" ' + isDisabled +
                ' placeholder="' + element.variableViewModel.question + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Numeric (Decimal)") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }
            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isDisabled = "";
            if (element.variableViewModel.id === 1) {

            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                ' name="inputField-' + varIndex + '" ' + isDisabled +

                ' placeholder="' + element.variableViewModel.question + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Formula") {
            middelcontent = "<input class='form-control form-control-sm dynamic-variables' name='formulavalue' id='formulavalue-" + varIndex + "' value='' />";
        }

        else if (element.variableType == "Date") {
            let PreviewPane_ValidationName = [];
            let PreviewPane_ValidationRegEx = [];
            let PreviewPane_ValidationMin = [];
            let PreviewPane_ValidationMax = [];
            let PreviewPane_ValidationErrorMessage = [];
            for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
            }

            $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
            $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
            $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));

            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-4 col-sm-6">' +
                '<div class="form-group mb-2">' +
                '<label class="control-label">' + element.variableViewModel.valueDescription + isRequired + '</label>' +
                '<input id="inputField-' + varIndex + '" class="singledate form-control form-control-line dynamic-variables"' +
                ' name="inputField-' + varIndex + '" ' +
                ' placeholder="' + element.variableViewModel.valueDescription + '" ' +
                ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                ' data-variable-guid="' + element.variableViewModel.id + '"' +
                ' data-variable-required="' + element.isRequired + '"' +
                "onblur='Deployment.validateFreeText(this.value, this.id," +
                JSON.stringify(PreviewPane_ValidationName) + "," +
                JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                JSON.stringify(PreviewPane_ValidationMin) + "," +
                JSON.stringify(PreviewPane_ValidationMax) + "," +
                JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Heading") {
            let formField = '<div class="col-md-12 col-sm-12">' +
                '<div class="form-group mb-3">' +
                element.questionText +
                '</div>' +
                '</div>';
            container.append(formField);
        }

        else if (element.variableType == "Subheading") {
            let formField = '<div class="col-md-12 col-sm-12">' +
                '<div class="form-group mb-3">' +
                element.questionText +
                '</div>' +
                '</div>';
            container.append(formField);

        }
    });

    $("input:radio[name=RadioButtonValue]:first").attr('checked', true);

    $('.singledate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: Deployment.DateFormat,//'@Aspree.Utility.ConfigSettings.DateFormat.ToUpper()', // 'DD-MM-YYYY'

        }
    });
}



//===============================================================
//  lookup entity search
//===============================================================
Deployment.LookupEntitySearchOpen = function (variableId, entityType, entitySubtype) {

    let entType = "Person Registration";
    if (entityType == "Place/Group") {
        entType = "Place/Group Registration"
    } else if (entityType == "Participant") {
        entType = "Participant Registration";
    } else {
        entType = "Person Registration"
    }

    App.postData(App.ApiUrl + 'Form/GetProjectDefaultForms/' + $("#ProjectId").val(), {}, function (result) {
        var currentForm = {};
        var placeGroupForm = $.map(result, function (v) {
            if (v.formTitle == entType) {
                currentForm = v;
            }
        });

        Deployment.DrawSearchPopup(currentForm);
    }, 'GET', true);
}

Deployment.DrawSearchPopup = function (result) {
    variableList = [];
    let placeGroupForm = $.map(result.variables, function (v) {
        if (v.isSearchVisible) {
            variableList.push(v);
        }
    });
    variableList.sort(function (a, b) {
        return a.searchPageOrder - b.searchPageOrder
    })


    var container = $('#search-popup-template');


    let template = $('#search-popup-body');
    template.empty();

    let resultFooter = $('#search-popup-template-footer');
    resultFooter.empty();



    $("#searchHeader").text(result.formTitle);



    $.each(variableList, function (key, value) {

        let varName = "";
        let isRequired = "";
        let inputField = "";
        let orText = "";
        if (value.variableViewModel.variableName == "EntID") {
            varName = "Participant ID:";
            orText = "<div class=\"form-group row align-items-center mb-3\">\n                                    <div class=\"col-md-3 pr-md-0\">\n                                        <label>OR</label>\n                                    </div>\n                                    <div class=\"col-md-8\">\n                                    </div>\n                                </div>";

        } else if (value.variableViewModel.variableName == "Name") {
            varName = "Surname:";
        } else {
            varName = value.variableViewModel.valueDescription;
        }

        if (value.variableType == "Text Box" || value.variableType == "Free Text" || value.variableType == "Numeric") {
            inputField = "<input type=\"text\" id=\"Participant-" + value.variableName + "\" data-variable-key=\"" + value.variableViewModel.id + "\" data-variable-required=\"" + value.isRequired + "\" class =\"form-control form-control-line participant-search-form\" placeholder=\"" + value.variableViewModel.valueDescription + "\">";
        }
        else if (value.variableType == "Dropdown") {
            let options = "";
            for (var i = 0; i < value.variableViewModel.values.length; i++) {
                options += "<option value=\"" + value.variableViewModel.values[i] + "\">" + value.variableViewModel.variableValueDescription[i] + "</option>";
            }
            inputField = "<select id=\"Participant-" + value.variableName + "\" data-variable-key=\"" + value.variableViewModel.id + "\" data-variable-required=\"" + value.isRequired + "\" class =\"form-control form-control-line participant-search-form\">\n                <option value=\"\">Select</option>\n                " + options + "\n                </select>";

        }
        else if (value.variableType == "Date") {
            inputField = "<input type=\"text\" id=\"Participant-" + value.variableName + "\" data-variable-key=\"" + value.variableViewModel.id + "\" data-variable-required=\"" + value.isRequired + "\" class =\"form-control form-control-line singledate participant-search-form\" placeholder=\"" + value.variableViewModel.valueDescription + "\">";
        }
        if (value.isRequired) {
            isRequired = '<span class="warning" style="color:red">*</span>';
        }
        let source = "<div class=\"form-group row align-items-center\">\n                            <div class =\"col-md-4 pr-md-0\">\n                                <label>" + varName + " " + isRequired + " </label>\n                            </div>\n                            <div class=\"col-md-8\">\n                                     " + inputField + "\n                            </div>\n                            </div>\n                            " + orText;

        template.append(source);

    });

    let formType = 3;
    if (result.formTitle == "Place/Group Registration") {
        formType = 2;
    }
    else if (result.formTitle == "Participant Registration") {
        formType = 1;
    }
    else {
        formType = 3;
    }

    resultFooter.append($("<button type=\"button\" onclick=\"Deployment.participantSearch('" + result.guid + "', " + formType + ")\" class=\"btn waves-effect waves-light btn-info mr-2\">Search</button>"));
    resultFooter.append($("<button type=\"button\" class=\"btn btn-danger waves-effect waves-light\" data-dismiss=\"modal\">Cancel</button>"));

    $('#search-popup-template-model').modal('show');


}

//=============================================================
//  search participant search
//=============================================================
Deployment.participantSearch = function (formid, formType) {

    $('#footerAllDetails').removeClass('hide');
    $('#footerEntiId').addClass('hide');


    gFormType = formType;
    gFormGuid = formid;

    let sendData = {};

    let callApi = false;
    if (formType === 1) {
        if (!Deployment.validateSearchForm(formType)) {
            return false;
        }
        sendData.SearchVariables = [];

        $(".participant-search-form").each(function () {

            let middleNameId = $(this).attr('data-variable-key');
            if ($(this).val().length > 0) { //if ($(this).val().length > 0 || middleNameId == 15) {
                if ($(this).attr('data-variable-key') == 14 || $(this).attr('data-variable-key') == 15 || $(this).attr('data-variable-key') == 13) {
                    $(this).val(App.convertSentenceCase($(this).val()));
                }
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": $(this).val(),
                });
            }
        });
        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;

        callApi = true;
    }

    else if (formType === 2) {
        if (!Deployment.validateSearchForm(formType)) {
            return false;
        }

        sendData.SearchVariables = [];

        $(".participant-search-form").each(function () {
            if ($(this).val().length > 0) {
                if ($(this).attr('data-variable-key') == 14 || $(this).attr('data-variable-key') == 15 || $(this).attr('data-variable-key') == 13) {
                    $(this).val(App.convertSentenceCase($(this).val()));
                }
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": $(this).val(),
                });
            }
        });

        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;

        callApi = true;
    }

    else if (formType === 3) {
        if (!Deployment.validateSearchForm(formType)) {
            return false;
        }

        sendData.SearchVariables = [];

        $(".participant-search-form").each(function () {
            if ($(this).val().length > 0) {
                if ($(this).attr('data-variable-key') == 14 || $(this).attr('data-variable-key') == 15 || $(this).attr('data-variable-key') == 13) {
                    $(this).val(App.convertSentenceCase($(this).val()));
                }
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": $(this).val(),
                });
            }
        });

        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;

        callApi = true;
    }

    if (callApi === true) {
        App.postData(App.ApiUrl + '/FormDataEntry/SearchParticipant', sendData, function (result) {

            $('#search-variable-result-model-table').empty();
            $('#search-variable-result-model-footer').empty();

            if (result != null) {
                $('#search-for-tabular-data').removeAttr('style');

                if (formType == 1) {
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {

                        let isFound = false;
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");

                        jQuery.each(result, function (i, frm) {
                            let c = frm.find(function (x) { return x.variableId === 15; }).selectedValues;
                            if (c === "") {                              
                                let fn = "";
                                let sn = "";
                                let dob = "";
                                let gndr = "";
                                let entId = "";

                                jQuery.each(frm, function (i, val) {
                                    if (val.variableId == 1) {
                                        searchUserId = val.selectedValues;
                                        entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    if (val.variableId == 14) {
                                        fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    if (val.variableId == 13) {
                                        sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    if (val.variableId == 40) {
                                        dob = "<tr><td class='sf-hd-title'>Date of Birth:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    if (val.variableId == 41) {
                                        if (val.selectedValues == 1) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Male</td></tr>";
                                        } else if (val.selectedValues == 2) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Female</td></tr>";
                                        } else if (val.selectedValues == 3) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Other</td></tr>";
                                        }
                                    }
                                });

                                resultTable.append($(entId));
                                resultTable.append($(fn));
                                resultTable.append($(sn));
                                resultTable.append($(dob));
                                resultTable.append($(gndr));
                                isFound = true;
                                return false;
                            } else {
                               
                            }


                        });
                        if (isFound) {
                            resultFooter.append($('<button type=\'button\' class=\'btn btn waves-effect waves-light btn-info\' onclick="Search.resultRedirect(\'' + searchUserId + '\', \'' + gFormGuid + '\')">Yes</button>'));
                            resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));

                            $('#search-variable-result-model').modal('show');
                        } else {
                            $('#search-validation-popup').modal('show');
                            var popupBody = $("<p>There are multiple records with same details, please enter more details for exact search.</p>");
                            $('#popup-body').html(popupBody);
                        }
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");

                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let dob = "";
                            let gndr = "";
                            let entId = "";

                            jQuery.each(frm, function (i, val) {
                                if (val.variableId == 1) {
                                    searchUserId = val.selectedValues;
                                    entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 14) {
                                    fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 13) {
                                    sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 40) {
                                    dob = "<tr><td class='sf-hd-title'>Date of Birth:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 41) {
                                    if (val.selectedValues == 1) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Male</td></tr>";
                                    } else if (val.selectedValues == 2) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Female</td></tr>";
                                    } else if (val.selectedValues == 3) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Other</td></tr>";
                                    }
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(fn));
                            resultTable.append($(sn));
                            resultTable.append($(dob));
                            resultTable.append($(gndr));
                        });
                        resultFooter.append($('<button type=\'button\' class=\'btn btn waves-effect waves-light btn-info\' onclick="Search.resultRedirect(\'' + searchUserId + '\', \'' + gFormGuid + '\')">Yes</button>'));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));

                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {

                        if ($('#Participant-EntID').val().length > 0) {

                            $('#footerAllDetails').addClass('hide');
                            $('#footerEntiId').removeClass('hide');
                        }
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }

                else if (formType == 2) {
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {
                        $('#search-for-tabular-data').css('max-width', 900);
                        resultTable.append($("<tr><th>First Name</th><th>Surname</th><th>Sub-type</th><th>Email</th><th></th></tr>"));
                        $('#search-variable-result-model-title').text('Are any of these person the one you are looking for?');
                        let responseBody = ""

                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let subtyp = "";
                            let email = "";
                            let entId = "";

                            jQuery.each(frm, function (i, val) {
                                if (val.variableId == 1) {
                                    entId = val.selectedValues;
                                }
                                if (val.variableId == 14) {
                                    fn = val.selectedValues;
                                }
                                if (val.variableId == 13) {
                                    sn = val.selectedValues;
                                }
                                if (val.variableId == 4) {
                                    if (val.selectedValues == 1) {
                                        subtyp = 'Medical Pracitioner/Allied Health';
                                    } else if (val.selectedValues == 2) {
                                        subtyp = 'Non-Medical Practitioner';
                                    }
                                }
                                if (val.variableId == 35) {
                                    email = val.selectedValues;
                                }

                            });
                            resultTable.append($("<tr><td>" + fn + "</td><td>" + sn + "</td><td>" + subtyp + "</td><td>" + email + "</td><td class='text-right'><button type='button' onclick = \"Search.resultRedirect('" + entId + "', '" + gFormGuid + "')\" class='btn waves-effect waves-light btn-info'>Yes</button></td></tr>"));

                        });

                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light btn-info" " data-dismiss="modal">No, none of these</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");
                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let dob = "";
                            let gndr = "";
                            let entId = "";

                            jQuery.each(frm, function (i, val) {
                                if (val.variableId == 1) {
                                    entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                    searchUserId = val.selectedValues;
                                }
                                if (val.variableId == 14) {
                                    fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 13) {
                                    sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 4) {
                                    if (val.selectedValues == 1) {
                                        subtyp = "<tr><td class='sf-hd-title'>Sub-type:</td><td>Medical Pracitioner/Allied Health</td></tr>";
                                    } else if (val.selectedValues == 2) {
                                        subtyp = "<tr><td class='sf-hd-title'>Sub-type:</td><td>Non-Medical Practitioner</td></tr>";
                                    }
                                }
                                if (val.variableId == 35) {
                                    email = "<tr><td class='sf-hd-title'>Email:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(fn));
                            resultTable.append($(sn));
                            resultTable.append($(subtyp));
                            resultTable.append($(email));
                        });
                        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' onclick=\"Search.resultRedirect('" + searchUserId + "', '" + gFormGuid + "')\">Yes</button>"));

                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));

                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }

                else if (formType == 3) {

                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {
                        $('#search-for-tabular-data').css('max-width', 700);
                        resultTable.append($("<tr><th>Name</th><th>Postcode</th><th>State</th><th></th></tr>"));
                        $('#search-variable-result-model-title').text('Are any of these place the one you are looking for?');
                        let responseBody = ""

                        jQuery.each(result, function (i, frm) {
                            let name = "";
                            let suburb = "";
                            let state = "";

                            let entId = "";

                            jQuery.each(frm, function (i, val) {
                                if (val.variableId == 1) {
                                    entId = val.selectedValues;
                                }
                                if (val.variableId == 13) {
                                    name = val.selectedValues;
                                }
                                if (val.variableId == 25) {
                                    if (val.selectedValues !== "") {
                                        let sname = Search.PostCodeList.find(function (x) { return x.guid === val.selectedValues; }).postalCode;
                                        val.selectedValues = sname;
                                    }
                                    suburb = val.selectedValues;
                                }
                                if (val.variableId == 24) {
                                    if (val.selectedValues !== "") {
                                        let sname = Search.StateList.find(function (x) { return x.guid === val.selectedValues; }).name;
                                        val.selectedValues = sname;
                                    }
                                    state = val.selectedValues;
                                }
                            });
                            resultTable.append($("<tr><td>" + name + "</td><td>" + suburb + "</td><td>" + state + "</td><td class='text-right'><button type='button' onclick=\"Search.resultRedirect('" + entId + "', '" + gFormGuid + "')\" class='btn waves-effect waves-light btn-info'>Yes</button></td></tr>"));

                        });

                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light btn-info" " data-dismiss="modal">No, none of these</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this what you were looking for?");
                        jQuery.each(result, function (i, frm) {
                            let name = "";
                            let suburb = "";
                            let state = "";

                            let entId = "";

                            jQuery.each(frm, function (i, val) {
                                if (val.variableId == 1) {
                                    searchUserId = val.selectedValues;
                                    entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 13) {
                                    name = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 25) {

                                    if (val.selectedValues !== "") {
                                        let sname = Search.PostCodeList.find(function (x) { return x.guid === val.selectedValues; }).postalCode;
                                        val.selectedValues = sname;
                                    }
                                    suburb = val.selectedValues;

                                    suburb = "<tr><td class='sf-hd-title'>Postcode:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableId == 24) {

                                    if (val.selectedValues !== "") {
                                        let sname = Search.StateList.find(function (x) { return x.guid === val.selectedValues; }).name;
                                        val.selectedValues = sname;
                                    }
                                    suburb = val.selectedValues;

                                    state = "<tr><td class='sf-hd-title'>State:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(name));
                            resultTable.append($(suburb));
                            resultTable.append($(state));
                        });

                        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' onclick=\"Search.resultRedirect('" + searchUserId + "', '" + gFormGuid + "')\">Yes</button>"));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }
            }
            else {
                if (formType == 1) {
                    if ($('#Participant-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');
                    }
                } else if (formType == 2) {
                    if ($('#Person-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');
                    }
                } else if (formType == 3) {
                    if ($('#PlaceGroup-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');
                    }
                }
                $('#sf-not-found-search-popus').modal('show');
            }


        }, 'POST');
    }
}

Deployment.validateSearchForm = function (formType) {
    let resultCount = 0;
    $(".participant-search-form").removeClass("input-validation-error-search");
    $(".participant-search-form").each(function () {
        let isRequired = $(this).attr('data-variable-required');
        if (isRequired == "false") {
            return;
        }

        if ($(this).attr('data-variable-key') == 1) {
            if ($(this).val().length > 0) {
                resultCount = 0;
                return false;
            } else {
                return;
            }
        }
        if ($(this).val() == "") {
            $(this).addClass("input-validation-error-search");
            resultCount++;
        }
    });

    if (resultCount == 0) {
        return true;
    } else {
        return false;
    }
}

//========================================================================================
//      Set Child Variables(dependent variables)
//========================================================================================
Deployment.setChildVariables = function (e) {


    if ($(e).prop('type') == "checkbox") {
        if ($(e).prop('checked'))
            $(e).val("1");
        else
            $(e).val("0");
    }

    let childVarType = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-typename");
    if (childVarType == "Numeric (Integer)" || childVarType == "Numeric (Decimal)") {

        var thisVarType = $(e).attr("data-variable-typename");

        if (thisVarType == "Date") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().addClass("hide");
        }
        else if (thisVarType == "Numeric (Integer)") {



            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().parent().removeClass("hide");


            let parentVariableResponse = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-parent-variable-response");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().addClass("hide");
        }
        else if (thisVarType == "Numeric (Decimal)") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().addClass("hide");
        }
        else {

            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().parent().addClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().removeClass("hide");
        }
    }
    else {
        var thisVarType = $(e).attr("data-variable-typename");
        if (thisVarType == "Date") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().addClass("hide");
        }
        else if (thisVarType == "Numeric (Integer)") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().addClass("hide");



            let parentVariableResponse = $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-parent-variable-response");

            let direction = parentVariableResponse.charAt(0);
            let cutPoint = parseInt(parentVariableResponse.substring(1, parentVariableResponse.length));
            let currentVal = parseInt($(e).val());

            //show current child variable and add required attribute of current child variable
            if (direction == "<") {
                if (currentVal < cutPoint) {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "true");
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().removeClass("hide");
                }
            }
            if (direction == ">") {
                if (currentVal > cutPoint) {
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "true");
                    $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().removeClass("hide");
                }
            }

            ////show current child variable and add required attribute of current child variable
            //$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            //$("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().addClass("hide");
        }
        else if (thisVarType == "Numeric (Decimal)") {
            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().removeClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().addClass("hide");
        }
        else {

            //hide all child variable and remove required attribute of all child variables
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").attr("data-variable-required", "false");

            if ($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").prop("type") == "checkbox")
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().parent().addClass("hide");
            else
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "']").parent().parent().parent().parent().addClass("hide");

            //show current child variable and add required attribute of current child variable
            $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").attr("data-variable-required", "true");
            if ($("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").prop("type") == "checkbox")
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().parent().removeClass("hide");
            else
                $("#frm-1").find("[data-parent-variable-id='" + $(e).attr("data-variable-guid-id") + "'][data-parent-variable-response='" + $(e).val() + "']").parent().parent().parent().parent().removeClass("hide");
        }
    }


}

//========================================================================================
//      Set State based on country change
//========================================================================================
Deployment.StateListByCountry = function (e) {
    let countryId = $(e).val();

    let dropdown = $('.state-list');
    dropdown.empty();
    dropdown.append('<option value="">Select</option>');
    dropdown.prop('selectedIndex', 0);

    $.each(Deployment.AllStates, function (key, entry) {
        if (entry.countryId == countryId) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        }
    });
}

//======================================================
//  search exesting activities from activity library in testing/deployment page
//======================================================
Deployment.searchExistingActivities = function (type) {
    var text = $('#search-var-text').val().trim().toLowerCase();
    if (type) {
        if (text.length > 0) {
            $(".sub-category-txt").each(function () {
                var subName = $(this).data('text');
                if (subName.indexOf(text) > -1) {
                    $(this).parent().show();
                    $(this).parent().parent().parent().show();
                    $(this).parent().parent().parent().parent().addClass("active");
                } else {
                    $(this).parent().hide();
                }
            });
        }
        else {
            $(".sub-category-txt").each(function () {
                $(this).parent().show();
            });
            $(".sub-category-txt").parent().show();
            $(".sub-category-txt").parent().parent().parent().hide();
            $(".sub-category-txt").parent().parent().parent().parent().removeClass("active");
        }
    } else {
        if (text.length < 1) {
            $(".sub-category-txt").each(function () {
                $(this).parent().show();
            });
            $(".sub-category-txt").parent().show();
            $(".sub-category-txt").parent().parent().parent().hide();
            $(".sub-category-txt").parent().parent().parent().parent().removeClass("active");
        }
    }
}

//======================================================
//  deployment page load- remove unscheduled activities
//======================================================
Deployment.GetScheduledActivity = function () {
    App.postData(App.ApiUrl + 'Scheduling/GetAllScheduledActivityByProjectId/' + $("#ProjectId").val(), {}, function (result) {

        Deployment.dbContextScheduledActivities = result;
        let ScheduledActivityList = [];
        $.each(result, function (key, entry) {
            ScheduledActivityList.push(entry.activityId);
        });
        $(".activities-list-entered-label").each(function () {
            var _this = this;

            var isScheduled = ScheduledActivityList.some(function (v) {
                return $(_this).attr('data-activity-id').indexOf(v) !== -1;
            });
            if (!isScheduled) {
                $(this).remove();
            }
        });
    }, 'GET');
}

//===============================================================
//  check username existence in userlogin table
//===============================================================
Deployment.checkUserNameExistence = function (userName, authType, userid) {

    let isAsync = false;
    let response = null;
    App.postData(App.ApiUrl + "Account/CheckUsernameAvailability/" + userName + "/" + authType + "/" + userid, {}, function (result) {
        response = result;
    }, "Get", undefined, isAsync);

    return response;
}

//===============================================================
//  open form in model  14-Mar-2019
//===============================================================
Deployment.OpenActivityForm1 = function (entId, formId, activityId, e, fname) {

    Deployment.StatusId = "";

    let summarypageactivity = "";
    if (jQuery.type(e) === "object") {
        Deployment.StatusId = $(e).prop('id');
        summarypageactivity = $(e).attr("data-summarypage-userentity");
    } else {

        Deployment.StatusId = formId + "-" + e;
        summarypageactivity = e;
    }

    //http://localhost:55330/Summary/Index?participant=3223&formId=0967d260-1b98-484f-b252-6f6169ae1cd7&guid=594b06f8-9c48-4c02-a488-5dcb37cfc9ca&entityActivity=9108d8c3-7c36-4745-a80a-7dd71ac057f5&entitySummaryPageActivityId=148&entityProjectVersion=0



    let url = "Form/GetActivityFormBySearchedEntity/" + entId + "/" + formId + "/" + activityId + "/" + summarypageactivity;
    if (App.IsTestSite === true) {
        url = "Review/GetActivityFormBySearchedEntity/" + entId + "/" + formId + "/" + activityId + "/" + summarypageactivity;
    }
    let methodeType = "GET";
    let isFormDisabledCount = 0;

    App.postData(App.ApiUrl + url, {}, function (result) {


        if (result === null) {
            App.showError("This form " + fname + " is not available in current project");

            try {
                Summary.UpdateURL_AfterDataEntry();
            } catch (errURL) {
            }

            return false;
        }


        $('#frm-1').attr('data-form-guid', result.guid);

        $('#frm-1').attr('data-summarypageactivity', summarypageactivity);


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
        if (result.formDataEntryGuid != null)
            $("#hdnFormDataEntryGuid").val(result.formDataEntryGuid);
        else
            $("#hdnFormDataEntryGuid").val("");
        let varIndex = 0;
        let SubmitButtonText = "Submit";
        result.variables.forEach(function (element) {


            varIndex++;
            //=========================================================================================
            //============== can view permission start
            //=========================================================================================
            let roleIndexCanView = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
            if (roleIndexCanView > 0) {
                if (element.formVariableRoleViewModel[roleIndexCanView].canView == false) {

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

            if (element.variableViewModel.helpText) {
                helpTextSpan = '<span class="help-block"><small>' + element.variableViewModel.helpText + '</small></span><br>';
            }


            element.variableViewModel.helpText = (!element.variableViewModel.helpText ? element.variableViewModel.question : element.variableViewModel.helpText);


            if (element.variableViewModel.variableSelectedValues == "") {
                if (element.variableName != "MiddleName") {
                    if ($("#projectStaffMemberRoleName").val() != App.DefaultRoleNameStringEnum.System_Admin)
                        SubmitButtonText = "Submit for Review";
                }
            }


            if (element.variableName == "Email" && (result.formTitle == "Person Registration" || result.formTitle == "Place/Group Registration")) {
                element.isRequired = true;
            }


            let containerBody = "";
            let canEdit = "true";

            //for textbox type variables
            if (element.variableType == "Text Box") {

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }

                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" ' + requiredMessage;
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="text"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-username-guid="' + element.variableViewModel.userNameVariableGuid + '"' + //onchange +
                    jqueryValidator +

                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  />" +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableType == "Free Text") {

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });

                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" ' + requiredMessage;
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<textarea id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables" rows="2" cols="50"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-username-guid="' + element.variableViewModel.userNameVariableGuid + '"' + //onchange +
                    jqueryValidator +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
                    JSON.stringify(PreviewPane_ValidationName) + "," +
                    JSON.stringify(PreviewPane_ValidationRegEx) + "," +
                    JSON.stringify(PreviewPane_ValidationMin) + "," +
                    JSON.stringify(PreviewPane_ValidationMax) + "," +
                    JSON.stringify(PreviewPane_ValidationErrorMessage) + ")'  >" + element.variableViewModel.variableSelectedValues + "</textarea>" +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableType == "Dropdown") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        if (element.variableViewModel.variableName != "EntGrp")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }

                //EntGroup default settings
                if (result.formTitle == "Person Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 2;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Participant Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 1;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Place/Group Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 3;
                    isDisabled = "disabled";
                }

                if (result.formTitle == "Project Registration" && element.variableViewModel.variableName === "EntGrp") {
                    element.variableViewModel.variableSelectedValues = 4;
                    isDisabled = "disabled";
                }

                var options = "";
                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                        options = options + "<option selected value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                    } else {
                        options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                    }
                }

                //---------for country/state dropdown-----------
                let variableTypeClass = "";
                let countryChange = "";
                if (element.variableViewModel.variableName == "Country" || element.variableViewModel.variableName == "State") {
                    options = "";
                    if (element.variableViewModel.variableName == "Country") {
                        variableTypeClass = "country-list";
                        countryChange = "onchange=Deployment.StateListByCountry(this)";
                        $.each(Deployment.AllCountries, function (key, entry) {
                            if (entry.guid == element.variableViewModel.variableSelectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    } else if (element.variableViewModel.variableName == "State") {
                        variableTypeClass = "state-list";
                        $.each(Deployment.AllStates, function (key, entry) {
                            if (entry.guid == element.variableViewModel.variableSelectedValues) {
                                options = options + "<option selected value='" + entry.guid + "'>" + entry.name + "</option>";
                            } else {
                                options = options + "<option value='" + entry.guid + "'>" + entry.name + "</option>";
                            }
                        });
                    }
                }
                //--------------------

                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + '"' +
                    'id="inputField-' + varIndex + '"' + isDisabled +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' onblur="Deployment.validateFreeText(this.value, this.id, [], [], [], [], [])"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-guid-id="' + element.variableId + '"' + countryChange +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    jqueryValidator +
                    ' name="inputField-' + varIndex + '">' +
                    '<option value="">Select</option>' +
                    options +
                    '</select>' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableType == "LKUP") {
                let variableTypeClass = "";

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        if (element.variableViewModel.variableName != "EntGrp")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }

                if (result.formTitle == "Project Linkage" && element.variableName == "LnkPro") {
                    if (element.variableViewModel.variableSelectedValues != "") {
                        isDisabled = "disabled";
                    }
                }



                var options = "";
                if (result.formTitle == "Project Linkage" && element.variableName == "LnkPro") {
                    var temp1 = element.variableViewModel.linkedProjectListWithGroupList;
                    var arrReportType = temp1.map(x => x.groupName);
                    var arrUniqueReportType = arrReportType.filter(function (itm, i) {
                        return i == arrReportType.indexOf(itm);
                    });

                    arrUniqueReportType.forEach(function (item) {
                        var ddlOG = $("<optgroup></optgroup>").attr("label", item);
                        temp1.filter(function (fItem) {
                            if (item == fItem.groupName) {
                                var txtSelected = fItem.projectId == element.variableViewModel.variableSelectedValues ? "selected" : "";
                                ddlOG.append($("<option " + txtSelected + ">" + fItem.projectName + "</option>").attr("value", fItem.projectId))
                            }
                        });
                        options += ddlOG[0].outerHTML;
                    });               
                } else {
                    for (var i = 0; i < element.variableViewModel.values.length; i++) {
                        if (result.formTitle == "Project Linkage") {
                            var thisProj = Deployment.ThisUserProjectList.find(function (d) { return d.guid == element.variableViewModel.values[i] });

                            if (typeof thisProj !== 'undefined') {
                                if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                                    options = options + "<option selected value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                                } else {
                                    options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                                }
                            }
                        } else {
                            if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                                options = options + "<option selected value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                            } else {
                                options = options + "<option value='" + element.variableViewModel.values[i] + "'>" + element.variableViewModel.variableValueDescription[i] + "</option>";
                            }
                        }

                    }

                }


                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6"> ' +
                    '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + '"' +
                    'id="inputField-' + varIndex + '"' + isDisabled +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-guid-id="' + element.variableId + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' onblur="Deployment.validateFreeText(this.value, this.id, [], [], [], [], [])"' +
                    jqueryValidator +
                    ' name="inputField-' + varIndex + '">' +
                    '<option value="">Select</option>' +
                    options +
                    '</select>' +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableType == "Checkbox") {

                let options = "";
                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    if (element.variableViewModel.values[i] == element.variableViewModel.variableSelectedValues) {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" data-variable-guid-id="' + element.variableId + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"     data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
                    } else {
                        options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" data-variable-guid-id="' + element.variableId + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"     data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" /><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
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

            else if (element.variableType == "Radio") {
                let options = "";
                for (var i = 0; i < element.variableViewModel.values.length; i++) {
                    options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.variableViewModel.values[i] + '"  data-variable-guid="' + element.variableViewModel.id + '" data-variable-required="' + element.isRequired + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.variableViewModel.variableValueDescription[i] + '</span></label>';
                }
                let isRequired = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    options +
                    helpTextSpan +
                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-valmsg-replace="true"></span>' +
                    '</div>';
                container.append(formField);
            }

            else if (element.variableType == "Numeric (Integer)") {

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        if (element.variableViewModel.variableName != "EntID")
                            isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }
                if (element.variableViewModel.variableName === "EntID") {
                    isDisabled = "disabled";
                    element.variableViewModel.question = result.formTitle == "Project Registration" ? "Project ID" : result.formTitle == "Participant Registration" ? "Participant ID" : "ID";
                }

                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }


                let numberFieldClassStart = '<div class="col-md-4 pl-0">';
                let numberFieldClassEnd = '</div>';
                let dataVarTypeName = ' data-variable-typename="Numeric (Integer)"';


                let lnt = Math.max(...PreviewPane_ValidationMax);
                lnt = lnt + "";

                if (PreviewPane_ValidationMax.length == 1) {
                    lnt = "999999999999999";
                }
                if (element.variableViewModel.variableName === "Phone") {
                    numberFieldClassStart = '';
                    numberFieldClassEnd = '';
                    dataVarTypeName = '';
                    lnt = "999999999999999";
                }


                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +

                    numberFieldClassStart +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="text"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-guid-id="' + element.variableId + '"' +
                    ' maxlength="' + lnt.length + '"' +
                    dataVarTypeName +
                    jqueryValidator +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
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

            else if (element.variableType == "Numeric (Decimal)") {

                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled1 = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled1 = "disabled";
                        canEdit = "false";
                    }
                }

                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
                }

                let isDisabled = "";

                if (element.variableViewModel.variableName === "EntID") {
                    isDisabled = "disabled";
                }
                let isRequired = "";
                let jqueryValidator = "";
                if (element.isRequired) {
                    isRequired = "<span class='text-danger'>*</span>";
                    let requiredMessage = "";
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + isRequired + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<div class="col-md-4 pl-0">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    ' name="inputField-' + varIndex + '" ' + isDisabled + isDisabled1 +
                    'type="text"' +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-can-edit="' + canEdit + '"' +
                    ' data-validationname="' + JSON.stringify(PreviewPane_ValidationName) + '"' +
                    ' data-validation-regex="' + JSON.stringify(PreviewPane_ValidationRegEx) + '"' +
                    ' data-validation-min="' + JSON.stringify(PreviewPane_ValidationMin) + '"' +
                    ' data-validation-max="' + JSON.stringify(PreviewPane_ValidationMax) + '"' +
                    ' data-validation-error-message="' + JSON.stringify(PreviewPane_ValidationErrorMessage) + '"' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-guid-id="' + element.variableId + '"' +
                    ' data-variable-typename="Numeric (Decimal)"' +
                    jqueryValidator +
                    "onblur='Deployment.validateFreeText(this.value, this.id," +
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

            else if (element.variableType == "Date") {
                let roleIndex = element.formVariableRoleViewModel.findIndex(function (obj) { return obj.roleGuidId === $("#UserProjectRole").val(); });
                let isDisabled = "";
                if (roleIndex > 0) {
                    if (element.formVariableRoleViewModel[roleIndex].canCreate == false) {
                        isFormDisabledCount++;
                        isDisabled = "disabled";
                        canEdit = "false";
                    }
                }


                let PreviewPane_ValidationName = [];
                let PreviewPane_ValidationRegEx = [];
                let PreviewPane_ValidationMin = [];
                let PreviewPane_ValidationMax = [];
                let PreviewPane_ValidationErrorMessage = [];
                for (var key in element.variableViewModel.variableValidationRuleViewModel) {
                    PreviewPane_ValidationName.push(element.variableViewModel.variableValidationRuleViewModel[key].validationName);
                    PreviewPane_ValidationRegEx.push(element.variableViewModel.variableValidationRuleViewModel[key].regEx);
                    PreviewPane_ValidationMin.push(element.variableViewModel.variableValidationRuleViewModel[key].min);
                    PreviewPane_ValidationMax.push(element.variableViewModel.variableValidationRuleViewModel[key].max);
                    PreviewPane_ValidationErrorMessage.push(element.variableViewModel.variableValidationRuleViewModel[key].validationMessage);
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
                    if (element.variableViewModel.requiredMessage) {
                        requiredMessage = "data-val-required='" + element.variableViewModel.requiredMessage + "'";
                    } else {
                        requiredMessage = "data-val-required='This field is required'";
                    }
                    jqueryValidator = 'data-val="true" data-val-required="' + requiredMessage + '"';
                }

                let dateformat = "DDMMYYYY";
                $.each(element.variableViewModel.variableValidationRuleViewModel, function (key, value) {
                    if (value.validationName == "Date") {
                        dateformat = "DDMMYYYY";
                    } else {
                        dateformat = value.validationName.replace("Date_", "");
                    }
                });
                let datepickerformat = Deployment.getDatepickerFormat(dateformat);
                if (dateformat == "YYYY" && element.variableViewModel.variableSelectedValues != "") {
                    element.variableViewModel.variableSelectedValues = "01-01-" + element.variableViewModel.variableSelectedValues;
                }

                if (element.variableViewModel.variableSelectedValues.trim() == "") {
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
                    element.variableViewModel.variableSelectedValues = "";
                }
                if (element.variableViewModel.variableSelectedValues == "Empty") {
                    element.variableViewModel.variableSelectedValues = "";
                }

                containerBody = `<div class ="form-group mb-2 date-input">
                    <div class ="row align-items-center lable-tl"><div class ="col-sm-6 text-right">
                                        <label class ="control-label">${element.variableViewModel.question}${isRequired}</label>
                                        </div><div class="col-sm-6">
                                        <a href="#"
                                        id="inputField-${varIndex}"
                                        class ="singledate form-control form-control-line dynamic-variables customdatepicker"
                                        data-type="combodate"
                                        data-value="${element.variableViewModel.variableSelectedValues}"
                                        value="${element.variableViewModel.variableSelectedValues}"
                                        data-format="${datepickerformat}"
                                        data-viewformat="${datepickerformat}"
                                        data-template="D / MMM / YYYY"
                                        data-pk="1"
                                        name="inputField-${varIndex}"  ${isDisabled}
                                        data-title="${element.variableViewModel.question}"
                                        data-variable-guid="${element.variableViewModel.id}"
                                        data-variable-required-message="${element.variableViewModel.requiredMessage}"
                                        data-variable-required="${element.isRequired}"
                                        data-variable-guid-id="${element.variableId}"
                                        data-max-date="0d"
                                        data-can-edit="${canEdit}"
                                        data-can-future-date = "${element.variableViewModel.canFutureDate}"
                                        data-variable-typename="Date">${element.variableViewModel.variableSelectedValues}</a>
                                        ${helpTextSpan}
                                        <span class ="field-validation-valid form-control-feedback" data-valmsg-for="inputField-${varIndex}" data-valmsg-replace="true"></span>
                                        </div></div></div>`;
            }
            else if (element.variableType == "Heading") {
                containerBody = '<div class="form-group mb-3">' +
                    element.questionText +
                    '</div>';
            }
            else if (element.variableType == "Other Text") {
                containerBody = '<div class="form-group mb-3">' +
                    '<p>' +
                    element.questionText +
                    '</p>' +
                    '</div>';
            }
            else if (element.variableType == "FileType") {
                let thisFieldId = "#inputField-" + varIndex;
                let thisFieldChangEvent = `onchange="App.setImage(this, '${thisFieldId}')"`;
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="file"' +
                    thisFieldChangEvent +
                    ' name="inputField-' + varIndex + '" ' +
                    ' accept="image/gif,image/jpeg,image/png, image/jpg"' +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-username-guid="' + element.variableViewModel.userNameVariableGuid + '"' +
                    ' data-file-base="' + element.variableViewModel.variableSelectedValues + '"' +
                    '  />' +

                    helpTextSpan +

                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }

            else if (element.variableType == "ColorPicker") {
                if (element.variableViewModel.variableSelectedValues == "") {
                    element.variableViewModel.variableSelectedValues = "#1f88e5"
                    if (element.variableName == "ProjectDisplayNameTextColour") { element.variableViewModel.variableSelectedValues = "#ffffff" }
                }
                containerBody = '<div class="form-group mb-2">' +
                    '<div class="row align-items-center lable-tl"><div class="col-sm-6 text-right">' +
                    '<label class="control-label">' + element.variableViewModel.question + '</label>' +
                    '</div><div class="col-sm-6">' +
                    '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                    'type="color"' +
                    ' name="inputField-' + varIndex + '" ' +
                    ' value="' + element.variableViewModel.variableSelectedValues + '"' +
                    ' placeholder="' + element.variableViewModel.question + '" ' +
                    ' data-variable-guid="' + element.variableViewModel.id + '"' +
                    ' data-variable-required="' + element.isRequired + '"' +
                    ' data-variable-required-message="' + element.variableViewModel.requiredMessage + '"' +
                    ' data-variable-username-guid="' + element.variableViewModel.userNameVariableGuid + '"'
                ' />' +

                    helpTextSpan +

                    '<span class="field-validation-valid form-control-feedback" data-valmsg-for="inputField-' + varIndex + '" data-username-msg-for="spn-' + element.variableName + '" data-valmsg-replace="true"></span>' +
                    '</div></div></div>';
            }
            let bodyTemplate = '<div class="row pll-5 pr-5"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';

            if (result.formTitle == "Participant Registration") {
                if (element.variableName == "MiddleName") {
                    bodyTemplate = '<div class="row pll-5 pr-5" id="divMiddName"><div class="col-md-12 col-sm-6" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div></div>';
                } else if (element.variableName == "NoMidNm") {
                    bodyTemplate = '<div class="col-md-12 col-sm-6 pt-2" id = "divDependent-' + varIndex + '"> ' + containerBody + '</div>';
                } else {
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
            if (result.formTitle == "Participant Registration" && element.variableName == "NoMidNm") { $("#divDependent-" + varIndex).appendTo("#divMiddName"); }
            if (element.variableType == "Date") {
                $('.singledate').editable({
                    mode: 'inline',
                    maxDate: 0,
                });
            }
        });
        //parent variable display logic
        varIndex = 0;
        result.variables.forEach(function (element) {
            varIndex++;
            if (element.dependentVariableId != null && element.dependentVariableId != "") {
                if ($("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").prop('type') == "checkbox") {
                    if ($("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").prop('checked'))
                        $("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").val("1");
                    else
                        $("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").val("0")
                }
                $("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").attr('onchange', 'Deployment.setChildVariables(this)');
                $("#frm-1 ").find("[data-variable-guid='" + element.variableViewModel.id + "']").attr('data-parent-variable-id', element.dependentVariableId);
                $("#frm-1 ").find("[data-variable-guid='" + element.variableViewModel.id + "']").attr('data-parent-variable-response', element.responseOption);

                if ($("#frm-1 ").find("[data-variable-guid-id='" + element.dependentVariableId + "']").val() == element.responseOption) {
                    $("#divDependent-" + varIndex).removeClass("hide");
                    $("#inputField-" + varIndex).attr("data-variable-required", "true");
                } else {
                    $("#divDependent-" + varIndex).addClass("hide");
                    $("#inputField-" + varIndex).attr("data-variable-required", "false");
                }
            }
        });
        let isFormDisabledButton = "";
        if (isFormDisabledCount > 0) {
            isFormDisabledButton = "data-fields-for-review-count=" + isFormDisabledCount;
        }

        if (SubmitButtonText == "Submit") {
            resultFooter.append($("<button type='button' id='btnUndoSubmit_ActivityformVariableContainer' class='hide btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.UndoSubmitActivityForm(this)\">Undo Submit</button>"));
            resultFooter.append($("<button type='button' id='btnSubmit_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this, 4)\">" + SubmitButtonText + "</button>"));
            resultFooter.append($("<button type='button' id='btnSave_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this)\">Save</button>"));
        } else {
            resultFooter.append($("<button type='button' id='btnUndoSubmit_ActivityformVariableContainer' class='hide btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.UndoSubmitActivityForm(this)\">Undo Submit</button>"));
            resultFooter.append($("<button type='button' id='btnSubmit_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this)\">" + SubmitButtonText + "</button>"));
            resultFooter.append($("<button type='button' id='btnSave_ActivityformVariableContainer' class='btn btn waves-effect waves-light btn-info' " + isFormDisabledButton + " onclick=\"Deployment.saveActivityform(this)\">Save</button>"));
        }
        resultFooter.append($("<button type=\"button\" class=\"btn btn-danger waves-effect waves-light\" data-dismiss=\"modal\">Cancel</button>"));
        let footerModifiedBy = "<input type='text' disabled class='form-control' value='" + result.modifiedByString + "'>";
        let footerModifiedDate = "<input type='text' disabled class='form-control' value='" + result.modifiedDateString + "'>";
        var dotcolon = 'dotcolon';
        resultFooterModifiedby.append("Modified By <span class=" + dotcolon + ">:</span>");
        resultFooterModifiedby.append($(footerModifiedBy));
        resultFooterModifiedby.append("Modified Date: ");
        resultFooterModifiedby.append($(footerModifiedDate));
        $("#hdnFormDataEntryTypeIsNew").val(result.isNewForm);
        $('#activity-form-variables-model').modal('show');
        $("input:radio[name=RadioButtonValue]:first").attr('checked', true);
        $('#search-for-tabular-data').css('max-width', 900);
        Deployment.FormVariableContainer_onBlurEvent();
        Deployment.executeEventFirst();

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
                    Deployment.FormVariableContainer_onBlurEvent_Each();
                });
            }, 300);
        });
        $("#activity-form-variables-model").find("[data-dismiss]").on("click", function () {
            try {
                Deployment.UpdateURL_AfterDataEntry();
            } catch (errURL) {
            }
        });
    }, methodeType);
}

Deployment.GetSummaryPageScheduledActivity = function (projectId) {
    App.postData(App.ApiUrl + 'Scheduling/GetAllScheduledActivityByProjectId/' + projectId, {}, function (result) {
        let dropdown = $('#ActivityType');
        dropdown.empty();
        dropdown.append('<option selected="true" disabled>Select Activity</option>');
        dropdown.prop('selectedIndex', 0);
        $.each(result, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry.activityId).text(entry.activityName));
        });
    }, 'GET');
}

Deployment.GetSummaryPageUserList = function () {

    App.postData(App.ApiUrl + 'user/GetProjectAllUsers/' + $("#ProjectId").val(), {}, function (result) {
        let dropdown = $('#CompletedByUser');
        dropdown.empty();
        dropdown.append('<option selected="true" value="">Select User</option>');
        dropdown.prop('selectedIndex', 0);
        $.each(result, function (key, entry) {
            if (App.IsTestSite === true) {
                if (entry.userTypeId == 3)
                    dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.firstName + " " + entry.lastName));
            } else {
                if (entry.userTypeId != 3)
                    dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.firstName + " " + entry.lastName));
            }
        });
    }, 'GET');
}

$(".number-dd").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
$(".number-mm").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});
$(".number-yyyy").on("keypress keyup blur", function (event) {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

Deployment.AddActivity = function () {
    $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('');
    $('#ActivityType').removeClass("input-validation-error");
    $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('');
    $('#CompletedByUser').removeClass("input-validation-error");


    var aDate = $("#ActivityDate").text().split("/");
    var dd = aDate[0];
    var mm = aDate[1];
    var yy = aDate[2];

    if (dd > 31) { App.showError("Please enter a valid date."); return false; }
    if (mm > 12) { App.showError("Please enter a valid date."); return false; }

    var activityDate = mm + "-" + dd + "-" + yy;
    if (new Date(mm + "-" + dd + "-" + yy) < new Date($("#EntityCreatedDate").val())) {
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
        "ActivityId": $("#ActivityType").val(),
        "ActivityCompletedByUser": $("#CompletedByUser").val(),
        "ActivityDate": activityDate,
        "IsActivityAdded": true,
        "ProjectId": $("#ProjectId").val(),
        "PersonEntityId": $("#PersonEntityId").val(),
    };

    let url = "Activity/AddSummaryPageActivity";
    if (App.IsTestSite === true) {
        url = "Review/AddSummaryPageActivity_SQL";
    }
    let methodeType = "POST"; 
    App.postData(App.ApiUrl + url, sendData, function (result) {       
        Deployment.refreshScheduledActivities(result);
        App.showSuccess("Activity added successfully.");


        location.reload(true);

    }, methodeType);
}

Deployment.refreshScheduledActivities = function (result) {

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

    let replacedCompletedBy = $(template).html().replace('scheduled_activities_completedby', $("#CompletedByUser option:selected").text());
    $(template).html(replacedCompletedBy);


    template.find('.timeline-title').text(result.activityName);
    template.attr('data-activity-date', result.activityDate);

    for (var i = 0; i < result.forms.length; i++) {
        let form = result.forms[i];
        var contentBody = '<div class="timeline-body">' +
            '<div class="row"><div class="col-sm-4"><small class ="font-16">' + form.formTitle + '</small></div><div class="col-sm-4"> <small class="text-warning ml-5 link" id="formStatus-' + form.id + '-' + result.guid + '"   data-summarypage-userentity="' + result.id + '"   onclick="Deployment.OpenActivityForm1(\'' + result.personEntityId + '\', \'' + form.id + '\', \'' + result.activityId + '\', this)">Draft</small></div></div>' +
            '</div>';
        template.find('.timeline-panel').append(contentBody);
    }
    template.attr("id", "activityContainer-" + result.id);
    template.find("#datemonth-new").attr("id", "datemonth-" + result.id);
    template.find("#completedbyname-new").attr("id", "completedbyname-" + result.id);
    let acM = mmm + 1;
    let acD = dd;
    let acY = activityDate.getFullYear();
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-date", acY + "-" + acM + "-" + acD);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-activityid", result.activityId);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-summarypage-activityid", result.id);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-completedby", result.activityCompletedByUser);
    template.find("#btnEditSummaryPageActivity-new").attr("data-editactivity-entity-number", result.personEntityId);
    template.find("#btnEditSummaryPageActivity-new").attr("id", "btnEditSummaryPageActivity-" + result.id);
    template.find(".btn-danger").attr("onclick", "Deployment.DeleteSummaryPageActivity('" + result.id + "', '" + result.activityName + "')");
    container.append(template);
    $('#scheduledActivitiesContainer').sortChildren(Deployment.ddmmyyyClassSort);
}
Deployment.ddmmyyyClassSort = function (a, b) {

    var A = new Date(a.getAttribute('data-activity-date')),
        B = new Date(b.getAttribute('data-activity-date'))
    return A < B ? 1 : -1
}
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
Deployment.FormVariableContainer_onBlurEvent = function () {
    $('#activityformVariableContainer select, #activityformVariableContainer input:text, #activityformVariableContainer input:checkbox, #activityformVariableContainer a, #activityformVariableContainer textarea, #activityformVariableContainer input:file').on('blur change keyup', function () {

        if ($(this).prop('tagName').toLowerCase() == "a") {
            setTimeout(function () {
                Deployment.FormVariableContainer_onBlurEvent_Each();
            }, 300);
        } else {
            Deployment.FormVariableContainer_onBlurEvent_Each();
        }

    });
}
Deployment.FormVariableContainer_onBlurEvent_Each = function () {
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
            if ($("#projectStaffMemberRoleName").val() != App.DefaultRoleNameStringEnum.System_Admin)
                $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");

        }
    });
}
//===============================================================
//  manage form status
//===============================================================
Deployment.executeEventFirst = function () {
    $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSave_ActivityformVariableContainer").addClass("hide");
    $("#btnReset_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-info").removeClass("btn-warning").text("Submit");

    let isSubmitted = false;
    if (Deployment.StatusId != "")
        if ($("#" + Deployment.StatusId).text() == "Submitted" || $("#" + Summary.StatusId).text().trim().toLowerCase() == "submitted for review")
            isSubmitted = true;


    setTimeout(function () {
        let frmTitle = $("#search-variable-result-model-title").text();
        let cntVisibleVar = 0;
        $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer input[type=color]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer textarea:visible').each(function (index, element) {
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

            if (_thisElementValue == "") {
                if ($("#projectStaffMemberRoleName").val() != App.DefaultRoleNameStringEnum.System_Admin)
                    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
            }
            if (isSubmitted) {
                $(element).attr("disabled", true);
                $(element).addClass("disabled-button-cursor");
                try {
                    if (fieldTag.toLowerCase() == "a") {
                        $(element).css("pointer-events", "none");
                        $(element).attr("readonly", "readonly");
                    }
                } catch (e) {                   
                }
            }
        });
        if (cntVisibleVar === 0) {
            $("#activityformVariableContainer").append("You don't have permission to view this form components");
            $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSave_ActivityformVariableContainer").addClass("hide");
            $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
        }

        if (isSubmitted) {
            $("#btnUndoSubmit_ActivityformVariableContainer").removeClass("hide");
            $("#btnSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSave_ActivityformVariableContainer").addClass("hide");
        }
        else {
            $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
            $("#btnSubmit_ActivityformVariableContainer").removeClass("hide");
            $("#btnSave_ActivityformVariableContainer").removeClass("hide");
        }
    }, 300);
}



Deployment.UndoSubmitActivityForm = function () {
    $("#btnUndoSubmit_ActivityformVariableContainer").addClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").removeClass("hide");
    $("#btnSave_ActivityformVariableContainer").removeClass("hide");
    $("#btnSubmit_ActivityformVariableContainer").addClass("btn-info").removeClass("btn-warning").text("Submit");
    let frmTitle = $("#search-variable-result-model-title").text();
    $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer input[type=file]:visible, #activityformVariableContainer input[type=color]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible').each(function (index, element) {
        let _thisElementValue = "";
        _thisElementValue = $(element).val();
        if ($(element).prop('type') == "checkbox") {
            _thisElementValue = $('input[name="' + $(element).prop('id') + '"]:checked').map(function () { return this.value; }).get().join(',');
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
        if ($(element).prop('type') == "select-one") {
            if (_thisElementValue != "")
                $(element).removeClass('summary-page-missing-data-error');
            else
                $(element).addClass('summary-page-missing-data-error');
        }
        if ($(element).attr("data-variable-guid") == "3" || $(element).attr("data-variable-guid") == "4" || $(element).attr("data-variable-guid") == "52") {
            return;
        }
        if ($(element).attr("data-can-edit") == "false") {
            return;
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
        if ($(element).val() == "") {
            if ($("#projectStaffMemberRoleName").val() != App.DefaultRoleNameStringEnum.System_Admin)
                $("#btnSubmit_ActivityformVariableContainer").addClass("btn-warning").removeClass("btn-info").text("Submit for Review");
        }
        $(element).removeAttr("disabled");
        $(element).removeClass("disabled-button-cursor");
    });
}


















Deployment.DeploymentActivity = function (_this) {

    var e = $(_this).attr("data-deployment-status");
    var isDefault = $(_this).attr("data-is-default-variable");


    $('ul.variable-categories > li > a').removeClass('library-li-active');




    if (e === "4" && isDefault != "0") {
        let guid = $(_this).attr("data-id");
        $('#btnInactivateActivity').attr('onClick', 'Deployment.InactivateActivity("' + guid + '")');
        $("#btnInactivateActivity").removeClass("hide");

        $('ul.variable-categories > li[data-activity-id = ' + $(_this).attr("data-id") + '] > a').addClass('library-li-active');
    } else {
        $("#btnInactivateActivity").addClass("hide");
        $('#btnInactivateActivity').removeAttr('onClick');
    }
}


//==============================================================
// deploy activities (added on 29-03-2019)
//==============================================================
Deployment.deployActivities = function () {
    var DeploymentStatus = 4;
    let url = "ProjectDeploy/DeployProject/" + $("#ProjectId").val()
    //let url = "Scheduling/PushScheduledActivities/" + DeploymentStatus;
    let methodeType = "POST";
    var sendData = [];

    let deployedActivitiesList = [];
    $('#activitiesContainer div.row').each(function (i, row) {

        let guid = $(row).attr('data-id');
        var scheduling = Deployment.dbContextScheduledActivities.find(function (d) { return d.activityId == guid });
        sendData.push(scheduling.guid);

        deployedActivitiesList.push(guid);
    });

    if (sendData.length === 0) {
        App.showError("Please drag and drop activities to deploy.");
        return false;
    }

    App.postData(App.ApiUrl + url, sendData, function (result) {

        $.each(deployedActivitiesList, function (i, row) {

            $("#activityStatusLabel-" + row).text("(Active)");
            $("#activityStatusLabel-" + row).parent().attr("data-deployment-status", 4);
        });

        App.showSuccess("Project deployed successfully.");
        Deployment.EnableDisableDeploymentChecklist();
    }, methodeType);


}

Deployment.InactivateActivity = function (e) { 
    var arrLength = $('#activitiesContainer div.row').length;
    if (arrLength > 1) {
        var newStr = '';
        if (arrLength == 2) {
            newStr = Deployment.ScheduledActivitiesNameList.join(" and ")
        } else {
            var nameList = Deployment.ScheduledActivitiesNameList.join(", ")
            var lastCommaIndex = nameList.lastIndexOf(",");
            newStr = nameList.substr(0, lastCommaIndex) + " and" + nameList.substr(lastCommaIndex + 1);
        }
        $("#manage-confirm").find(".sf-submit-info").html("Are you sure you want to inactive the " + newStr + " activities?");
    } else
        $("#manage-confirm").find(".sf-submit-info").html("Are you sure you want to inactive the " + Deployment.ScheduledActivitiesNameList.join() + " activity?");
    $("#manage-confirm").find(".sf-submit-button").attr("onclick", "Deployment.InactivateActivityConfirmed(\"" + e + "\")");
    $('#manage-confirm').modal();



}
Deployment.InactivateActivityConfirmed = function (e) {

    var lst = e.split(',');
    let url = "Scheduling/InactivateActivity/" + $("#ProjectId").val();

    var sendData = [];

    sendData = lst;

    //App.postData(App.ApiUrl + 'activity/' + guid, {}, function (result) {
    App.postData(App.ApiUrl + url, sendData, function (result) {
        if (result) {
            $.each(sendData, function (index, value) {
                $("#activityStatusLabel-" + value).text("");
                $(".list-menu").find("li > a[data-id='" + value + "']").attr("data-deployment-status", 3);
                $('ul.variable-categories > li > a').removeClass('library-li-active');

            });
            $("#btnInactivateActivity").addClass("hide");
            $('#btnInactivateActivity').removeAttr('onClick');
            App.showSuccess("Activity " + Deployment.ScheduledActivitiesNameList.join() + " inactivated successfully.");


            Deployment.EnableDisableDeploymentChecklist();
        }
    }, 'POST');
}




Deployment.ThisUserProjectList = {};
Deployment.GetAllProjectByUserId = function () {
    App.postData(App.ApiUrl + 'Project/GetAllProjectByUserId/', {}, function (result) {
        Deployment.ThisUserProjectList = result;
    }, 'GET');
}








Deployment.PushActivitiesToTestSite = function () {
    var DeploymentStatus = 3;
    let url = "ProjectDeploy/PushTestProject/" + $("#ProjectId").val();
    let methodeType = "POST";
    var sendData = [];

    $('#activitiesContainer div.row').each(function (i, row) {
        let guid = $(row).attr('data-id');
        var scheduling = Deployment.dbContextScheduledActivities.find(function (d) { return d.activityId == guid });
        sendData.push(scheduling.guid);
    });

    if (sendData.length === 0) {
        App.showError("Please drag and drop activities to push.");
        return false;
    }

    App.postData(App.ApiUrl + url, sendData, function (result) {
        App.showSuccess("Activities push successfully.");
        window.open(
            Deployment.TestSiteWebUrl,
            //"http://uds-test.cloud.monash.edu/",
            '_blank' // <- This is what makes it open in a new window.
        );
    }, methodeType);
}



Deployment.EditSummaryPageActivity = function (_this) {

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
    } catch (e) { }

    //$("#ActivityDate").val($(_this).attr("data-editactivity-date"));
    $("#ActivityDate").attr("data-value", $(_this).attr("data-editactivity-date"));
    $("#ActivityDate").text(d + "/" + m + "/" + y);

    //"data-editactivity-entity-number"
    //value="${element.selectedValues}"

    $("#btnAddEditSummaryPageActivity").addClass("rounded-circle");
    $("#btnAddEditSummaryPageActivity").find(".fa-plus").removeClass("fa-plus").addClass("fa-check");
    //$("#ActivityType").attr("disabled", "disabled");
    $("#ActivityType").addClass("hide").attr("disabled", "disabled");
    $("#ActivityTypeNameDisplayOnly").removeClass("hide").attr("disabled", "disabled");

    let summarypageActId = $(_this).attr("data-editactivity-summarypage-activityid");
    $("#btnAddEditSummaryPageActivity").attr("onclick", "Deployment.EditSummaryPageActivityPost('" + summarypageActId + "' )");
}

Deployment.EditSummaryPageActivityPost = function (_id) {
    $('span[data-valmsg-for="ActivityType"]').addClass("field-validation-error").text('');
    $('#ActivityType').removeClass("input-validation-error");
    $('span[data-valmsg-for="CompletedByUser"]').addClass("field-validation-error").text('');
    $('#CompletedByUser').removeClass("input-validation-error");


    var aDate = $("#ActivityDate").text().split("/");
    var dd = aDate[0];
    var mm = aDate[1];
    var yy = aDate[2];

    if (dd > 31) { App.showError("Please enter a valid date."); return false; }
    if (mm > 12) { App.showError("Please enter a valid date."); return false; }

    var activityDate = mm + "-" + dd + "-" + yy;//new Date(mm + "-" + dd + "-" + yy);


    //if (new Date(mm + "-" + dd + "-" + yy) < new Date($("#EntityCreatedDate").val())) {
    //    App.showError("Activity date should greater than entity registration date.");
    //    return false;
    //}

    //if (!$('#frmAddSummaryPageActivity').valid()) return;


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
        "ActivityId": $("#EditedActivityType").val(),
        "ActivityCompletedByUser": $("#CompletedByUser").val(),
        "ActivityDate": activityDate,
        "IsActivityAdded": true,
        "ProjectId": $("#ProjectId").val(),
        "PersonEntityId": $("#PersonEntityId").val(),
    };

    let url = "Activity/EditSummaryPageActivity";
    if (App.IsTestSite === true) {
        url = "Review/EditSummaryPageActivity_SQL";
    }   
    let methodeType = "POST";    
    App.postData(App.ApiUrl + url, sendData, function (result) {
     
        App.showSuccess("Activity updated successfully.");
        location.reload(true);
        let activityDate = new Date(result.activityDate);
        let dd = activityDate.getDate();
        let mmm = activityDate.getMonth();
        var month_names_array = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        $("#datemonth-" + _id).html(dd + "<br>" + month_names_array[mmm]);
        $("#completedbyname-" + _id).text("Staff: " + result.activityCompletedByUserName);
        $('#scheduledActivitiesContainer').sortChildren(Deployment.ddmmyyyClassSort);
        $("#formType").removeClass("form-badge-edit").addClass("form-badge");
        $("#formType").find(".text-uppercase").text("NEW");
        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-activityid", result.activityId);
        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-completedby", result.activityCompletedByUser);
        let mnth = mmm + 1;
        $("#btnEditSummaryPageActivity-" + _id).attr("data-editactivity-date", activityDate.getFullYear() + "-" + mnth + "-" + dd);
        $("#btnAddEditSummaryPageActivity").removeClass("rounded-circle");
        $("#btnAddEditSummaryPageActivity").find(".fa-check").removeClass("fa-check").addClass("fa-plus");
        $("#btnAddEditSummaryPageActivity").attr("onclick", "Deployment.AddActivity()");
        $("#ActivityTypeNameDisplayOnly").addClass("hide").attr("disabled", "disabled");
        $("#ActivityType").removeClass("hide").removeAttr("disabled");
        $('#ActivityType').prop('selectedIndex', 0);
        $('#CompletedByUser').prop('selectedIndex', 0);
        $('#EditedActivityType').val("");
    }, methodeType);
}


Deployment.DeleteSummaryPageActivity = function (id, name) {

    $(".activity-delete-confirm-message").html("Are you sure want to remove " + name + "  from summary page?");
    $("#summaryactivity-delete-confirm").modal("show");

    $("#butDeleteConfirmed").attr("onclick", "Deployment.DeleteSummaryPageActivityPost('" + id + "' )");
}
Deployment.DeleteSummaryPageActivityPost = function (_id) {

    let url = "Activity/DeleteSummaryPageActivity/" + _id;
    if (App.IsTestSite === true) {
        url = "Review/DeleteSummaryPageActivity_SQL/" + _id;
    }
    let methodeType = "Delete";
    App.postData(App.ApiUrl + url, {}, function (result) {       
        App.showSuccess("Activity removed form summary page successfully.");
        location.reload(true);
        $("#butDeleteConfirmed").removeAttr("onclick");

        $("#activityContainer-" + _id).remove();


    }, methodeType);
}

Deployment.checkEntityLinkedProject = function (projectId, entityId) {

    let isAsync = false;
    let response = null;

    let url = "Project/CheckEntityLinkedProject/" + projectId + "/" + entityId;
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
Deployment.isValidDataEntryForm = function (e, isRequired) {
    let isValid = true;
    $('#activityformVariableContainer input[type=text]:visible, #activityformVariableContainer select:visible, #activityformVariableContainer a:visible, #activityformVariableContainer input[type=checkbox]:visible, #activityformVariableContainer textarea:visible').each(function (index, element) {
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

Deployment.EnableDisableDeploymentChecklist = function () {

    let droppedActivityCount = 0;
    try {
        droppedActivityCount = $('#activitiesContainer div.row').length;
        if (droppedActivityCount === 0) {
            $(".testing-checklist input:checkbox").each(function () {
                $(this).attr("disabled", true);
                $(this).prop('checked', false);
            });
            if ($('#activitiesContainer').find("#drag-message").length == 0) {
                $('#activitiesContainer').append('<h2 id="drag-message" class="text-center">Drag and drop your activities here</h2>');
            }
            $("#btnInactivateActivity").addClass("hide");
        }
        else {
            let isAnyActivityActive = false;
            Deployment.ScheduledActivitiesNameList = [];
            let activityActiveList = [];
            $('#activitiesContainer div.row').each(function (index, row) {
                let id = $(row).attr("data-id");
                let activityStatus = $("#scheduledActivitiesList").find("[data-activity-id='" + id + "']").find("[data-deployment-status]").attr("data-deployment-status");
                var isDefault = $("#scheduledActivitiesList").find("[data-activity-id='" + id + "']").find("[data-is-default-variable]").attr("data-is-default-variable");
                if (activityStatus == 4 && isDefault != "0") {
                    isAnyActivityActive = true;
                    activityActiveList.push(id);
                    try {
                        let activityName = $("#scheduledActivitiesList").find("[data-activity-id='" + id + "']").find("[data-text]").attr("data-text-display");
                        Deployment.ScheduledActivitiesNameList.push(activityName);
                    } catch (e) { }
                }
            })
            if (isAnyActivityActive) {
                $("#btnInactivateActivity").removeClass("hide");
                $('#btnInactivateActivity').attr('onClick', 'Deployment.InactivateActivity("' + activityActiveList + '")');
                $(".testing-checklist input:checkbox").each(function () {
                    $(this).attr("disabled", true);
                    $(this).prop('checked', false);
                });
            } else {
                $("#btnInactivateActivity").addClass("hide");
                $('#btnInactivateActivity').removeAttr('onClick');
                $(".testing-checklist input:checkbox").each(function () {
                    $(this).removeAttr("disabled");
                });
            }
        }
    } catch (e) {        
    }
}

Deployment.removeDeploymentDragDropActivity = function (btnRemove) {
    $(btnRemove).parent().parent().remove();
    Deployment.EnableDisableDeploymentChecklist();
};
Deployment.validateProjectLinkageForm = function (e) {
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
            } catch (e) { }

            if (!App.checkStartDateAndEndDate(joinDateValue, leftDateValue)) {
                $(dateLeftProject).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(dateLeftProject).prop('id') + '"]').addClass("field-validation-error").text("project left date should be greater than project join  date");
                success = false;
            }
        }
    }
    return success;
}



Deployment.CallEntityFormDataEntryModel = function () {
    let query_string = window.location.search;
    let search_params = new URLSearchParams(query_string);
    let entityId = search_params.get('participant');
    let formId = search_params.get('formId');
    let projectId = search_params.get('guid');
    let activityId = search_params.get('entityActivity');
    let summaryPageActivityId = search_params.get('entitySummaryPageActivityId');
    let projectVersion = search_params.get('entityProjectVersion');
    let fname = search_params.get('fname');

    if (activityId === null && summaryPageActivityId === null) {
        return false;
    }
    Deployment.OpenActivityForm1(entityId, formId, activityId, summaryPageActivityId, fname);    
}


Deployment.UpdateURL_AfterDataEntry = function () {

    let urlWithParam = window.location.search.toLowerCase();
    let urlActionMethod = window.location.pathname;
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
    } 
}