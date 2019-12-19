var Project = {};

$(function () {
    if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
   
    var columns = [
            { "className": "text-left", "data": "name" },
            { "className": "text-left", "data": "projectSubtype" },
            { "className": "text-left", "data": "confData" },
            { "className": "text-left", "data": "cnstModel" },
            { "className": "text-left", "data": "ethics" },
            { "className": "text-left", "data": "dataStore" },
            { "className": "text-left", "data": "proDt" },         
    ];
    Project.dataTable = App.setupSimpleTable('example23', 'FormDataEntry/GetAllFormDataEntryProjects/' + $("#ProjectId").val() + '/' + $("#FormId").val(), columns, {})
});
Project.add = function () {
    location.href = "/projects/Create";
}

Project.edit = function (guid, name) {
    location.href = "/projects/edit/" + guid;
}

Project.save = function (isEdit) {
    let url = isEdit == false ? "project" : "project/" + $('#Guid').val();
    let methodeType = isEdit ? "PUT" : "POST"
    let roleData = {
        Guid: $('#Guid').val(),
        ProjectName: $('#ProjectName').val(),
        Version: $('#Version').val(),
        ProjectUserId: $('#ProjectUserId').val(),        
    };
    App.postData(App.ApiUrl + url, roleData, function (result) {

        App.redirectSuccess(isEdit ? "Project was modified successfully." : "Project was added successfully.", "/Roles");
    }, methodeType);
}

Project.saveProject = function () {    
    $('button[title="Add New Value"]').removeAttr("style");    
    $('button[title="Add New Value"]').css('margin-bottom', '-60px');
    $('span[data-valmsg-for="Value"]').removeClass("field-validation-error").text('');
    let isEdit = $("#Guid").val().length > 0;
    let url = isEdit == false ? "project" : "project/" + $('#Guid').val();
    let methodeType = isEdit ? "PUT" : "POST"
    if (!$('#frmUser').valid()) return;
    let userData = {
        ProjectName: $('#ProjectName').val(),
        Version: $('#Version').val(),      
        TenantId: $('#TenantId').val(),
        RoleGuid: $('#RoleGuid').val(),
        EndDate: new Date(),
        StartDate: new Date(),
    };    

    userData.ProjectStaffMembersRoles = [];
    $('#Values li').each(function (i, row) {

        let projectUserGuid = $(row).attr('data-project-user');
        let projectUserRoleGuid = $(row).attr('data-project-user-role');
        let userjoindate = $(row).attr('data-project-user-join-date');      
        if (userjoindate == undefined) {
            userjoindate = null;
        }
            userData.ProjectStaffMembersRoles.push({
                UserGuid: projectUserGuid,
                RoleGuid: projectUserRoleGuid,
                StaffCreatedDate: userjoindate,
            });      
        
    });
    if (userData.ProjectStaffMembersRoles.length > 0) {
        App.postData(App.ApiUrl + url, userData, function (result) {
            if (!result.Content) {
                url = ''
                App.postData(App.ApiUrl + url, userData, function (result) {
             
                    if (!result.Content) { }
                }, methodeType);
                App.redirectSuccess(isEdit ? "Project was modified successfully." : "Project was added successfully.", "/projects");
            } else {
                App.showError(JSON.parse(result.Content).message);
            }
        }, methodeType);
    } else {
        $('span[data-valmsg-for="Value"]').addClass("field-validation-error").text('please add project user and role');
        $('button[title="Add New Value"]').css('border', '3px solid rgb(255, 0, 0)');
        $('button[title="Add New Value"]').focus();        
    }
}


Project.deleteConfirm = function (guid, btn) {
    Project.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected project?")
}

Project.delete = function (guid) {
    App.postData(App.ApiUrl + "/project/" + Project.Current, {}, function (result) {
        App.showSuccess("Project was deleted successfully.");
        Project.dataTable.refresh()
    }, "Delete");
}
Project.addValue = function (value, description) {

    $('span[data-valmsg-for="Value"]').removeClass("field-validation-error").text('');
    $('#ProjectUserId').removeClass("input-validation-error")
    $('#ProjectUserRoleId').removeClass("input-validation-error")

    var currentVal = value || $('#ProjectUserId').val().trim();
    var currentValDesc = description || $('#ProjectUserRoleId').val().trim();

    if (currentVal === "0" || currentValDesc === "0") {
        if(currentVal === "0"){
            $('#ProjectUserId').addClass("input-validation-error");
        }
        if (currentValDesc === "0") {
            $('#ProjectUserRoleId').addClass("input-validation-error");
        }        
        $('span[data-valmsg-for="Value"]').addClass("field-validation-error").text('please select project user and role');
        return false;
    }

    try {
        Project.checkDuplicate(currentVal)
    } catch (e) {
        $('span[data-valmsg-for="Value"]').addClass("field-validation-error").text('This user is already added');
        return false;
    }

    var currentValText = value || $("#ProjectUserId option:selected").text().trim();
    var currentValDescText = description || $("#ProjectUserRoleId option:selected").text().trim();

    if (currentVal.length > 0 && currentValDesc.length > 0) {       
        var newValue = $("<li class=\"list-group-item\" data-project-user='" + currentVal + "' data-project-user-role='" + currentValDesc + "'><div class=\"row\"><div class=\"col-sm-3\">" + currentValText + "</div><div class=\"col-sm-6\">" + currentValDescText + "</div>  <div class=\"col-sm-3\"><button type=\"button\" title=\"Remove\" onclick=\"$(this).parent().parent().parent().remove()\" class=\"btn btn-sm waves-effect waves-light btn-danger pull-right\">\n                                    <i class =\"far fa-times-circle\"></i>\n                                </button> </div></div></li>");
        $('#Values').append(newValue);
        if (!value) {
            $('#Value').val('');
            $('#Value').focus();
            $('#Description').val('');
        }
        $('#ProjectUserId').val('0');
        $('#ProjectUserRoleId').val('0');
    }
}

Project.checkDuplicate = function (currentUserGuid) {
    $('#Values li').each(function (i, row) {
        let projectUserGuid = $(row).attr('data-project-user');
        let projectUserRoleGuid = $(row).attr('data-project-user-role');
        if (projectUserGuid == currentUserGuid) {
            throw false;
        }
    });
    return true;
}