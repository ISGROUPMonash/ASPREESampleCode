var Variable = {};
Variable.ActivityStatusId = 0;
Variable.EntityType = [];
Variable.EntitySubType = [];
Variable.IsEntitySubType = false;


$(function () {
    $('#search-var-text').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            Variable.search(true);
            return false;
        }
    });

    $('input:text').on('drop', function (e) {
        e.preventDefault();
    });

    $("#NoMinimum").click(function () {
        if ($(this).is(":checked")) {
            $("#fromDataEntryRange").val("");
            $("#fromDataEntryRange").attr("disabled", "disabled");

            let PreviewPane_ValidationMax = [null, null];
            $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMax));

        } else {
            $("#fromDataEntryRange").removeAttr("disabled");
            $("#fromDataEntryRange").focus();
        }


        let noMinimum = $("#NoMinimum").is(":checked");
        let noMaximum = $("#NoMaximum").is(":checked");
        noMinimum && noMaximum
            ? $("#OutsideDataEntryRangeRequiredStat").text("")
            : $("#OutsideDataEntryRangeRequiredStat").text("*");
    });

    $("#NoMaximum").click(function () {
        if ($(this).is(":checked")) {
            $("#toDataEntryRange").val("");
            $("#toDataEntryRange").attr("disabled", "disabled");
            let PreviewPane_ValidationMin = [null, null];
            $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMin));

        } else {
            $("#toDataEntryRange").removeAttr("disabled");
            $("#toDataEntryRange").focus();
        }

        let noMinimum = $("#NoMinimum").is(":checked");
        let noMaximum = $("#NoMaximum").is(":checked");
        noMinimum && noMaximum
            ? $("#OutsideDataEntryRangeRequiredStat").text("")
            : $("#OutsideDataEntryRangeRequiredStat").text("*");
    });

    $('#Formula').on("drop", function (e) {
        e.preventDefault();
        var el = this;
        var caretPos = el.selectionStart;
        var textAreaTxt = el.value;
        el.value = textAreaTxt.substring(0, caretPos) + Variable.selectedVariable + textAreaTxt.substring(caretPos);
    });

    $('#isHelpTextRequired').prop('checked', true);
    $("#isHelpTextRequired").click(function () {
        if ($(this).is(":checked")) {
            $("#divHelpText").removeClass("hide");
        } else {

            $("#divHelpText").addClass("hide");
        }
    });

    $(".numberonly").keypress(function (e) {
        if ($("#VariableType").find('option:selected').data('text') == "Numeric (Integer)") {
            if (e.which == 45)
                return true;
            if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57))
                return false;
        }
    });

    $('#Description').keypress(function (event) {
        if (event.keyCode == 34) {
            event.preventDefault();
        }
    });
});

Variable.add = function () {
    location.href = "/variables/Create";
}

Variable.onVariableTypeChange = function (ddl) {
    var ddl = $(ddl);

    Variable.refresh();

    $('#value-container, .range-container, #formula-container, #lkup-container').addClass('hide');
    $("#previewHelpText").text("");
    $('#typeNumeric').addClass('hide');
    $("#selectdropdownvalue").addClass('hide');
    $("#selectLKUPvalue").addClass('hide');
    $('#selectcheckboxvalue').addClass('hide');
    $('#selectradiobuttonvalue').addClass('hide');
    $('#numericvalue').addClass('hide');
    $('#formulavalue').addClass('hide');
    $('#typeTextArea').addClass('hide');
    $('#divDatepicker').addClass('hide');
    $(".datetype-container").addClass("hide");
    $("#previewColorPicker").addClass("hide");
    $('#previewFileType').addClass('hide');

    switch (ddl.find('option:selected').data('text')) {
        case "Text Box":
            $('#typeNumeric').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");         
            break;
        case "Free Text":
            $('#typeTextArea').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");           
            break;
        case "Dropdown":
            $('#value-container').removeClass('hide');
            $("#selectdropdownvalue").removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            break;
        case "LKUP":     
            $("#selectdropdownvalue").addClass('hide');
            $("#selectLKUPvalue").removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            $('#lkup-container').removeClass('hide');
            break;
        case "Checkbox":          
            $('#selectcheckboxvalue').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            var selectchck = $("#selectcheckboxvalue");
            selectchck.children().remove();
            var tempQuestion = $('#Question').val();
            if ($("#VariableType").find('option:selected').data('text') == "Checkbox") {
                selectchck.append($('<label class="custom-control custom-checkbox"><input type="checkbox" name="CheckboxValue" id="CheckboxValue" class="custom-control-input" checked=""/><span class="custom-control-label">' + tempQuestion + '</span></label>'));
            }
            break;
        case "Radio":
            $('#value-container').removeClass('hide');
            $('#selectradiobuttonvalue').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            break;
        case "Numeric (Integer)":
            $('.range-container').removeClass('hide');
            $('#numericvalue').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            $('#fromDataEntryRange').addClass('numberonly');
            $('#toDataEntryRange').addClass('numberonly');
            var d = "^-?\d+$";
            let outsideRangeValidation = $("#OutsideRangeValidation").val();
            try {
                outsideRangeValidation = outsideRangeValidation.length > 0 ? outsideRangeValidation : "Invalid range entered";
            } catch (e) { }
            $("#PreviewPane_ValidationName").val('["Numeric", "Range"]');            
            $("#PreviewPane_ValidationRegEx").val("[" + d + "], null]");
            $("#PreviewPane_ValidationMin").val('[null, null]');
            $("#PreviewPane_ValidationMax").val('[null, null]');
            $("#PreviewPane_ValidationErrorMessage").val('["Numeric Only", "' + outsideRangeValidation + '"]');
            break;
        case "Numeric (Decimal)":
            $('.range-container').removeClass('hide');
            $('#numericvalue').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            $('#fromDataEntryRange').removeClass('numberonly');
            $('#toDataEntryRange').removeClass('numberonly');
            var d = "^-?\\d*\\.{0,1}\\d+$";
            let outsideRangeValidation1 = $("#OutsideRangeValidation").val();
            try {
                outsideRangeValidation1 = outsideRangeValidation1.length > 0 ? outsideRangeValidation1 : "Invalid range entered";
            } catch (e) { }

            $("#PreviewPane_ValidationName").val('["Decimal", "Range"]');    
            $("#PreviewPane_ValidationRegEx").val("[" + d + "], null]");
            $("#PreviewPane_ValidationMin").val('[null, null]');
            $("#PreviewPane_ValidationMax").val('[null, null]');
            $("#PreviewPane_ValidationErrorMessage").val('["Only numbers with decimal", "' + outsideRangeValidation1 + '"]');
            break;
        case "Formula":
            $('#formula-container').removeClass('hide');
            $('#formulavalue').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $('#divValidationRuleIds').addClass('hide');
            break;

        case "Date":
            $('#divDatepicker').removeClass('hide');
            $("#ValidationRuleIds").selectpicker("refresh");
            $(".datetype-container").removeClass("hide");          
            Variable.setDatepicker();

            break;
        case "ColorPicker":
            $("#previewColorPicker").removeClass("hide");           
            break;

        case "FileType":
            $('#previewFileType').removeClass('hide');
            break;

        default:
            break;
    } 
    $("#fromDataEntryRange,#toDataEntryRange,#Value,#Description").val("");
    $("#OutsideRangeValidation").val("");
    $('#fromDataEntryRange,#toDataEntryRange').removeAttr('disabled');
    $("#NoMinimum,#NoMaximum").prop("checked", false);
    $('#Values li').remove();
    $("#DateFormat")[0].selectedIndex = 0;
    $("#CanFutureDate")[0].selectedIndex = 0;
    $("#LookupVariableEntityType")[0].selectedIndex = 0;
    $("#LookupVariableEntitySubtype")[0].selectedIndex = 0;
};

