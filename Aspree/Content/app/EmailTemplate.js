var EmailTemplate = {};

$(function () {
    if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
    var columns = [
        { "data": "eventName", "className": "text-left" },
        { "data": "subject", "className": "text-left" },
        { "data": "createdDate", render: function (CreatedDate, type, role) { return (role.modifiedDate == null ? role.createdDate.split("T")[0] : role.modifiedDate.split("T")[0]); } },
        {
            "data": "guid", render: function (guid, type, role) {
                return `<button title="Edit Email Template" onclick="EmailTemplate.editEmailTemplate('${role.guid}')" type="button" class="btn-sm waves-effect waves-light btn-info"><i class="far fa-edit"></i></button>`
            }
        },
    ];
    EmailTemplate.dataTable = App.setupSimpleTable('example23', 'EmailTemplate', columns, {})
});

EmailTemplate.editEmailTemplate = function (guid) {
    location.href = "EmailTemplate/edit/" + guid;
}

EmailTemplate.saveTemplate = function () {
    let url = "/EmailTemplate/" + $('#Guid').val();

    let roleData = {
        Guid: $('#Guid').val(),
        Subject: $('#Subject').val(),
        MailBody: $('#MailBody').val()
    };

    App.postData(App.ApiUrl + url, roleData, function (result) {
        App.redirectSuccess("Email Template was modified successfully.", "/EmailTemplate");
    }, "PUT");
}

