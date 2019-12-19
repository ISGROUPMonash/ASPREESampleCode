var ActionList = {};
var Summary = {};
var Deployment = {};
ActionList.ActivityList = [];
ActionList.FormList = [];
ActionList.StatusList = [];
ActionList.EntityTypes = [];

$(function () {
    ActionList.LoadActivityList($("#ProjectId").val());

    if (window.location.pathname.toLowerCase().indexOf("/actionlist") > -1) {
        actionListData = {
            dt: null,
            init: function () {
                dt = $('#actionlistDataTable').DataTable({
                    "serverSide": true,
                    "processing": true,
                    "ordering": false,
                    "searching": false,
                    "bLengthChange": false,
                    "fixedHeader": true,
                    "language": { processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> ' },
                    "ajax": { "url": "/ActionList/GetActionList?projectId=" + $("#ProjectId").val() },
                    "columns": [
                        { "title": "Entity Type", "data": "EntityTypeName", "searchable": true },
                        {
                            "title": "Entity ID", "data": "EntityNumber", render: function (FontColor, type, user) {
                                if (user.ActivityGuid === "00000000-0000-0000-0000-000000000000" && user.ActivityName === "Variable" && user.EntityTypeName === "Project")
                                    return `<a href="javascript:void(0)" onclick="ActionList.redierctVariablePage('${user.EntityNumber}', '${user.FormGuid}')">${user.EntityNumber}</a>`;
                                else
                                    return `<a href="javascript:void(0)" data-current-entity-project-id="${user.EntityProjectGuid}" onclick="ActionList.redierctEntitySummary(this, '${user.EntityNumber}', '${user.FormGuid}')">${user.EntityNumber}</a>`;
                            }
                        },
                        { "title": "Activity", "data": "ActivityName", "searchable": true },
                        { "title": "Form", "data": "FormName", "searchable": true },
                        {
                            "title": "Form Status", "data": "FormStatusName", "searchable": true, render: function (a, b, data) {

                                if (data.ActivityGuid === "00000000-0000-0000-0000-000000000000" && data.ActivityName === "Variable" && data.EntityTypeName === "Project")
                                    return data.FormStatusId === 5
                                        ? `<div><a class="text-submit-for-review" href="javascript:void(0)" onclick="ActionList.redierctVariablePage('${data.EntityNumber}', '${data.FormGuid}')">${data.FormStatusName}</a></div>`
                                        : `<div><a class ="text-warning" href="javascript:void(0)" onclick="ActionList.redierctVariablePage('${data.EntityNumber}', '${data.FormGuid}')">${data.FormStatusName}</a></div>`;
                                else
                                    return data.FormStatusId === 5
                                        ? `<div><a class="text-submit-for-review" href="javascript:void(0)" data-current-entity-project-id="${data.EntityProjectGuid}" onclick="ActionList.redierctEntitySummary(this, '${data.EntityNumber}', '${data.FormGuid}', '${data.ActivityGuid}', '${data.SummaryPageActivityId}', '${data.ProjectVersion}', '${data.FormName}')">${data.FormStatusName}</a></div>`
                                        : `<div><a class ="text-warning" href="javascript:void(0)" data-current-entity-project-id="${data.EntityProjectGuid}" onclick="ActionList.redierctEntitySummary(this, '${data.EntityNumber}', '${data.FormGuid}', '${data.ActivityGuid}', '${data.SummaryPageActivityId}', '${data.ProjectVersion}', '${data.FormName}')">${data.FormStatusName}</a></div>`;

                            }
                        },
                    ],
                    "lengthMenu": [[15, 25, 50, 100, 150], [15, 25, 50, 100, 150]],
                    initComplete: function () {
                        ActionList.AddIndividualColumnSearchField(this);
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

                dt.ajax.url('/ActionList/GetActionList?projectId=' + $("#ProjectId").val() + EntityType + EntityNumber + Activity + Form + FormStatus).load();
            }
        }
        actionListData.init();
    }

});


ActionList.LoadActivityList = function (projectId) {
    let isAsync = false;
    ActionList.LoadAllEntityTypes();
    App.postData(App.ApiUrl + "/ActionList/GetAllActionListActivities/" + projectId, {}, function (result) {
        ActionList.ActivityList = [];

        $.each(result, function (index, value) {
            ActionList.ActivityList.push(value.activityName);
            ActionList.FormList.push(value.formName);
            ActionList.StatusList.push(value.formStatusName);
        });
        if (!ActionList.StatusList.includes("Draft")) {
            ActionList.StatusList.push("Draft");
        }
        if (!ActionList.StatusList.includes("For Review")) {
            ActionList.StatusList.push("For Review");
        }
        if (!ActionList.StatusList.includes("Not Entered")) {
            ActionList.StatusList.push("Not Entered");
        }
    }, "Get", undefined, isAsync);
}

ActionList.AddIndividualColumnSearchField = function (dt) {
    $('#actionlistDataTable thead').find('th').each(function (index) {
        var title = $(this).text();
        var column = dt.api().columns(index);
        if (index > 1) {
            let select = $('<select id="search' + index + '" class="form-control" ><option value=""> Search ' + title + '</option></select>')
                .appendTo($(column.footer()).empty())
                .on('change', actionListData.refresh);
            let arr = column.data()[0];
            if (index === 2) {
                arr = ActionList.GetUniqueElements(ActionList.ActivityList).sort(ActionList.SortThings).reverse();
                let alloweRole = [App.DefaultRoleNameStringEnum.System_Admin, App.DefaultRoleNameStringEnum.Project_Admin];
                if (!alloweRole.includes($("#LoggedInUserRoleName").val())) {
                    let removeActivities = [App.DefaultFormNamesStringEnum.Project_Registration, App.DefaultFormNamesStringEnum.Project_Linkage];
                    arr = arr.filter(function (el) {
                        return !removeActivities.includes(el);
                    });
                }
            }
            if (index === 3) {
                arr = ActionList.GetUniqueElements(ActionList.FormList).sort(ActionList.SortThings).reverse();
                let alloweRole = [App.DefaultRoleNameStringEnum.System_Admin, App.DefaultRoleNameStringEnum.Project_Admin];
                if (!alloweRole.includes($("#LoggedInUserRoleName").val())) {
                    let removeActivities = [App.DefaultFormNamesStringEnum.Project_Registration, App.DefaultFormNamesStringEnum.Project_Linkage];
                    arr = arr.filter(function (el) {
                        return !removeActivities.includes(el);
                    });
                }
            }
            if (index === 4)
                arr = ActionList.GetUniqueElements(ActionList.StatusList).sort(ActionList.SortThings).reverse();
            $.each(arr, function (d, j) {
                if (j !== "Active")
                    select.append('<option value="' + j + '">' + j + '</option>')
            });

            $(this).html('<p class="sorting_disabled">' + title + '</p>').append(select);
        }
        else if (index === 0) {
            let select = $('<select id="search' + index + '" class="form-control" ><option value=""> Search ' + title + '</option></select>')
                .appendTo($(column.footer()).empty())
                .on('change', actionListData.refresh);

            let arr = column.data()[0];
            if (index === 0) {

                arr = ActionList.EntityTypes;
                let alloweRole = [App.DefaultRoleNameStringEnum.System_Admin, App.DefaultRoleNameStringEnum.Project_Admin];
                if (!alloweRole.includes($("#LoggedInUserRoleName").val())) { arr = arr.filter((item) => item.name !== "Project"); }
            }
            $.each(arr, function (d, j) {
                select.append('<option value="' + j.name + '">' + j.name + '</option>')
            });

            $(this).html('<p class="sorting_disabled">' + title + '</p>').append(select);
        }
        else {
            $(this).html('<p class="sorting_disabled">' + title + '</p><input id="search' + index + '" type="text" class="form-control"  placeholder="Search ' + title + '" />');
            $('input', this).on('keyup', actionListData.refresh);
        }
    });
};

ActionList.redierctEntitySummary = function (_this
    , entityNumber
    , entityForm
    , entityActivity
    , entitySummaryPageActivityId
    , entityProjectVersion
    , fname
) {
    let entityModelFormURL =
        "&entityActivity=" + entityActivity
        + "&entitySummaryPageActivityId=" + entitySummaryPageActivityId
        + "&entityProjectVersion=" + entityProjectVersion
        + "&fname=" + fname;
    if (typeof entityActivity === 'undefined' || typeof entitySummaryPageActivityId === 'undefined') {
        entityModelFormURL = "";
    }
    let projectId = $("#ProjectId").val();
    if ($(_this).attr("data-current-entity-project-id") != projectId) {
        projectId = $(_this).attr("data-current-entity-project-id");

        App.postData("/ActionList/UpdateProjectId?newProjectId=" + projectId, {}, function (result) {

            if (result) {
                location.href = "/Summary/Index?participant=" + entityNumber + "&formId=" + entityForm + "&guid=" + projectId + entityModelFormURL;
            }

        }, "Get");
    }

    location.href = "/Summary/Index?participant=" + entityNumber + "&formId=" + entityForm + "&guid=" + projectId + entityModelFormURL;
}

ActionList.redierctVariablePage = function (entityNumber, formid) {
    location.href = "/ProjectBuilder/Variables/" + $("#ProjectId").val();
}

ActionList.GetUniqueElements = function (array) {
    return $.grep(array, function (el, index) {
        return index === $.inArray(el, array);
    });
}

ActionList.formatEntityNumber = function (number, length) {
    var my_string = '' + number;
    while (my_string.length < length) {
        my_string = '0' + my_string;
    }
    return my_string;
}
//=====================================================
//      get all entity types
//=====================================================
ActionList.LoadAllEntityTypes = function () {
    App.postData(App.ApiUrl + 'EntityType/', {}, function (result) {
        ActionList.EntityTypes = result;
    }, 'GET');
}
ActionList.SortThings = function (a, b) {
    a = a.toLowerCase();
    b = b.toLowerCase();
    return a > b ? -1 : b > a ? 1 : 0;
}
