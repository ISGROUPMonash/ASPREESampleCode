
var Form = {};
Form.activityStatusId = 0;

Form.variableLibrary = [];
Form.varIndex = 0;

Form.clickedVariableID;
Form.DatePickerFormat = "";
$(function () {

    try {
        $("#IsBlank").click(function () {
            Form.IsBlankChange();
        });
    } catch (eIsBlank) {
      
    }


    ///hide published button
    $('#btnPublishForm').addClass("hide");


    $('#search-var-text').keypress(function (e) {
        var key = e.which;
        if (key == 13)  // the enter key code
        {
            Form.search(true);
            return false;
        }
    });

    $('#variableContainer').on("drop", function (e) {
        e.preventDefault();
        if (!Form.currentVariable.category)
            Form.addVariable(Form.currentVariable.guid, Form.currentVariable.name, 0, 1, Form.currentVariable.defaultVariableType);
        else {
            $('a[data-id="' + Form.currentVariable.guid + '"]').parent().find('ul li a').each(function (i, d) {
                Form.addVariable($(d).attr('data-id'), $(d).text(), 0, 1, $(d).attr('data-default-variable-type'));
            });
        }
        Form.Multiselect(e);
        return false;
    }).on('dragover', function (e) { e.preventDefault(); });

    $('#variableContainer').sortable({ 'handle': 'i.mdi-menu' });
    $('#EntityTypes').selectpicker();

    $('#ddlSetAllEditPermission').selectpicker();
    $('#ddlSetAllViewPermission').selectpicker();

    App.postData(App.ApiUrl + 'Variable/GetAllVariables/' + $("#ProjectId").val(), {}, function (result) {
        
        Form.variableLibrary = result;
    }, 'GET', true);

    $('#variableEdit input, #variableEdit select').on('blur', function () {
        Form.onBlurParentVariable();       
    });
    $(".option-group-custom").on("click", function (e) {
        e.preventDefault();  
    });
});


Form.onBlurParentVariable = function () {
    let guid = $('#VariableId').val();
    //let row = $('#variableContainer div.row[data-id="' + guid + '"]'); //25012019-change becouse multiple heading required
    let row = $('#variableContainer div.row[data-id-new="' + guid + '"]');
    row.attr('data-help-text', $('#HelpText').val());
    row.attr('data-validation-message', $('#ValidationMessage').val());

    row.attr('data-dependent-variable-id', $('#DependentVariableId').val());
    row.attr('data-dependent-variable-value', $('#Value').val());
    row.attr('data-question-text', $('#QuestionText').val());
    row.attr('data-dependent-variable-value-isblank', $('#IsBlank').prop("checked"));

    try {
        Form.IsBlankChange();
    } catch (eiss) {
    }
    let varName = typeof (Form.variableLibrary.find(function (d) { return d.guid == row.attr('data-id') })) != "undefined" ? Form.variableLibrary.find(function (d) { return d.guid == row.attr('data-id') }).variableName : "";
    varName = varName != null ? varName.toLowerCase() : varName;
    if (varName == "heading") {
        if ($("#HeadingType").val() == null) {
            $("#HeadingType").val("h2");
        }
        row.attr("data-question-heading-tag", $("#HeadingType").val()); 
        row.attr("data-question-text", $("#HeadingText").val());
    }

    if (varName == "othertext") {
        row.attr("data-question-text", $("#OtherText").val());
    }

    let varTypeName = Form.variableLibrary.find(function (d) { return d.guid == $('#DependentVariableId').val() });    
    if (typeof varTypeName !== "undefined") {
        if (varTypeName.variableTypeName == "Date") {
            row.attr('data-dependent-variable-value', $('#ValueDate').val());
        }
        else if (varTypeName.variableTypeName == "Numeric (Integer)") {
            let CutpointDirection = $("#cutpointDirection").val();
            row.attr('data-dependent-variable-value', CutpointDirection + "" + $('#ValueNumberInt').val());

        }
        else if (varTypeName.variableTypeName == "Numeric (Decimal)") {
            let CutpointDirection = $("#cutpointDirection").val();
            row.attr('data-dependent-variable-value', CutpointDirection + "" + $('#ValueNumberDecimal').val());
        }
        else if (varTypeName.variableTypeName == "Text Box") {           
            row.attr('data-dependent-variable-value', $('#ValueTextBox').val());
        }
        else if (varTypeName.variableTypeName == "Free Text") {            
            row.attr('data-dependent-variable-value', $('#ValueTextBox').val());
        }
    }
}


