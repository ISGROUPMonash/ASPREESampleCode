var Authentication = {};

$(function () {

    if (window.location.pathname.toLowerCase().indexOf("manageauthentication") > -1) {
        var columns = [
            { "className": "text-left", "data": "authTypeName" },
            {
                "className": "text-left", "data": "authType", render: function (user) { return $("#AuthType option[value='" + user + "']").text(); }
            },
            { "width": "80", "data": "dateDeactivated", render: function (guid, type, user) { return user.status == 1 ? "<span class='badge badge-success'>Active</span>" : "<span class='badge badge-danger'>Inactive</span>"; } },
            { "width": "80", "data": "createdDate", render: function (createdDate, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {                    
                    if (user.clientId != null)
                        return `<button title="Edit" onclick="Authentication.editAuthenticationType('${user.guid}',this)"
                                data-auth-username="${user.userName}"
                                data-auth-name="${user.authTypeName}"
                            data-auth-type="${user.authType}"
                            data-auth-domain="${user.domain}"
                            data-auth-client-id="${user.clientId}"
                            data-auth-client-secret="${user.clientSecret}"
                            data-auth-scope="${user.scope}"
                            data-auth-state="${user.state}"

                            data-auth-provider-claim="${user.authenticationProviderClaim}"
                            data-auth-authorize-endpoint="${user.authorizeEndpoint}"
                            data-auth-token-endpoint="${user.tokenEndpoint}"
                            data-auth-introspect-endpoint="${user.introspectEndpoint}"
                            data-auth-revoke-endpoint="${user.revokeEndpoint}"
                            data-auth-logout-endpoint="${user.logoutEndpoint}"
                            data-auth-keys-endpoint="${user.keysEndpoint}"
                            data-auth-userinfo-endpoint="${user.userinfoEndpoint}"
                            type="button" class ="btn-sm waves-effect waves-light btn-info"><i class ="far fa-edit"></i></button>
                            <button title="${user.status == 1 ? 'Inactivate' : 'Activate'}"  onclick="Authentication.activeInactiveConfirm('${user.guid}',this)" type="button" class ="btn-sm btn-danger waves-effect waves-light mr-1" data-toggle="modal" data-target="#active-inactive-confirm">
                            ${(user.status == 1 ? "<i class='mdi mdi-account'></i>" : "<i class='mdi mdi-account-off'></i>")}
                            <button title="Delete" onclick="Authentication.deleteConfirm('${user.guid}',this)" type="button" class ="btn-sm btn-danger waves-effect waves-light" data-toggle="modal" data-target="#delete-confirm-modal"><i class ='fa fa-trash'></i></button>`
                    else
                        return '';
                }
            },
        ];

        Authentication.dataTable = App.setupSimpleTable('example23', 'AuthenticationType', columns, {});
        $("#example23").css("width", "");
    }
});

//=======================================================================
//  open model for add new authentication-type
//=======================================================================
Authentication.addAuthenticationType = function () {
    $('#model-popup-title').text("Add Authentication Type");
    $('#add-login-auth-type-master-modal').modal('show');
    Authentication.resetAuthenticationTypeModel();
}

//=======================================================================
//  open model for edit authentication-type by guid
//=======================================================================
Authentication.editAuthenticationType = function (guid, _this) {
    Authentication.resetAuthenticationTypeModel();
    $("#Guid").val(guid);
    $('#model-popup-title').text("Edit Authentication Type");
    $("#UserName").val($(_this).attr("data-auth-username"));
    $("#AuthTypeName").val($(_this).attr("data-auth-name"));
    $("#AuthType").val($(_this).attr("data-auth-type"));
    $("#Domain").val($(_this).attr("data-auth-domain"));
    $("#ClientId").val($(_this).attr("data-auth-client-id"));
    $("#ClientSecret").val($(_this).attr("data-auth-client-secret"));
    $("#Scope").val($(_this).attr("data-auth-scope"));
    $("#State").val($(_this).attr("data-auth-state"));

    $("#AuthenticationProviderClaim").val($(_this).attr("data-auth-provider-claim"));
    $("#AuthorizeEndpoint").val($(_this).attr("data-auth-authorize-endpoint"));
    $("#TokenEndpoint").val($(_this).attr("data-auth-token-endpoint"));
    $("#IntrospectEndpoint").val($(_this).attr("data-auth-introspect-endpoint"));
    $("#RevokeEndpoint").val($(_this).attr("data-auth-revoke-endpoint"));
    $("#LogoutEndpoint").val($(_this).attr("data-auth-logout-endpoint"));
    $("#KeysEndpoint").val($(_this).attr("data-auth-keys-endpoint"));
    $("#UserinfoEndpoint").val($(_this).attr("data-auth-userinfo-endpoint"));

    $('#add-login-auth-type-master-modal').modal('show');

}

//=======================================================================
//  save/update authentication-type in database
//=======================================================================
Authentication.saveAuthenticationType = function () {
    let isEdit = $("#Guid").val().length > 0;
    let methodeType = isEdit ? "PUT" : "POST"
    let url = $('#Guid').val() == "" ? "/AuthenticationType" : "/AuthenticationType/" + $('#Guid').val();

    if (!$('#frmAuthenticationType').valid()) return;

    let sendData = {
        UserName: $("#UserName").val(),
        AuthTypeName: $("#AuthTypeName").val(),
        AuthType: $("#AuthType").val(),
        Domain: $("#Domain").val(),
        ClientId: $("#ClientId").val(),
        ClientSecret: $("#ClientSecret").val(),
        Scope: $("#Scope").val(),
        State: $("#State").val(),

        AuthenticationProviderClaim: $("#AuthenticationProviderClaim").val(),
        AuthorizeEndpoint: $("#AuthorizeEndpoint").val(),
        TokenEndpoint: $("#TokenEndpoint").val(),
        IntrospectEndpoint: $("#IntrospectEndpoint").val(),
        RevokeEndpoint: $("#RevokeEndpoint").val(),
        LogoutEndpoint: $("#LogoutEndpoint").val(),
        KeysEndpoint: $("#KeysEndpoint").val(),
        UserinfoEndpoint: $("#UserinfoEndpoint").val(),

    };

    App.postData(App.ApiUrl + url, sendData, function (result) {
        if (result != null) {
            Authentication.dataTable.refresh();
            App.showSuccess(isEdit ? "authentication type updated successfully." : "authentication type added successfully.");
            $('#add-login-auth-type-master-modal').modal('hide');
        } else {
            App.showError(JSON.parse(result.Content).message);
        }
    }, methodeType);
}

//=======================================================================
//  delete confirmation model for authentication type
//=======================================================================
Authentication.deleteConfirm = function (guid, btn) {
    Authentication.Current = guid;

    $('.modal-body .sf-submit-info').html('Are you sure want to delete selected authentication type?');
}

//=======================================================================
//  delete authentication type from database
//=======================================================================
Authentication.deleteAuthenticationType = function (guid) {
    App.postData(App.ApiUrl + "/AuthenticationType/" + Authentication.Current, {}, function (result) {

        if (result.dateDeactivated != null) {
            App.showSuccess("Authentication Type deleted successfully.");
        }
        Authentication.dataTable.refresh()
    }, "DELETE");
}


Authentication.activeInactiveConfirm = function (guid, btn) {
    Authentication.Current = guid;
    var action = 'Activate';
    if ($(btn).attr('title') == 'Activate') action = 'Inactivate';
    action = action == 'Activate' ? 'inactivate' : 'activate';
    $('#active-inactive-confirm .sf-submit-info').html('Are you sure you want to ' + action + ' selected authentication type?');
}
Authentication.activeInactivePost = function (guid, btn) {
    App.postData(App.ApiUrl + "/AuthenticationType/InActiveByGuid/" + Authentication.Current, {}, function (result) {
        if (result.status == 1) {
            App.showSuccess("Authentication Type was activated successfully.");
        }
        else {
            App.showSuccess("Authentication Type was inactivated successfully.");
        }
        Authentication.dataTable.refresh()
    }, "Get");
}
//=======================================================================
//  reset authentication type from
//=======================================================================
Authentication.resetAuthenticationTypeModel = function () {
    $("#Guid").val("");

    $("#UserName").removeClass("input-validation-error").val("");
    $("#AuthTypeName").removeClass("input-validation-error").val("");
    $("#AuthType").removeClass("input-validation-error").prop('selectedIndex', 0);
    $("#Domain").removeClass("input-validation-error").val("");
    $("#ClientId").removeClass("input-validation-error").val("");
    $("#ClientSecret").removeClass("input-validation-error").val("");
    $("#Scope").removeClass("input-validation-error").val("");
    $("#State").removeClass("input-validation-error").val("");

    $("#AuthenticationProviderClaim").removeClass("input-validation-error").val("");
    $("#AuthorizeEndpoint").removeClass("input-validation-error").val("");
    $("#TokenEndpoint").removeClass("input-validation-error").val("");
    $("#IntrospectEndpoint").removeClass("input-validation-error").val("");
    $("#RevokeEndpoint").removeClass("input-validation-error").val("");
    $("#LogoutEndpoint").removeClass("input-validation-error").val("");
    $("#KeysEndpoint").removeClass("input-validation-error").val("");
    $("#UserinfoEndpoint").removeClass("input-validation-error").val("");


    $('span[data-valmsg-for="UserName"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="AuthTypeName"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="AuthType"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="Domain"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="ClientId"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="ClientSecret"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="Scope"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="State"]').removeClass("field-validation-error").text("");

    $('span[data-valmsg-for="AuthenticationProviderClaim"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="AuthorizeEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="TokenEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="IntrospectEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="RevokeEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="LogoutEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="KeysEndpoint"]').removeClass("field-validation-error").text("");
    $('span[data-valmsg-for="UserinfoEndpoint"]').removeClass("field-validation-error").text("");
}





