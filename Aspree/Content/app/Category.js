var Category = {};
var guidnew;
var EntityType = {};


Category.addCategory = function () {
    $("#addnewCategory").val("");
    $('#save-category-modal').modal('show');
};

Category.savenewcategory = function () {
    if (!$('#frm-add-category').valid()) return;

    let url = "/VariableCategory/";

    let categoryData = {
        Category: $("#addnewCategory").val()
    };
    App.postData(App.ApiUrl + url, categoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            var c = $(".list-menu").append('<li class="list-dropdown"><button title="Edit" onclick="Category.editCategories(\'' + result.guid + '\',\'' + categoryData.Category + '\')" type="button" class="btn-sm waves-effect waves-light btn-info"><i class="far fa-edit"></i></button><a id=' + result.guid + ' href="javascript:void(0);">' + " " + categoryData.Category + '</a><div class="list-submenu" style="display: none;"><ul></ul></div></li>');
            $("#VariableCategoryId").append('<option id=\"' + result.guid + '\" value=\"' + result.guid + '\">' + categoryData.Category + '</option>');
            $("#VariableCategoryId").val(result.guid);
        }
    }, "POST");
}

Category.editCategories = function (guid, categoryname) {
    $("#editnewCategory").val("");
    categoryname = $(".list-dropdown").find('a[id=' + guid + ']').text();
    $("#editnewCategory").val(categoryname);
    guidnew = guid;
    $('#change-categoryname-modal').modal('show');
}

Category.editCategoryName = function () {
    if (!$('#frm-edit-category').valid()) return;
    let url = "/VariableCategory/" + guidnew;
    let editcategoryData = {
        Category: $("#editnewCategory").val()
    };
    App.postData(App.ApiUrl + url, editcategoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            var name = $("#editnewCategory").val();
            $('#' + guidnew).text(name);
            $('#VariableCategoryId option').filter('[value="' + guidnew + '"]').text(name);
        }
    }, "PUT");
}