Form.addVariableCategory = function (category) {
    $('a[data-id="' + Form.currentVariable.guid + '"]').parent().find('ul li a').each(function (i, d) {        
        Form.addVariable($(d).attr('data-id'), $(d).text(), 0, 1, $(d).attr('data-default-variable-type'));
    });
}
Form.editVariable = function (div) {    
    let guid = $(div).parents('div.row:first').attr('data-id');
    let formVariablePlaceId = $(div).parents('div.row:first').attr('data-id-new');
    var variable = Form.variableLibrary.find(function (d) { return d.guid == guid });
    Form.clickedVariableID = guid;
    if (variable) {
        $('#HeadingText').val("");
        var row = $(div).parents('div.row:first');
        if (variable.variableName.toLowerCase() != "heading" && variable.variableName.toLowerCase() != "othertext") {
            row.attr('data-question-text', variable.question);
        }
        
        $('#VariableId').val(formVariablePlaceId);
        $('#VariableCategoryId').val(variable.variableCategoryId);
        $('#VariableName').val(variable.variableName);
        $('#VariableLabel').val(variable.variableLabel);
        //ASPMONASH-507-(Project builder) Add "is blank" tick box to the "select variable" box on the forms page of the project builder to enable dependencies based upon whether a variable is blank
        try {
            let isBlank = row.attr('data-dependent-variable-value-isblank');
            if (isBlank == "true") {
                $('#IsBlank').prop("checked", true);
            } else {
                $('#IsBlank').prop("checked", false);
            }
            Form.IsBlankChange();

        } catch (eIsBlank) {
            
        }
        $('#HelpText').val(row.attr('data-help-text'));
        $('#ValidationMessage').val(row.attr('data-validation-message'));      
        $('#QuestionText').val(variable.question);
        $('#QuestionText').attr("title", variable.question);
        $("#DependentVariableId").val($("#DependentVariableId option:first").val());
        $("#Value").val($("#Value option:first").val());
        $("#ValueDate").val("");
        $("#ValueNumberInt").val("");
        $("#ValueNumberDecimal").val("");
        $("#DependentVariableId").children('option').show();
        $("#DependentVariableId option[value=" + variable.guid + "]").hide();
        let currentDepandent = row.attr('data-dependent-variable-id');
        let isVariableDroped = false;
        $('#variableContainer div.row').each(function (i, row) {
            let guid = $(row).attr('data-id');
            if (guid == currentDepandent) {
                isVariableDroped = true;
            }
        });
        if (isVariableDroped) {
            $("#DependentVariableId").val(row.attr('data-dependent-variable-id'));
            Form.onParentVariableChange($("#DependentVariableId")[0]);
            $("#Value").val(row.attr('data-dependent-variable-value'));
        } else {
            Form.onParentVariableChange($("#DependentVariableId")[0]);
        }
        //09092019
        if (variable.variableName != null && variable.variableName.toLowerCase() == "othertext") {
            $('#OtherText').val(row.attr('data-question-text'));
        }
        if (variable.variableName != null && variable.variableName.toLowerCase() == "heading") {
              if (typeof row.attr('data-question-heading-tag') !== 'undefined') {
                $('#HeadingType').val(row.attr('data-question-heading-tag'));
            } else {
                $("#HeadingType").val("h2");
            }
            if (row.attr('data-question-text') != "Insert heading text")
                $('#HeadingText').val(row.attr('data-question-text'));

        }
        var variableTypeName = Form.variableLibrary.find(function (d) { return d.guid == row.attr('data-dependent-variable-id') });
        if (typeof variableTypeName !== "undefined") {

            if (variableTypeName.variableTypeName == "Date") {

                $("#ValueDate").val(row.attr('data-dependent-variable-value'));
                Form.setDatepickerAfterEdit(Form.DatePickerFormat);
            }
            else if (variableTypeName.variableTypeName == "Numeric (Integer)") {              
                try {
                    let currentValue = row.attr('data-dependent-variable-value');
                    $("#cutpointDirection").val(currentValue.charAt(0));
                    $("#ValueNumberInt").val(currentValue.substring(1, currentValue.length));
                } catch (e) {
                    
                }


            }
            else if (variableTypeName.variableTypeName == "Numeric (Decimal)") {            
                try {
                    let currentValue = row.attr('data-dependent-variable-value');
                    $("#cutpointDirection").val(currentValue.charAt(0));
                    $("#ValueNumberDecimal").val(currentValue.substring(1, currentValue.length));
                } catch (e) {
                  
                }
            }
            else if (variableTypeName.variableTypeName == "Text Box") {
                try {
                    let currentValue = row.attr('data-dependent-variable-value');
                    if (currentValue == 'undefined' || currentValue == null)
                        currentValue = "";
                    $("#ValueTextBox").val(currentValue);
                } catch (e) {
                  
                }
            }
            else if (variableTypeName.variableTypeName == "Free Text") {
                try {
                    let currentValue = row.attr('data-dependent-variable-value');
                    if (currentValue == 'undefined' || currentValue == null)
                        currentValue = "";
                    $("#ValueTextBox").val(currentValue);
                } catch (e) {
                   
                }
            }
            else {          
            }
        }
        //Only display variables on form page i.e. variables droped on forms
        $("#DependentVariableId").children('option').hide();
        $('#variableContainer div.row').each(function (i, row) {
            let guid = $(row).attr('data-id');
            $("#DependentVariableId option[value='']").show();
            $("#DependentVariableId option[value=" + guid + "]").show();
        });

        $("#DependentVariableId option[value=" + variable.guid + "]").hide();
        let varName = variable.variableName;
        if (varName != null)
            varName = varName.toLowerCase();

        if (varName == "heading") {
            $("#HTMLheadingType").removeClass("hide");
            $(".headingTextType").removeClass("hide");
            if (typeof row.attr('data-question-heading-tag') !== 'undefined') {
                $('#HeadingType').val(row.attr('data-question-heading-tag'));
            } else {
                $("#HeadingType").val("h1");
            }
            if (row.attr('data-question-text') != "Insert heading text")
                $('#HeadingText').val(row.attr('data-question-text'));
        } else {
            $("#HTMLheadingType").addClass("hide");
            $(".headingTextType").addClass("hide");
        }


        if (varName == "othertext") {
            $(".otherTextType").removeClass("hide");
            if (row.attr('data-question-text') != "Insert other text")
                $('#OtherText').val(row.attr('data-question-text'));
        } else {
            $(".otherTextType").addClass("hide");
        }
    }
}
Form.onParentVariableChange = function (ddl) {

    let guid = $(ddl).val();
    if (guid == "") {
        $('#Value').val("");
        $("#ValueDate").val("");
        $("#ValueNumberInt").val("");
        $("#ValueNumberDecimal").val("");

        $("#HeadingText").val("");
        $("#OtherText").val("");


        $("#cutpointDirection_div").addClass("hide");

        $("#CutpointLabel").text("Value");

        $("#Value").removeClass("hide");
        $("#ValueNumberDecimal").addClass("hide");
        $("#ValueNumberInt").addClass("hide");
        $("#ValueDate").addClass("hide");
        $("#ValueTextBox").addClass("hide");


    }
    var variable = Form.variableLibrary.find(function (d) { return d.guid == guid });
    $('#Value').empty();
    if (variable != null) {
        $("#CutpointLabel").text("Value");
        if (variable.variableTypeName == "Dropdown") {
            $('#Value').removeClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");
            $("#cutpointDirection_div").addClass("hide");
            $("#ValueTextBox").addClass("hide");

            $('#Value').empty();
            $('#Value').append('<option></option>')
            for (var i = 0; i < variable.values.length; i++) {
                let value = variable.values[i].trim();
                let valueDescription = variable.variableValueDescription[i].trim();
                if (value.length > 0) {
                    $('#Value').append('<option value="' + value + '">' + valueDescription + '</option>');
                }
            }
        }
        else if (variable.variableTypeName == "Checkbox") {
            $('#Value').removeClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");
            $("#cutpointDirection_div").addClass("hide");
            $("#ValueTextBox").addClass("hide");

            $('#Value').empty();
            $('#Value').append('<option></option>');
            $('#Value').append('<option value="1">Checked</option>');
            $('#Value').append('<option value="0">Not-checked</option>');
        }
        else if (variable.variableTypeName == "Date") {
            $('#Value').addClass("hide");
            $("#ValueDate").removeClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");
            $("#cutpointDirection_div").addClass("hide");

            $("#ValueTextBox").addClass("hide");


            let row = $("#variableContainer").find("[data-id='" + Form.clickedVariableID + "']");
            $("#ValueDate").val(row.attr('data-dependent-variable-value'));
            Form.setDatepicker(variable, $("#ValueDate").val());           
        }
        else if (variable.variableTypeName == "Numeric (Integer)") {
            $('#Value').addClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").removeClass("hide");
            $("#ValueNumberDecimal").addClass("hide");
            $("#ValueTextBox").addClass("hide");

            $("#cutpointDirection_div").removeClass("hide");
            $("#CutpointLabel").text("Cutpoint");            
        }
        else if (variable.variableTypeName == "Numeric (Decimal)") {
            $('#Value').addClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").removeClass("hide");
            $("#ValueTextBox").addClass("hide");

            $("#cutpointDirection_div").removeClass("hide");
            $("#CutpointLabel").text("Cutpoint");     
        }

        else if (variable.variableTypeName == "LKUP") {
            $('#Value').removeClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");
            $("#ValueTextBox").addClass("hide");
            $("#cutpointDirection_div").addClass("hide");

            $('#Value').empty();
            $('#Value').append('<option></option>')
            for (var i = 0; i < variable.values.length; i++) {
                let value = variable.values[i].trim();
                let valueDescription = variable.variableValueDescription[i].trim();
                if (value.length > 0) {
                    $('#Value').append('<option value="' + value + '">' + valueDescription + '</option>');
                }
            }
        }
        else if (variable.variableTypeName == "Text Box") {
            $('#Value').addClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");

            $("#cutpointDirection_div").addClass("hide");
            $("#ValueTextBox").removeClass("hide");
        }
        else if (variable.variableTypeName == "Free Text") {
            $('#Value').addClass("hide");
            $("#ValueDate").addClass("hide");
            $("#ValueNumberInt").addClass("hide");
            $("#ValueNumberDecimal").addClass("hide");

            $("#cutpointDirection_div").addClass("hide");
            $("#ValueTextBox").removeClass("hide");
        }

    }
}
Form.edit = function (guid) {
    Form.activityStatusId = 0;
    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('ul.variable-categories > li[data-variable = ' + guid + '] > a').addClass('library-li-active');
    $("#DependentVariableId").children('option').hide();
    $("#DependentVariableId option[value='']").show();  
    var projectid = $("#ProjectId").val();
    App.postData(App.ApiUrl + 'form/GetformbyGuid/' + guid + "/" + projectid, {}, function (result) {
        if (result.formIsInDeployeActiviti === true)
            Form.activityStatusId = App.ActivityStatusEnum.Active;

        $("#DeleteForms").attr("onclick", "Form.deleteConfirm('" + guid + "')");


        $('#Guid').val(result.guid);
        $('#FormTitle').val(result.formTitle);
        $('#FormCategoryId').val(result.formCategoryId);
        $('#EntityTypes').selectpicker('val', result.entityTypes);

        if (result.isDefaultForm == 0) {

            $('#FormTitle').attr("disabled", "disabled");
            $('#FormCategoryId').attr("disabled", "disabled");
            $('#EntityTypes').attr("disabled", "disabled");
            $('#SaveForms').attr("disabled", "disabled");
            $('#SaveForms').addClass("disabled-button-cursor");
            $("#DeleteForms").attr("disabled", "disabled");
            $('#variableContainer').sortable("disable");        

        } else {
            $('#FormTitle').removeAttr("disabled");
            $('#FormCategoryId').removeAttr("disabled");
            $('#EntityTypes').removeAttr("disabled");
            $('#SaveForms').removeAttr("disabled");
            $('#SaveForms').removeClass("disabled-button-cursor");
            $('#DeleteForms').removeClass("hide");
            $('#variableContainer').sortable("enable");
        }
        if (result.formIsInDeployeActiviti == true && result.userTypeRole == "Project Admin") {
            $('#DeleteForms').addClass("hide");
        }
        if (result.isFormContaindData) {
            $('#DeleteForms').addClass("hide");
        }
        if (Form.activityStatusId === App.ActivityStatusEnum.Active) {
            $('#DeleteForms').addClass("hide");
        }
        $('#variableContainer').empty();
        for (var i = 0; i < result.variables.length; i++) {
            var variable = result.variables[i];
            Form.addVariable(variable.variableId, variable.variableName, variable, result.isDefaultForm, variable.isDefaultVariableType);
        }
        Form.Multiselect(this);
        var arrSelectedItem = $("#variableContainerHeader .field-permision-view:visible select option:disabled").map(function (i, item) { return item.value });
        $("#variableContainerHeader .field-permision-view:visible select option:not(:disabled)").each(function (idx, item) {
            if ($("#variableContainer .field-permision-view:visible select option[value='" + this.value + "']:selected").length == $("#variableContainer .field-permision-view:visible select").length) {
                arrSelectedItem.push(this.value)
            }
        });
        $("#variableContainerHeader .field-permision-view:visible select").selectpicker('val', arrSelectedItem);

        var arrSelectenterem1 = $("#variableContainerHeader .field-permision-enter:visible select option:disabled").map(function (i, item) { return item.value });
        $("#variableContainerHeader .field-permision-enter:visible select option:not(:disabled)").each(function (idx, item) {
            if ($("#variableContainer .field-permision-enter:visible select option[value='" + this.value + "']:selected").length == $("#variableContainer .field-permision-enter:visible select").length) {
                arrSelectenterem1.push(this.value)
            }
        });
        $("#variableContainerHeader .field-permision-enter:visible select").selectpicker('val', arrSelectenterem1);

        $('#isPublishedForm').val(result.isPublished);
        if (result.isPublished) {
            $('#btnPublishForm').attr("disabled", "disabled");
            $('#btnPublishForm').addClass("disabled-button-cursor");            
        }
        else {
            $('#btnPublishForm').removeAttr("disabled");
            $('#btnPublishForm').removeClass("disabled-button-cursor");
        }
    }, 'GET');
    Form.resetSelect_A_Variable_Section();
}

