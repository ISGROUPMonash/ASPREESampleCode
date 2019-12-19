
var Schedule = {};
let otherActivity = null;
let specifiedActivity = null;
Schedule.activityStatusId = 0;
Schedule.ActivityId = "";
Schedule.UnscheduledActivityList = [];
Schedule.CurrentActivityEntityType = [];
$(function () {
    $('#search-var-text').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            Schedule.search(true);
            return false;
        }
    });

    $('#frm-schedule-activity input:text, #frm-schedule-activity select').on('blur', function () {
        let guid = $('#ActivityGuid').val();
        let row = $('#formContainer div.row[data-id="' + guid + '"]');

        row.attr('data-activity-schedule-type', $('#ScheduleType').val());
        row.attr('data-start-date', $('#StartDate').val());
        row.attr('data-end-date', $('#EndDate').val());
        row.attr('data-repeatation-offset', $('#RepeatationOffset').val());
        row.attr('data-question1-schedule-value', $('#ddlScheduleStart').val());
        row.attr('data-question2-schedule-value', $('#ddlActivityCreated').val());
        row.attr('data-question3-schedule-value', $('#ddlEntityType').val());
        row.attr('data-question4-schedule-value', $('#ddlScheduledRole').val());
        row.attr('data-question5-schedule-value', $('#ddlRegardlessScheduledRole').val());

    });

    if ($("#ScheduledToBeCompleted").val() == "") {
        $('.divBased_on_scheduled_date').addClass('hide');
    }

});

//not usable
Schedule.onScheduleTypeChange = function (ddl) {
    $('.form-group1, .form-group2, .form-group3').addClass('hide');
    $('.form-group' + $(ddl).val()).removeClass('hide');
}

//not usable
Schedule.getActivity = function (guid, name) {
    App.postData(App.ApiUrl + 'activity/' + guid, {}, function (result) {
        Schedule.activity = result;

        $('#Guid').val(result.guid);
        $('#ActivityName').val(result.activityName);
        $('#ActivityCategory').val(name);
        $('#CurrentRepeat').val(result.repeatationCount);
        if (result.startDate) {
            $('#StartDate').val(moment(result.startDate).format('MM/DD/YYYY'));
        }
        if (result.endDate) {
            $('#EndDate').val(moment(result.endDate).format('MM/DD/YYYY'));
        }
        $('#RepeatationOffset').val(result.repeatationOffset);
        $('#ScheduleType').val(result.scheduleType).change();
    }, 'GET');
};

//not usable
Schedule.save1 = function () {
    if ($('#frm-schedule-activity').valid()) {
        var sendData = {};
        sendData.ScheduleActivityList = [];
        $('#formContainer div.row').each(function (i, row) {

            let guid = $(row).attr('data-id');
            let activityCategory = $(row).attr('data-category');
            let activityName = $(row).attr('data-name');
            let repeatationCount = $(row).attr('data-repeatation-count');
            let activityScheduleType = $(row).attr('data-activity-schedule-type');
            let startDate = $(row).attr('data-start-date');
            let endDate = $(row).attr('data-end-date');
            startDate = App.convertStringToDate(startDate);
            endDate = App.convertStringToDate(endDate);
            let repeatationOffset = $(row).attr('data-repeatation-offset');
            let activityCategoryGuid = $(row).attr('data-category-guid');
            if (typeof $(row).attr('data-activity-schedule-type') == "undefined") {
                App.showSuccess('please schedule activity ' + activityName);
                return false;
            }
            sendData.ScheduleActivityList.push({
                "Guid": guid,
                "ActivityName": activityName,
                "ActivityCategoryId": activityCategoryGuid,
                "ScheduleType": activityScheduleType,
                "RepeatationOffset": repeatationOffset,
                "StartDate": startDate,
                "EndDate": endDate,
                "RepeatationCount": 0,
                "RepeatationType": 0,

            });
        });
        App.postData(App.ApiUrl + "Activity/ScheduleActivityList", sendData, function (result) {
            App.redirectSuccess("Preview generated successfully", '/ProjectBuilder/PreviewScheduling/' + $('#ProjectId').val());
        }, 'Post');
    }
}