$(function () {

    if (window.location.pathname.toLowerCase().indexOf("category/activities") > -1) {
        var columns = [
            { "className": "text-left", "data": "categoryName" },
            { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },   //"width": "60", 
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {
                    return "<button title=\"Edit\" onclick=\"Category.editActivityCategoryModel('" + user.guid + "', '" + user.categoryName + "')\" type=\"button\" class=\"btn-sm waves-effect waves-light btn-info\"><i class=\"far fa-edit\"></i></button>\n                    <button title=\"" + (user.dateDeactivated ? "Inactive" : "Active") + "\" onclick=\"Category.deleteConfirm('" + user.guid + "',this)\" type=\"button\" " + (user.isSystemRole ? "disabled" : "") + " class =\"btn-sm btn-danger waves-effect waves-light\" data-toggle=\"modal\" data-target=\"#manage-confirm\">" + (user.dateDeactivated ? "<i class='fa fa-trash'></i>" : "<i class='fa fa-trash'></i>") + "</button>";
                }
            },
        ];

        Category.dataTable = App.setupSimpleTable('example23', 'ActivityCategory', columns, {})

    }
    else if (window.location.pathname.toLowerCase().indexOf("category/forms") > -1) {
        var columns = [
            { "className": "text-left", "data": "categoryName" },
            { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },   //"width": "60", 
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {
                    if (user.isDefaultFormCategory != 0) {
                        return "<button title=\"Edit\" onclick=\"Category.editFormCategoryModel('" + user.guid + "', '" + user.categoryName + "')\" type=\"button\" class=\"btn-sm waves-effect waves-light btn-info\"><i class=\"far fa-edit\"></i></button>\n                    <button title=\"" + (user.dateDeactivated ? "Inactive" : "Active") + "\" onclick=\"Category.deleteConfirmFormCategory('" + user.guid + "',this)\" type=\"button\" " + (user.isSystemRole ? "disabled" : "") + " class =\"btn-sm btn-danger waves-effect waves-light\" data-toggle=\"modal\" data-target=\"#manage-confirm\">" + (user.dateDeactivated ? "<i class='fa fa-trash'></i>" : "<i class='fa fa-trash'></i>") + "</button>";
                    } else {
                        return '';
                    }
                }
            },
        ];

        Category.dataTable = App.setupSimpleTable('example23', 'FormCategory', columns, {})
    }
    else if (window.location.pathname.toLowerCase().indexOf("category/variables") > -1) {
        var columns = [
            { "className": "text-left", "data": "categoryName" },
            { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },   //"width": "60", 
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {
                    if (user.isDefaultVariableCategory != 0) {
                        return "<button title=\"Edit\" onclick=\"Category.editVariablesCategoryModel('" + user.guid + "', '" + user.categoryName + "')\" type=\"button\" class=\"btn-sm waves-effect waves-light btn-info\"><i class=\"far fa-edit\"></i></button>\n                    <button title=\"" + (user.dateDeactivated ? "Inactive" : "Active") + "\" onclick=\"Category.deleteConfirmVariablesCategory('" + user.guid + "',this)\" type=\"button\" " + (user.isSystemRole ? "disabled" : "") + " class =\"btn-sm btn-danger waves-effect waves-light\" data-toggle=\"modal\" data-target=\"#manage-confirm\">" + (user.dateDeactivated ? "<i class='fa fa-trash'></i>" : "<i class='fa fa-trash'></i>") + "</button>";
                    } else {
                        return '';
                    }
                }
            },
        ];
        Category.dataTable = App.setupSimpleTable('example23', 'VariableCategory', columns, {})
    }
    else if (window.location.pathname.toLowerCase().indexOf("category/entitytype") > -1) {
        var columns = [
            { "className": "text-left", "data": "name" },
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {
                    return "<button title=\"Edit\" onclick=\"Category.editEntityTypeModel('" + user.guid + "', '" + user.name + "')\" type=\"button\" class=\"btn-sm waves-effect waves-light btn-info\"><i class=\"far fa-edit\"></i></button>\n                        <button title=\"Active\" onclick=\"Category.deleteConfirmEntityType('" + user.guid + "',this)\" type=\"button\" class =\"btn-sm btn-danger waves-effect waves-light\" data-toggle=\"modal\" data-target=\"#manage-confirm\">\n                            <i class='fa fa-trash'></i>\n                        </button>";
                }
            },
        ];

        Category.dataTable = App.setupSimpleTable('example23', 'EntityType', columns, {})

    }
    else if (window.location.pathname.toLowerCase().indexOf("category/entitysubtype") > -1) {


        var columns = [
            { "className": "text-left", "data": "name" },
            { "className": "text-left", "data": "entityTypeName" },
            {
                "width": "120",
                "data": "guid", render: function (guid, type, user) {
                    return "<button title=\"Edit\" onclick=\"Category.editEntitySubTypeModel('" + user.guid + "', '" + user.name + "', '" + user.entityTypeId + "')\" type=\"button\" class=\"btn-sm waves-effect waves-light btn-info\"><i class=\"far fa-edit\"></i></button>\n                        <button title=\"Active\" onclick=\"Category.deleteConfirmEntitySubType('" + user.guid + "',this)\" type=\"button\" class =\"btn-sm btn-danger waves-effect waves-light\" data-toggle=\"modal\" data-target=\"#manage-confirm\">\n                            <i class='fa fa-trash'></i>\n                        </button>";

                }
            },
        ];

        Category.dataTable = App.setupSimpleTable('example23', 'EntitySubType', columns, {})

        getEntityTypeList();
    }
    else if (window.location.pathname.toLowerCase().indexOf("systemadmintools/managecategories") > -1) {

        if ($('#routPath').val() == "2") {
            var columns = [
                { "className": "text-left", "data": "categoryName" },
                { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
            ];

            Category.dataTable = App.setupSimpleTable('example23', 'ActivityCategory', columns, {})
        } else if ($('#routPath').val() == "1") {
            var columns = [
                { "className": "text-left", "data": "categoryName" },
                { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
            ];

            Category.dataTable = App.setupSimpleTable('example23', 'FormCategory', columns, {})
        } else {
            var columns = [
                { "className": "text-left", "data": "categoryName" },
                { "data": "modifiedDate", render: function (createdDte, type, user) { return (user.modifiedDate == null ? user.createdDate.split("T")[0] : user.modifiedDate.split("T")[0]); } },
            ];
            Category.dataTable = App.setupSimpleTable('example23', 'VariableCategory', columns, {})
        }
    }
});

//Activity category
Category.editActivityCategoryModel = function (guid, name) {
    if (guid == undefined) {
        $('#add-categoryname-modal').modal('show');
    } else {
        $("#editActivityCategory").val(name);
        guidnew = guid;

        $('#change-categoryname-modal').modal('show');
    }
}