Form.save = function () {
    let url = $('#Guid').val() == "" ? "form" : "form/" + $('#Guid').val();
    let methodeType = $('#Guid').val() == "" ? "POST" : "PUT";
    if (methodeType == "PUT") {
        if (Form.activityStatusId === App.ActivityStatusEnum.Active) {
            App.showError("This form is used in an activity that is currently active (on the Summary page). To edit this form, you must first inactivate all the activities that are linked to this form on the Testing/Deployment page!");
            return false;
        }
    }
    if ($("#FormTitle").val().length > 50) {
        App.showError("The form name must be less than 50 characters.");
        return false;
    }
    if ($("#FormTitle").val() == "" && $("#FormCategoryId").val() == "") {
    }

    if ($('#isPublishedForm').val() === 'true') {
        url = "form";
        methodeType = "POST";
    }
    var sendData = {};
    sendData.FormTitle = $('#FormTitle').val();
    sendData.FormCategoryId = $('#FormCategoryId').val();
    sendData.EntityTypes = $('#EntityTypes').val();
    if (sendData.EntityTypes.length == 0) {
        sendData.EntityTypes = null;
    }
    sendData.Variables = [];
    $('#variableContainer div.row').each(function (i, row) {
        let guid = $(row).attr('data-id');
        let roles = $(row).find('select.field-permision').val();
        let formVariableRoleViewModel = [];
        let roles1 = [];
        let formVarAccessRole = [];
        let crt = false;
        let slt = false;
        let edt = false;
        let dlt = false;
        roles = [];       
        let rolesView = $(row).find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
        let rolesEnter = $(row).find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();

        var opt = $('#drpRoleSelectView option:selected').map(function (i, v) {
            return this.value;
        }).get(); 
        $.each(rolesView, function (index, value) {
            roles.push(value);
            formVariableRoleViewModel.push({
                "RoleGuidId": value,
                "CanCreate": false,
                "CanView": true,
                "CanEdit": false,
                "CanDelete": false,
            });
        });

        $.each(rolesEnter, function (index, value) {
            roles.push(value);           
            var objIndex = formVariableRoleViewModel.findIndex(function (obj) {
                return obj.RoleGuidId === value;
            });

            if (objIndex > -1) {
                formVariableRoleViewModel[objIndex].CanCreate = true;
                formVariableRoleViewModel[objIndex].CanEdit = true;
            } else {
                formVariableRoleViewModel.push({
                    "RoleGuidId": value,
                    "CanCreate": true,
                    "CanView": false,
                    "CanEdit": true,
                    "CanDelete": false,
                });
            }
        });
        let required = $(row).find('select.is-required').val();
        let helpText = $(row).attr('data-help-text');
        let validationMessage = $(row).attr('data-validation-message');
        let isBlank = $(row).attr('data-dependent-variable-value-isblank');
        let dependentVariableId = $(row).attr('data-dependent-variable-id');
        let dependentVariableValue = $(row).attr('data-dependent-variable-value');
        let QuestionText = $(row).attr('data-question-text');//Extra field
        if (QuestionText == "" || QuestionText === undefined) { QuestionText = null; }
        let varName = Form.variableLibrary.find(function (d) { return d.guid == guid }).variableName;
        varName = varName != null ? varName.toLowerCase() : varName;
        if (varName == "heading") {
            let headType = $(row).attr('data-question-heading-tag')
            QuestionText = Form.designHeading(headType, QuestionText);
        }
        if (varName == "othertext") {
            QuestionText = $(row).attr('data-question-text');
        }
        let isDefaultVariableType = $(row).attr('data-default-variable-type');
        sendData.Variables.push({
            "VariableId": guid,
            "IsRequired": required,
            "ValidationMessage": validationMessage,
            "HelpText": helpText,
            "FormVariableRoles": roles,
            "formVariableRoleViewModel": formVariableRoleViewModel,
            "DependentVariableId": dependentVariableId,
            "ResponseOption": dependentVariableValue,
            "IsDefaultVariableType": isDefaultVariableType,
            "QuestionText": QuestionText,
            "IsBlank": isBlank
        });
    });
    sendData.ProjectId = $('#ProjectId').val();
    App.postData(App.ApiUrl + url, sendData, function (result) {
    
        App.redirectSuccess($('#Guid').val() != "" ? "Form was modified successfully." : "Form was added successfully.", '/ProjectBuilder/Forms/' + $('#ProjectId').val());
    }, methodeType);
}

