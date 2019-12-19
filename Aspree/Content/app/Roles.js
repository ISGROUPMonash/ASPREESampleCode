var Role = {};
$(function () {
    if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
    var columns = [
        { "className": "text-left", "width": "400", "data": "name" },
        { "data": "isSystemRole", render: function (isSystemRole) { return isSystemRole ? "System" : "Project"; } },
        { "data": "dateDeactivated", render: function (dateDeactivated) { return dateDeactivated ? "<span class='badge badge-danger'>Inactive</span>" : "<span class='badge badge-success'>Active</span>"; } },
        { "data": "createdDate", render: function (createdDate, type, role) { return (role.modifiedDate == null ? role.createdDate.split("T")[0] : role.modifiedDate.split("T")[0]); } },
        {
            "data": "guid", render: function (guid, type, role) {
                return "<button title=\"" + (role.dateDeactivated ? "Activate" : "Inactivate") + "\" onclick=\"Role.deleteConfirm('" + role.guid + "',this)\" type=\"button\" " + (role.isSystemRole && role.name == "System Admin" ? "disabled" : "") + " class =\"btn-sm btn-danger waves-effect waves-light\" style=\"" +
                    (role.isSystemRole && role.name == "System Admin" ? 'visibility:hidden' : '') +
                    "\" data-toggle=\"modal\" data-target=\"#manage-confirm\">" + (role.dateDeactivated ? "<i class='mdi mdi-account-off'></i>" : "<i class='mdi mdi-account'></i>") + "</button>";
            }
        },
    ];
    Role.dataTable = App.setupSimpleTable('example23', 'Role', columns, {})
    $("#example23").css("width", "");
});

Role.addRole = function () {

}
Role.focusText = function () {

}
Role.editRole = function (guid, name) {
    location.href = "/roles/edit/" + guid;
}
Role.submitForm = function () {
    $('#Privileges').parents(".form-group:first").removeClass('has-danger');
    $('#Privileges').next('.form-control-feedback').html("");
    var error = false;
    var valid = $('#roleForm').valid();
    var p = $('[name=Privileges]:checked').map(function () { return this.value; }).get();
    if (p.length == 0) {
        $('#Privileges').parents(".form-group:first").addClass('has-danger');
        $('#Privileges').next('.form-control-feedback').html("The Privileges field is required.");
    }
    else {
        $('#roleForm').submit();
        App.showLoader();
    }

}
Role.saveRole = function (isEdit) {
    var error = false;
    var valid = $('#roleForm').valid();

    let url = isEdit == false ? "role" : "role/" + $('#Guid').val();
    let methodeType = isEdit ? "PUT" : "POST"
    let roleData = {
        Guid: $('#Guid').val(),
        Name: $('#Name').val(),
        TenantId: $('#TenantId').val()
    };

    App.postData(App.ApiUrl + url, roleData, function (result) {
        App.redirectSuccess(isEdit ? "Role was modified successfully." : "Role was added successfully.", "/Roles");
    }, methodeType);
}
Role.deleteConfirm = function (guid, btn) {
    Role.Current = guid;
    var action = 'Activate';
    if ($(btn).attr('title') == 'Activate') action = 'Inactivate';
    action = action == 'Activate' ? 'inactivate' : 'activate';
    $('.modal-body .sf-submit-info').html('Are you sure you want to ' + action + ' selected role?');
}

Role.deleteRole = function (guid) {
    App.postData("/roles/delete/" + Role.Current, {}, function (result) {
        if (result.MessageType == "Success") {
            var role = JSON.parse(result.Content);
            if (role.dateDeactivated == null) {
                App.showSuccess("Role was activated successfully.");
            }
            else {
                App.showSuccess("Role was inactivated successfully.");
            }
            Role.dataTable.refresh()
        }
    });
}

Role.addRoleRedict = function () {
    location.href = "/Roles/Create";
}
Role.selectAll = function (chk, column) {
    $('.chkPermission' + column).prop('checked', chk.checked);
}
Role.onSelected = function (column) {
    $('#chkPermission' + column).prop('checked', $('.chkPermission' + column).length == $('.chkPermission' + column + ':checked').length);
}
Role.selectAllOnLoad = function (count) {
    for (var i = 1; i <= count; i++) {
        Role.onSelected(i);
    }
}