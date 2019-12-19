var Dashboard = {};

Dashboard.dateFrom = $("#date-from");
Dashboard.dateTo = $("#date-to");
Dashboard.userCount = $("#user-count");
Dashboard.activeUser = $("#active-user");
Dashboard.roleCount = $("#role-count");
Dashboard.activeRoles = $("#active-roles");
Dashboard.search = $('#filterStatus');
Dashboard.init = function () {
    var sendData = {
        start: Dashboard.dateFrom.val(),
        end: Dashboard.dateTo.val(),
    };

    App.postData(App.ApiUrl + 'Dashboard', sendData, function (result) {
        Dashboard.userCount.text(result.userCount);
        Dashboard.activeUser.text(result.activeUser);
        Dashboard.roleCount.text(result.roleCount);
        Dashboard.activeRoles.text(result.activeRoles);
    });
}

Dashboard.search.off("click").on("click", function () {
    Dashboard.init();
})


Dashboard.RedirectToSearchPage = function (guid) {

    let url = "/Home/ListOfProject?projectGuid=" + guid;
    let methodType = "POST";

    App.showLoader();
    $.ajax({
        url: url,
        headers: { "Authorization": "Bearer " + App.AccessToken },
        contentType: 'application/json',
        data: JSON.stringify({}),
        type: methodType,
        dataType: 'json',
        crossDomain: true,
        success: function (result, textStatus, xhr) {
            window.location.href = '/Search/Index/' + guid;
        },
        error: function (xhr, status, error) {
            App.hideLoader();          
            if (xhr.status == 400) {
                if (xhr.responseText.indexOf('"key"') > -1)
                    App.showValidationErrors('body', xhr.responseJSON);
                else
                    App.showError(xhr.responseJSON.message, selector);
            } else if (xhr.status == 300) {
                var selector = $('.role-model-error').is(":visible") ? '.role-model-error' : '.alerts';
                if (xhr.responseText.indexOf('"key"') > -1)
                    App.showValidationErrors('body', xhr.responseJSON);
                else
                    App.showError(xhr.responseJSON.message, selector);
            }
            else {
                var selector = $('.role-model-error').is(":visible") ? '.role-model-error' : '.alerts';
                App.showError(xhr.responseJSON.message, selector);
            }
        },

    });
}