Form.setCurrentVariable = function (guid, name, category, defaultVariableType) {
    Form.currentVariable = {};
    Form.currentVariable.guid = guid;
    Form.currentVariable.name = name;
    Form.currentVariable.category = category;
    Form.currentVariable.defaultVariableType = defaultVariableType;
}
Form.removeVariable = function (event, div) {
    event.stopPropagation();
    $(div).parent().parent().remove();
    $('#variableEdit')[0].reset();
    //Only display variables on form page i.e. variables droped on forms
    $("#DependentVariableId").children('option').hide();
    $('#variableContainer div.row').each(function (i, row) {
        let guid = $(row).attr('data-id');
        $("#DependentVariableId option[value='']").show();
        $("#DependentVariableId option[value=" + guid + "]").show();
    });
    $('#Value').empty();}
Form.addVariable = function (guid, name, variable, isDefaultForm, defaultVariableType) {
    $('#variableEdit')[0].reset();
    var container = $('#variableContainer');
    let isCollectMultipleTime = Form.variableLibrary.find(function (d) { return d.guid == guid }).canCollectMultiple;
    let variableUsedInFormsList = Form.variableLibrary.find(function (d) { return d.guid == guid }).variableUsedInFormsList;
    variableUsedInFormsList = jQuery.grep(variableUsedInFormsList, function (value) {
        return value != $('#Guid').val();
    });  
    if (defaultVariableType != 2 && isCollectMultipleTime == false && variableUsedInFormsList.length > 0 && isDefaultForm != 0) {
        App.showError("variable is already used by another form.");
        return; // prevent can collect multiple time variable entry
    }
    if (defaultVariableType != 2)
        if ($('div.row[data-id="' + guid + '"]', container).length > 0) return; // prevent duplicate variable entry
    var template = $('#variableTemplate').children().clone();
    template.find('.var-label').text(name);   
    template.find('.field-permision-view').selectpicker();
    template.find('.field-permision-enter').selectpicker();
    if (isDefaultForm == 0) {
        template.find('.btn-formedit').attr("disabled", "disabled");
        template.find('.btn-formedit').addClass("disabled-button-cursor");
        template.find('.mdi-menu').css("cursor", "not-allowed");
    } else {
        template.find('.btn-formedit').removeAttr("disabled");
        template.find('.btn-formedit').removeClass("disabled-button-cursor");
        template.find('.mdi-menu').css("cursor", "ns-resize");
    }
    if (variable) {
        let selectedOptnView = [];
        let selectedOptnEnter = [];
        $.each(variable.formVariableRoleViewModel, function (index, value) {
            if (value.canCreate) {
                selectedOptnEnter.push(value.roleGuidId);
            }
            if (value.canView) {
                selectedOptnView.push(value.roleGuidId);
            }           
        });

        template.find('.field-permision-view').selectpicker('val', selectedOptnView);
        template.find('.field-permision-enter').selectpicker('val', selectedOptnEnter);
        template.find('.is-required').val(variable.isRequired.toString());
        template.attr('data-help-text', variable.helpText);
        template.attr('data-validation-message', variable.validationMessage);
        template.attr('data-dependent-variable-id', variable.dependentVariableId);
        template.attr('data-dependent-variable-value', variable.responseOption);
        template.attr('data-question-text', variable.questionText);
        let varName = variable.variableName != null ? variable.variableName.toLowerCase() : variable.variableName;
        if (variable.isDefaultVariableType == 2 && varName == "heading") {
            if (variable.questionText != null) {
                let teg = $.parseHTML(variable.questionText);
                let div = document.createElement("div");
                div.innerHTML = variable.questionText;
                let text = div.textContent || div.innerText || "";
                let teg1 = teg.length > 0 ? teg[0].localName : "h1";
                template.attr('data-question-heading-tag', teg1);
                template.attr('data-question-text', text);
            }
        }
    } else {
        var tempVar = Form.variableLibrary.find(function (d) { return d.guid == guid });
        template.attr('data-help-text', tempVar.helpText);
        template.attr('data-validation-message', tempVar.validationMessage);
    }
    template.attr('data-default-variable-type', defaultVariableType);
    if (defaultVariableType == 2) {
        //[ASPMONASH-431] add only can view permission should be visible
        $(template).find(".access-roles").find(".field-permision-enter").addClass('hide');
    }
    template.attr('data-id', guid);
    Form.varIndex++;
    template.attr('data-id-new', guid + "-" + Form.varIndex);
    try {
        template.attr('data-dependent-variable-value-isblank', variable.isBlank);
        try {
            Form.IsBlankChange();
        } catch (eiss) {

        }
    } catch (eIsBlank) {      
    }
    $('#drag-message').remove();
    container.append(template);
    container.sortable('refresh');
    Form.autoApplyViewPermissionsBasedOnEditPermissions();
}