Variable.setFormula = function (variable) {
    Variable.selectedVariable = variable;
};

Variable.addValue = function (value, description) {
    var currentVal = value || $('#Value').val().trim();
    var currentValDesc = description || $('#Description').val().trim();

    if (Variable.checkDuplicate(currentVal, currentValDesc)) {
        if (currentVal.length > 0 && currentValDesc.length > 0) {
            var newValue = $('<li class="list-group-item" data-value="' + currentVal + '" data-description="' + currentValDesc + '"><div class="row"><div class="col-sm-3">' + currentVal + '</div><div class="col-sm-6">' + currentValDesc + '</div>  <div class="col-sm-3"><button type="button" title="Remove" onclick="$(this).parent().parent().parent().remove()" class="btn btn-sm waves-effect waves-light btn-danger pull-right"><i class ="far fa-times-circle"></i></button> </div></div></li>');
            $('#Values').append(newValue);
            if (!value) {
                $('#Value').val('');
                $('#Value').focus();
                $('#Description').val('');
            }
        }
    }
}

Variable.checkDuplicate = function (val, desc) {
    let result = true;
    $('#Values li').each(function (i, row) {
        let dataVal = $(row).attr('data-value');
        let dataDescription = $(row).attr('data-description');
        if (dataVal === val || dataDescription.toLowerCase() === desc.toLowerCase()) {
            result = false;
            return false;
        }
    });
    return result;
}

Variable.onApprovalChange = function (lbl) {
    if ($(lbl).find('input:checkbox').is(':checked')) {
        $('#Comment').parents('.form-group:first').removeClass('hide');
    } else {
        $('#Comment').parents('.form-group:first').addClass('hide');
    }
};

Variable.reset = function () {
    App.resetForm('#variableForm');
    $('#Guid').val('');
    $('#VariableType').change();
};

Variable.confirmEdit = function (guid, isDefaultVariable) {

    if (isDefaultVariable == "0" || isDefaultVariable == "2") {
        $('#btnEditVatiable').text("View");
    } else {
        $('#btnEditVatiable').text("Edit");
    }
    $('#confirm-modal').modal('show');
    Variable.guid = guid;
};

Variable.confirmEditDefault = function (guid, isDefaultVariable) {

    if (isDefaultVariable == "0" || isDefaultVariable == "2") {
        $('#btnEditVatiable').text("View");
    } else {
        $('#btnEditVatiable').text("Edit");
    } 
    Variable.guid = guid;
    Variable.edit(true);
};

