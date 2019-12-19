
var User = {};
$(function () {
    if (window.location.pathname.toLowerCase().indexOf("create") > -1 || window.location.pathname.toLowerCase().indexOf("edit") > -1) return;
    User.FillDatatable();   
    $("#example23").css("width", "");
   
});




User.FillDatatable = function () {

    entitiesListData = {
        dt: null,
        init: function () {
            dt = $('#example23').DataTable({
                "ordering": false,               
                "language": { processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> ' },
                "ajax": {
                    "url": App.ApiUrl + "User/GetAllSystemAdminToolsUser/" + $("#ProjectGuid").val(),
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

                        User.AddIndividualColumnSearchField(dt);
                    },
                    error: function (xhr, textStatus, errorThrown) {

                        App.hideLoader();
                    }
                },
                "columns": [

            { "className": "", "data": "entID", render: function (entId, type, user) { return `<a href="javascript:void(0)" onclick="User.redierctEntitySummary('${user.entID}', '${user.formId}')">${user.entID}</a>`; } },
            { "className": "text-left", "data": "firstName", render: function (firstName, type, user) { return user.firstName + ' ' + user.lastName } },
            { "className": "text-left", "data": "roleName" },
            { "className": "text-left", "data": "email" },
            { "width": "10", "data": "createdDate", render: function (createdDate, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
                ],
                "lengthMenu": [[15, 25, 50, 100, 150], [15, 25, 50, 100, 150]],
                "initComplete": function () {
                    alert("");
                    User.AddIndividualColumnSearchField(this);
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

User.AddIndividualColumnSearchField = function (dt) {
    
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
        } else if (index === 2) {
            var select = $('<select class="form-control"><option value="">' + title + '</option></select>')
                .appendTo($(this).empty())
                .on('change', function () {

                    dt.column(index)
                        .search($(this).val())
                        .draw();
                });

            dt.column(index).data().unique().sort().each(function (d, j) {
                if (d == "" || d == "null") {
                } else {
                    select.append('<option value="' + d + '">' + d + '</option>')
                }

            });
        } else {
            $(this).html('');
        }
    });
    $('#example23 thead tr').before($(row))
}



User.addUser = function () {
    window.location.href = '/users/create'
}
User.focusText = function () {
    setTimeout(function () {
        User.FirstName.focus();
        User.FirstName.select();
    }, 500);
}
User.editUser = function (guid, name) {
    window.location.href = '/users/edit/' + guid;
}

User.saveUser = function () {
    let isEdit = $("#Guid").val().length > 0;
    let methodeType = isEdit ? "PUT" : "POST"
    let url = $('#Guid').val() == "" ? "/user" : "/user/" + $('#Guid').val();

    if (!$('#frmUser').valid()) return;

    let userData = {
        FirstName: $('#FirstName').val(),
        LastName: $('#LastName').val(),
        Email: $('#Email').val(),
        Mobile: $('#Mobile').val(),
        Address: $('#Address').val(),
        TenantId: $('#TenantId').val(),
        RoleId: $('#RoleId').val(),
        Profile: $('#Profile').val()
    };

    App.postData(App.ApiUrl + url, userData, function (result) {
       
        if (!result.Content) {
            App.redirectSuccess(isEdit ? "User was modified successfully." : "User was added successfully.", "/users");
        } else {
            App.showError(JSON.parse(result.Content).message);
        }
    }, methodeType);
}

User.deleteConfirm = function (guid, btn) {
    User.Current = guid;
    var action = 'Active';
    if ($(btn).attr('title') == 'Active') action = 'Inactive';
    $('.modal-body .sf-submit-info').html('Are you sure want to ' + action + ' selected user?');
}

User.deleteUser = function (guid) {
    App.postData(App.ApiUrl + "/user/" + User.Current, {}, function (result) {
        if (result.dateDeactivated == null) {
            App.showSuccess("User was activated successfully.");
        }
        else {
            App.showSuccess("User was deactivated successfully.");
        }
        User.dataTable.refresh()      
    }, "DELETE");
}

User.addUserRedirect = function () {
    location.href = "/Users/Create";
}
User.changePassword = function (event) {
    event.preventDefault();
    var cpFrm = $('#frmChangePassword');
    if (cpFrm.valid()) {
        var sendData = {};
        sendData.OldPassword = $('input[name=OldPassword]', cpFrm).val().trim();
        sendData.Password = $('input[name=Password]', cpFrm).val().trim();
        sendData.ConfirmPassword = $('input[name=ConfirmPassword]', cpFrm).val().trim();

        let chkPwurl = "Account/CheckUserpassword/" ;
        App.postData(App.ApiUrl + chkPwurl, sendData.Password, function (result) {
            if (result) {
                App.showError1("This an unsafe password that has been used previously in data breaches. Please choose a different password.");
                return;
            } else {
                var strength = 0;
                var arr = [/.{10,}/, /[A-Z]+/, /[-!$%^&#*()_+|~@@=`{}\[\]:";'<>?,.\/]/];

                jQuery.map(arr, function (regexp) {
                    if (sendData.Password.match(regexp))
                        strength++;
                });
                if (strength >= 3) {
                    App.postData(App.ApiUrl + "/Account/ChangePassword", sendData, function (result) {
                        if (result==true) {
                            App.showSuccess("Password was updated successfully.");

                            $('#OldPassword').val("");
                            $('#Password').val("");
                            $('#ConfirmPassword').val("");

                        } else {
                            try {
                                App.showError(result.message);                                
                            } catch (ed) {
                                App.showError("Password update failed, Please try again.");
                            }
                            
                        }
                    }, "PUT");

                }
                else {
                    App.showError1("Your password does not meet complexity requirements. It must contain at least 10 characters including one upper case and one non-alphanumeric character.");
                    return;
                }
            }
        }, "POST");
    }
}
User.updatePassword = function (event) {
    event.preventDefault();
    var cpFrm = $('#frmUser');
    if (cpFrm.valid()) {
        var sendData = {};
        sendData.FirstName = $('input[name=FirstName]', cpFrm).val();
        sendData.LastName = $('input[name=LastName]', cpFrm).val();
        sendData.Email = $('input[name=Email]', cpFrm).val();
        sendData.Mobile = $('input[name=Mobile]', cpFrm).val();
        sendData.Address = $('input[name=Address]', cpFrm).val();
        sendData.TenantId = $('input[name=TenantId]', cpFrm).val();
        sendData.RoleId = $('input[name=RoleId]', cpFrm).val();
        sendData.Guid = $('input[name=Guid]', cpFrm).val();
        sendData.Profile = $('input[name=Profile]', cpFrm).val();
        sendData.UserName = $('input[name=UserName]', cpFrm).val();

        sendData.IsUserApprovedBySystemAdmin = $('input[name=IsUserApprovedBySystemAdmin]', cpFrm).val();
        sendData.Status = $('input[name=Status]', cpFrm).val();

        let url = "/user/" + sendData.Guid;
        if (App.IsTestSite === true) {
            url = "/Review/UpdateMyProfile/" + sendData.Guid;
        }
      
        App.postData(App.ApiUrl + url, sendData, function (result) {
            App.showSuccess("Profile was updated successfully.");
        }, "PUT");
    }
}
User.changeSecurityQuestion = function (event) {
    event.preventDefault();
    var cpFrm = $('#frmSecuriryQuestion');
    if (cpFrm.valid()) {
        var sendData = {};
        sendData.Answer = $('input[name=Answer]', cpFrm).val();
        sendData.Guid = $('select[name=QuestionGuid]', cpFrm).val();

        App.postData(App.ApiUrl + "account/SecurityQuestion", sendData, function (result) {
            App.showSuccess("Security question and answer was updated successfully.");
        }, "PUT");
    }
}
User.getBase64 = function (file) {

    var reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {
       
    };
    reader.onerror = function (error) {
      
    };
}
User.setImage = function (file, update) {
    App.clearValidationErrors();
    var reader = new FileReader;
    reader.onload = function () {
        var img = new Image;
        img.onload = function () {
            if (file.files[0].size > 1000000) {
                App.showValidationErrors('body', [{ "key": "fileProfile", "message": "Image size can not be more than 1MB." }])
                $(file).val("");
                return;
            }
            if (img.width > 200 || img.height > 200) {
                App.showValidationErrors('body', [{ "key": "fileProfile", "message": "Invalid profile image dimension size." }])
                $(file).val("");
                return;
            }
            $(update).val(reader.result);
        };

        img.src = reader.result;
    };

    reader.readAsDataURL(file.files[0]);
}


User.redierctEntitySummary = function (entityNumber, formId) {   
    location.href = "/Summary/Index?participant=" + entityNumber + "&formId=" + formId + "&guid=" + $("#ProjectGuid").val();
}