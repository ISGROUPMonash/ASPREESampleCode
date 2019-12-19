
var Search = {};

let gFormGuid = "";
let gFormType = "";
let CountryList = null;
let StateList = null;
let PostCodeList = null;
$(function () {
    $(".list-unstyled .check").on('click', function (event) {
        event.stopPropagation();
        event.stopImmediatePropagation();

        let clickedForm = $(event.currentTarget).attr("data-form-type");
        switch (clickedForm) {
            case "Participant":
                Search.ClearParticipant(1);
                break;
            case "Person":
                Search.ClearParticipant(2);
                break;
            case "Place":
                Search.ClearParticipant(3);
                break;
            case "Project":
                Search.ClearParticipant(4);
                break;
            default:
                break;
        }
    });

});

Search.participantSearch = function (formid, formType) {

    $('#footerAllDetails').removeClass('hide');
    $('#footerEntiId').addClass('hide');


    gFormType = formType;
    gFormGuid = formid;

    let sendData = {};

    let callApi = false;
    if (formType === 1) {       
        if ($("#ProjectEthicsApproval").val() == "true") {
            try {
                let startDateString = $("#RecruitmentStartDate").val();
                let endDateString = $("#RecruitmentEndDate").val();
                var qdd = new Date();
                var m_p = qdd.getMonth() + 1;
                if (m_p < 10) { m_p = '0' + m_p; }
                var d_p = qdd.getDate();
                if (d_p < 10) { d_p = '0' + d_p; }
                var y_p = qdd.getFullYear();       
                let todayDateString = y_p + "-" + m_p + "-" + d_p;
                if (startDateString == "" && endDateString != "" && (new Date(endDateString) < new Date(todayDateString))) {
                    App.showError("Recruitment for the project \"" + $("#displayProjName").html() + "\" has ended.");
                    return false;
                }
                if (startDateString == "") {
                    App.showError("Recruitment for the project \"" + $("#displayProjName").html() + "\" has not yet started.");
                    return false;
                }
                else if (startDateString != "") {
                    startDate = new Date(startDateString);
                    if (startDate > new Date(todayDateString)) {
                        App.showError("Recruitment for the project \"" + $("#displayProjName").html() + "\" has not yet started.");
                        return false;
                    }
                }
                if (endDateString != "") {
                    endDate = new Date(endDateString);
                    if (endDate < new Date(todayDateString)) {
                        App.showError("Recruitment for the project \"" + $("#displayProjName").html() + "\" has ended.");
                        return false;
                    }
                }

            } catch (e) {
               
            }
        } else {
            App.showError("Project does not have ethical approval.");
            return false;
        }
        let checkDOBDefaultVal = $('#Participant-DOB').text();
        if (checkDOBDefaultVal == "Date of Birth") {
            $('#Participant-DOB').text("");
        } else if (checkDOBDefaultVal == "Empty") {
            $('#Participant-DOB').text("");
        } else if (checkDOBDefaultVal == "Invalid date") {
            $('#Participant-DOB').text("");
        }

        if (!Search.validateSearchForm(formType)) {
            if ($('#Participant-EntID').val() == "" && $('#Participant-FirstName').val() == "") {
                $('#search-validation-popup').modal('show');                
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter either an ID or enter details in the name search section and then click search.</p>");
                $('#popup-body').html(popupBody);
                return false;
            }
            if ($('#Participant-FirstName').val() == "" || $('#Participant-Name').val() == "" || $('#Participant-DOB').text() == "" || $('#Participant-Gender').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter first name, surname, DOB and gender and then.</p>");
                $('#popup-body').html(popupBody);
                return false;
            }
        }
        sendData.SearchVariables = [];
        var currentValue = "";
        $(".participant-search-form").each(function () {
            currentValue = $(this).val();

            if ($(this).attr("data-variable-name") == "DOB") {
                currentValue = $(this).text();
            }
            let middleNameId = $(this).attr('data-variable-key');
            if (currentValue.length > 0) {             
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": currentValue,
                });
            }
        });
        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;
        callApi = true;
    }

    else if (formType === 2) {
        if (!Search.validateSearchForm(formType)) {
            if ($('#Person-EntID').val() == "" && $('#Person-FirstName').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter either an ID or enter details in the name search section and then click search.</p>");
                $('#popup-body').html(popupBody);
                return false;
            }

            if ($('#Person-FirstName').val() == "" || $('#Person-Name').val() == "" || $('#Person-PerSType').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter first name, surname and sub-type and then click search.</p>");
                $('#popup-body').html(popupBody);
                return false;
            }
        }

        sendData.SearchVariables = [];

        $(".person-search-form").each(function () {
            if ($(this).val().length > 0) {
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": $(this).val(),
                });
            }
        });

        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;

        callApi = true;
    }

    else if (formType === 3) {
        if (!Search.validateSearchForm(formType)) {
            if ($('#PlaceGroup-EntID').val() == "" && $('#PlaceGroup-Name').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter either an ID or enter details in the name search section and then click search.</p>");
                $('#popup-body').html(popupBody);
                return false;
            }

            if ($('#PlaceGroup-Name').val() == "" || $('#PlaceGroup-EntType').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter atleast name and type and then click search. It is recommended that you search name, type and state. </p>");
                $('#popup-body').html(popupBody);
                return false;
            }
        }
        sendData.SearchVariables = [];
        $(".place-group-search-form").each(function () {
            if ($(this).val().length > 0) {
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": $(this).val(),
                });
            }
        });
        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;
        callApi = true;
    }

    else if (formType === 4) {
        if (!Search.validateSearchForm(formType)) {
            if ($('#ProjectRegestration-EntID').val() == "" && $('#ProjectRegestration-Name').val() == "") {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter atleast name and project sub-type and then click search. It is recommended that you search name, project sub-type etc. </p>");
                $('#popup-body').html(popupBody);
                return false;
            }

            if ($('#ProjectRegestration-name').val() == ""
                || $('#ProjectRegestration-ProSType').val() == ""
                ) {
                $('#search-validation-popup').modal('show');
                var popupBody = $("<p>You have not entered enough details to perform a search." + "\r\n" + "Please enter atleast name and project sub-type and then click search. It is recommended that you search name, project sub-type etc. </p>");
                $('#popup-body').html(popupBody);
                return false;
            }
        }

        sendData.SearchVariables = [];

        let projStartDate = "";
        let projEndDate = "";

        $(".project-search-form").each(function () {

            var currentVal = $(this).val();
            if ($(this).attr('data-variable-name') == "ProDt") {
                currentVal = $(this).text();
                if (currentVal == "Project Activation Date") {
                    currentVal = "";
                } else if (currentVal == "Empty") {
                    currentVal = "";
                } else if (currentVal == "Invalid date") {
                    currentVal = "";
                }
            }
            if ($(this).attr('data-variable-name') == "RecruitStart") {
                currentVal = $(this).text();
                if (currentVal == "Recruitment Start Date") {
                    currentVal = "";
                } else if (currentVal == "Empty") {
                    currentVal = "";
                } else if (currentVal == "Invalid date") {
                    currentVal = "";
                }
                projStartDate = currentVal;
            }
            if ($(this).attr('data-variable-name') == "RecruitEnd") {
                currentVal = $(this).text();

                if (currentVal == "Recruitment End Date") {
                    currentVal = "";
                } else if (currentVal == "Empty") {
                    currentVal = "";
                } else if (currentVal == "Invalid date") {
                    currentVal = "";
                }
                projEndDate = currentVal;

            }

            if (currentVal.length > 0) {               
                sendData.SearchVariables.push({
                    "Key": $(this).attr('data-variable-key'),
                    "Value": currentVal,
                });
            }
        });


        try {
            projStartDate = projStartDate != "" ? App.convertStringToDate(projStartDate) : "";
        } catch (e) {
            
        }
        try {
            projEndDate = projEndDate != "" ? App.convertStringToDate(projEndDate) : "";
        } catch (e) {
     
        }
        if (projStartDate != "" && projEndDate != "") {
            if (projStartDate > projEndDate) {
                App.showError("Recruitment End Date should be greater then Recruitment Start Date.");
                return false;
            }
        }
        sendData.ProjectId = $('#ProjectId').val();
        sendData.FormId = formid;
        callApi = true;
    }

    if (callApi === true) {
        let url = '/FormDataEntry/SearchParticipant';
        if (App.IsTestSite === true) {
            url = 'Review/SearchTestEntities';
        }
        
        App.postData(App.ApiUrl + url, sendData, function (result) {
            $('#search-variable-result-model-table').empty();
            $('#search-variable-result-model-footer').empty();
            if (result != null) {
               
                $('#search-for-tabular-data').removeAttr('style');
                if (formType == 1) {
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');
                    if (result.length > 1) {
                        let isFound = false;
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");
                        jQuery.each(result, function (i, frm) {
                            let c = "";
                            try {
                                c = frm.find(function (x) {                                    
                                    return x.variableName === "MiddleName";
                                }).selectedValues;
                            } catch (e) {
                              
                            }
                            if (c === "") {
                          
                                let fn = "";
                                let sn = "";
                                let dob = "";
                                let gndr = "";
                                let entId = "";

                                jQuery.each(frm, function (i, val) {
                                    if (val.variableName == "EntID") {                                      
                                        searchUserId = val.selectedValues;
                                        entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    
                                    if (val.variableName == "FirstName") {
                                        fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                    }                                    
                                    if (val.variableName == "Name") {
                                        sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                  
                                    if (val.variableName == "DOB") {
                                        dob = "<tr><td class='sf-hd-title'>Date of Birth:</td><td>" + val.selectedValues + "</td></tr>";
                                    }
                                    
                                    if (val.variableName == "Gender") {
                                        if (val.selectedValues == 1) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Male</td></tr>";
                                        } else if (val.selectedValues == 2) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Female</td></tr>";
                                        } else if (val.selectedValues == 3) {
                                            gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Other</td></tr>";
                                        }
                                    }
                                });

                                resultTable.append($(entId));
                                resultTable.append($(fn));
                                resultTable.append($(sn));
                                resultTable.append($(dob));
                                resultTable.append($(gndr));
                                isFound = true;
                                return false;
                            } else {
                                
                            }
                        });
                        if (isFound) {
                            resultFooter.append($('<button type=\'button\' class=\'btn btn waves-effect waves-light btn-info\' onclick="Search.resultRedirect(\'' + searchUserId + '\', \'' + gFormGuid + '\')">Yes</button>'));
                            resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));

                            $('#search-variable-result-model').modal('show');
                        } else {
                            $('#search-validation-popup').modal('show');
                            var popupBody = $("<p>There are multiple records with same details, please enter more details for exact search.</p>");
                            $('#popup-body').html(popupBody);
                        }

                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");

                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let dob = "";
                            let gndr = "";
                            let entId = "";

                            jQuery.each(frm, function (i, val) {                             
                                if (val.variableName == "EntID") {
                                    searchUserId = val.selectedValues;
                                    entId = "<tr><td class='sf-hd-title'>Participant ID:</td><td>" + val.selectedValues + "</td></tr>";
                                }                              
                                if (val.variableName == "FirstName") {
                                    fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }                             
                                if (val.variableName == "Name") {
                                    sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                }                            
                                if (val.variableName == "DOB") {
                                    dob = "<tr><td class='sf-hd-title'>Date of Birth:</td><td>" + val.selectedValues + "</td></tr>";
                                }                                
                                if (val.variableName == "Gender") {
                                    if (val.selectedValues == 1) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Male</td></tr>";
                                    } else if (val.selectedValues == 2) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Female</td></tr>";
                                    } else if (val.selectedValues == 3) {
                                        gndr = "<tr><td class='sf-hd-title'>Gender:</td><td>Other</td></tr>";
                                    }
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(fn));
                            resultTable.append($(sn));
                            resultTable.append($(dob));
                            resultTable.append($(gndr));
                        });
                        resultFooter.append($('<button type=\'button\' class=\'btn btn waves-effect waves-light btn-info\' onclick="Search.resultRedirect(\'' + searchUserId + '\', \'' + gFormGuid + '\')">Yes</button>'));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {
                        if ($('#Participant-EntID').val().length > 0) {
                            $('#footerAllDetails').addClass('hide');
                            $('#footerEntiId').removeClass('hide');
                        }
                        $("#exampleModalLongTitle").text("New Participant?");
                        $(".sf-submit-info").text("This participant could not be found in your project. Please confirm if this is a new participant.");
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }
                else if (formType == 2) {
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {
                        $('#search-for-tabular-data').css('max-width', 1000);
                        resultTable.append($("<tr><th>Id</th><th>First Name</th><th>Surname</th><th>Sub-type</th><th>Email</th><th></th></tr>"));
                        $('#search-variable-result-model-title').text('Are any of these person the one you are looking for?');
                        let responseBody = ""
                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let subtyp = "";
                            let email = "";
                            let entId = "";
                            let username = "";
                            let authType = "";
                           
                            jQuery.each(frm, function (i, val) {                              
                                if (val.variableName == "EntID") {
                                    entId = val.selectedValues;
                                }                              
                                if (val.variableName == "FirstName") {
                                    fn = val.selectedValues;
                                }                               
                                if (val.variableName == "Name") {
                                    sn = val.selectedValues;
                                }                               
                                if (val.variableName == "PerSType") {
                                    if (val.selectedValues == 1) {
                                        subtyp = 'Medical Pracitioner/Allied Health';
                                    } else if (val.selectedValues == 2) {
                                        subtyp = 'Non-Medical Practitioner';
                                    }
                                }                                
                                if (val.variableName == "Email") {
                                    email = val.selectedValues;
                                }
                            });
                            resultTable.append($("<tr><td>" + entId + "</td><td>" + fn + "</td><td>" + sn + "</td><td>" + subtyp + "</td><td>" + email + "</td><td class='text-right'><button type='button' onclick = \"Search.resultRedirect('" + entId + "', '" + gFormGuid + "')\" class='btn waves-effect waves-light btn-info'>Yes</button></td></tr>"));                           
                        });
                        resultFooter.append($('<button type="button" onclick="Search.save()" class="btn btn waves-effect waves-light btn-warning" data-dismiss="modal">Create New</button>'));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light btn-info" " data-dismiss="modal">Cancel</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this who you're looking for?");
                        jQuery.each(result, function (i, frm) {
                            let fn = "";
                            let sn = "";
                            let dob = "";
                            let gndr = "";
                            let entId = "";
                            let subtyp = "";
                            let email = "";
                            jQuery.each(frm, function (i, val) {                              
                                if (val.variableName == "EntID") {
                                    entId = "<tr><td class='sf-hd-title'>Person ID:</td><td>" + val.selectedValues + "</td></tr>";
                                    searchUserId = val.selectedValues;
                                }                                
                                if (val.variableName == "FirstName") {
                                    fn = "<tr><td class='sf-hd-title'>First Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }                            
                                if (val.variableName == "Name") {
                                    sn = "<tr><td class='sf-hd-title'>Surname:</td><td>" + val.selectedValues + "</td></tr>";
                                }                              
                                if (val.variableName == "PerSType") {
                                    if (val.selectedValues == 1) {
                                        subtyp = "<tr><td class='sf-hd-title'>Sub-type:</td><td>Medical Pracitioner/Allied Health</td></tr>";
                                    } else if (val.selectedValues == 2) {
                                        subtyp = "<tr><td class='sf-hd-title'>Sub-type:</td><td>Non-Medical Practitioner</td></tr>";
                                    }
                                }                              
                                if (val.variableName == "Email") {
                                    email = "<tr><td class='sf-hd-title'>Email:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(fn));
                            resultTable.append($(sn));
                            resultTable.append($(subtyp));
                            resultTable.append($(email));
                        });                     
                        resultFooter.append($('<button type="button" onclick="Search.save()" class="btn btn waves-effect waves-light btn-warning" data-dismiss="modal">Create New</button>'));
                        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' onclick=\"Search.resultRedirect('" + searchUserId + "', '" + gFormGuid + "')\">Yes</button>"));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {

                        $("#exampleModalLongTitle").text("New Person?");
                        $(".sf-submit-info").text("The person you entered could not be found. Would you like to create a new PERSON?");
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }
                else if (formType == 3) {
                   
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {
                        $('#search-for-tabular-data').css('max-width', 700);
                        resultTable.append($("<tr><th>Name</th><th>Postcode</th><th>State</th><th></th></tr>"));
                        $('#search-variable-result-model-title').text('Are any of these place the one you are looking for?');
                        let responseBody = ""

                        jQuery.each(result, function (i, frm) {
                            let name = "";
                            let suburb = "";
                            let state = "";

                            let entId = "";

                            jQuery.each(frm, function (i, val) {                          
                                if (val.variableName == "EntID") {
                                    entId = val.selectedValues;
                                }                          
                                if (val.variableName == "Name") {
                                    name = val.selectedValues;
                                }                              
                                if (val.variableName == "Postcode") {
                                    if (val.selectedValues !== "") {
                                        
                                    }
                                    suburb = val.selectedValues;
                                }
                                
                                if (val.variableName == "State") {
                                    if (val.selectedValues !== "") {                                    
                                        var sname = Search.StateList.find(function (x) {
                                            return x.guid === val.selectedValues;
                                        }).name;
                                        val.selectedValues = sname;
                                    }
                                    state = val.selectedValues;
                                }

                            });                       
                            resultTable.append($("<tr><td>" + name + "</td><td>" + suburb + "</td><td>" + state + "</td><td class='text-right'><button type='button' onclick=\"Search.resultRedirect('" + entId + "', '" + gFormGuid + "')\" class='btn waves-effect waves-light btn-info'>Yes</button></td></tr>"));
                            
                        });                       
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light btn-info" " data-dismiss="modal">Cancel</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this what you were looking for?");
                        jQuery.each(result, function (i, frm) {
                            let name = "";
                            let suburb = "";
                            let state = "";
                            let entId = "";

                            jQuery.each(frm, function (i, val) {                               
                                if (val.variableName == "EntID") {
                                    searchUserId = val.selectedValues;
                                    entId = "<tr><td class='sf-hd-title'>Place ID:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                
                                if (val.variableName == "Name") {
                                    name = "<tr><td class='sf-hd-title'>Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                              
                                if (val.variableName == "Postcode") {
                                    if (val.selectedValues !== "") {
                                      
                                    }
                                    suburb = val.selectedValues;

                                    suburb = "<tr><td class='sf-hd-title'>Postcode:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                             
                                if (val.variableName == "State") {
                                    if (val.selectedValues !== "") {
                                             var sname = Search.StateList.find(function (x) {
                                            return x.guid === val.selectedValues;
                                        }).name;
                                        val.selectedValues = sname;                                    }
                                    suburb = val.selectedValues;
                                    state = "<tr><td class='sf-hd-title'>State:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(name));
                            resultTable.append($(suburb));
                            resultTable.append($(state));
                        });
                        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' onclick=\"Search.resultRedirect('" + searchUserId + "', '" + gFormGuid + "')\">Yes</button>"));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));
                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {
                        $("#exampleModalLongTitle").text("New " + $("#PlaceGroup-EntType option:selected").html() + "?");
                        $(".sf-submit-info").text("The " + $("#PlaceGroup-EntType option:selected").html() + " you entered could not be found. Would you like to create a new " + $("#PlaceGroup-EntType option:selected").html() + "?");
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }

                else if (formType == 4) {
                   
                    let resultTable = $('#search-variable-result-model-table');
                    let resultFooter = $('#search-variable-result-model-footer');

                    if (result.length > 1) {
                        $('#search-for-tabular-data').css('max-width', 700);
                        resultTable.append($("<tr><th>Name</th><th>Postcode</th><th>State</th><th></th></tr>"));
                        $('#search-variable-result-model-title').text('Are any of these place the one you are looking for?');
                        let responseBody = ""

                        jQuery.each(result, function (i, frm) {
                            let name = "";
                            let suburb = "";
                            let state = "";

                            let entId = "";

                            jQuery.each(frm, function (i, val) {                               
                                if (val.variableName == "EntID") {
                                    entId = val.selectedValues;
                                }
                                if (val.variableName == "Name") {
                                    name = val.selectedValues;
                                }
                                
                                if (val.variableName == "Postcode") {
                                    if (val.selectedValues !== "") {                                       
                                    }
                                    suburb = val.selectedValues;
                                }
                               
                                if (val.variableName == "State") {
                                    if (val.selectedValues !== "") {                                    
                                        var sname = Search.StateList.find(function (x) {
                                            return x.guid === val.selectedValues;
                                        }).name;
                                        val.selectedValues = sname;
                                    }
                                    state = val.selectedValues;
                                }

                            });
                           
                            resultTable.append($("<tr><td>" + name + "</td><td>" + suburb + "</td><td>" + state + "</td><td class='text-right'><button type='button' onclick=\"Search.resultRedirect('" + entId + "', '" + gFormGuid + "')\" class='btn waves-effect waves-light btn-info'>Yes</button></td></tr>"));
                            
                        });                        
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light btn-info" " data-dismiss="modal">Cancel</button>'));

                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 1) {
                        let searchUserId = "";
                        $('#search-variable-result-model-title').text("Is this what you were looking for?");
                        jQuery.each(result, function (i, frm) {
                            let entId = "";
                            let entGrp = "";
                            let proSType = "";
                            let confData = "";
                            let cnstModel = "";
                            let ethics = "";
                            let dataStore = "";
                            let proDt = "";

                            jQuery.each(frm, function (i, val) {

                                if (val.variableName == "EntID") {
                                    searchUserId = val.selectedValues;
                                    entId = "<tr><td class='sf-hd-title'>Project ID:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableName == "Name") {
                                    entGrp = "<tr><td class='sf-hd-title'>Name:</td><td>" + val.selectedValues + "</td></tr>";
                                }
                                if (val.variableName == "ProSType") {
                                    $("#ProjectRegestration-ProSType").val(val.selectedValues);
                                    proSType = "<tr><td class='sf-hd-title' style='width: 70%;'>Project Sub-Type:</td><td>" + $("#ProjectRegestration-ProSType option:selected").text() + "</td></tr>";
                                }
                                if (val.variableName == "ConfData") {
                                    $("#ProjectRegestration-ConfData").val(val.selectedValues);
                                    confData = "<tr><td class='sf-hd-title'>Will this Project hold fully identified patient data:</td><td>" + $("#ProjectRegestration-ConfData option:selected").text() + "</td></tr>";
                                }
                                if (val.variableName == "CnstModel") {
                                    $("#ProjectRegestration-CnstModel").val(val.selectedValues);
                                    cnstModel = "<tr><td class='sf-hd-title'>What is the consent model for this Project:</td><td>" + $("#ProjectRegestration-CnstModel option:selected").text() + "</td></tr>";

                                }
                                if (val.variableName == "Ethics") {
                                    $("#ProjectRegestration-Ethics").val(val.selectedValues);
                                    ethics = "<tr><td class='sf-hd-title'>Does this Project have ethics approval:</td><td>" + $("#ProjectRegestration-Ethics option:selected").text() + "</td></tr>";
                                }
                                if (val.variableName == "DataStore") {
                                    $("#ProjectRegestration-DataStore").val(val.selectedValues);
                                    dataStore = "<tr><td class='sf-hd-title'>What are the data storage requirements based on your ethic approval:</td><td>" + $("#ProjectRegestration-DataStore option:selected").text() + "</td></tr>";
                                }
                                if (val.variableName == "ProDt") {
                                    proDt = "<tr><td class='sf-hd-title'>Project Activation Date:</td><td>" + val.selectedValues + "</td></tr>";
                                }

                                if ($("#ProjectRegestration-EntID").val().length > 0) {
                                    $('#ProjectRegestration-ProSType').prop('selectedIndex', 0);
                                    $('#ProjectRegestration-ConfData').prop('selectedIndex', 0);
                                    $('#ProjectRegestration-CnstModel').prop('selectedIndex', 0);
                                    $('#ProjectRegestration-Ethics').prop('selectedIndex', 0);
                                    $('#ProjectRegestration-DataStore').prop('selectedIndex', 0);
                                }
                            });

                            resultTable.append($(entId));
                            resultTable.append($(entGrp));
                            resultTable.append($(proSType));
                            resultTable.append($(confData));
                            resultTable.append($(cnstModel));
                            resultTable.append($(ethics));
                            resultTable.append($(dataStore));
                            resultTable.append($(proDt));
                        });

                        //resultFooter.append($(`<button type='button' class='btn btn waves-effect waves-light btn-info' onclick="Search.resultRedirect('${searchUserId}', '${gFormGuid}')">Yes</button>`));
                        resultFooter.append($("<button type='button' class='btn btn waves-effect waves-light btn-info' onclick=\"Search.resultRedirect('" + searchUserId + "', '" + gFormGuid + "')\">Yes</button>"));


                        //resultFooter.append($('<button type="button" class="btn btn waves-effect waves-light btn-info" onclick="Search.resultRedirect(' + $('#ProjectId').val() + ')">Yes</button>'));
                        resultFooter.append($('<button type="button" class="btn btn-danger waves-effect waves-light" data-dismiss="modal">No</button>'));

                        $('#search-variable-result-model').modal('show');
                    }
                    else if (result.length == 0) {
                        $("#exampleModalLongTitle").text("New Project?");
                        $(".sf-submit-info").text("The project you entered could not be found. Would you like to create a new project?");
                        $('#sf-not-found-search-popus').modal('show');
                    }
                }
            }

            else {
                if (formType == 1) {
                    if ($('#Participant-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');

                        $("#exampleModalLongTitle").text("New Participant?");
                        $(".sf-submit-info").text("The ID you have entered does not match a participant in your project. Please try another ID or search by first name, surname, date of birth and gender.");
                    }
                    else {
                        //if ($("#LoggedInUserRole").val() != "System Admin") {
                        //$('#footerAllDetails').addClass('hide');
                        //$('#footerEntiId').removeClass('hide');
                        $("#exampleModalLongTitle").text("New Participant?");
                        $(".sf-submit-info").text("This participant could not be found in your project. Please confirm if this is a new participant.");
                        $('#sf-not-found-search-popus').modal('show');
                        return false;
                        //}
                    }
                }
                else if (formType == 2) {
                    if ($('#Person-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');

                        $("#exampleModalLongTitle").text("New Person?");
                        $(".sf-submit-info").text("The ID you have entered does not match a person on file. Please try another ID or search by first name, surname, sub-type and email.");
                    }
                    else {
                        //if ($("#LoggedInUserRole").val() != "System Admin") {
                        //$('#footerAllDetails').addClass('hide');
                        //$('#footerEntiId').removeClass('hide');
                        $("#exampleModalLongTitle").text("New Person?");
                        $(".sf-submit-info").text("The person you entered could not be found. Would you like to create a new PERSON?");
                        $('#sf-not-found-search-popus').modal('show');
                        return false;
                        //}
                    }
                }
                else if (formType == 3) {
                    if ($('#PlaceGroup-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');

                        $("#exampleModalLongTitle").text("New Place?");
                        $(".sf-submit-info").text("The ID you entered does not match a place on file. Please try another ID or search by name, type and country.");
                    }
                    else {
                        //if ($("#LoggedInUserRole").val() != "System Admin") {
                        //$('#footerAllDetails').addClass('hide');
                        //$('#footerEntiId').removeClass('hide');
                        $("#exampleModalLongTitle").text("New " + $("#PlaceGroup-EntType option:selected").html() + "?");
                        $(".sf-submit-info").text("The " + $("#PlaceGroup-EntType option:selected").html() + " you entered could not be found. Would you like to create a new " + $("#PlaceGroup-EntType option:selected").html() + "?");
                        $('#sf-not-found-search-popus').modal('show');

                        return false;
                        //}
                    }
                }
                else if (formType == 4) {
                    if ($('#ProjectRegestration-EntID').val().length > 0) {

                        $('#footerAllDetails').addClass('hide');
                        $('#footerEntiId').removeClass('hide');

                        $("#exampleModalLongTitle").text("New Project?");
                        $(".sf-submit-info").text("The ID you entered does not match a project on file. Please try another ID or search by name, project subtype  etc.");
                    }
                    else {
                        //if ($("#LoggedInUserRole").val() != "System Admin") {
                        //$('#footerAllDetails').addClass('hide');
                        //$('#footerEntiId').removeClass('hide');

                        $("#exampleModalLongTitle").text("New Project?");
                        $(".sf-submit-info").text("The project you entered could not be found. Would you like to create a new project?");
                        $('#sf-not-found-search-popus').modal('show');
                        return false;
                        //}
                    }
                }

                $('#sf-not-found-search-popus').modal('show');

                //$('#sf-not-found-search-popus').modal('show');
            }
            
        }, 'POST');
    }
}

Search.validateSearchForm = function (formType) {
    if (formType == 1) {

        let isRequired = $(this).attr('data-variable-required');

        let resultCount = 0;
        let isId = 0;



        $(".participant-search-form").each(function () {
            var currentVal = $(this).val();

            if ($(this).attr('data-variable-name') == "DOB") {
                currentVal = $(this).text();
            }

            //if ($(this).attr('data-variable-key') == 1) {
            if ($(this).attr('data-variable-name') == "EntID") {
                if (currentVal.length > 0) {

                    resultCount = 0;
                    return false;
                } else {
                    isId++;
                    return;
                }
            }

            if (currentVal == "") {

                resultCount = 1;
                //resultCount1++;
                return false;
            }
        });


        if (resultCount == 0) {
            return true;
        } else if (resultCount == 1) {
            return false;
        } else {
            return false;
        }
    }

    else if (formType == 2) {

        let resultCount = 0;
        $(".person-search-form").each(function () {

            //if ($(this).attr('data-variable-key') == 1) {
            if ($(this).attr('data-variable-name') == "EntID") {
                if ($(this).val().length > 0) {

                    resultCount = 0;
                    return false;
                } else {
                    return;
                }
            }

            if ($(this).val() == "") {

                resultCount = 1;
                return false;
            }
        });


        if (resultCount == 0) {
            return true;
        } else if (resultCount == 1) {
            return false;
        } else {
            return false;
        }
    }

    else if (formType == 3) {

        let resultCount = 0;
        $(".place-group-search-form").each(function () {

            //if ($(this).attr('data-variable-key') == 1) {
            if ($(this).attr('data-variable-name') == "EntID") {
                if ($(this).val().length > 0) {

                    resultCount = 0;
                    return false;
                } else {
                    return;
                }
            }

            if ($(this).val() == "") {

                resultCount = 1;
                return false;
            }
        });


        if (resultCount == 0) {
            return true;
        } else if (resultCount == 1) {
            return false;
        } else {
            return false;
        }
    }

    else if (formType == 4) {
        let resultCount = 0;
        $(".project-search-form").each(function () {
            var currentVal = $(this).val();

            if ($(this).attr('data-variable-name') == "ProDt") {
                currentVal = $(this).text();
            }

            if ($(this).attr('data-variable-name') == "EntID") {
                if (currentVal.length > 0) {
                    resultCount = 0;
                    return false;
                } else {
                    return;
                }
            }

            if (currentVal == "") {

                resultCount = 1;
                return false;
            }




        });

        if (resultCount == 0) {
            return true;
        } else if (resultCount == 1) {
            return false;
        } else {
            return false;
        }
    }


}

Search.resultRedirect = function (participantId, fromId) {
    /////--------local host test url.
    //let currentUrl = window.location.href.toLowerCase().indexOf("uds-test");

    ////let currentUrl = window.location.pathname.toLowerCase().split('/');
    ////let urlSegments = currentUrl[1];

    ////---------live domain test url.
    //let currentUrl = window.location.href.toLowerCase().indexOf("uds-test");
    if (App.IsTestSite === true) {

    //}
    //if (currentUrl > 0) {
        /////--------local host test url.
        //location.href = "/Test/Summary/Index/" + participantId + "/" + fromId + "/" + $('#ProjectId').val();

        ////---------live domain test url. (becouse live project is on seperate domain)
        location.href = "/Summary/Index?participant=" + participantId + "&formId=" + fromId + "&guid=" + $('#ProjectId').val();
        //location.href = "/Summary/Index/" + participantId + "/" + fromId + "/" + $('#ProjectId').val();
    }
    else {
        location.href = "/Summary/Index?participant=" + participantId + "&formId=" + fromId + "&guid=" + $('#ProjectId').val();
        //location.href = "/Summary/Index/" + participantId + "/" + fromId + "/" + $('#ProjectId').val();
    }
}

Search.ClearParticipant = function (formType) {

    if (formType == 1) {
        $(".participant-search-form").each(function () {
            $(this).val('');
            if ($(this).prop("tagName") == "A") {
                $(this).editable('setValue', null);
                $(this).text('Date of Birth');
                $(this).removeClass('editable-unsaved');
            }
        });
    } else if (formType == 2) {
        $(".person-search-form").each(function () {
            $(this).val('');
        });

    } else if (formType == 3) {
        $(".place-group-search-form").each(function () {
            $(this).val('');
        });

    } else if (formType == 4) {
        $(".project-search-form").each(function () {
            $(this).val('');
            if ($(this).prop("tagName") == "A") {
                $(this).editable('setValue', null);
                $(this).removeClass('editable-unsaved').text($(this).attr("data-title"));
            }
        });

    }




}

Search.save = function () {

    let callApi = false;

    let sendData = {};

    sendData.ProjectId = $('#ProjectId').val();
    sendData.FormId = gFormGuid;

    sendData.FormDataEntryVariable = [];

    if (gFormType === 1) {
        $(".participant-search-form").each(function () {
            let currentValue = $(this).val();
            if ($(this).attr("data-variable-name") == "DOB") {
                currentValue = $(this).text();
            }
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-key'),
                "selectedValues": currentValue,
            });
        });
        callApi = true;
    }

    else if (gFormType === 2) {
        $(".person-search-form").each(function () {
            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-key'),
                "selectedValues": $(this).val(),
            });
        });
        callApi = true;
    }

    else if (gFormType === 3) {
        $(".place-group-search-form").each(function () {
            let countryAttr = $(this).attr('data-country-guid');
            let stateAttr = $(this).attr('data-state-guid');
            let postcodeAttr = $(this).attr('data-postcode-guid');

            if (typeof countryAttr !== typeof undefined && countryAttr !== false) {
                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-key'),
                    "selectedValues": countryAttr,
                });
            }
            else if (typeof stateAttr !== typeof undefined && stateAttr !== false) {
                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-key'),
                    "selectedValues": stateAttr,
                });
            }
            else if (typeof postcodeAttr !== typeof undefined && postcodeAttr !== false) {
                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-key'),
                    "selectedValues": postcodeAttr,
                });
            }
            else {
                sendData.FormDataEntryVariable.push({
                    "variableId": $(this).attr('data-variable-key'),
                    "selectedValues": $(this).val(),
                });
            }
            callApi = true;
        });
    }

    else if (gFormType === 4) {


        $(".project-search-form").each(function () {
            let currentValue = $(this).val();
            if ($(this).attr("data-variable-name") == "ProDt") {
                currentValue = $(this).text();
            }
            if ($(this).attr('data-variable-name') == "RecruitStart") {
                currentValue = $(this).text();
            }
            if ($(this).attr('data-variable-name') == "RecruitEnd") {
                currentValue = $(this).text();
            }

            sendData.FormDataEntryVariable.push({
                "variableId": $(this).attr('data-variable-key'),
                "selectedValues": currentValue,
            });
        });
        callApi = true;
    }  

    if (callApi === true) {

        
        let url = '/FormDataEntry/Save/';
        if (App.IsTestSite === true) {
            url = 'Review/FormDataEntrySave/';
        }

        //let isTestUrl = window.location.href.toLowerCase().indexOf("uds-test");
        //try {
        //    if (isTestUrl > 0) {
        //        url = 'Review/FormDataEntrySave/';
        //    }
        //} catch (ex) { }

        App.postData(App.ApiUrl + url, sendData, function (result) {

            if (result.participantId != "" && result.participantId != null) {

                if (isTestUrl > 0) {

                    /////--------local host test url.
                    //location.href = "/Test/Summary/Index/" + result.participantId + "/" + gFormGuid + "/" + $('#ProjectId').val();

                    ////---------live domain test url. (becouse live project is on seperate domain)
                    location.href = "/Summary/Index?participant=" + result.participantId + "&formId=" + gFormGuid + "&guid=" + $('#ProjectId').val();
                    //location.href = "/Summary/Index/" + result.participantId + "/" + gFormGuid + "/" + $('#ProjectId').val();
                }
                else {
                    location.href = "/Summary/Index?participant=" + result.participantId + "&formId=" + gFormGuid + "&guid=" + $('#ProjectId').val();
                    //location.href = "/Summary/Index/" + result.participantId + "/" + gFormGuid + "/" + $('#ProjectId').val();
                }
            }
            if (result != null) {
                var ids = result.map(function (search) {

                    if (search.variableId == 3) {
                        $('#spnParticipantId').text("000" + search.selectedValues);
                    }
                    if (search.variableId == 16) {
                        $('#spnFirstname').text(search.selectedValues);
                    }
                    if (search.variableId == 17) {
                        $('#spnSurname').text(search.selectedValues);
                    }
                    if (search.variableId == 44) {
                        $('#spnDob').text(search.selectedValues);
                    }
                    if (search.variableId == 45) {

                        if (search.selectedValues == 1) {
                            $('#spnGender').text('Male');
                        } else if (search.selectedValues == 2) {
                            $('#spnGender').text('Female');
                        } else if (search.selectedValues == 3) {
                            $('#spnGender').text('Other');
                        }
                    }
                    return search.id;
                })
                $('#sf-search-popus').modal('show');
            } else {
                $('#sf-not-found-search-popus').modal('show');
            }



        }, 'POST');
    }
}