Category.editActivityCategorySubmit = function (isEdit) {
    var formname = "frmSaveData";
    if (isEdit) {
        formname = "frmEditData";
    }

    if ($('#' + formname).valid()) {
        let url = isEdit == false ? "ActivityCategory" : "ActivityCategory/" + guidnew;
        let methodeType = isEdit ? "PUT" : "POST"

        var Cate = $("#editActivityCategory").val()
        if (!isEdit) {
            Cate = $("#addActivityCategory").val()
        }
        let editcategoryData = {
            Category: Cate,//$("#editActivityCategory").val()
        };

        App.postData(App.ApiUrl + url, editcategoryData, function (result) {
            App.showSuccess(isEdit ? "Activity category updated successfully." : "Activity category added successfully.");
            Category.dataTable.refresh();
            if (isEdit)
                $('#change-categoryname-modal').modal('hide');
            else
                $('#add-categoryname-modal').modal('hide');

        }, methodeType);
    }
}

Category.deleteConfirm = function (guid) {
    Category.Current = guid;
    $('.modal-body .sf-submit-info').html('Are you sure want to delete selected activity category?');
}

Category.delete = function (guid) {

    App.postData(App.ApiUrl + "/ActivityCategory/" + Category.Current, {}, function (result) {
        App.showSuccess("Activity category was deleted successfully.");
        Category.dataTable.refresh()
    }, "Delete");
}

//form category
Category.editFormCategoryModel = function (guid, name) {
    if (guid == undefined) {
        $('#add-categoryname-modal').modal('show');
    } else {
        $("#editFormCategory").val(name);
        guidnew = guid;

        $('#change-categoryname-modal').modal('show');
    }
}

Category.editFormCategorySubmit = function (isEdit) {

    var formname = "frmSaveData";
    if (isEdit) {
        formname = "frmEditData";
    }
    if ($('#' + formname).valid()) {

        let url = isEdit == false ? "FormCategory" : "FormCategory/" + guidnew;
        let methodeType = isEdit ? "PUT" : "POST"

        var category = $("#editFormCategory").val()
        if (!isEdit) {
            category = $("#addActivityCategory").val()
        }
        let editcategoryData = {
            Category: category,//$("#editFormCategory").val()
        };

        App.postData(App.ApiUrl + url, editcategoryData, function (result) {
            App.showSuccess(isEdit ? "Form category updated successfully." : "Form category added successfully.");
            Category.dataTable.refresh();
            if (isEdit)
                $('#change-categoryname-modal').modal('hide');
            else
                $('#add-categoryname-modal').modal('hide');
        }, methodeType);
    }
}

Category.deleteConfirmFormCategory = function (guid) {
    Category.Current = guid;
    $('.modal-body .sf-submit-info').html('Are you sure want to delete selected form category?');
}

Category.deleteFormCategory = function (guid) {
    App.postData(App.ApiUrl + "/FormCategory/" + Category.Current, {}, function (result) {
        App.showSuccess("Form category was deleted successfully.");
        Category.dataTable.refresh()
    }, "Delete");
}

//variables category
Category.editVariablesCategoryModel = function (guid, name) {
    if (guid == undefined) {
        $('#add-categoryname-modal').modal('show');
    } else {
        $("#editFormCategory").val(name);
        guidnew = guid;
        $('#change-categoryname-modal').modal('show');
    }
}

Category.editVariablesCategorySubmit = function (isEdit) {
    var formname = "frmSaveData";
    if (isEdit) {
        formname = "frmEditData";
    }
    if ($('#' + formname).valid()) {
        let url = isEdit == false ? "VariableCategory" : "VariableCategory/" + guidnew;
        let methodeType = isEdit ? "PUT" : "POST"

        var cat = $("#editFormCategory").val()
        if (!isEdit) {
            cat = $("#addActivityCategory").val()
        }
        let editcategoryData = {
            Category: cat,
        };

        App.postData(App.ApiUrl + url, editcategoryData, function (result) {
            App.showSuccess(isEdit ? "Variable category updated successfully." : "Variable category added successfully.");
            Category.dataTable.refresh();
            if (isEdit)
                $('#change-categoryname-modal').modal('hide');
            else
                $('#add-categoryname-modal').modal('hide');
        }, methodeType);
    }
}

Category.deleteConfirmVariablesCategory = function (guid) {
    Category.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected variable category?");
}