Schedule.setCurrentForm = function (guid, name, category, categoryName, repeatationCount, categoryGuid, isDefaultVariable) {
    $("#ActivityName").val(name);
    Schedule.ActivityId = guid;
    Schedule.currentForm = {};
    Schedule.currentForm.guid = guid;
    Schedule.currentForm.name = name;
    Schedule.currentForm.category = category;
    Schedule.currentForm.categoryName = categoryName;
    Schedule.currentForm.repeatationCount = repeatationCount;
    Schedule.currentForm.categoryGuid = categoryGuid;
    Schedule.currentForm.isDefaultVariable = isDefaultVariable;
}

// not usable (drag and drop)
Schedule.addForm = function (guid, name, repeatationCount, categoryName, categoryGuid, form, isDefaultVariable) {
    var container = $('#formContainer');

    if ($('div.row[data-id="' + guid + '"]', container).length > 0) return; // prevent duplicate variable entry

    var template = $('#formTemplate').children().clone();
    template.find('.var-label').text(name);
    template.find('.field-permision').selectpicker();
    if (form) {
        template.find('.field-permision').selectpicker('val', form.roles);
    }
    template.attr('data-id', guid);

    template.attr('data-category', categoryName);
    template.attr('data-name', name);
    template.attr('data-repeatation-count', repeatationCount);
    template.attr('data-category-guid', categoryGuid);


    template.attr('data-start-date', Schedule.SetDate());
    template.attr('data-end-date', Schedule.SetDate());
    template.attr('data-repeatation-offset', 1);
    template.attr('data-activity-schedule-type', 1);

    template.attr('data-isdefault-activity-type', isDefaultVariable);





    $('#drag-message').remove();
    container.append(template);
    container.sortable('refresh');
}

//not usable
Schedule.clearScheduleActivity = function () {
    $('#ActivityName').val('');
    $('#ActivityCategory').val('');
    $('#CurrentRepeat').val('');
    $('#ActivityGuid').val('');
    $('#ActivityCategoryGuid').val('');
}

//not usable
Schedule.scheduleActivity = function (e) {

    var row = $(e).parents('div.row:first');

    $('#ActivityGuid').val(row.attr('data-id'));
    $('#ActivityName').val(row.attr('data-name'));
    $('#ActivityCategory').val(row.attr('data-category'));
    $('#CurrentRepeat').val(row.attr('data-repeatation-count'));
    $('#ActivityCategoryGuid').val(row.attr('data-category-guid'));
    $('#ScheduleType').val(row.attr('data-activity-schedule-type'));
    $('#StartDate').val(row.attr('data-start-date'));
    $('#EndDate').val(row.attr('data-end-date'));
    $('#RepeatationOffset').val(row.attr('data-repeatation-offset'));
    $('.form-group1, .form-group2, .form-group3').addClass('hide');
    $('.form-group' + row.attr('data-activity-schedule-type')).removeClass('hide');
}

Schedule.scheduleActivity_New = function (e) {
    var row = $(e).parents('div.row:first');
    $('#ActivityGuid').val(row.attr('data-id'));
    $('#ActivityName').val(row.attr('data-name'));
    $('#ActivityCategory').val(row.attr('data-category'));
    $('#CurrentRepeat').val(row.attr('data-repeatation-count'));
    $('#ActivityCategoryGuid').val(row.attr('data-category-guid'));
    $('#ScheduleType').val(row.attr('data-activity-schedule-type'));
    $('#StartDate').val(row.attr('data-start-date'));
    $('#EndDate').val(row.attr('data-end-date'));
    $('#RepeatationOffset').val(row.attr('data-repeatation-offset'));
}

//not usable
Schedule.SetDate = function () {
   
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    return today = dd + '-' + mm + '-' + yyyy;
}

//not usable
Schedule.removeExistingActivity = function (e, guid) {
    Schedule.clearScheduleActivity();
    App.postData(App.ApiUrl + "Activity/RemoveScheduledActivity/" + guid, {}, function (result) {
        App.showSuccess('Activity Scheduling removed successfully.');
        $(e).parent().parent().remove();
    }, 'GET', true);
}