Form.publish = function () {
    let url = $('#Guid').val() == "" ? "form" : "form/" + $('#Guid').val();
    let methodeType = $('#Guid').val() == "" ? "POST" : "PUT";
    var sendData = {};
    sendData.FormTitle = $('#FormTitle').val();
    sendData.FormCategoryId = $('#FormCategoryId').val();
    sendData.EntityTypes = $('#EntityTypes').val();
    if (sendData.EntityTypes.length == 0) {
        sendData.EntityTypes = null;
    }
    sendData.Variables = [];
    $('#variableContainer div.row').each(function (i, row) {
        let guid = $(row).attr('data-id');
        let roles = $(row).find('select.field-permision').val();
        let formVariableRoleViewModel = [];
        let roles1 = [];
        let formVarAccessRole = [];
        let crt = false;
        let slt = false;
        let edt = false;
        let dlt = false;
        roles = [];
        let rolesView = $(row).find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
        let rolesEnter = $(row).find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();

        $.each(rolesView, function (index, value) {
            roles.push(value);
            formVariableRoleViewModel.push({
                "RoleGuidId": value,
                "CanCreate": false,
                "CanView": true,
                "CanEdit": false,
                "CanDelete": false,
            });
        });
        $.each(rolesEnter, function (index, value) {
            roles.push(value);
            var objIndex = formVariableRoleViewModel.findIndex(function (obj) {
                return obj.RoleGuidId === value;
            });
            if (objIndex > 0) {
                formVariableRoleViewModel[objIndex].CanCreate = true;
                formVariableRoleViewModel[objIndex].CanEdit = true;
            } else {
                formVariableRoleViewModel.push({
                    "RoleGuidId": value,
                    "CanCreate": true,
                    "CanView": false,
                    "CanEdit": true,
                    "CanDelete": false,
                });
            }
        });
        let required = $(row).find('select.is-required').val();
        let helpText = $(row).attr('data-help-text');
        let validationMessage = $(row).attr('data-validation-message');
        let dependentVariableId = $(row).attr('data-dependent-variable-id');
        let dependentVariableValue = $(row).attr('data-dependent-variable-value');
        let QuestionText = $(row).attr('data-question-text');//Extra field
        if (QuestionText == "" || QuestionText === undefined) { QuestionText = null; }
        let varName = Form.variableLibrary.find(function (d) { return d.guid == guid }).variableName;
        varName = varName != null ? varName.toLowerCase() : varName;
        if (varName == "heading") {
            let headType = $(row).attr('data-question-heading-tag')
            QuestionText = Form.designHeading(headType, QuestionText);
        }
        let isDefaultVariableType = $(row).attr('data-default-variable-type');
        sendData.Variables.push({
            "VariableId": guid,
            "IsRequired": required,
            "ValidationMessage": validationMessage,
            "HelpText": helpText,
            "FormVariableRoles": roles,
            "formVariableRoleViewModel": formVariableRoleViewModel,
            "DependentVariableId": dependentVariableId,
            "ResponseOption": dependentVariableValue,
            "IsDefaultVariableType": isDefaultVariableType,
            "QuestionText": QuestionText,
        });
    });
    sendData.ProjectId = $('#ProjectId').val();
    sendData.IsPublished = true;
    App.postData(App.ApiUrl + url, sendData, function (result) {        
        App.redirectSuccess($('#Guid').val() != "" ? "Form was modified successfully." : "Form was added successfully.", '/ProjectBuilder/Forms/' + $('#ProjectId').val());
    }, methodeType);
}