Search.GetCountryList = function () {

    App.postData(App.ApiUrl + "/Country", {}, function (result) {

        Search.CountryList = result;
        var countries = [];

        let dropdown = $('#PlaceGroup-Country');
        dropdown.empty();
        dropdown.append('<option value="">Select</option>');
        dropdown.prop('selectedIndex', 0);

        dropdown.attr("onchange", "Search.StateListByCountry(this)");
        dropdown.attr("onblur", "Search.StateListByCountry(this)");

        $.each(Search.CountryList, function (key, entry) {
            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));
            countries.push(
                {
                    value: entry.name,
                    data: entry.guid
                }
            );
        });




        $('#PlaceGroup-Country1').autocomplete({
            source: countries,
            select: function (event, ui) {
                $("#PlaceGroup-Country").attr('data-country-guid', ui.item.data);
            },
            minLength: 0,

        }).focus(function () { $(this).data("uiAutocomplete").search($(this).val()); });

    }, "Get", true);


}

Search.GetStateList = function () {

    App.postData(App.ApiUrl + "/State", {}, function (result) {

        Search.StateList = result;
        var countries = [];
        $.each(Search.StateList, function (key, entry) {
            countries.push(
                {
                    value: entry.name, data: entry.guid
                }
            );
        });
    }, "Get", true);


}