//not usable
Schedule.savePreviewScheduling = function (guids) {
    var listGuid = [];
    $(".guid-list").each(function () {
        listGuid.push($(this).val());
    });
    App.postData(App.ApiUrl + "Activity/SavePreviewScheduledActivity/" + listGuid.toString(), {}, function (result) {
        App.redirectSuccess("Preview Saved successfully", '/ProjectBuilder/Deployment/' + $('#ProjectId').val());
    }, 'GET', true);
}

//=====================================================
//      get all entity types
//=====================================================
Schedule.GetAllEntityTypes = function () {
    App.postData(App.ApiUrl + 'EntityType/', {}, function (result) {
        let dropdown = $('#ddlEntityType');
        dropdown.empty();
        dropdown.append('<option selected="true" disabled>Please Select</option>');
        dropdown.prop('selectedIndex', 0);
        $.each(result, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        });
    }, 'GET');
}

//=====================================================
//      get all roles
//=====================================================
Schedule.GetAllRoles = function () {
    App.postData(App.ApiUrl + 'Role', {}, function (result) {
        let ddlScheduledRole = $('#RolesToCreateActivity');
        let ddlRegardlessScheduledRole = $('#ddlRegardlessScheduledRole');
        ddlScheduledRole.empty();
        ddlScheduledRole.prop('selectedIndex', 0);
        ddlRegardlessScheduledRole.empty();
        ddlRegardlessScheduledRole.prop('selectedIndex', 0);
        $.each(result, function (key, entry) {
            ddlScheduledRole.append($('<option></option>').attr('value', entry.guid).text(entry.name));
            ddlRegardlessScheduledRole.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        });
        $("#RolesToCreateActivity, #ddlRegardlessScheduledRole").attr("multiple", true);
        $('#RolesToCreateActivity, #ddlRegardlessScheduledRole').selectpicker();
        $('#RolesToCreateActivity, #ddlRegardlessScheduledRole').selectpicker('deselectAll');
    }, 'GET');
}

//=====================================================
//          Save Scheduleing New      27-Dec-2018
//=====================================================
Schedule.saveScheduleing = function () {
    var sendData = {};

    sendData.ScheduleActivityList = [];
    sendData.ActivitySchedulingViewModel = [];

    var validationCount = 0;
    $('#formContainer div.row').each(function (i, row) {
        let guid = $(row).attr('data-id');
        let startDate = null;
        let endDate = null;
        let selectedval1 = $(row).attr('data-question1-schedule-value');
        let selectedval2 = $(row).attr('data-question2-schedule-value');
        let selectedval3 = $(row).attr('data-question3-schedule-value');
        let selectedval4 = $(row).attr('data-question4-schedule-value');
        let selectedval5 = $(row).attr('data-question5-schedule-value');

        // 1
        //
        sendData.ActivitySchedulingViewModel.push({
            "ScheduleType": 1,
            "ScheduleValue": selectedval1,
            "ActivityId": guid,
        });
        sendData.ActivitySchedulingViewModel.push({
            "ScheduleType": 2,
            "ScheduleValue": selectedval2,
            "ActivityId": guid,
        });

        // 3
        //
        sendData.ActivitySchedulingViewModel.push({
            "ScheduleType": 3,
            "ScheduleValue": selectedval3,
            "ActivityId": guid,
        });

        // 4
        //
        sendData.ActivitySchedulingViewModel.push({
            "ScheduleType": 4,
            "ScheduleValue": selectedval4,
            "ActivityId": guid,
        });

        // 5
        //
        sendData.ActivitySchedulingViewModel.push({
            "ScheduleType": 5,
            "ScheduleValue": selectedval5,
            "ActivityId": guid,
        });
    });
    if (validationCount === 0) {
        App.postData(App.ApiUrl + "Activity/ScheduleActivityList_New", sendData, function (result) {
            App.showSuccess('Activity schedule was saved successfully.');  //Added on 15-jan-2019
        }, 'Post');
    }
}