Category.deleteVariablesCategory = function (guid) {
    App.postData(App.ApiUrl + "/VariableCategory/" + Category.Current, {}, function (result) {
        App.showSuccess("Variable category was deleted successfully.");
        Category.dataTable.refresh()
    }, "Delete");
}
//Entity Type
Category.editEntityTypeModel = function (guid, name) {
    if (guid == undefined) {
        $('#add-categoryname-modal').modal('show');
    } else {
        $("#editFormCategory").val(name);
        guidnew = guid;
        $('#change-categoryname-modal').modal('show');
    }
}

Category.editEntityTypeSubmit = function (isEdit) {
    var formname = "frmSaveData";
    if (isEdit) {
        formname = "frmEditData";
    }

    if ($('#' + formname).valid()) {

        let url = isEdit == false ? "EntityType" : "EntityType/" + guidnew;
        let methodeType = isEdit ? "PUT" : "POST"

        var Name = $("#editFormCategory").val()
        if (!isEdit) {
            Name = $("#addActivityCategory").val()
        }
        let editcategoryData = {
            Name: Name,
        };

        App.postData(App.ApiUrl + url, editcategoryData, function (result) {
            App.showSuccess(isEdit ? "Entity type updated successfully." : "Entity type added successfully.");
            Category.dataTable.refresh();
            if (isEdit)
                $('#change-categoryname-modal').modal('hide');
            else
                $('#add-categoryname-modal').modal('hide');
        }, methodeType);
    }
}

Category.deleteConfirmEntityType = function (guid) {
    Category.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected entity type?");
}

Category.deleteEntityType = function (guid) {
    App.postData(App.ApiUrl + "/EntityType/" + Category.Current, {}, function (result) {
        App.showSuccess("entity type was deleted successfully.");
        Category.dataTable.refresh()
    }, "Delete");
}

//Entity Sub Type
Category.editEntitySubTypeModel = function (guid, name, entityTypeGuid) {
    if (guid == undefined) {
        $('#add-categoryname-modal').modal('show');
        let dropdown = $('#addEntityType');
        dropdown.empty();
        dropdown.append('<option selected="true" value="" disabled>Entity Type</option>');
        dropdown.prop('selectedIndex', 0);

        $.each(EntityType, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
        });
    } else {
        $("#editFormCategory").val(name);
        guidnew = guid;
        $('#change-categoryname-modal').modal('show');

        let dropdown = $('#editEntityType');
        dropdown.empty();
        dropdown.append('<option selected="true" disabled>Entity Type</option>');
        dropdown.prop('selectedIndex', 0);

        $.each(EntityType, function (key, entry) {
            if (entry.guid == entityTypeGuid) {
                dropdown.append($('<option selected="true"></option>').attr('value', entry.guid).text(entry.name));
            } else {
                dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
            }

        });
    }
}

Category.editEntitySubTypeSubmit = function (isEdit) {

    var formname = "frmSaveData";
    if (isEdit) {
        formname = "frmEditData";
    }

    if ($('#' + formname).valid()) {
        let url = isEdit == false ? "EntitySubType" : "EntitySubType/" + guidnew;
        let methodeType = isEdit ? "PUT" : "POST"

        var Name = $("#editFormCategory").val();
        var EntityTypeId = $("#editEntityType").val();

        if (!isEdit) {
            Name = $("#addActivityCategory").val(),
                EntityTypeId = $("#addEntityType").val()
        }

        let editcategoryData = {
            Name: Name,
            EntityTypeId: EntityTypeId
        };

        App.postData(App.ApiUrl + url, editcategoryData, function (result) {
            App.showSuccess(isEdit ? "Entity sub type updated successfully." : "Entity sub type added successfully.");
            Category.dataTable.refresh();
            if (isEdit)
                $('#change-categoryname-modal').modal('hide');
            else
                $('#add-categoryname-modal').modal('hide');

        }, methodeType);
    }
}

Category.deleteConfirmEntitySubType = function (guid) {
    Category.Current = guid;
    $('.modal-body .sf-submit-info').html("Are you sure want to delete selected entity sub type?");
}

Category.deleteEntitySubType = function (guid) {
    App.postData(App.ApiUrl + "/EntitySubType/" + Category.Current, {}, function (result) {
        App.showSuccess("entity sub type was deleted successfully.");
        Category.dataTable.refresh()
    }, "Delete");
}

function getEntityTypeList() {
    App.postData(App.ApiUrl + "/EntityType", {}, function (result) {
        EntityType = result;
    }, "Get");
}



//=======================================================================
//  open model for add new form category from form builder
//=======================================================================
Category.addFormBuilderCategory = function () {
    $("#addnewCategory").val("");
    $('#save-category-modal').modal('show');
};

