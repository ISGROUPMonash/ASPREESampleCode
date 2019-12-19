
var Activity = {};
Activity.activityStatusId = 0;
Activity.formLibrary = [];
$(function () {
    $('#search-var-text').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            Activity.search(true);
            return false;
        }
    });

    $('#formContainer').on("drop", function (e) {
        e.preventDefault();

        if (!Activity.currentForm.category) {
            Activity.addForm(Activity.currentForm.guid, Activity.currentForm.name);
        }
        else {
            $('a[data-id="' + Activity.currentForm.guid + '"]').parent().find('ul li a').each(function (i, d) {
                if (!$(d).parent().hasClass("list-element-disabled"))
                    Activity.addForm($(d).attr('data-id'), $(d).text());
            });
        }
        return false;
    }).on('dragover', function (e) { e.preventDefault(); });

    $('#formContainer').sortable({ 'handle': 'i.mdi-menu' });

    App.postData(App.ApiUrl + 'Form/GetAllForms/' + $("#ProjectId").val(), {}, function (result) {
        Activity.formLibrary = result;        
    }, 'GET', true);
});

Activity.addFormCategory = function (category) {
    $('a[data-id="' + Activity.currentForm.guid + '"]').parent().find('ul li a').each(function (i, d) {
        if (!$(d).parent().hasClass("list-element-disabled"))
            Activity.addForm($(d).attr('data-id'), $(d).text());
    });
}

Activity.edit = function (guid) {

    Activity.activityStatusId = 0;

    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('ul.variable-categories > li[data-variable = ' + guid + '] > a').addClass('library-li-active');
    var projectid = $("#ProjectId").val();
    App.postData(App.ApiUrl + 'activity/Getactivity/' + guid + "/" + projectid, {}, function (result) {
        Activity.activityStatusId = result.activityStatusIdInt;
        $('#Guid').val(result.guid);
        $('#delete-activity').removeClass('hide');
        $('#ActivityName').val(result.activityName);
        $('#ActivityCategoryId').val(result.activityCategoryId);
        $('#EntityTypes').val(result.entityTypes);
        $('#ActivityRoles').selectpicker('val', result.activityRoles);
        $('#DependentActivityId').val(result.dependentActivityId);
        $('#RepeatationCount').val(result.repeatationCount);
        if (result.repeatationCount != 0) {
            $('#IsRepeat').val('true').change();
        }
        $('#formContainer').empty();
        for (var i = 0; i < result.forms.length; i++) {
            var form = result.forms[i];
            Activity.addForm(form.id, form.formTitle, form, result.isDefaultActivity);
        }
        $('#DependentActivityId option').show();
        $("#DependentActivityId option[value=" + result.guid + "]").hide();
        if (result.isDefaultActivity == 0) {
            $('#ActivityName').attr("disabled", "disabled");
            $('#ActivityCategoryId').attr("disabled", "disabled");
            $('#IsRepeat').attr("disabled", "disabled");
            $('#DependentActivityId').attr("disabled", "disabled");
            $('#EntityTypes').attr("disabled", "disabled");
            $('#ActivityRoles').attr("disabled", "disabled");
            $('#SaveActivity').attr("disabled", "disabled");
            $('#SaveActivity').addClass("disabled-button-cursor");
            $('#delete-activity').attr("disabled", "disabled");
            $('#delete-activity').addClass("disabled-button-cursor");
            //Sprint-12 [ASPMONASH-218]
            $('#IsActivityRequireAnEntity').attr("disabled", "disabled");
        } else {
            $('#ActivityName').removeAttr("disabled");
            $('#ActivityCategoryId').removeAttr("disabled");
            $('#IsRepeat').removeAttr("disabled");
            $('#DependentActivityId').removeAttr("disabled");
            $('#EntityTypes').removeAttr("disabled");
            $('#ActivityRoles').removeAttr("disabled");
            $('#SaveActivity').removeAttr("disabled");
            $('#SaveActivity').removeClass("disabled-button-cursor");
            $('#delete-activity').removeAttr("disabled");
            $('#delete-activity').removeClass("disabled-button-cursor");
            //Sprint-12 [ASPMONASH-218]
            $('#IsActivityRequireAnEntity').removeAttr("disabled");
        }

        if (result.activityStatusName == "Active" && result.userTypeRole == "Project Admin") {
            $('#delete-activity').addClass("hide");
        }
        if (result.isFormContaindData) {
            $('#delete-activity').addClass("hide");
        }
        if (Activity.activityStatusId === App.ActivityStatusEnum.Active) {
            $('#delete-activity').addClass("hide");
        }
        $("#IsActivityRequireAnEntity").val(result.isActivityRequireAnEntity != null ? result.isActivityRequireAnEntity.toString() : 'false');



        let entType = $("#EntityTypes option:selected").html();
        if (entType == "Person") {
            $("#IsActivityRequireAnEntityLabel").html('Does the activity require the person to be living?');
            $("#divIsActivityRequireAnEntity").removeClass("hide");
        } else if (entType == "Participant") {
            $("#IsActivityRequireAnEntityLabel").html('Does the activity require the participant to be living?');
            $("#divIsActivityRequireAnEntity").removeClass("hide");
        } else {
            $("#IsActivityRequireAnEntityLabel").html('');
            $("#divIsActivityRequireAnEntity").addClass("hide");
        }

        Activity.onEntityTypesChange($("#EntityTypes"));
    }, 'GET');
}
Activity.onRepeatationChange = function (ddl) {
    if (ddl.value == "true")
        $('.repeatation-count').removeClass('hide');
    else
        $('.repeatation-count').addClass('hide');
}
Activity.save = function () {
    let url = $('#Guid').val() == "" ? "activity" : "activity/" + $('#Guid').val();
    let methodeType = $('#Guid').val() == "" ? "POST" : "PUT";

    if (methodeType == "PUT") {
        if (Activity.activityStatusId === App.ActivityStatusEnum.Active) {
            App.showError("This activity is currently active (on the Summary page). To edit this activity, you must first inactivate the activity on the Testing/Deployment page!");
            return false;
        }
    }


    var sendData = {};
    if ($('#frmSaveHeaderData').valid()) {
        sendData.ActivityName = $('#ActivityName').val();
        sendData.ActivityCategoryId = $('#ActivityCategoryId').val();
        sendData.RepeatationCount = $('#IsRepeat').val() == "true" ? $('#RepeatationCount').val() : 0;
        sendData.DependentActivityId = $('#DependentActivityId').val();
        sendData.RepeatationType = 1;
        sendData.ProjectId = $('#ProjectId').val();

        let entityTypes = $('#EntityTypes').val();
        if (entityTypes.length == 0) {
            entityTypes = null;
        }
        sendData.EntityTypes = [];
        sendData.EntityTypes.push(entityTypes);
        sendData.Forms = [];
        $('#formContainer div.row').each(function (i, row) {
            let guid = $(row).attr('data-id');
            let roles = $(row).find('select.field-permision').val();

            sendData.Forms.push({
                "Id": guid,
                "Roles": roles
            });
        });
        sendData.IsActivityRequireAnEntity = $("#IsActivityRequireAnEntity").val();
        App.postData(App.ApiUrl + url, sendData, function (result) {           
            App.redirectSuccess($('#Guid').val() != "" ? "Activity was modified successfully." : "Activity was added successfully.", '/ProjectBuilder/Activities/' + $('#ProjectId').val());
        }, methodeType);
    }
}