Schedule.setOffsetOtherActivity = function () {
    let x = 0;
    $('#offset-from-other-activity-popus .validate-form').each(function (i, row) {
        let fieldtype = $(row).prop('type');
        $(row).removeClass("input-validation-error");
        $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('');
        $("#endsTypeValueOn").removeClass("input-validation-error");
        $('span[data-valmsg-for="endsTypeValueOn"]').addClass("field-validation-error").text('');
        $("#endsTypeValueAfter").removeClass("input-validation-error");
        $('span[data-valmsg-for="endsTypeValueAfter"]').addClass("field-validation-error").text('');
        if (fieldtype == "text") {
            let val = $(row).val();
            if (val.length < 1) {
                x++;
                $(row).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            }
        }
        else if (fieldtype == "select-one") {
            let val = $(row).val();
            if (val.length < 1) {
                x++;
                $(row).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            }
        }
        else if (fieldtype == "radio") {
            let name = $(row).attr("name");

            let radio = $('input[name="' + name + '"]:checked').val();
            if (radio == 2) {
                if ($("#endsTypeValueOn").val().length < 1) {
                    x++;
                    $("#endsTypeValueOn").addClass("input-validation-error");
                    $('span[data-valmsg-for="endsTypeValueOn"]').addClass("field-validation-error").text('this field is required');
                }
            } else if (radio == 3) {
                if ($("#endsTypeValueAfter").val().length < 1) {
                    x++;
                    $("#endsTypeValueAfter").addClass("input-validation-error");
                    $('span[data-valmsg-for="endsTypeValueAfter"]').addClass("field-validation-error").text('this field is required');
                }
            }
        }
        else if (fieldtype == "checkbox") {
            let name = $(row).attr("name");
            let chk = 0;
            $('input[name="' + name + '"]:checked').each(function () {
                chk++;
            });
            $(this).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            return false;
        }
        else if (fieldtype == "textarea") {
            let val = $(row).val();
            $(this).addClass("input-validation-error");
            $('span[data-valmsg-for="' + $(this).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            return false;
        }
        else {
            let val = $(row).val();
            if (val.length < 1) {
                x++;
                $(row).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            }

        }

    });

    if (x == 0) {
        $('#offset-from-other-activity-popus').modal('hide');
        return true;
    } else {
        $('#offset-from-other-activity-popus').modal('show');
        return false;
    }

}

Schedule.setActivityCreationWindow = function () {
    let x = 0;
    $('#time-period-scheduled-time-popus .validate-form').each(function (i, row) {

        let fieldtype = $(row).prop('type');

        $(row).removeClass("input-validation-error");
        $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('');
        if (fieldtype == "text") {
        }
        else if (fieldtype == "select-one") {
            let val = $(row).val();
            if (val.length < 1) {
                x++;
                $(row).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            }
        }
        else if (fieldtype == "radio") {
        }
        else if (fieldtype == "checkbox") {
        }
        else if (fieldtype == "textarea") {
        }
        else {
            let val = $(row).val();
            if (val.length < 1) {
                x++;
                $(row).addClass("input-validation-error");
                $('span[data-valmsg-for="' + $(row).prop('id') + '"]').addClass("field-validation-error").text('this field is required');
            }
        }
    });

    if (x == 0) {
        $('#time-period-scheduled-time-popus').modal('hide');
    }

}

//=====================================================
//      open popup for option 'Offset from another activity'
//=====================================================
Schedule.changeddlScheduleStart = function (e) {
    if ($(e).val() === "4") {
        $('#offset-from-other-activity-popus').modal('show');
    }
}
Schedule.changeActivityCreatedInSystem = function (e) {
    if ($(e).val() === "3") {
        $('#time-period-scheduled-time-popus').modal('show');
    }
}

//not usable
//=====================================================
//    get scheduled activity questions   02-Jan-2019
//=====================================================
Schedule.GetAllActivityScheduling = function () {


    let firstActivityguid = $('#formContainer div.row:first').attr('data-id');

    App.postData(App.ApiUrl + 'Activity/GetAllActivityScheduling/?activityId=' + firstActivityguid, {}, function (result) {


        $(result).each(function (i, element) {

            if (element.scheduleType == 1) {
                $('#ddlScheduleStart').val(element.scheduleValue);
                if (element.scheduleValue == "4") {
                    $("#parentActivityId").val(element.parentActivityId);
                    $("#offset").val(element.offset);
                    $("#afterActivity").val(element.offset);

                    $("input[name='endsType'][value='" + element.endsType + "']").prop('checked', true);
                    if (element.endsType == 2)
                        $("#endsTypeValueOn").val(element.endsValue);
                    else if (element.endsType == 3)
                        $("#endsTypeValueAfter").val(element.endsValue);
                }
            }
            else if (element.scheduleType == 2) {
                $('#ddlActivityCreated').val(element.scheduleValue);
                if (element.scheduleValue == "3") {
                    let afterSchedule = element.afterSchedule.split(',');
                    $("#afterCount").val(afterSchedule[0]);
                    $("#afterType").val(afterSchedule[1]);

                    let beforeSchedule = element.beforeSchedule.split(',');
                    $("#beforeCount").val(beforeSchedule[0]);
                    $("#beforeType").val(beforeSchedule[1]);
                }
            }
            else if (element.scheduleType == 3) {
                $('#ddlEntityType').val(element.scheduleValue);
            }
            else if (element.scheduleType == 4) {
                $('#ddlScheduledRole').val(element.scheduleValue);
            }
            else if (element.scheduleType == 5) {
                $('#ddlRegardlessScheduledRole').val(element.scheduleValue);
            }

        });
    }, 'GET');
}
//==================================================================
//Activity Scheduled To Be Completed Change(question 1 changes)
//==================================================================
Schedule.onScheduledToBeCompletedChange = function (ddl) {
    if ($(ddl).val() === "2") {
        $("#parentActivityId").attr("data-val", true);
        $("#parentActivityId").attr("data-val-required", "This field is required.");
        $("#parentActivityId").prop('required', true);

        $('.divOffset_from_another_activity').removeClass('hide');

        $('#ActivityAvailableForCreation option[value=3]').removeClass('hide');
        $('#ActivityAvailableForCreation option[value=4]').removeClass('hide');
    } else {
        $("#parentActivityId").removeAttr("data-val");
        $("#parentActivityId").removeAttr("data-val-required");
        $("#parentActivityId").prop('required', false);

        $('.divOffset_from_another_activity').addClass('hide');

        $('#ActivityAvailableForCreation').val("");
        $('.divBased_on_scheduled_date').addClass('hide');
        $('#ActivityAvailableForCreation option[value=3]').addClass('hide');
        $('#ActivityAvailableForCreation option[value=4]').addClass('hide');
    }
}


Schedule.onActivityAvailableForCreationChange = function (ddl) {
    if ($(ddl).val() === "2") {
        $("#txtActivityAlreadyCreated").attr("data-val", true);
        $("#txtActivityAlreadyCreated").attr("data-val-required", "This field is required.");
        $("#txtActivityAlreadyCreated").prop('required', true);

        $('.divActivity_already_created').removeClass('hide');
    } else {
        $("#txtActivityAlreadyCreated").removeAttr("data-val");
        $("#txtActivityAlreadyCreated").removeAttr("data-val-required");
        $("#txtActivityAlreadyCreated").prop('required', false);

        $('.divActivity_already_created').addClass('hide');
    }
    if ($(ddl).val() === "3" || $(ddl).val() === "4") {
        $(".creation-window").attr("data-val", true);
        $(".creation-window").attr("data-val-required", "This field is required.");
        $(".creation-window").prop('required', true);

        $('.divBased_on_scheduled_date').removeClass('hide');
    } else {
        $(".creation-window").removeAttr("data-val");
        $(".creation-window").removeAttr("data-val-required");
        $(".creation-window").prop('required', false);

        $('.divBased_on_scheduled_date').addClass('hide');
    }
}

Schedule.edit = function (guid, name) {
    Schedule.activityStatusId = 0;
    var activityType = $('ul.variable-categories > li[data-variable = ' + guid + '] > a').attr("data-default-activity-type");
    Schedule.CurrentActivityEntityType = $('ul.variable-categories > li[data-variable = ' + guid + '] > a').attr("data-entity-type-guid");
    if (activityType == "0") {
        $('#btnSaveSchedule').attr("disabled", "disabled");
        $('#btnSaveSchedule').addClass("disabled-button-cursor");
    }
    else {
        $('#btnSaveSchedule').removeAttr("disabled");
        $('#btnSaveSchedule').removeClass("disabled-button-cursor");
    }


    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('ul.variable-categories > li[data-variable = ' + guid + '] > a').addClass('library-li-active');
    $('#parentActivityId > option').addClass('dropdown-disabled-options-background').attr("disabled", true);
    $('#txtActivityAlreadyCreated > option').addClass('dropdown-disabled-options-background').attr("disabled", true);


    $.each(Schedule.UnscheduledActivityList, function (key, entry) {

        $("#parentActivityId option[value=" + entry + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled");
        $("#txtActivityAlreadyCreated option[value=" + entry + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled");
    });

    $("#parentActivityId > option").each(function () {
        if ($(this).attr("data-is-default") == "true") {
            $("#parentActivityId option[value=" + this.value + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled", true);
        }
    });
    $("#txtActivityAlreadyCreated > option").each(function () {

        if ($(this).attr("data-is-default") == "true") {
            $("#txtActivityAlreadyCreated option[value=" + this.value + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled", true);
        }
    });
    $("#parentActivityId option[value=" + guid + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
    $("#txtActivityAlreadyCreated option[value=" + guid + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
    $("#txtActivityAlreadyCreated option[value=" + guid + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);

    App.postData(App.ApiUrl + 'Scheduling/GetSchedulingByActivityId/' + guid, {}, function (result) {
   
        if (result != null) {
            Schedule.activityStatusId = result.activityStatusId;

            if (!result.isScheduled) {
                Schedule.resetForm();
                $('#Guid').val(result.guid);
                $('#ActivityGuid').val(guid);
                $('#ActivityName').val(name);
                Schedule.ActivityId = guid;
                return;
            }
            otherActivity = result.usedAsOtherActivitiesList;
            specifiedActivity = result.usedAsSpecifiedActivitiesList;
            Schedule.ActivityId = result.activityId;
            $('#Guid').val(result.guid);
            $('#ActivityGuid').val(result.activityId);
            $('#ActivityName').val(result.activityName);
            if (result.canCreatedMultipleTime === true) {
                $("#ddlCanCreatedMultipleTime").val("true");
            } else {
                $("#ddlCanCreatedMultipleTime").val("false");
            }

            $("#ScheduledToBeCompleted").val(result.scheduledToBeCompleted);
            $("#ActivityAvailableForCreation").val(result.activityAvailableForCreation);
            $("#RolesToCreateActivity").selectpicker('val', result.rolesToCreateActivity);
            $("#ddlRegardlessScheduledRole").selectpicker('val', result.roleToCreateActivityRegardlessScheduled);

            if (result.scheduledToBeCompleted == 2) {
                $("#parentActivityId").val(result.otherActivity);
                $("#offsetCount").val(result.offsetCount);
                $("#offsetType").val(result.offsetType);

                $("#parentActivityId").attr("data-val", true);
                $("#parentActivityId").attr("data-val-required", "This field is required.");
                $("#parentActivityId").prop('required', true);
                $('.divOffset_from_another_activity').removeClass('hide');
            }
            else {
                $("#parentActivityId").removeAttr("data-val");
                $("#parentActivityId").removeAttr("data-val-required");
                $("#parentActivityId").prop('required', false);
                $('.divOffset_from_another_activity').addClass('hide');
            }
            if (result.activityAvailableForCreation == 2) {
                $("#txtActivityAlreadyCreated").val(result.specifiedActivity);
                $("#txtActivityAlreadyCreated").attr("data-val", true);
                $("#txtActivityAlreadyCreated").attr("data-val-required", "This field is required.");
                $("#txtActivityAlreadyCreated").prop('required', true);
                $('.divActivity_already_created').removeClass('hide');
            }
            else {
                $("#txtActivityAlreadyCreated").removeAttr("data-val");
                $("#txtActivityAlreadyCreated").removeAttr("data-val-required");
                $("#txtActivityAlreadyCreated").prop('required', false);
                $('.divActivity_already_created').addClass('hide');
            }
            if (result.activityAvailableForCreation == 3 || result.activityAvailableForCreation == 4) {
                $("#txtCreationWindowClose").val(result.creationWindowClose);
                $("#txtCreationWindowOpen").val(result.creationWindowOpens);
                $(".creation-window").attr("data-val", true);
                $(".creation-window").attr("data-val-required", "This field is required.");
                $(".creation-window").prop('required', true);
                $('.divBased_on_scheduled_date').removeClass('hide');
            }
            else {
                $(".creation-window").removeAttr("data-val");
                $(".creation-window").removeAttr("data-val-required");
                $(".creation-window").prop('required', false);
                $('.divBased_on_scheduled_date').addClass('hide');
            }
        } else {
            Schedule.resetForm();
            $('#ActivityGuid').val(guid);
            $('#ActivityName').val(name);
            Schedule.ActivityId = guid;
        }
    }, 'GET');
    Schedule.GetScheduledActivity(guid);
}

Schedule.save = function () {
    if (Schedule.activityStatusId === App.ActivityStatusEnum.Active) {
        App.showError("This activity is currently active (on the Summary page). To edit this activity, you must first inactivate the activity on the Testing/Deployment page!");
        return false;
    }
    let url = $('#Guid').val() == "" ? "Scheduling" : "Scheduling/" + $('#Guid').val();
    let methodeType = $('#Guid').val() == "" ? "POST" : "PUT";
    if ($('#frm-schedule-activity').valid()) {
        let scheduledToBeCompleted = $("#ScheduledToBeCompleted").val();
        let activityAvailableForCreation = $("#ActivityAvailableForCreation").val();
        let rolesToCreateActivity = $("#RolesToCreateActivity").val();
        let regardlessScheduledRole = $("#ddlRegardlessScheduledRole").val();
        let canCreatedMultipleTime = $("#ddlCanCreatedMultipleTime").val();
        let otherActivity = null;
        let offsetCount = null;
        let offsetType = null;
        let creationWindowOpen = null;
        let creationWindowClose = null;
        let activityAlreadyCreated = null;
        var sendData = {};
        if (scheduledToBeCompleted == 2) {
            otherActivity = $("#parentActivityId").val();
            offsetCount = $("#offsetCount").val();
            offsetType = $("#offsetType").val();
        }

        if (activityAvailableForCreation == 2) {
            activityAlreadyCreated = $("#txtActivityAlreadyCreated").val();
        } else if (activityAvailableForCreation == 3) {
            creationWindowClose = $("#txtCreationWindowClose").val();
            creationWindowOpen = $("#txtCreationWindowOpen").val();
        } else if (activityAvailableForCreation == 4) {
            creationWindowClose = $("#txtCreationWindowClose").val();
            creationWindowOpen = $("#txtCreationWindowOpen").val();
        }

        sendData.ActivityId = Schedule.ActivityId;
        sendData.ScheduledToBeCompleted = scheduledToBeCompleted;
        sendData.ActivityAvailableForCreation = activityAvailableForCreation;
        sendData.RolesToCreateActivity = rolesToCreateActivity;
        sendData.RoleToCreateActivityRegardlessScheduled = regardlessScheduledRole;
        sendData.OtherActivity = otherActivity;
        sendData.OffsetCount = offsetCount;
        sendData.OffsetType = offsetType;
        sendData.SpecifiedActivity = activityAlreadyCreated;
        sendData.CreationWindowOpens = creationWindowOpen;
        sendData.CreationWindowClose = creationWindowClose;
        sendData.ProjectId = $("#ProjectId").val();
        sendData.CanCreatedMultipleTime = canCreatedMultipleTime;
        App.postData(App.ApiUrl + url, sendData, function (result) {
            App.showSuccess($('#Guid').val() != "" ? "Activity Scheduled updated successfully." : "Activity Scheduled successfully.");
            Schedule.resetForm();
            $("#parentActivityId option[value=" + Schedule.ActivityId + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
            Schedule.UnscheduledActivityList.push(Schedule.ActivityId);
            $("#li-" + Schedule.ActivityId).text("(Entered)");
        }, methodeType);
    }
}

Schedule.resetForm = function () {
    $('#Guid').val("");
    $('#ActivityGuid').val("");
    $('#ActivityName').removeClass("input-validation-error").val("");
    $("#ScheduledToBeCompleted").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#ddlCanCreatedMultipleTime").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#ActivityAvailableForCreation").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#RolesToCreateActivity, #ddlRegardlessScheduledRole").removeClass("input-validation-error");
    $('#RolesToCreateActivity, #ddlRegardlessScheduledRole').selectpicker('deselectAll');
    $("#parentActivityId").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#offsetCount").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#offsetType").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#txtActivityAlreadyCreated").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#txtCreationWindowClose").removeClass("input-validation-error").val("");
    $("#txtCreationWindowOpen").removeClass("input-validation-error").val("");
    $("#parentActivityId").removeAttr("data-val");
    $("#parentActivityId").removeAttr("data-val-required");
    $("#parentActivityId").prop('required', false);
    $("#txtActivityAlreadyCreated").removeAttr("data-val");
    $("#txtActivityAlreadyCreated").removeAttr("data-val-required");
    $("#txtActivityAlreadyCreated").prop('required', false);
    $(".creation-window").removeAttr("data-val");
    $(".creation-window").removeAttr("data-val-required");
    $(".creation-window").prop('required', false);
    $('.divOffset_from_another_activity').addClass('hide');
    $('.divActivity_already_created').addClass('hide');
    $('.divBased_on_scheduled_date').addClass('hide');
    $('ul.variable-categories > li > a').removeClass('library-li-active');
}

Schedule.GetScheduledActivity = function (currentGuid) {

    App.postData(App.ApiUrl + 'Scheduling/GetAllScheduledActivityByProjectId/' + $("#ProjectId").val(), {}, function (result) {

        let ScheduledActivityList = [];
        Schedule.UnscheduledActivityList = [];
        $.each(result, function (key, entry) {
            ScheduledActivityList.push(entry.activityId);
            if ((entry.scheduledToBeCompleted == 1 && Schedule.CurrentActivityEntityType == entry.entityTypes[0]) || (entry.scheduledToBeCompleted == 2 && Schedule.CurrentActivityEntityType == entry.entityTypes[0])) {
                Schedule.UnscheduledActivityList.push(entry.activityId);
                $("#parentActivityId option[value=" + entry.activityId + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled");
                $("#txtActivityAlreadyCreated option[value=" + entry.activityId + "]").removeClass("dropdown-disabled-options-background").removeAttr("disabled");
            } else {
                $("#parentActivityId option[value=" + entry.activityId + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
                $("#txtActivityAlreadyCreated option[value=" + entry.activityId + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
            }
        });
        $(".activities-list-entered-label").each(function () {
            var _this = this;
            var isScheduled = ScheduledActivityList.some(function (v) {
                return $(_this).attr('data-activity-id').indexOf(v) !== -1;
            });
            if (isScheduled) {
                $(this).text("(Entered)");
            } else {
                $(this).text("(Not Entered)");
            }
        });
        $("#parentActivityId option[value=" + currentGuid + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
        $("#txtActivityAlreadyCreated option[value=" + currentGuid + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);

        if (otherActivity != null) {
            $.each(otherActivity, function (key, value) {
                $("#parentActivityId option[value=" + value + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
            });
        }
        if (specifiedActivity != null) {
            $.each(specifiedActivity, function (key, value) {
                $("#txtActivityAlreadyCreated option[value=" + value + "]").addClass("dropdown-disabled-options-background").attr("disabled", true);
            });
        }
    }, 'GET');
}

//======================================================
//  search exesting activities from activity library for scheduling
//======================================================
Schedule.search = function (type) {
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