Variable.edit = function (isTemplate) {
    let guid = Variable.guid;
    Variable.ActivityStatusId = 0;
    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('ul.variable-categories > li[data-variable = ' + guid + '] > a').addClass('library-li-active');
    var projectid = $("#ProjectId").val();
    App.postData(App.ApiUrl + 'variable/Getactivity/' + guid + "/" + projectid, {}, function (result) {
   
        Variable.ActivityStatusId = result.activityStatusId;
        $("#btnDeleteVariable").attr("onclick", "Variable.deleteConfirm('" + guid + "')");
        if (result.isDefaultVariable == 0 || result.isDefaultVariable == 2) {
            $('#btnSaveVariable').attr("disabled", "disabled");
            $('#btnSaveVariable').addClass("disabled-button-cursor");
            $('#btnDeleteVariable').addClass("hide");
        } else {
            $('#btnSaveVariable').removeAttr("disabled");
            $('#btnSaveVariable').removeClass("disabled-button-cursor");
            $('#btnDeleteVariable').removeClass("hide");
        }

        if (result.isApproved == true && result.userTypeRole == "Project Admin") {
            $('#btnDeleteVariable').addClass("hide");
        }

        if (isTemplate) {
            $('#Guid').val("");
            result.isApproved = false;
            result.comment = "";

            $('#btnDeleteVariable').addClass("hide");
        }
        else {
            $('#Guid').val(guid);
        }


        if (Variable.ActivityStatusId === App.ActivityStatusEnum.Active) {
            $('#btnDeleteVariable').addClass("hide");
        }

        $('#VariableCategoryId').val(result.variableCategoryId);
        $('#VariableName').val(result.variableName);
        $('#VariableLabel').val(result.variableLabel);
        $('#Question').val(result.question);
        $('#VariableType').val(result.variableTypeId).change();
        $('#ValueDescription').val(result.valueDescription);
        $('#HelpText').val(result.helpText);
        $('#ValidationMessage').val(result.validationMessage)      
        $('#VariableRoles').selectpicker('val', result.variableRoles);       
        $('#IsRequired').prop('checked', false);
        $('#CanCollectMultiple').prop('checked', result.canCollectMultiple);
        $('#ValidationRuleId').val(result.validationRuleId);        
        $('#Comment').attr('data-comment-required', false);     
        if (result.isApproved) {
            $('#IsDraft').val('false');
            $('#IsApproved').val('true');
        } else {
            $('#IsDraft').val('true');
            $('#IsApproved').val('false');
        }


        if ($('#IsSystemAdmin').val() == "true") {
            if (!result.isApproved) {                
                $('#IsVariableLogTable').val('true');
            } else {                
                $('#IsVariableLogTable').val('false');
            }
        } else {
            $('#IsVariableLogTable').val('false');
        }

        $('#Comment').val(result.comment);
        $('#Values').empty();
        $('#Formula').val(result.values[0]);
        $('#PreviewPane_ValidationMessage').empty();
        $('#PreviewPane_IsRequiredMessage').empty();

        $('#ValidationRuleIds').selectpicker('val', result.validationRuleIds);

        //Changes due to Jira Ticket Id ASPMONASH-214
        if (result.variableName == "ProRole") {
            let intarray = result.values.map(function (item) {
                let a = (parseInt(item));
                a = --a;
                return a + "";
            });
            result.values = intarray;
        }

        for (var i = 0; i < result.values.length; i++) {
            var value = result.values[i];
            var description = result.variableValueDescription[i];
            Variable.addValue(value, description);
        }
      
        $('#toDataEntryRange').val(result.maxRange);
        $('#fromDataEntryRange').val(result.minRange);

        result.maxRange === null ? $('#NoMaximum').prop('checked', true) : $('#NoMaximum').prop('checked', false);
        result.minRange === null ? $('#NoMinimum').prop('checked', true) : $('#NoMinimum').prop('checked', false);

        result.maxRange === null ? $('#toDataEntryRange').attr('disabled', 'disabled') : $('#toDataEntryRange').removeAttr('disabled');
        result.minRange === null ? $('#fromDataEntryRange').attr('disabled', 'disabled') : $('#fromDataEntryRange').removeAttr('disabled');

        result.helpText != null && result.helpText != "" ? $('#isHelpTextRequired').prop('checked', true) : $('#isHelpTextRequired').prop('checked', false);
        result.helpText != null && result.helpText != "" ? $('#divHelpText').removeClass('hide') : $('#divHelpText').addClass('hide');



        var PreviewPane_ValidationTypes = [];
        for (var key in result.validationRuleViewModel) {

            PreviewPane_ValidationTypes.push(result.validationRuleViewModel[key].ruleType);
        }
        $('#PreviewPane_ValidationTypes').val(PreviewPane_ValidationTypes);



        var PreviewPane_ValidationName = [];
        var PreviewPane_ValidationRegEx = [];
        var PreviewPane_ValidationMin = [];
        var PreviewPane_ValidationMax = [];
        var PreviewPane_ValidationErrorMessage = [];

        for (var key in result.variableValidationRuleViewModel) {

            PreviewPane_ValidationName.push(result.variableValidationRuleViewModel[key].validationName);
            PreviewPane_ValidationRegEx.push(result.variableValidationRuleViewModel[key].regEx);
            PreviewPane_ValidationMin.push(result.variableValidationRuleViewModel[key].min);
            PreviewPane_ValidationMax.push(result.variableValidationRuleViewModel[key].max);
            PreviewPane_ValidationErrorMessage.push(result.variableValidationRuleViewModel[key].validationMessage);

            if (result.variableValidationRuleViewModel[key].validationName == "Range" && (result.variableTypeName == "Numeric (Integer)" || result.variableTypeName == "Numeric (Decimal)")) {
                $("#OutsideRangeValidation").val(result.variableValidationRuleViewModel[key].validationMessage);
            }   
        }
        $("#OutsideDataEntryRangeRequiredStat").text(result.maxRange == null && result.minRange == null ? "" : "*"); 
        $("#MissingValidation").val(result.requiredMessage);       
        $('#PreviewPane_ValidationName').val(JSON.stringify(PreviewPane_ValidationName));
        $('#PreviewPane_ValidationRegEx').val(JSON.stringify(PreviewPane_ValidationRegEx));
        $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
        $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
        $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage)); 
        if (result.variableTypeName == "LKUP") {
            $("#LookupVariableEntityType").val(result.lookupEntityType);
            Variable.EntityTypeChange($("#LookupVariableEntityType"));
            $("#LookupVariableEntitySubtype").val(result.lookupEntitySubtype);
        }
        if (result.variableTypeName == "Date") {

            var dateformate = "DDMMYYYY";
            $.each(result.variableValidationRuleViewModel, function (key, value) {
                if (value.validationName == "Date") {
                    dateformate = "DDMMYYYY";
                } else {
                    dateformate = value.validationName.replace("Date_", "");
                }
            });
            $("#DateFormat").val(dateformate);
            $("#CanFutureDate").val('' + result.canFutureDate);
            $('#StartYear').val(result.minRange);
            $('#StopYear').val(result.maxRange);
            Variable.setDatepicker(dateformate);
        }
        Variable.refresh();
    }, "GET");
    $('#confirm-modal').modal('hide');
}