Activity.setCurrentForm = function (guid, name, category) {
    Activity.currentForm = {};
    Activity.currentForm.guid = guid;
    Activity.currentForm.name = name;
    Activity.currentForm.category = category;
}

Activity.addForm = function (guid, name, form, isDefaultActivity) {
    if (guid == undefined) return;
    var container = $('#formContainer');
    let _thisFormUsedInActivityList = Activity.formLibrary.find(function (d) { return d.guid == guid }).formUsedInActivityList;
    let _thisIsDefaultForm = Activity.formLibrary.find(function (d) { return d.guid == guid }).isDefaultForm;
    let _usedVariablesNameList = Activity.formLibrary.find(function (d) { return d.guid == guid }).usedVariablesNameList;
    if ($('div.row[data-id="' + guid + '"]', container).length > 0) return; // prevent duplicate variable entry
    _thisFormUsedInActivityList = jQuery.grep(_thisFormUsedInActivityList, function (value) {
        return value != $('#Guid').val();
    });

    if (_thisFormUsedInActivityList.length > 0 && _thisIsDefaultForm != 0) {
        App.showError("Form containing the variable '" + _usedVariablesNameList.join(", ") + "' does NOT have the ‘Can this be collected multiple times’ box check and is already in use on another form or activity. Please update variable details.");
        return; // prevent use in multiple activity
    }
    var template = $('#formTemplate').children().clone();
    template.find('.var-label').text(name);
    template.find('.field-permision').selectpicker();

    if (isDefaultActivity == 0) {
        template.find('.btn-activityedit').attr("disabled", "disabled");
        template.find('.btn-activityedit').addClass("disabled-button-cursor");
        template.find('.mdi-menu').css("cursor", "not-allowed");
        template.addClass("disabled-button-cursor");
    } else {
        template.find('.btn-activityedit').removeAttr("disabled");
        template.find('.btn-activityedit').removeClass("disabled-button-cursor");
        template.find('.mdi-menu').css("cursor", "ns-resize");
        template.removeClass("disabled-button-cursor");
    }
    if (form) {
        template.find('.field-permision').selectpicker('val', form.roles);
    }
    template.attr('data-id', guid);
    $('#drag-message').remove();
    container.append(template);
    container.sortable('refresh');
}