Form.Multiselect = function (e) {
    $('.dropdown-header.option-group-custom').on("click", function (e) {
        e.preventDefault();        
        let sd = $(e.currentTarget).attr('data-optgroup');
        $(this).parents(".dropdown-menu.inner").find('li[data-optgroup = ' + sd + '] a').click();
    });
}

//======================================================
//  Reset form (clear form)
//======================================================
Form.resetForm = function () {
    $('#Guid').val('');
    $('#FormTitle').val('');
    $('#variableContainer').empty();
    $('#variableContainer').append('<h2 id="drag-message" class="text-center">Drag and Drop your variables here</h2>');
    $('#FormCategoryId').prop('selectedIndex', 0);
    $('#EntityTypes').selectpicker('deselectAll');
    $('#FormTitle').removeAttr("disabled");
    $('#FormCategoryId').removeAttr("disabled");
    $('#EntityTypes').removeAttr("disabled");
    $('#SaveForms').removeAttr("disabled");
    $('#SaveForms').removeClass("disabled-button-cursor");
    $('#variableContainer').sortable("enable");
    $('ul.variable-categories > li > a').removeClass('library-li-active');
    $('#FormTitle').removeClass("input-validation-error");
    $('span[data-valmsg-for="FormTitle"]').addClass("field-validation-error").text("");
    $('span[data-valmsg-for="EntityTypes"]').addClass("field-validation-error").text("");
    $('#DeleteForms').addClass("hide");//PC
    //clear "Select a Variable" section
    $('#QuestionText').attr("title", '');
    $('#QuestionText').val("");
    $('#VariableName').val("");
    $('#DependentVariableId').val("");
    $("#DependentVariableId").children('option').hide();
    $('#Value').val("");
    $('#ValueNumberInt').val("");
    $('#cutpointDirection').val("");
    $("#IsBlank").prop("checked", false);
    Form.activityStatusId = 0;
}