Variable.search = function (type) {
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

Variable.save = function () {
    let url = $('#Guid').val() == "" ? "variable" : "variable/" + $('#Guid').val();
    let methodeType = $('#Guid').val() == "" ? "POST" : "PUT";
    var varType = $('#VariableType option:selected').data('text');

    if (methodeType == "PUT") {
        if (Variable.ActivityStatusId === App.ActivityStatusEnum.Active) {
            App.showError("This variable is used in an activity that is currently active (on the Summary page). To edit this variable, you must first inactivate all the activities that are linked to this variable on the Testing/Deployment page!");
            return false;
        }
    }
    

    if (varType == "Numeric (Integer)" || varType == "Numeric (Decimal)") {
        varType = "Numeric";
    }

    if (!Variable.isValidForm()) {
        return false;
    }

    var IsApproved = false;
    IsApproved = $('#IsApproved').val();   
    if (!$('#isHelpTextRequired').is(':checked')) {
        $("#HelpText").val("");
    }
    let sendData = {
        Guid: $('#Guid').val(),
        VariableCategoryId: $('#VariableCategoryId').val(),
        VariableName: $('#VariableName').val(),
        VariableLabel: $('#VariableName').val(),     
        Question: $.trim($('#Question').val()),
        VariableTypeId: $('#VariableType').val(),
        ValueDescription: $('#ValueDescription').val(),
        HelpText: $('#HelpText').val(),
        VariableRoles: $('#VariableRoles').val(),
        ValidationMessage: $('#ValidationMessage').val(),
        IsRequired: false,
        CanCollectMultiple: $('#CanCollectMultiple').is(':checked'),
        ValidationRuleId: $('#ValidationRuleId').val(),     
        IsApproved: IsApproved,
        Comment: $('#Comment').val(),
        Values: [],
        VariableValueDescription: [],   
        ValidationRuleIds: $('#ValidationRuleIds').val(),
        CustomRegEx: $('#CustomRegEx').val(),
        IsVariableLogTable: $('#IsVariableLogTable').val(),
        DateFormat: $("#DateFormat").val(),
        CanFutureDate: varType == "Date" ? $("#CanFutureDate").val() : null,
    };

    sendData.RequiredMessage = $('#MissingValidation').val().length > 0 ? $('#MissingValidation').val() : null;
    if (varType != 'Free Text' && varType != 'Numeric' && varType != 'Text Box' && varType != 'Date') {
        sendData.Values = $('#Values li').map(function () { return $(this).data('value').toString() }).get();
        sendData.VariableValueDescription = $('#Values li').map(function () { return $(this).data('description').toString() }).get();

        if (varType == "Checkbox") {
            sendData.Values = [];
            sendData.VariableValueDescription = [];
            sendData.Values.push(1);
            sendData.VariableValueDescription.push(sendData.Question);
        }
    }

    if (varType == 'Formula') {
        sendData.Values = [$('#Formula').val()];
    }

    if (varType == "LKUP") {
        sendData.LookupEntityType = $("#LookupVariableEntityType").val();
        sendData.LookupEntitySubtype = $("#LookupVariableEntitySubtype").val();
    }

    var slider = $("#AbsoluteRange").data("ionRangeSlider");

    if (varType == 'Numeric') {
        sendData.OutsideRangeValidation = $('#OutsideRangeValidation').val();
        sendData.MissingValidation = $('#MissingValidation').val();
        sendData.MinRange = $('#fromDataEntryRange').val();
        sendData.MaxRange = $('#toDataEntryRange').val();
    }
    if (varType == "Date") {
        sendData.MinRange = $('#StartYear').val();
        sendData.MaxRange = $('#StopYear').val();
    }
    App.postData(App.ApiUrl + url, sendData, function (result) {
        var redirect = '/Variables';
        if (window.location.pathname.toLowerCase().indexOf("projectbuilder") > -1) {
            redirect = "/ProjectBuilder/Variables/" + $('#ProjectId').val();
        }        
        App.redirectSuccess($('#Guid').val() != "" ? "Variable was modified successfully." : "Variable was added successfully.", redirect);
    }, methodeType);
}

Variable.isValidForm = function () {
    var vnTxt = $('#VariableName').val();
    var vqTxt = $('#Question').val();
    var cmTxt = $('#Comment').val();

    let numTyp = $("#VariableType option:selected").html();
    let isComment = $('#Comment').attr('data-comment-required');
    let variableCategoryId = $('#VariableCategoryId').val();

    let result = 0;
    if (vnTxt.length <= 0) {
        $('#VariableName').addClass('input-validation-error');
        result++;
    } else {
        $('#VariableName').removeClass('input-validation-error');
    }
    if (vqTxt.length <= 0) {
        $('#Question').addClass('input-validation-error');
        result++;
    } else {
        $('#Question').removeClass('input-validation-error');
    }

    if ($('#isHelpTextRequired').is(':checked')) {
        $('#divHelpText').removeClass("hide");
        if ($("#HelpText").val().length <= 0) {
            $('#HelpText').addClass('input-validation-error');
            result++;
        } else {
            $('#HelpText').removeClass('input-validation-error');
        }
    }





    if (isComment == 'true') {
        if (cmTxt.length <= 0) {
            $('#Comment').addClass('input-validation-error');
            result++;
        }
    } else {
        $('#Comment').removeClass('input-validation-error');
    }

    if (numTyp == "Numeric (Integer)" || numTyp == "Numeric (Decimal)") {
        let outsiderangeValidation = $("#OutsideRangeValidation").val();     
        try { outsiderangeValidation = outsiderangeValidation.trim(); } catch (e) {  }

        if (outsiderangeValidation.length <= 0) {

            if ($("#toDataEntryRange").val().length <= 0 && $("#fromDataEntryRange").val().length <= 0) {
                $('#OutsideRangeValidation').removeClass('input-validation-error');
            } else {
                $('#OutsideRangeValidation').addClass('input-validation-error');
                result++;
            }
        } else {
            $('#OutsideRangeValidation').removeClass('input-validation-error');
        }      

        let MinRange = $("#fromDataEntryRange").val();
        let MaxRange = $("#toDataEntryRange").val();

        try {
            MinRange = parseFloat(MinRange);
            MaxRange = parseFloat(MaxRange);
            if (MaxRange < MinRange) {
                result++;
                $("#fromDataEntryRange").addClass('input-validation-error');
                $("#toDataEntryRange").addClass('input-validation-error');
                $('span[data-valmsg-for="AbsoluteRange"]').addClass("field-validation-error").text('Enter maximum value greater than minimum value');
            } else {
                $("#fromDataEntryRange").removeClass('input-validation-error');
                $("#toDataEntryRange").removeClass('input-validation-error');
                $('span[data-valmsg-for="AbsoluteRange"]').addClass("field-validation-error").text('');
            }
        } catch (e) {
           
        }

        var isMinCheck = $("#NoMinimum").is(":checked");
        var isMaxCheck = $("#NoMaximum").is(":checked");
        $("#fromDataEntryRange, #toDataEntryRange").removeClass('input-validation-error');
        if (!isMinCheck && $("#fromDataEntryRange").val().trim() == "") {
            $("#fromDataEntryRange").addClass('input-validation-error');
            result++;
        }
        if (!isMaxCheck && $("#toDataEntryRange").val().trim() == "") {
            $("#toDataEntryRange").addClass('input-validation-error');
            result++;
        }

    }

    if (numTyp == "LKUP") {
        let entityType = $("#LookupVariableEntityType").val();
        let entitySubtype = $("#LookupVariableEntitySubtype").val();
        if (entityType != null ? entityType.length <= 0 : true) {
            $('#LookupVariableEntityType').addClass('input-validation-error');
            result++;
        } else {
            $('#LookupVariableEntityType').removeClass('input-validation-error');
        }
        if (Variable.IsEntitySubType) {
            if (entitySubtype != null ? entitySubtype.length <= 0 : true) {
                $('#LookupVariableEntitySubtype').addClass('input-validation-error');
                result++;
            } else {
                $('#LookupVariableEntitySubtype').removeClass('input-validation-error');
            }
        } else {
            $('#LookupVariableEntitySubtype').removeClass('input-validation-error');
        }
    }

    if (numTyp == "Dropdown") {
        $('#Value').removeClass("input-validation-error");
        $('#Description').removeClass("input-validation-error");
        $("#btnAddValue").removeClass("input-validation-error");
        let optionValues = [];
        optionValues = $('#Values li').map(function () { return $(this).data('value').toString() }).get();

        if (optionValues.length <= 0) {
            result++;

            if ($('#Value').val().length < 1) {
                $('#Value').addClass("input-validation-error");
            }
            if ($('#Description').val().length < 1) {
                $('#Description').addClass("input-validation-error");
            }
            if ($('#Value').val().length > 0 && $("#Description").val().length > 0) {
                $("#btnAddValue").addClass("input-validation-error");
            } else {
                $("#btnAddValue").removeClass("input-validation-error");
            }
        }
    }

    if (numTyp == "Date") {
        $('#DateFormat').removeClass("input-validation-error");
        $('#CanFutureDate').removeClass("input-validation-error");
        $('#StartYear').removeClass('input-validation-error');
        $('#StopYear').removeClass('input-validation-error');
        $('span[data-valmsg-for="YearRange"]').addClass("field-validation-error").text('');

        let dateformat = $("#DateFormat").val();
        let canfuturedate = $("#CanFutureDate").val();

        if (dateformat.length <= 0) {
            $('#DateFormat').addClass('input-validation-error');
            result++;
        }
        if (canfuturedate <= 0) {
            $('#CanFutureDate').addClass('input-validation-error');
            result++;
        }
        let startYear = $("#StartYear").val();
        if (startYear.length <= 0) {
            $('#StartYear').addClass('input-validation-error');
            result++;
        }
        let stopYear = $("#StopYear").val();
        if (stopYear.length <= 0) {
            $('#StopYear').addClass('input-validation-error');
            result++;
        }

        if (startYear > stopYear) {
            $('#StopYear').addClass('input-validation-error');
            $('#StartYear').addClass('input-validation-error');
            $('span[data-valmsg-for="YearRange"]').addClass("field-validation-error").text('Start date should be greater than end date');
            result++;
        }
    }

    if (variableCategoryId.length <= 0) {
        $('#VariableCategoryId').addClass('input-validation-error');
        result++;
    } else {
        $('#VariableCategoryId').removeClass('input-validation-error');
    }

    if (result > 0) {
        return false;
    } else {
        return true;
    }
}


Variable.deleteConfirm = function (guid, btn) {
    Variable.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected variable?");
}

Variable.delete = function (guid) {
    App.postData("/variable/delete/" + Variable.Current, {}, function (result) {
        if (result.MessageType == "Success") {
            var role = JSON.parse(result.Content);
            if (role.dateDeactivated == null) {
                App.showSuccess("Variable was activated successfully.");
            }
            else {
                App.showSuccess("Variable was deactivated successfully.");
            }
            Variable.dataTable.refresh()
        }
    });
}

Variable.refresh = function () {
    $("#typeNumeric").val("");
    $("#QuestionType").empty();
    $("#typeTextArea").val("");
    $("#PreviewLKUPDD").children('option').hide();
    $("#PreviewLKUPDD").children("option[value='0']").show();

    $("#PreviewPane_IsRequiredMessage").text("");
    $("#PreviewPane_ValidationMessage").text("");
    var tempQuestion = $('#Question').val();
    $("#ShowQuestion").text(tempQuestion);


    if ($('#IsRequired').is(':checked')) {
    }
    
    $("#PreviewPane_ValidationMessage").text("");
    Variable.onQuestionTypeChange();
};

Variable.onQuestionTypeChange = function () {
    $("#previewHelpText").text("");
    var text = $("#VariableType");
    var selectdropdown = $("#QuestionType");
    selectdropdown.children().remove();
    selectdropdown.append($("<option>").val("0").text("Select"));
    $(".list-group-item").each(function (e) {
        var dataVal = $(this).attr('data-value');
        var dataDescription = $(this).attr('data-description');
        selectdropdown.append($("<option>").val(dataVal).text(dataDescription));
    });

    var selectchck = $("#selectcheckboxvalue");
    selectchck.children().remove();
    var tempQuestion = $('#Question').val();
    if ($("#VariableType").find('option:selected').data('text') == "Checkbox") {
        selectchck.append($('<label class="custom-control custom-checkbox"><input type="checkbox" name="CheckboxValue" id="CheckboxValue" class="custom-control-input" checked=""/><span class="custom-control-label">' + tempQuestion + '</span></label>'));
    }
    else {
        $(".list-group-item").each(function (e) {
            var dataVal = $(this).attr('data-value');
            var dataDescription = $(this).attr('data-description');
            selectchck.append($('<label class="custom-control custom-checkbox"><input type="checkbox" name="CheckboxValue" id="CheckboxValue" class="custom-control-input" checked=""/><span class="custom-control-label">' + dataDescription + '</span></label>'));
        });
    }

    var selectradio = $("#selectradiobuttonvalue");
    selectradio.children().remove();
    $(".list-group-item").each(function (e) {
        var dataVal = $(this).attr('data-value');
        var dataDescription = $(this).attr('data-description');
        selectradio.append($('<label class="custom-control custom-radio"><input type="radio" name="RadioButtonValue" id="RadioButtonValue" value="{0}" class="custom-control-input"/><span class="custom-control-label">' + dataDescription + '</span></label>'));
    });
    $("input:radio[name=RadioButtonValue]:first").attr('checked', true); // default select first checked
    if ($(text).find("option:selected").data("text") == "ColorPicker") {
        $("#previewColorPicker").removeClass("hide");
    }

    if ($(text).find("option:selected").data("text") == "Date") {
        Variable.setDatepicker($("#DateFormat").val());
    }


    if ($(text).find("option:selected").data("text") == "Numeric (Integer)") {
        $("#numericvalue").val("");


        Variable.validMinimumAndMaximumRange();

    }
    if ($(text).find("option:selected").data("text") == "Numeric (Decimal)") {
        $("#numericvalue").val("");
        Variable.validMinimumAndMaximumRange();
    }


    if ($(text).find("option:selected").data("text") == "LKUP") {
        Variable.FillLKUPPreview();
    }
};

$('.preview-variable').on('blur change', function () {
    let inputVal = $(this).val();
    let PreviewPane_ValidationName = JSON.parse($('#PreviewPane_ValidationName').val());
    let PreviewPane_ValidationMin = JSON.parse($('#PreviewPane_ValidationMin').val());
    let PreviewPane_ValidationMax = JSON.parse($('#PreviewPane_ValidationMax').val());
    let PreviewPane_ValidationErrorMessage = JSON.parse($('#PreviewPane_ValidationErrorMessage').val());
    let PreviewPane_ValidationRegEx = [];
    try {
        PreviewPane_ValidationRegEx = JSON.parse($('#PreviewPane_ValidationRegEx').val());
    } catch (e) {
        if ($("#VariableType").find('option:selected').data('text') == "Numeric (Integer)") {
            PreviewPane_ValidationRegEx.push("^-?\\d+$");
            PreviewPane_ValidationRegEx.push(null);
        }
        if ($("#VariableType").find('option:selected').data('text') == "Numeric (Decimal)") {
            PreviewPane_ValidationRegEx.push("^-?\\d*\\.{0,1}\\d+$");
            PreviewPane_ValidationRegEx.push(null);
        }
       
    }
    if ($('#PreviewPane_IsRequiredMessage:visible').length == 0) {
    } else {
        if (inputVal != "") {
            $('#PreviewPane_IsRequiredMessage').empty();
        } else {
            
        }
    }
    var a = App.checkValidation(inputVal, PreviewPane_ValidationName, PreviewPane_ValidationRegEx, PreviewPane_ValidationMin, PreviewPane_ValidationMax, PreviewPane_ValidationErrorMessage)

    if (inputVal == "") {
        a.result1 = "";
    }
    $('#PreviewPane_ValidationMessage').text(a.result1);
});

$('#QuestionType').on('change', function () {
  
});
$('#RadioButtonValue').on('selectradiobuttonvalue', function () {
    let inputVal = $('#RadioButtonValue').val();

    let PreviewPane_ValidationName = JSON.parse($('#PreviewPane_ValidationName').val());
    let PreviewPane_ValidationRegEx = JSON.parse($('#PreviewPane_ValidationRegEx').val());
    let PreviewPane_ValidationMin = JSON.parse($('#PreviewPane_ValidationMin').val());
    let PreviewPane_ValidationMax = JSON.parse($('#PreviewPane_ValidationMax').val());
    let PreviewPane_ValidationErrorMessage = JSON.parse($('#PreviewPane_ValidationErrorMessage').val());

    if ($('#PreviewPane_IsRequiredMessage:visible').length == 0) {
    } else {
        if ($('#RadioButtonValue').val() != "0") {
            $('#PreviewPane_IsRequiredMessage').empty();
        } else {
            $('#PreviewPane_IsRequiredMessage').text('This field is required');
        }
    }
    var a = Variable.checkValidation(inputVal, PreviewPane_ValidationName, PreviewPane_ValidationRegEx, PreviewPane_ValidationMin, PreviewPane_ValidationMax, PreviewPane_ValidationErrorMessage)
    $('#PreviewPane_ValidationMessage').text(a);
});

$("#ValidationRuleIds").change(function () {
    let selectedValue = $(this).val();

    let selectedText = [];
    let selectedValuesArray = [];
    for (var i = 0; i < selectedValue.length; i++) {
        selectedText.push($("#ValidationRuleIds option[value='" + selectedValue[i] + "']").text());
        selectedValuesArray.push({
            key: selectedValue[i],
            value: $("#ValidationRuleIds option[value='" + selectedValue[i] + "']").text()
        });
    }
    if (selectedValue.includes("{00000000-0000-0000-0000-000000000000}")) {
        $('#divCustomRegEx').show();      
    } else {
        $('#divCustomRegEx').hide();        
    }
});

Variable.checkValidation = function (inputField, ValidationType, RegExPattern, minLength, maxLength, errorMessage) {

    let result = false;
    let result1 = "";
    for (var i = 0; i < ValidationType.length; i++) {

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

            case "Numeric (Integer)": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Numeric (Decimal)": {

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

                if (parseInt(inputField) >= minLength[i] && parseInt(inputField) <= maxLength[i])
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }

            case "Required": {

                if (inputField.trim() != "")
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

            case "Custom": {

                if (new RegExp(RegExPattern[i]).test(inputField))
                    result = true;
                else
                    result1 = errorMessage[i];
                break;
            }
            default: {
                result = true;
                break;
            }
        }
        if (!result) {


            break;
        }
    }
    return result1;
}