Search.StateListByCountry = function (e) {
    //let countryId = $(e).attr('data-country-guid');
    let countryId = $(e).val();

    let state = [];

    //let country = $('#PlaceGroup-Country');
    let dropdown = $('#PlaceGroup-State');
    dropdown.empty();
    dropdown.append('<option value="">Select</option>');
    dropdown.prop('selectedIndex', 0);
    //dropdown.attr("onchange", "Search.StateListByCountry(this)");
    //dropdown.attr("onblur", "Search.StateListByCountry(this)");

    $.each(Search.StateList, function (key, entry) {


        if (entry.countryId == countryId) {

            dropdown.append($('<option></option>').attr('value', entry.guid).text(entry.name));

            state.push(
                {
                    value: entry.name, data: entry.guid
                }
            );
        }
    });

    $('#PlaceGroup-State1').autocomplete({
        source: state,
        select: function (event, ui) {
            $("#PlaceGroup-State").attr('data-state-guid', ui.item.data);
        },
        minLength: 0,
    }).focus(function () { $(this).data("uiAutocomplete").search($(this).val()); });


    //    .focus(function () {
    //    $(this).autocomplete("search", "");
    //});

    //    .focus(function () {
    //    // The following works only once.
    //    // $(this).trigger('keydown.autocomplete');
    //    // As suggested by digitalPBK, works multiple times
    //    // $(this).data("autocomplete").search($(this).val());
    //    // As noted by Jonny in his answer, with newer versions use uiAutocomplete
    //    $(this).data("uiAutocomplete").search($(this).val());
    //});



}

Search.GetPostCodeList = function () {

    App.postData(App.ApiUrl + "/PostCode", {}, function (result) {

        Search.PostCodeList = result;
        //var countries = [];
        //$.each(Search.StateList, function (key, entry) {
        //    
        //    countries.push(
        //        {
        //            value: entry.name, data: entry.guid
        //        }
        //    );
        //});
    }, "Get", true);


}

Search.PostCodeListByState = function (e) {

    let stateId = $(e).attr('data-state-guid');

    let postcode = [];

    $.each(Search.PostCodeList, function (key, entry) {
        if (entry.stateId == stateId) {
            postcode.push(
                {
                    value: entry.postalCode, data: entry.guid,
                }
            );
        }
    });

    $('#PlaceGroup-Postcode').autocomplete({
        source: postcode,
        select: function (event, ui) {
            $("#PlaceGroup-Postcode").attr('data-postcode-guid', ui.item.data);
        },
        minLength: 0,
    }).focus(function () { $(this).data("uiAutocomplete").search($(this).val()); });
}