Activity.deleteConfirm = function (guid) {

    if (Activity.activityStatusId === App.ActivityStatusEnum.Active) {
        App.showError("This activity is currently active (on the Summary page). To edit this activity, you must first inactivate the activity on the Testing/Deployment page!");
        return false;
    }
    Activity.Current = guid;
    Activity.Current = $('#Guid').val();
    $('.modal-body .sf-submit-info').html("Are you sure to delete selected activity?");
}

Activity.delete = function (guid) {

    $('#delete-activity').attr("disabled", "disabled");
    $('#SaveActivity').attr("disabled", "disabled");

    App.postData(App.ApiUrl + "/Activity/" + Activity.Current, {}, function (result) {
        App.redirectSuccess($('#Guid').val() != "" ? "Activity was deleted successfully." : "Activity was added successfully.", '/ProjectBuilder/Activities/' + $('#ProjectId').val());
        $("#delete-activity").removeAttr("disabled");
        $("#SaveActivity").removeAttr("disabled");
    }, "Delete");
}

//======================================================
//  Reset activities (clear activities)
//======================================================
Activity.resetActivity = function () {

    $('#Guid').val('')
    $('#ActivityName').val('');
    $('#ActivityCategoryId').prop('selectedIndex', 0);
    $('#EntityTypes').prop('selectedIndex', 0);
    $("#IsActivityRequireAnEntity").val("false");
    $('#formContainer').empty();
    $('#formContainer').append('<h2 id="drag-message" class="text-center">Drag and Drop your forms here</h2>');
    $('#ActivityName').removeAttr("disabled");
    $('#ActivityCategoryId').removeAttr("disabled");
    $('#IsRepeat').removeAttr("disabled");
    $('#DependentActivityId').removeAttr("disabled");
    $('#EntityTypes').removeAttr("disabled");
    $('#ActivityRoles').removeAttr("disabled");
    $('#SaveActivity').removeAttr("disabled");
    $('#SaveActivity').removeClass("disabled-button-cursor");
    $('#delete-activity').removeAttr("disabled");
    $('#delete-activity').removeClass("disabled-button-cursor");
    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('#delete-activity').addClass('hide');
    Activity.activityStatusId = 0;
    Activity.onEntityTypesChange($("#EntityTypes"));
}

//====================================================================
//================= Entity Types Change ==============================
//====================================================================
Activity.onEntityTypesChange = function (ddl) {
    Activity.currentForm = {};

    $('#FormLibraryList ul > li').each(function (i, row) {
        let guid = $(row).attr('data-entity-type-list');
        var formEntityTypes = guid.replace("[", "").replace("]", "").replace(/"/g, '').split(',');
        var isExist = formEntityTypes.some(function (v) { return $(ddl).val().indexOf(v) !== -1; });
        if (isExist) {
            $(this).removeClass("list-element-disabled");
            $($(this).children()).removeClass("list-element-disabled");
        } else {
            $(this).addClass("list-element-disabled");
            $($(this).children()).addClass("list-element-disabled");
        }
    });

    let entType = $("#" + $(ddl).prop('id') + " option:selected").html();
    if (entType == "Person") {
        $("#IsActivityRequireAnEntityLabel").html('Does the activity require the person to be living?');
        $("#divIsActivityRequireAnEntity").removeClass("hide");
    } else if (entType == "Participant") {
        $("#IsActivityRequireAnEntityLabel").html('Does the activity require the participant to be living?');
        $("#divIsActivityRequireAnEntity").removeClass("hide");
    } else {
        $("#IsActivityRequireAnEntityLabel").html('');
        $("#divIsActivityRequireAnEntity").addClass("hide");
    }

    $('#formContainer div.row').each(function (i, row) {
        let entityTypesData = Activity.formLibrary.find(function (d) { return d.guid == $(row).attr('data-id') }).entityTypes;

        if (!entityTypesData.includes($(ddl).val())) {
            $(row).remove();
        }
    });


}

//======================================================
//  search exesting activities from activity library
//======================================================
Activity.search = function (type) {
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