Variable.AllEntityTypeType = function () {
    Variable.EntityType = [];
    Variable.EntitySubType = [];

    let dropdown = $('#LookupVariableEntityType');
    dropdown.empty();
    dropdown.append('<option selected="true" value="" disabled>Entity Type</option>');
    dropdown.prop('selectedIndex', 0);

    App.postData(App.ApiUrl + "/EntityType", {}, function (result) {
        Variable.EntityType = result;
        $.each(result, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        });
    }, "Get", true);
}

Variable.AllEntityTypeSubtype = function () {
    Variable.EntityType = [];
    Variable.EntitySubType = [];

    let dropdown = $('#LookupVariableEntityType');
    dropdown.empty();
    dropdown.append('<option selected="true" value="" disabled>Entity Type</option>');
    dropdown.prop('selectedIndex', 0);

    App.postData(App.ApiUrl + "/EntitySubType", {}, function (result) {
        Variable.EntitySubType = result;
        $.each(result, function (key, entry) {
            let isEntityType = $.grep(Variable.EntityType, function (obj) { return obj.guid == entry.entityTypeId; });
            if (isEntityType.length === 0) {
                Variable.EntityType.push({
                    'guid': entry.entityTypeId,
                    'name': entry.entityTypeName,
                });
                dropdown.append($('<option></option>').attr('value', entry.entityTypeId).text(entry.entityTypeName));
            }
        });
    }, "Get", true);
}