//=======================================================================
//  save category from form builder
//=======================================================================
Category.saveFormBuilderCategory = function () {
    if (!$('#frm-add-category').valid()) return;

    let url = "FormCategory";

    let categoryData = {
        Category: $("#addnewCategory").val()
    };

    App.postData(App.ApiUrl + url, categoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            let c = $(".list-menu.form-categories").append('<li class="list-dropdown"><button title="Edit" onclick="Category.editFormBuilderCategories(\'' + result.guid + '\',\'' + categoryData.Category + '\')" type="button" class="btn-sm waves-effect waves-light btn-info"><i class="far fa-edit"></i></button><a id=' + result.guid + ' href="javascript:void(0);">' + " " + categoryData.Category + '</a><div class="list-submenu" style="display: none;"><ul></ul></div></li>');
            $("#FormCategoryId").append('<option id=\"' + result.guid + '\" value=\"' + result.guid + '\">' + categoryData.Category + '</option>');
            $("#FormCategoryId").val(result.guid);
        }
    }, "POST");
};

//=======================================================================
//  open model for edit form category from form builder
//=======================================================================
Category.editFormBuilderCategories = function (catguid, categoryname) {
    $("#editnewCategory").val("");
    categoryname = $(".list-dropdown").find('a[id=' + catguid + ']').text();
    $("#editnewCategory").val(categoryname);
    guidnew = catguid;
    $('#change-categoryname-modal').modal('show');
}

//=======================================================================
//  open model for edit form category from form builder
//=======================================================================
Category.updateFormBuilderCategoryName = function () {
    if (!$('#frm-edit-category').valid()) return;
    let url = "/FormCategory/" + guidnew;
    let editcategoryData = {
        Category: $("#editnewCategory").val()
    };
    App.postData(App.ApiUrl + url, editcategoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            var name = $("#editnewCategory").val();
            $('#' + guidnew).text(name);
            $('#FormCategoryId option').filter('[value="' + guidnew + '"]').text(name);
        }
    }, "PUT");
}
//=======================================================================
//  open model for add new activity category from form builder
//=======================================================================
Category.addActivityCategoryFormBuilder = function () {
    $("#addnewCategory").val("");
    $('#add-categoryname-modal').modal('show');
};

//=======================================================================
//  save activity category from form builder
//=======================================================================
Category.saveActivityCategoryFormBuilder = function () {
    if (!$('#frm-add-category').valid()) return;

    let url = "ActivityCategory";

    let categoryData = {
        Category: $("#addActivityCategory").val()
    };

    App.postData(App.ApiUrl + url, categoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            let c = $(".list-menu.activity-categories").append('<li class="list-dropdown"><button title="Edit" onclick="Category.editActivityCategoriesFormBuilder(\'' + result.guid + '\',\'' + categoryData.Category + '\')" type="button" class="btn-sm waves-effect waves-light btn-info"><i class="far fa-edit"></i></button><a id=' + result.guid + ' href="javascript:void(0);">' + " " + categoryData.Category + '</a><div class="list-submenu" style="display: none;"><ul></ul></div></li>');
            $("#ActivityCategoryId").append('<option id=\"' + result.guid + '\" value=\"' + result.guid + '\">' + categoryData.Category + '</option>');

            if ($("#ActivityCategoryId").find('option:selected').text() != "Default")
                $("#ActivityCategoryId").val(result.guid);
        }
    }, "POST");
};

//=======================================================================
//  open model for edit activity category from form builder
//=======================================================================
Category.editActivityCategoriesFormBuilder = function (catguid, categoryname) {
    $("#editActivityCategory").val("");
    categoryname = $(".list-dropdown").find('a[id=' + catguid + ']').text();
    $("#editActivityCategory").val(categoryname);
    guidnew = catguid;
    $('#change-categoryname-modal').modal('show');
}

//=======================================================================
//  update activity category from form builder
//=======================================================================
Category.updateActivityCategoryNameFormBuilder = function () {
    if (!$('#frm-edit-category').valid()) return;
    let url = "/ActivityCategory/" + guidnew;
    let editcategoryData = {
        Category: $("#editActivityCategory").val()
    };
    App.postData(App.ApiUrl + url, editcategoryData, function (result) {
        $('.btn-cancel').click();
        if (result) {
            var name = $("#editActivityCategory").val();
            $('#' + guidnew).text(name);
            $('#ActivityCategoryId option').filter('[value="' + guidnew + '"]').text(name);
        }
    }, "PUT");
}



