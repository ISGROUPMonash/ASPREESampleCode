(function () {   

    $.validator.unobtrusive.adapters.addSingleVal("minimumfilesize", "size");
    $.validator.unobtrusive.adapters.addSingleVal("maximumfilesize", "size");
    $.validator.unobtrusive.adapters.addSingleVal("validfiletype", "filetypes");
    //$.validator.unobtrusive.adapters.addSingleVal("imagedimension", "width", "height");
    $.validator.unobtrusive.adapters.addSingleVal("imagedimension", "width");

    $.validator.addMethod('minimumfilesize', function (value, element, minSize) {
        return convertBytesToMegabytes(element.files[0].size) >= parseFloat(minSize);
    });

    $.validator.addMethod('maximumfilesize', function (value, element, maxSize) {
        return convertBytesToMegabytes(element.files[0].size) <= parseFloat(maxSize);
    });

    $.validator.addMethod('imagedimension', function (value, element, maxWidth, maxHeight) {
        return validateImageDimensions(element.files[0], maxWidth, maxWidth);
    });

    $.validator.addMethod('validfiletype', function (value, element, validFileTypes) {
        if (validFileTypes.indexOf(',') > -1) {
            validFileTypes = validFileTypes.split(',');
        } else {
            validFileTypes = [validFileTypes];
        }

        var fileType = value.split('.')[value.split('.').length - 1];

        for (var i = 0; i < validFileTypes.length; i++) {
            if (validFileTypes[i] === fileType) {
                return true;
            }
        }

        return false;
    });

    $.validator.unobtrusive.adapters.add('fileuploadvalidator', ['clientvalidationmethods', 'parameters', 'errormessages'], function (options) {
        options.rules['fileuploadvalidator'] = {
            clientvalidationmethods: options.params['clientvalidationmethods'].split(','),
            parameters: options.params['parameters'].split('|'),
            errormessages: options.params['errormessages'].split(',')
        };
    });

    $.validator.addMethod("fileuploadvalidator", function (value, element, param) {
        if (value == "" || value == null || value == undefined) {
            return true;
        }
        //array of jquery validation rule names
        var validationrules = param["clientvalidationmethods"];

        //array of paramteres required by rules, in this case regex patterns
        var patterns = param["parameters"];

        //array of error messages for each rule
        var rulesErrormessages = param["errormessages"];

        var validNameErrorMessage = new Array();
        var index = 0

        for (i = 0; i < patterns.length; i++) {
            var valid = true;
            var pattern = patterns[i].trim();

            //get a jquery validator method.  
            var rule = $.validator.methods[validationrules[i].trim()];

            //create a paramtere object
            var parameter = new Object();
            parameter = pattern;

            //execute the rule
            var isValid = rule.call(this, value, element, parameter);

            if (!isValid) {
                //if rule fails, add error message
                validNameErrorMessage[index] = rulesErrormessages[i];
                index++;
            }
        }
        //if we have more than on error message, one of the rule has failed
        if (validNameErrorMessage.length > 0) {
            //update the error message for 'validname' rule
            $.validator.messages.fileuploadvalidator = validNameErrorMessage.toString();
            return false;
        }
        return true;
      }, "This is not a valid individual name"//default error message
    );

    function convertBytesToMegabytes(bytes) {
        return (bytes / 1024) / 1024;
    }
    let imageDimensionResult = true;
    function validateImageDimensions(file, maxWidth, maxHeight) {
        let reader = new FileReader;
        reader.onload = function () {
            let img = new Image;

            img.onload = function () {
                if (img.width > parseInt(maxWidth) || img.height > parseInt(maxHeight)) {
                    imageDimensionResult = false;
                }
            };

            img.src = reader.result;
        };

        reader.readAsDataURL(file);

        return imageDimensionResult;
    }
})();