Variable.EntityTypeChange = function (e) {
    Variable.IsEntitySubType = false;
    let dropdown = $('#LookupVariableEntitySubtype');
    let entityTypeId = $(e).val();

    dropdown.empty();
    dropdown.append('<option selected="true" disabled>Entity Subtype</option>');
    dropdown.prop('selectedIndex', 0);

    if (Variable.EntitySubType.length > 0) {

    }


    $.each(Variable.EntitySubType, function (key, entry) {
        if (entry.entityTypeId == entityTypeId) {
            Variable.IsEntitySubType = true;
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        }
    });

    $("#LookupVariableEntitySubtype option").eq(1).before($("<option></option>").val("00000000-0000-0000-0000-000000000000").text("All"));



}


Variable.EntitySubTypeChange = function (e) {



}


//======================================================
//  Reset variabl (clear variable)
//======================================================
Variable.resetVariable = function () {

    $('#Guid').val("");    
    $('#VariableName').val("");
    $('#Question').val("");
    $('#VariableType').prop('selectedIndex', 0);
    $('#ValueDescription').val("");
    $('#HelpText').val("");

    $('#ValidationRuleIds').selectpicker('deselectAll');
    $('#ValidationMessage').val("");
    $('#CanCollectMultiple').prop('checked', false);
    $('#VariableRoles').selectpicker('deselectAll');

    $('#ValidationRuleId').val("");//??
    $('#IsApproved').prop('selectedIndex', 0);
    $('#Comment').val("");
    $('#CustomRegEx').val("");
    $('#IsVariableLogTable').val("");//??
    $('#Values').empty();
    $("#LookupVariableEntityType").prop('selectedIndex', 0);
    $("#LookupVariableEntitySubtype").prop('selectedIndex', 0);
    $('#OutsideRangeValidation').val("");
    $('#MissingValidation').val("");
    $('#MaxRange').val("");
    $('#MinRange').val("");
    $('#btnDeleteVariable').addClass("hide");//PC
    $('#value-container').addClass("hide");
    $('#lkup-container').addClass("hide");
    $('#range-container').addClass("hide");
    $('#formula-container').addClass("hide");
    $('.range-container').addClass('hide');
    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('#btnSaveVariable').removeAttr("disabled");
    $('#btnSaveVariable').removeClass("disabled-button-cursor");
    $('#VariableCategoryId').prop('selectedIndex', -1);
    $('#VariableCategoryId > option').each(function (index) {
        let optionType = $(this).attr('disabled');
        if (optionType === undefined) {
            $('#VariableCategoryId').prop('selectedIndex', index);
            return false;
        }
    });
    $(".input-validation-error").removeClass("input-validation-error");
    $(".field-validation-error").text("");
    $(".datetype-container").addClass("hide");
    $("#StartYear").val("");
    $("#StopYear").val("");
    Variable.refresh();
}

