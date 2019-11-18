﻿$(function () {
    $.fn.formValidation = function () {
        var formContainer = this;
        var method = {
            validate: function (element) {
                var validationArr = method.getFormValidateAttributes(element),
                    hasError = false,
                    message;
                for (let i = 0; i < validationArr.length; i++) {
                    var check,
                        validationType = validationArr[i];
                    switch (validationType.name) {
                        case "required":
                            check = method.checkRequire(element);
                            break;
                        case "maxtextlength":
                            check = method.checkMaxLength(element, validationType.value)
                            break;
                        case "maxsize":
                            check = method.checkMaxSize(element, validationType.value)
                            break;
                        default:
                    }
                    if (check.isError) {
                        hasError = true;
                        message = check.message;
                        break;
                    }
                }
                if (hasError) {
                    method.setFormError(element, message);
                    return true;
                } else {
                    method.removeFormError(element);
                    return false;
                }
            },
            validates: function () {

                var texts = $(formContainer).find(`input[type=text]`),
                    files = $(formContainer).find(`input[type=file]`),
                    radios = $(formContainer).find(`input[type=radio]`),
                    checkboxs = $(formContainer).find(`input[type=checkbox]`),
                    numbers = $(formContainer).find(`input[type=number]`),
                    dates = $(formContainer).find(`input[type=date]`),
                    selects = $(formContainer).find(`select`),
                    textarea = $(formContainer).find(`textarea`);

                var input = [...texts, ...files, ...radios, ...checkboxs, ...numbers, ...dates, ...selects, ...textarea];
                var formHasError = false;
                for (var i = 0; i < input.length; i++) {
                    var element = $(input[i]);
                    var isError = method.validate(element);
                    if (isError && !formHasError) {
                        formHasError = true;
                    }
                }
                return formHasError;
            },
            getFormValidateAttributes: function ($node) {
                var attrs = [];
                $.each($node[0].attributes, function (index, attribute) {
                    var validateAttr = ["required", "maxtextlength", "maxsize"]
                    if (validateAttr.includes(attribute.name)) {
                        //attrs[attribute.name] = attribute.value;
                        var attr = {
                            name: attribute.name,
                            value: attribute.value
                        }
                        attrs.push(attr);
                    } 
                });

                return attrs;
            },
            checkRequire: function ($node) {
                var element = $node;
                var container = element.closest("div.form-group");
                var val = element.val().trim();
                var error = {}
                if (val != "" || element[0].hasAttribute("data-default-file")) {
                    error["isError"] = false;
                } else {
                    error["isError"] = true;
                    error["message"] = "This field is required";
                }
                return error;
            },
            checkMaxLength: function ($node, length) {
                var element = $node;
                var val;
                if (element.attr("type") == "tinymce") {
                    var id = element.attr("id");
                    val = tinyMCE.get(id).getContent({ format: 'text' }).replace(/\r?\n|\r/g, "");
                } else {
                    val = element.val().trim();
                }
                
                var error = {}
                if (val.length > length) {
                    error["isError"] = true;
                    error["message"] = `This field max length is ${length} characters`;
                } else {
                    error["isError"] = false;
                }
                return error;
            },
            checkMaxSize: function ($node, size) {
                var element = $node,
                    file = element[0].files[0],
                    error = {};
                if (file != null) {
                    var fileSize = file.size;
                    var num = parseInt(size, 10),
                        unit = size.replace(num, "").toLowerCase().trim(),
                        pow;
                    switch (unit) {
                        case "kb":
                        case "k":
                            pow = 1;
                            break;
                        case "mb":
                        case "m":
                            pow = 2;
                            break;
                        case "gb":
                        case "g":
                            pow = 3;
                            break;
                        case "byte":
                        case "b":
                        default:
                            pow = 0;
                    }
                    var maxFileSize = num * Math.pow(1024, pow);
                    if (fileSize > maxFileSize) {
                        error["isError"] = true;
                        error["message"] = `This file is too big (${num} ${unit} maximum)`;
                    } else {
                        error["isError"] = false;
                    }
                } else {
                    error["isError"] = false;
                }
                return error;
            },
            setFormError: function ($node, message) {
                var element = $node;
                var container = element.closest("div.form-group");
                container.addClass("error");
                if (container.find(".error-message").length == 0) {
                    container.append(`<span class="error-message">${message}</span>`)
                } else {
                    container.find(".error-message").text(message);
                }
            },
            removeFormError: function ($node) {
                var element = $node;
                var container = element.closest("div.form-group");
                container.removeClass("error").find(".error-message").fadeOut(500, function () { $(this).remove(); });
            }

        }
        $(formContainer).find(`input[type=text], textarea, input[type=number], input[type=tel]`).on("change paste keyup focusout", function () {
            var element = $(this);
            method.validate(element);
        });
        //$(formContainer).find(`input[type=file]`).on("change", function () {
        //    var element = $(this);
        //    method.validate(element);
        //})
        return method;

    }
})