Form.designHeading = function (headType, QuestionText) {
    if (QuestionText == null || QuestionText =="undefined")
        QuestionText = "";    
    if (headType == "h1") {
        return "<h1>" + QuestionText + "</h1>";
    } else if (headType == "h2") {
        return "<h2>" + QuestionText + "</h2>";
    } else if (headType == "h3") {
        return "<h3>" + QuestionText + "</h3>";
    } else if (headType == "h4") {
        return "<h4>" + QuestionText + "</h4>";
    } else if (headType == "h5") {
        return "<h5>" + QuestionText + "</h5>";
    } else if (headType == "h6") {
        return "<h6>" + QuestionText + "</h6>";
    }
}

//======================================================
//  search exesting form from form library
//======================================================
Form.search = function (type) {
    var text = $('#search-var-text').val().trim().toLowerCase();
    if (type) {
        if (text.length > 0) {
            $(".sub-category-txt").each(function () {
                var subName = $(this).data('text');
                try {
                    subName = subName.toString();
                } catch (err) {
                   
                }
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
Form.setViewPermissionToAll = function () {
    $('#variableContainer div.row').each(function (i, row) {
        $(row).find('select.field-permision-view').selectpicker('val', $("#ddlSetAllViewPermission option:selected").map(function (i, v) { return this.value; }).get());
    });
}
Form.setEditPermissionToAll = function () {

    $('#variableContainer div.row').each(function (i, row) {
        $(row).find('select.field-permision-enter').selectpicker('val', $("#ddlSetAllEditPermission option:selected").map(function (i, v) { return this.value; }).get());
    });
}
Form.resetSelect_A_Variable_Section = function () {
    $("#cutpointDirection_div").addClass("hide");
    $("#HTMLheadingType").addClass("hide");
    $(".headingTextType").addClass("hide");
}
Form.setDatepicker = function (_variable, selectedDate) {
    var dateformate = "DDMMYYYY";
    $.each(_variable.variableValidationRuleViewModel, function (key, value) {
        if (value.validationName == "Date") {
            dateformate = "DDMMYYYY";
        } else {
            dateformate = value.validationName.replace("Date_", "");
        }
    });

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
    container.append(template);
    container.find('.singledate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,    
        locale: {
            format: format//'YYYY-MM-DD'
        },
        autoApply: false,     
    });
    $("#ValueDate").val("");
    Form.DatePickerFormat = format  
    container.find('.singledate').attr('onchange', 'Form.onBlurParentVariable()');
}

// This code to show popup delete confirmation for the Form elements
Form.deleteConfirm = function (guid) {
    if (Form.activityStatusId === App.ActivityStatusEnum.Active) {
        App.showError("This form is used in an activity that is currently active (on the Summary page). To edit this form, you must first inactivate all the activities that are linked to this form on the Testing/Deployment page!");
        return false;
    }

    Form.Current = guid;
    Form.Current = $('#Guid').val();
    $('.modal-body .sf-submit-info').html("Are you sure you want to delete the selected Form?");
}
// This code to show popup delete confirmation for the Form elements
Form.delete = function (guid) {
    App.postData(App.ApiUrl + "/Form/" + Form.Current, {}, function (result) {
        
        App.redirectSuccess($('#Guid').val() != "" ? "Form was deleted successfully." : "Form was added successfully.", '/ProjectBuilder/forms/' + $('#ProjectId').val());
    }, "Delete");
}


Form.setDatepickerAfterEdit = function (format) {
    $('#ValueDate').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        locale: {
            format: format//'YYYY-MM-DD'
        },
        autoApply: false,        
    });
}

Form.autoApplyViewPermissionsBasedOnEditPermissions = function () {
    //for edit permision drop-down
    $(".field-permision-enter").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
        if (newValue === undefined && e.target.selectedOptions.length == e.target.options.length) {
            var dataId = $(this).parents(".row:first").attr("data-id");
            if (dataId === undefined) {
                $('#variableContainer div.row').each(function (i, row) {
                    dataId = $(row).attr("data-id");
                    var isEnterDropDown = $(row).find(".field-permision-enter.multiple-select-custom").length > 0;
                    if (isEnterDropDown) {
                        var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();                      
                        enterDRPSelectedVal.push($("div[data-id='" + dataId + "'] select.field-permision-view option:selected'").map(function (i, v) { return this.value; }).get())
                        $("div[data-id='" + dataId + "'] select.field-permision-view").selectpicker('val', enterDRPSelectedVal);
                    }
                })
            } else {
                var isEnterDropDown = $(this).parents(".access-roles:first .field-permision-enter.multiple-select-custom").length > 0;
                if (isEnterDropDown) {
                    var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();
                    $("div[data-id='" + dataId + "'] select.field-permision-view").selectpicker('val', enterDRPSelectedVal);
                }
            }

        } else {
            var dataId = $(this).parents(".row:first").attr("data-id");

            if (dataId === undefined) {
                $('#variableContainer div.row').each(function (i, row) {
                    dataId = $(row).attr("data-id");
                    var isEnterDropDown = $(row).find(".field-permision-enter.multiple-select-custom").length > 0;
                    if (isEnterDropDown && newValue) {
                        var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();
                        var tempSelected = $("div[data-id='" + dataId + "'] select.field-permision-view option:selected").map(function (i, v) { return this.value; }).get();
                        $.each(tempSelected, function (idx, itemVal) { enterDRPSelectedVal.push(itemVal) });
                        $("div[data-id='" + dataId + "'] select.field-permision-view").selectpicker('val', enterDRPSelectedVal);
                    }
                })
            } else {
                var isEnterDropDown = $(this).parents(".access-roles:first .field-permision-enter.multiple-select-custom").length > 0;
                if (isEnterDropDown && newValue) {
                    var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();
                    var tempSelected = $("div[data-id='" + dataId + "'] select.field-permision-view option:selected").map(function (i, v) { return this.value; }).get();
                    $.each(tempSelected, function (idx, itemVal) { enterDRPSelectedVal.push(itemVal) });
                    $("div[data-id='" + dataId + "'] select.field-permision-view").selectpicker('val', enterDRPSelectedVal);
                }
            }
        }       
    });

    //for view permision drop-down 
    $(".field-permision-view").on("changed.bs.select", function (e, clickedIndex, newValue, oldValue) {
        if (newValue === undefined && e.target.selectedOptions.length != e.target.options.length) {
            var dataId = $(this).parents(".row:first").attr("data-id");
            if (dataId === undefined) {
                $('#variableContainer div.row').each(function (i, row) {
                    dataId = $(row).attr("data-id");
                    var isEnterDropDown = $(row).find(".field-permision-view.multiple-select-custom").length > 0;

                    if (isEnterDropDown) {
                        var viewDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
                        var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();

                        let enterDRPSelectedValNew = enterDRPSelectedVal;
                        enterDRPSelectedVal.forEach(function (s) {
                            if (!viewDRPSelectedVal.includes(s)) {
                                var index = enterDRPSelectedValNew.indexOf(s);
                                if (index > -1) {
                                    enterDRPSelectedValNew.splice(index, 1);
                                }
                            }
                        });
                        $("div[data-id='" + dataId + "'] select.field-permision-enter").selectpicker('val', enterDRPSelectedValNew);

                    }
                })
            } else {
                var isEnterDropDown = $(this).parents(".access-roles:first .field-permision-view.multiple-select-custom").length > 0;
                if (isEnterDropDown) {
                    var viewDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
                    var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();

                    let enterDRPSelectedValNew = enterDRPSelectedVal;
                    enterDRPSelectedVal.forEach(function (s) {
                        if (!viewDRPSelectedVal.includes(s)) {
                            var index = enterDRPSelectedValNew.indexOf(s);
                            if (index > -1) {
                                enterDRPSelectedValNew.splice(index, 1);
                            }
                        }
                    });
                    $("div[data-id='" + dataId + "'] select.field-permision-enter").selectpicker('val', enterDRPSelectedValNew);
                }
            }
        }
        else {
            var dataId = $(this).parents(".row:first").attr("data-id");

            if (dataId === undefined) {
                $('#variableContainer div.row').each(function (i, row) {
                    dataId = $(row).attr("data-id");
                    var isEnterDropDown = $(row).find(".field-permision-view.multiple-select-custom").length > 0;
                    if (isEnterDropDown && !newValue) {
                        var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();
                        var viewDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
                        let enterDRPSelectedValNew = enterDRPSelectedVal;
                        enterDRPSelectedVal.forEach(function (s) {
                            if (!viewDRPSelectedVal.includes(s)) {
                                var index = enterDRPSelectedValNew.indexOf(s);
                                if (index > -1) {
                                    enterDRPSelectedValNew.splice(index, 1);
                                }
                            }
                        });
                        $("div[data-id='" + dataId + "'] select.field-permision-enter").selectpicker('val', enterDRPSelectedValNew);
                    }
                })
            } else {
                var isEnterDropDown = $(this).parents(".access-roles:first .field-permision-view.multiple-select-custom").length > 0;
                if (isEnterDropDown && !newValue) {
                    var enterDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-enter option:selected').map(function (i, v) { return this.value; }).get();
                    var viewDRPSelectedVal = $("div[data-id='" + dataId + "']").find('select.field-permision-view option:selected').map(function (i, v) { return this.value; }).get();
                    let enterDRPSelectedValNew = enterDRPSelectedVal;
                    enterDRPSelectedVal.forEach(function (s) {
                        if (!viewDRPSelectedVal.includes(s)) {
                            var index = enterDRPSelectedValNew.indexOf(s);
                            if (index > -1) {
                                enterDRPSelectedValNew.splice(index, 1);
                            }
                        }
                    });
                    $("div[data-id='" + dataId + "'] select.field-permision-enter").selectpicker('val', enterDRPSelectedValNew);                    
                }
            }
        }     
    });
}

Form.IsBlankChange = function () {
    if ($("#IsBlank").is(":checked")) {
        $("#ValueDate").val("").attr("disabled", "disabled");
        $("#Value").val("").attr("disabled", "disabled");
        $("#ValueNumberDecimal").val("").attr("disabled", "disabled");
        $("#ValueNumberInt").val("").attr("disabled", "disabled");
        $("#cutpointDirection").attr("disabled", "disabled");
        $("#ValueTextBox").val("").attr("disabled", "disabled"); 
    } else {
        $("#ValueDate").removeAttr("disabled");
        $("#Value").removeAttr("disabled");
        $("#ValueNumberDecimal").removeAttr("disabled");
        $("#ValueNumberInt").removeAttr("disabled");
        $("#cutpointDirection").removeAttr("disabled");
        $("#ValueTextBox").removeAttr("disabled");
    }
}

