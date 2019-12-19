var Entity = {};
var EntitySubtype = {};
Entity.DateFormat = "";
$(function () {
    GetEntitySubtypeList();
    if (window.location.pathname.toLowerCase().indexOf("entities/configuration") > -1) {
        if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
        var columns = [            
            { "className": "text-left", "data": "entityTypeName" },
            { "className": "text-left", "data": "entitySubtypeName" },
            { "width": "60", "data": "modifiedDate", render: function (createdDate, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
        ];
        Entity.dataTable = App.setupSimpleTable('example23', 'Entity', columns, {})
        $("#example23").css("width", "");
    }
    else if (window.location.pathname.toLowerCase().indexOf("entities") > -1) {
        if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
        Entity.FillEntityTypeAndSubType();
        Entity.FillDatatable();       
        $("#example23").css("width", "");
    }
});


Entity.FillDatatable = function () {
    entitiesListData = {
        dt: null,
        init: function () {
            dt = $('#example23').DataTable({               
                "ordering": false,             
                "language": { processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> ' },
                "ajax": {
                    "url": App.ApiUrl + "/Entity/GetAllEntitiesCreatedBySearch",
                    "headers": { "Authorization": "Bearer " + App.AccessToken },
                    "dataType": "json",
                    "contentType": "application/json",
                    "crossDomain": true,
                    success: function (data, textStatus, xhr) {

                        dt.clear().draw();
                        for (var i = 0; i < data.length; i++) {
                            dt.row.add(data[i])
                        }
                        dt.draw();
                        App.hideLoader();

                        Entity.AddIndividualColumnSearchField(dt);
                    },
                    error: function (xhr, textStatus, errorThrown) {

                        App.hideLoader();
                    }
                },
                "columns": [
                    { "className": "", "data": "entID", render: function (entId, type, user) { return `<a href="javascript:void(0)" onclick="Entity.redierctEntitySummary('${user.entID}', '${user.formGuid}')">${user.entID}</a>`; } },

                    { "className": "text-left", "data": "name" },
                    { "className": "text-left", "data": "entityTypeName" },
                    { "className": "text-left", "data": "entitySubtypeName" },
                    { "width": "60", "data": "createdDate", render: function (createdDate, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },

                ],
                "lengthMenu": [[15, 25, 50, 100, 150], [15, 25, 50, 100, 150]],

                "initComplete": function () {
                    alert("");
                    Entity.AddIndividualColumnSearchField(this);
                }
            });
        },
        refresh: function () {
            let EntityType = $("#search0").val();
            let EntityNumber = $("#search1").val();
            let Activity = $("#search2").val();
            let Form = $("#search3").val();
            let FormStatus = $("#search4").val();

            EntityType = EntityType.length === 0 ? '' : '&EntityType=' + EntityType;
            EntityNumber = EntityNumber.length === 0 ? '' : '&EntityNumber=' + EntityNumber;
            Activity = Activity.length === 0 ? '' : '&Activity=' + Activity;
            Form = Form.length === 0 ? '' : '&Form=' + Form;
            FormStatus = FormStatus.length === 0 ? '' : '&FormStatus=' + FormStatus;
        }
    }
    entitiesListData.init();

}

Entity.AddIndividualColumnSearchField = function (dt) {    
    let row = $('#example23 thead tr').clone(true);//.appendTo('#example23 thead');
    $('#example23 thead').find('th').each(function (index) {
        var title = $(this).text();
        var column = dt.columns(index);
        if (index === 0) {
            $(this).html('<input id="search' + index + '" type="text" class="form-control"  placeholder="Search ' + title + '" />');
            $('input', this).on('keyup change', function () {
                if (dt.column(index).search() !== this.value) {
                    dt
                        .column(index)
                        .search(this.value)
                        .draw();
                }
            });

        } else if (index > 1 && index < 4) {
            let dd =title.replace(/ /g,'');         
            var select = $('<select id="' + dd + '"   class="form-control"><option value="">' + title + '</option></select>')
                .appendTo($(this).empty())
                .on('change', function () {                    
                    if ($(this).prop("id") == "Type") {                        
                        $("#SubType").prop('selectedIndex', 0);
                        dt.column(3)
                        .search("")
                        .draw();
                        Entity.FilterEntitySubType($(this));
                    }                    
                    dt.column(index)
                        .search($(this).val())
                        .draw();
                });

            if (index === 2) {                
                $.each(Entity.EntityTypeList.sort(), function (d, j) {
                    if (j == "" || j == "null" || j == null) {
                    } else {
                        select.append('<option value="' + j + '">' + j + '</option>')
                    }
                });
            } else if (index === 3) {       
                $.each(Entity.EntitySubTypeList.sort(), function (d, j) {
                    if (j == "" || j == "null" || j == null) {
                    } else {
                        select.append('<option value="' + j + '">' + j + '</option>')
                    }
                });
            } else {
            }
        } else {
            $(this).html('');
        }
    });
    $('#example23 thead tr').before($(row))
}

//===============================================================
//          drag and drop variables
//===============================================================
$(function () {
    $('#variableContainer').on("drop", function (e) {
        e.preventDefault();

        if (!Entity.currentVariable.category)
            Entity.addVariableContainer(Entity.currentVariable.guid);
        else {
            $('a[data-id="' + Entity.currentVariable.guid + '"]').parent().find('ul li a').each(function (i, d) {
                Entity.addVariableContainer($(d).attr('data-id'));
            });
        }
        return false;
    }).on('dragover', function (e) { e.preventDefault(); });

    $('#variableContainer').sortable({
        'handle': 'i.mdi-menu',
        start: function (event, ui) {

            $(ui.item).css("height", "auto");
        },
        stop: function (event, ui) {

            $(ui.item).css("height", "auto");
        }
    });

});


Entity.addVariableContainer = function (guid) {

    var mainContainer = $('#variableContainer');
    if ($('div.row[data-id="' + guid + '"]', mainContainer).length > 0) return; // prevent duplicate variable entry

    App.postData(App.ApiUrl + 'Variable/' + guid, {}, function (element) {

        let varIndex = 0;




        var container = "";//mainContainer.find('.draw-variable');


        var middelcontent = "</br>";
        if (element.variableTypeName == "Text Box") {
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                                            ' name="inputField-' + varIndex + '" ' +
                                            ' placeholder="' + element.question + '" />' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Free Text") {
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +

                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<textarea id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables" rows="2" cols="50"' +
                                            ' name="inputField-' + varIndex + '" ' +
                                            ' placeholder="' + element.question + '" ></textarea>' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Dropdown") {
            var options = "";
            for (var i = 0; i < element.values.length; i++) {
                options = options + "<option value='" + element.values[i] + "'>" + element.variableValueDescription[i] + "</option>";
            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<select class="form-control form-control-line dynamic-variables"' +
                                            'id="inputField-' + varIndex + '"' +
                                            ' name="inputField-' + varIndex + '">' +
                                            '<option value="">Select</option>' +
                                            options +
                                            '</select>' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "LKUP") {
            let options = "";
            let variableTypeClass = "";   
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<select class="form-control form-control-line dynamic-variables ' + variableTypeClass + ' "' +
                                            'id="inputField-' + varIndex + '"' +
                                            ' name="inputField-' + varIndex + '" >' +
                                            '<option value="">Select</option>' +
                                            options +
                                            '</select>' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Checkbox") {
            let options = "";
            for (var i = 0; i < element.values.length; i++) {
                options = options + '<label class="custom-control custom-checkbox"><input type="checkbox" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.values[i] + '"     data-variable-guid="' + element.id + '" data-variable-required="' + element.isRequired + '"  class="custom-control-input dynamic-variables" checked=""/><span class="custom-control-label">' + element.variableValueDescription[i] + '</span></label>';
            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            options +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Radio") {
            let options = "";
            for (var i = 0; i < element.values.length; i++) {
                options = options + '<label class="custom-control custom-radio"><input type="radio" name="inputField-' + varIndex + '" id="inputField-' + varIndex + '" value="' + element.values[i] + '"  data-variable-guid="' + element.id + '" data-variable-required="' + element.isRequired + '" class="custom-control-input dynamic-variables"/><span class="custom-control-label">' + element.variableValueDescription[i] + '</span></label>';
            }
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            options +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Numeric") {
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<input id="inputField-' + varIndex + '" class="form-control form-control-line dynamic-variables"' +
                                            ' name="inputField-' + varIndex + '" ' +
                                            ' placeholder="' + element.question + '" />' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Formula") {
            middelcontent = "<input class='form-control form-control-sm dynamic-variables' name='formulavalue' id='formulavalue-" + varIndex + "' value='' />";
        }

        else if (element.variableTypeName == "Date") {
            let isRequired = "";
            if (element.isRequired) {
                isRequired = "<span class='text-danger'>*</span>";
            }
            let formField = '<div class="col-md-6 col-sm-6">' +
                                        '<div class="form-group mb-2">' +
                                            '<label class="control-label">' + element.question + isRequired + '</label>' +
                                            '<input id="inputField-' + varIndex + '" class="singledate form-control form-control-line dynamic-variables"' +
                                            ' name="inputField-' + varIndex + '" ' +
                                            ' placeholder="' + element.question + '" />' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Heading") {

            let formField = '<div class="col-md-12 col-sm-12">' +
                                        '<div class="form-group mb-3">' +
                                            '<h2 class="control-label">' + element.question + '</h2>' +
                                        '</div>' +
                                    '</div>';
            container = formField;
        }

        else if (element.variableTypeName == "Subheading") {
            let formField = '<div class="col-md-12 col-sm-12">' +
                                        '<div class="form-group mb-3">' +
                                            '<h3 class="control-label">' + element.question + '</h3>' +
                                        '</div>' +
                                    '</div>';
            container = formField;

        }




        var template = '<div class="row" data-id="' + element.guid + '" style="height: auto;"><div class="col-lg-2  p-l-0"><i class="mdi mdi-menu" style="font-size:22px;cursor: ns-resize" title="Move Up/Down"></i></div><div class="col-lg-9  p-l-0 var-label"><div class="col-md-12 col-sm-12"><div class="form-group mb-2 draw-variable">' +

            container +

            '</div></div></div><div class="col-lg-1  p-l-0 p-r-0"> ' +
            '<button type="button" title="Remove" onclick="$(this).parent().parent().remove()" class="btn btn-sm waves-effect waves-light btn-danger pull-right"><i class="far fa-times-circle"></i></button>' +
            '</div></div>';
        $('#drag-message').remove();
        mainContainer.append(template);
        $('.singledate').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            locale: {
                format: Entity.DateFormat,//'@Aspree.Utility.ConfigSettings.DateFormat.ToUpper()', // 'DD-MM-YYYY'         
            }
        });

    }, "GET");



}
Entity.setCurrentVariable = function (guid, category) {
    Entity.currentVariable = {};
    Entity.currentVariable.guid = guid;
    Entity.currentVariable.category = category;
}

Entity.addVariableCategory = function (category) {
    $('a[data-id="' + Entity.currentActivity.guid + '"]').parent().find('ul li a').each(function (i, d) {
        Entity.addVariableContainer($(d).attr('data-id'));
    });
}
Entity.AddEntity = function () {
    location.href = "/entities/create";
}
Entity.ConfigureEntity = function () {
    location.href = "/entities/configuration";
}
Entity.AddEntityConfiguration = function () {
    location.href = "/entities/addentityconfiguration";
}

Entity.EditEntity = function (guid, name) {
    location.href = "/entities/edit/" + guid;
}

function getAllDroppedVariableGuids() {
    var guids = [];
    $("#variableContainer > .row").each(function () {
        guids.push($(this).attr('data-id'));
    });
    return guids;
}
Entity.save = function (isEdit) {

    var dropedDataList = getAllDroppedVariableGuids();

    if (isEdit)
        dropedDataList = getAllValues();

    let url = isEdit == false ? "entity" : "entity/" + $('#Guid').val();
    let methodeType = isEdit ? "PUT" : "POST"
    let roleData = {
        Guid: $('#Guid').val(),        
        Name: $('#EntityName').val(),
        EntityTypeId: $('#EntityType').val(),
        EntitySubTypeId: $('#EntitySubtype').val(),
        TenantId: $('#TenantId').val(),
        Privileges: $('[name=Privileges]:checked').map(function () { return this.value; }).get(),
        DroppedVariablesList: dropedDataList//d//ropedDataList
    };
    App.postData(App.ApiUrl + url, roleData, function (result) {
        App.redirectSuccess(isEdit ? "Entity was modified successfully." : "Entity was added successfully.", "/Entities/Configuration");
    }, methodeType);
}
Entity.deleteConfirm = function (guid, btn) {
    Entity.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected entity?")
}

Entity.delete = function (guid) {

    App.postData(App.ApiUrl + "/EntityFormDataVariable/" + Entity.Current, {}, function (result) {       
        App.showSuccess("Entity was deleted successfully.");      
        Entity.dataTable.refresh()    
    }, "Delete");
}

Entity.deleteEntityConfiguration = function (guid) {
    App.postData(App.ApiUrl + "/Entity/" + Entity.Current, {}, function (result) {
        App.showSuccess("Entity was deleted successfully.");
        Entity.dataTable.refresh()
    }, "Delete");
}

$('#EntityType').change(function () {
    let dropdown = $('#EntitySubtype');
    let entityTypeId = $('#EntityType').val();
    dropdown.empty();
    dropdown.append('<option selected="true" disabled>Entity Subtype</option>');
    dropdown.prop('selectedIndex', 0);
    var isSubtype = false;
    $.each(EntitySubtype, function (key, entry) {
        if (entry.entityTypeId == entityTypeId) {
            isSubtype = true;
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        }
        if (isSubtype) {
            $('#divEntitySubtype').show();
        } else {
            $('#divEntitySubtype').hide();
        }
    })

    if (!isSubtype) {
        $.ajax({
            type: "GET",
            url: "/Entities/VariablesList",
            contentType: "application/json; charset=utf-8",
            data:
                {
                    "typeGuid": $("#EntityType").val(),
                    "subTypeGuid": null
                },
            success: function (result, status, xhr) {

                $("#variableContiner").html(result);
            },
            error: function (xhr, status, error) {                
            }
        });
    }
});

function GetEntitySubtypeList() {
    App.postData(App.ApiUrl + "/EntitySubType", {}, function (result) {
        EntitySubtype = result;
    }, "Get");    
}

function getAllValues() {
    var guids = [];
    $(".variable-guid").each(function () {
        guids.push($(this).val());
    });
    return guids;
}

function getvariablesValue() {
    var question = "";
    var answer = "";

    jsonObj = [];
    for (var x = 1; x <= $('#variableCount').val() ; x++) {

        question = $('#question-' + x).text();
        var ansType = $('#answerType-' + x).val();

        if (ansType == "Free Text") {
            //answer.push($('#answerVal-' + x).val());
            answer = $('#answerVal-' + x).val();

        } else if (ansType == "select") {
            //answer.push($('#answerVal-' + x).val());
            answer = $('#answerVal-' + x).val();
        } else if (ansType == "checkbox") {
            var favorite = [];
            $.each($("input[name='answerVal-" + x + "']:checked"), function () {
                favorite.push($(this).val());
            });

            //answer.push(favorite);
            answer = favorite;

        } else if (ansType == "radio") {
            var radioValue = $("input[name='answerVal-" + x + "']:checked").val();
            //answer.push(radioValue);
            answer = radioValue;

        } else if (ansType == "Numeric") {
            //answer.push($('#answerVal-' + x).val());
            answer = $('#answerVal-' + x).val();

        } else if (ansType == "Formula") {
            //answer.push($('#answerVal-' + x).val());
            answer = $('#answerVal-' + x).val();
        }

        item = {}
        item["question"] = question;
        item["answer"] = answer;
        item["variableid"] = $('#variableid-' + x).val();
        item["variabletype"] = ansType;
        jsonObj.push(item);
    }
    return jsonObj;

}

function convertToList(dropedDataList) {
    var i;
    var fruits = [];
    for (i = 0; i < dropedDataList.length; i++) {
        //         text += cars[i] + "<br>";
        fruits.push(dropedDataList[i]);
    }
    return fruits;
}
Entity.refresh = function () {
    var tempQuestion = $('#Question').val();
    $("#ShowQuestion").text(tempQuestion);
    Entity.onQuestionTypeChange();
};

Entity.onQuestionTypeChange = function () {
    var text = $("#VariableType");
    var selectdropdown = $("#QuestionType");
    selectdropdown.children().remove();
    selectdropdown.append($("<option>").val("0").text("Select"));
    $(".list-group-item").each(function (e) {
        var dataVal = $(this).attr('data-value');
        selectdropdown.append($("<option>").val(dataVal).text(dataVal));
    });

    var selectchck = $("#selectcheckboxvalue");
    selectchck.children().remove();
    $(".list-group-item").each(function (e) {

        var dataVal = $(this).attr('data-value');
        selectchck.append($('<label class="custom-control custom-checkbox"><input type="checkbox" name="CheckboxValue" id="CheckboxValue" class="custom-control-input" checked=""/><span class="custom-control-label">' + dataVal + '</span></label>'));
    });

    var selectradio = $("#selectradiobuttonvalue");
    selectradio.children().remove();
    $(".list-group-item").each(function (e) {

        var dataVal = $(this).attr('data-value');
        selectradio.append($('<label class="custom-control custom-radio"><input type="radio" name="RadioButtonValue" id="RadioButtonValue" value="{0}" class="custom-control-input"/><span class="custom-control-label">' + dataVal + '</span></label>'));
    });
};

function GetVariableDetails(guid) {

}
///save entity form data variables
Entity.EntityFormDataVariable = function (isEdit) {
    var dropedDataList = getvariablesValue();

    var VariableId = "";
    if (dropedDataList.length > 0) {
        VariableId = dropedDataList[0].variableid
    }
    jsonObj = [];
    item = {}
    //item["EntityTypeGuid"] = $('#EntityGuid').val();
    item["Guid"] = $('#Guid').val();
    item["EntityTypeGuid"] = $('#EntityType').val();
    item["EntitySubTypeGuid"] = $('#EntitySubtype').val();
    item["EntityName"] = $('#EntityName').val();
    item["TenantId"] = $('#TenantId').val();
    item["Json"] = dropedDataList;
    jsonObj.push(item);
    var myJSON = JSON.stringify(jsonObj);
    myJSON = myJSON.slice(1, -1);
    let url = isEdit == false ? "EntityFormDataVariable" : "EntityFormDataVariable/" + $('#Guid').val();
    let methodeType = isEdit ? "PUT" : "POST"
    let roleData = {            
        EntityTypeGuid: $('#EntityType').val(),
        EntitySubTypeGuid: $('#EntitySubtype').val(),
        Json: myJSON,
        EntityId: $('#EntityId').val(),
        EntityGuid: $('#EntityGuid').val(),
        EntityName: $('#EntityName').val(),
        VariableGuid: VariableId,     
    };

    App.postData(App.ApiUrl + url, roleData, function (result) {
        App.redirectSuccess(isEdit ? "Entity was modified successfully." : "Entity was added successfully.", "/Entities");
    }, methodeType);
}



/// edit entity configuration
Entity.EditEntityConfiguration = function (guid) {
    location.href = "editentityconfiguration/" + guid;
}


Entity.redierctEntitySummary = function (entityNumber, formId) {
    //Model Form Details
    //summarypage Activityid
    location.href = "/Summary/Index?participant=" + entityNumber + "&formId=" + formId + "&guid=" + $("#ProjectGuid").val();
}


Entity.FillEntityTypeAndSubType = function () {
    Entity.EntitySubTypeList = [];
    Entity.EntityTypeList = [
        App.CommonEntityType.Participant
        , App.CommonEntityType.Person
        , App.CommonEntityType.Place_Group
        , App.CommonEntityType.Project
    ];




    App.postData(App.ApiUrl + "/EntityType", {}, function (result) {

        $.each(result, function (d, j) {
            if (!Entity.EntityTypeList.includes(j.name)) {
                Entity.EntitySubTypeList.push(j.name);
            }
        });
        if (!Entity.EntitySubTypeList.includes("API"))
            Entity.EntitySubTypeList.push("API");

        if (!Entity.EntitySubTypeList.includes("Medical Practitioner/Allied Health"))
            Entity.EntitySubTypeList.push("Medical Practitioner/Allied Health");
        if (!Entity.EntitySubTypeList.includes("Non-Medical Practitioner"))
            Entity.EntitySubTypeList.push("Non-Medical Practitioner");
        if (!Entity.EntitySubTypeList.includes("Registry"))
            Entity.EntitySubTypeList.push("Registry");
        if (!Entity.EntitySubTypeList.includes("Clinical Trial"))
            Entity.EntitySubTypeList.push("Clinical Trial");
        if (!Entity.EntitySubTypeList.includes("Cohort Study"))
            Entity.EntitySubTypeList.push("Cohort Study");
        if (!Entity.EntitySubTypeList.includes("Other"))
            Entity.EntitySubTypeList.push("Other");
    }, "Get");


}
Entity.FilterEntitySubType = function (entityType) {
    var eType = $(entityType).val();

    if (eType == App.CommonEntityType.Person) {
        $("#SubType").children('option').hide();

        $("#SubType option[value='']").show();

        $("#SubType option[value='Medical Practitioner/Allied Health']").show();
        $("#SubType option[value='Non-Medical Practitioner']").show();
    }else if (eType == App.CommonEntityType.Place_Group) {
        $("#SubType").children('option').show();

        $("#SubType option[value='Medical Practitioner/Allied Health']").hide();
        $("#SubType option[value='Non-Medical Practitioner']").hide();

        $("#SubType option[value='Registry']").hide();
        $("#SubType option[value='Clinical Trial']").hide();
        $("#SubType option[value='Cohort Study']").hide();
        $("#SubType option[value='Other']").hide();

    }
    else if (eType == App.CommonEntityType.Project) {
        $("#SubType").children('option').hide();
        $("#SubType option[value='']").show();

        $("#SubType option[value='Registry']").show();
        $("#SubType option[value='Clinical Trial']").show();
        $("#SubType option[value='Cohort Study']").show();
        $("#SubType option[value='Other']").show();

    }
    else if (eType == App.CommonEntityType.Participant) {
        $("#SubType option[value='']").show();
      $("#SubType").children('option').hide();

  } else {
      $("#SubType").children('option').show();
        
  }
}