//==============================================================
//          Is comment textbox validation
//==============================================================
Variable.onIsApprovedChange = function (ddl) {
    let ddlVal = $(ddl);
    switch (ddlVal.val()) {
        case "false":
            $('#Comment').attr('data-comment-required', false);
            break;
        case "true":
            $('#Comment').attr('data-comment-required', true);
            break;
        default:
            break;
    }
};

$("#toDataEntryRange, #fromDataEntryRange, #OutsideRangeValidation").on('change', function () {
    var tempMax = $('#toDataEntryRange').val();
    try { tempMax = tempMax.length > 0 ? tempMax : null; } catch (e) { }
    var PreviewPane_ValidationMax = [null, tempMax];
    $('#PreviewPane_ValidationMax').val(JSON.stringify(PreviewPane_ValidationMax));
    var tempMin = $('#fromDataEntryRange').val();
    try { tempMin = tempMin.length > 0 ? tempMin : null; } catch (e) { }
    var PreviewPane_ValidationMin = [null, tempMin];
    $('#PreviewPane_ValidationMin').val(JSON.stringify(PreviewPane_ValidationMin));
    var tempValMsg = $('#OutsideRangeValidation').val();
    try { tempValMsg = tempValMsg.length > 0 ? tempValMsg : "Invalid range entered"; } catch (e) { }
    var PreviewPane_ValidationErrorMessage = ["Numeric Only", tempValMsg];
    $('#PreviewPane_ValidationErrorMessage').val(JSON.stringify(PreviewPane_ValidationErrorMessage));
    $('#PreviewPane_ValidationMessage').val(tempValMsg);
    if ($("#VariableType").find('option:selected').data('text') == "Numeric (Integer)") {
        let d = "^-?\\d*\\.{0,1}\\d+$";
        let outsideRangeValidation1 = $("#OutsideRangeValidation").val();
        try { outsideRangeValidation1 = outsideRangeValidation1.length > 0 ? outsideRangeValidation1 : "Invalid range entered"; } catch (e) { }
        $("#PreviewPane_ValidationErrorMessage").val('["Numeric Only", "' + outsideRangeValidation1 + '"]');
        $("#PreviewPane_ValidationName").val('["Numeric", "Range"]');
        $("#PreviewPane_ValidationRegEx").val("[" + d + "], null]");
    }
    else if ($("#VariableType").find('option:selected').data('text') == "Numeric (Decimal)") {
        let d = "^-?\\d+$";
        let outsideRangeValidation1 = $("#OutsideRangeValidation").val();
        try { outsideRangeValidation1 = outsideRangeValidation1.length > 0 ? outsideRangeValidation1 : "Invalid range entered"; } catch (e) { }
        $("#PreviewPane_ValidationErrorMessage").val('["Only numbers with decimal", "' + outsideRangeValidation1 + '"]');
        $("#PreviewPane_ValidationName").val('["Decimal", "Range"]');
        $("#PreviewPane_ValidationRegEx").val("[" + d + "], null]");
    }
});

//==============================================================
//          sat datepicker
//==============================================================
Variable.setDatepicker = function (dateformate) {

    $('#divDatepicker').empty();
    var container = $('#divDatepicker');
    var template = $('#datepickerTemplate').children().clone();

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

    let canfutureDate = $("#CanFutureDate").val();
    let maxYear = $("#StopYear").val();//new Date().getYear();
    let minYear = $("#StartYear").val();//new Date().getYear();
    template.find('.singledate').editable({      
        mode: 'inline',
        maxDate: 0,
        format: format,
        viewformat: format,//'DD.MM.YYYY',    
        template: format,
        combodate: {
            minYear: minYear,
            maxYear: maxYear,
            maxDate: 5,
            minuteStep: 1
        },
        datepicker: {
            onChangeMonthYear: function (year, month, inst) {

                $(this).datepicker('setDate', new Date(year, month - 1, 1));
            }
        }
    });
    container.append(template);
    $(".customdatepicker").on("click", function (dateElement) {
        let e1 = $(dateElement).prop("target");
        $('#PreviewPane_ValidationMessage').text("");
        setTimeout(function () {
            $("form .editableform").on("submit", function () {
                let canfuturedateElement = $(dateElement).prop("target");
                if (canfutureDate === "false") {
                    let dateFormat = $("#DateFormat").val();
                    let tempnewDate = $(canfuturedateElement).text();
                    let dateArray = tempnewDate.split("-");
                    let currentDate = new Date().getDate();
                    let currentMonth = new Date().getMonth() + 1;
                    let currentYear = new Date().getFullYear();
                    let currentDateValue = new Date(currentMonth + "-" + currentDate + "-" + currentYear);
                    let enteredDate;
                    let enteredMonth;
                    let enteredYear;
                    let enteredDateValue;
                    let newDate;
                    let format = "DD-MM-YYYY";
                    switch (dateFormat) {
                        case "DDMMYYYY":
                            format = "DD-MM-YYYY";
                            enteredDate = dateArray[0];
                            enteredMonth = dateArray[1];
                            enteredYear = dateArray[2];
                            enteredDateValue = new Date(enteredMonth + "-" + enteredDate + "-" + enteredYear);
                            break;
                        case "MMDDYYYY":
                            format = "MM-DD-YYYY";
                            enteredDate = dateArray[1];
                            enteredMonth = dateArray[0];
                            enteredYear = dateArray[2];
                            enteredDateValue = new Date(enteredMonth + "-" + enteredDate + "-" + enteredYear);
                            break;
                        case "MMYYYY":
                            format = "MM-YYYY";
                            enteredDate = new Date().getDate();
                            enteredMonth = dateArray[0];
                            enteredYear = dateArray[1];
                            enteredDateValue = new Date(enteredMonth + "-" + enteredDate + "-" + enteredYear);
                            break;
                        case "YYYY":
                            format = "YYYY";
                            enteredDate = new Date().getDate();
                            enteredMonth = new Date().getMonth() + 1;
                            enteredYear = dateArray[0];
                            enteredDateValue = new Date(enteredMonth + "-" + enteredDate + "-" + enteredYear);
                            break;
                        case "DDMMMYYYY":
                            format = "DD-MMM-YYYY";
                            enteredDate = dateArray[0];
                            enteredMonth = dateArray[1];
                            enteredYear = dateArray[2];
                            enteredDateValue = new Date(enteredDate + "-" + enteredMonth + "-" + enteredYear);
                            break;
                        case "MMMYYYY":
                            format = "MMM-YYYY";
                            enteredDate = new Date().getDate();
                            enteredMonth = dateArray[0];
                            enteredYear = dateArray[1];
                            enteredDateValue = new Date(enteredDate + "-" + enteredMonth + "-" + enteredYear);
                            break;
                        default:
                            break;
                    }
                    if (enteredDateValue > currentDateValue || enteredDateValue == "Invalid Date") {
                        //if (enteredDateValue > currentDateValue) {
                        $(canfuturedateElement).text("");
                        $('#PreviewPane_ValidationMessage').text("Future dates are not allowed.");
                    }
                    else {

                    }
                }
            });
        }, 300);
    });
}

// This code to show popup delete confirmation for the Form elements
Variable.deleteConfirm = function (guid) {
    Variable.Current = guid;
    Variable.Current = $('#Guid').val();
    $('#manage-confirm .modal-body .sf-submit-info').html("Are you sure to delete selected Variable?");
}
// This code to show popup delete confirmation for the Form elements
Variable.delete = function (guid) {
    
    App.postData(App.ApiUrl + "/Variable/" + Variable.Current, {}, function (result) {
        
        App.redirectSuccess($('#Guid').val() != "" ? "Variable was deleted successfully." : "Variable was added successfully.", '/ProjectBuilder/variables/' + $('#ProjectId').val());
    }, "Delete");
}


Variable.validMinimumAndMaximumRange = function () {


    let MinRange = $("#fromDataEntryRange").val();
    let MaxRange = $("#toDataEntryRange").val();

    try {
        MinRange = parseFloat(MinRange);
        MaxRange = parseFloat(MaxRange);
        if (MaxRange < MinRange) {

            $("#fromDataEntryRange").addClass('input-validation-error');
            $("#toDataEntryRange").addClass('input-validation-error');
            $('span[data-valmsg-for="AbsoluteRange"]').addClass("field-validation-error").text('Enter maximum value greater than minimum value');
        } else {
            $("#fromDataEntryRange").removeClass('input-validation-error');
            $("#toDataEntryRange").removeClass('input-validation-error');
            $('span[data-valmsg-for="AbsoluteRange"]').addClass("field-validation-error").text('');
        }
    } catch (e) {
        
    }


}




Variable.FillLKUPPreview = function () {    
    $("#PreviewPane_IsRequiredMessage").text("");
    $("#PreviewPane_ValidationMessage").text("");
    $("#PreviewLKUPDD").children('option').hide();
    $("#PreviewLKUPDD").children("option[value='0']").show();
    $("#PreviewLKUPDD").prop('selectedIndex', 0);
    let etype = $("#LookupVariableEntityType").val();
    let esubtype = $("#LookupVariableEntitySubtype").val();
    if (esubtype == null || esubtype == "") {
        esubtype = "00000000-0000-0000-0000-000000000000";
    }
    if (etype == "") {
        $("#PreviewLKUPDD").children('option').show();
        $("#PreviewLKUPDD").children('option').hide();
    } else if (etype != "" && esubtype == "00000000-0000-0000-0000-000000000000") {
        if ($("#LookupVariableEntityType").find('option:selected').text() == "Place/Group") {
            $("#PreviewLKUPDD > option").each(function (e, v) {
                if ($(this).attr("data-entity-group-guid") == etype) {
                    $("#PreviewLKUPDD").children("option[data-id=" + $(this).attr('data-id') + "]").show();
                }
            });
        } else {
            $("#PreviewLKUPDD > option").each(function (e, v) {
                if ($(this).attr("data-entity-type") == etype) {
                    $("#PreviewLKUPDD").children("option[data-id=" + $(this).attr('data-id') + "]").show();
                }
            });
        }
    }
    else if (etype != "" && esubtype != "00000000-0000-0000-0000-000000000000") {
        $("#PreviewLKUPDD > option").each(function (e, v) {
            if ($(this).attr("data-entity-type") == etype && $(this).attr("data-entity-sub-type") == esubtype) {
                
                $("#PreviewLKUPDD").children("option[data-id=" + $(this).attr('data-id') + "]").show();
            }
        });
    }
}

Variable.FillYearDropdown = function () {

    try {
        //Start Year
        let dropdownStartYear = $('#StartYear');
        dropdownStartYear.empty();
        dropdownStartYear.append('<option selected="true" value="">Start Year</option>');
        dropdownStartYear.prop('selectedIndex', 0);
        for (var startY = 1919; startY < 2120; startY++) {
            dropdownStartYear.append($('<option></option>').attr('value', startY).text(startY));
        }
    } catch (eStart) {
   
    }

    try {
        //Start Year
        let dropdownStopYear = $('#StopYear');
        dropdownStopYear.empty();
        dropdownStopYear.append('<option selected="true" value="">Stop Year</option>');
        dropdownStopYear.prop('selectedIndex', 0);
        for (var stopY = 1919; stopY < 2120; stopY++) {
            dropdownStopYear.append($('<option></option>').attr('value', stopY).text(stopY));
        }
    } catch (eStop) {
       
    }
}

