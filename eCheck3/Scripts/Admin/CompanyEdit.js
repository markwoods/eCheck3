$(document).ready(function () {
    $("#tbAddSelfRegistrationEmail").bind("keydown", function (e) {
        if (e.keyCode == 13) {
            addToSelfRegistrationEmail();
            return false;
        }
    });

    if ($('#tbCompany_Company_AllowSelfRegistration').is(":checked") == true) {
        // Hide/Show self registration details depending on whether feature is permitted
        $(".div-self-registration").show();
    } else {
        $(".div-self-registration").hide();
    }
    $("input:text").focus(function () { $(this).select(); });
    $('#MainDataTable').colResizable({ resizeMode: "flex" });
    $('#tbCompany_Company_AllowSelfRegistration').on('change', function () {
        // Hide/Show self registration details depending on whether feature is permitted
        if ($(this).is(":checked") == true) {
            $(".div-self-registration").show();
        } else {
            $(".div-self-registration").hide();
        }
    });
    $('#tbCompany_Company_AllowLive').on("change", function () {
        // If allowlive is unchecked, then allow training must be checked,
        // and default live must not be checked
        if ($(this).is(":checked") == false) {
            $('#tbCompany_Company_AllowTraining').prop("checked", true);
            $('#tbCompany_Company_DefaultLive').prop("checked", false);
            $('#tbCompany_Company_DefaultLive').prop("disabled", true);
        }
        else {
            if ($("#tbCompany_Company_AllowTraining").is(":checked") == true) {
                // both checked, enable live vs training default selector
                $('#tbCompany_Company_DefaultLive').prop("disabled", false);
            }
        }
    });

    $('#tbCompany_Company_AllowTraining').on("change", function () {
        if ($(this).is(":checked") == false) {
            $('#tbCompany_Company_AllowLive').prop("checked", true);
            $('#tbCompany_Company_DefaultLive').prop("checked", true);
            $('#tbCompany_Company_DefaultLive').prop("disabled", true);
        }
        else {
            if ($("#tbCompany_Company_AllowLive").is(":checked") == true) {
                // both checked, enable live vs training default selector
                $('#tbCompany_Company_DefaultLive').prop("disabled", false);
            }
        }
    });
});

function addToSelfRegistrationEmail() {
    //
    // Validate and add email suffix to self-registration list
    //

    var strEmailSuffix = $("#tbAddSelfRegistrationEmail").val();

    // Validate Email Suffix
    if (isValidEmailAddress('user@' + strEmailSuffix) == false) {
        $('#fgSelfRegistrationEmail').addClass("has-error");
        $('#helpEmail').html('Invalid eMail suffix.  Suffix is everthing after the @.  (ie mycompany.com)');
        $('#tbAddSelfRegistrationEmail').focus();
        return false;
    }
    // Else, all good, clear any error messages
    $('#fgSelfRegistrationEmail').removeClass("has-error");
    $('#helpEmail').html('');

    // Build URL String
    var sb = tools.getStringBuilder();
    sb.append("../../Service/Administration.svc/Company/" + $("#tbCompany_Company_ID").val());
    sb.append("/SelfRegistration?strEmailSuffix=" + $("#tbAddSelfRegistrationEmail").val());

    // Set Wait Cursor
    $('html,body').css('cursor', 'wait');

    // Call DataService to add to database
    $.ajax({
        type: "POST",
        url: sb.toString(),
        cache: false,
        async: true,
        success: function (data, textStatus, xmlhttprequest) {
            // Add to select list
            var o = new Option(strEmailSuffix, strEmailSuffix);
            $(o).html(strEmailSuffix);
            $("#selectSelfRegistrationAddresses").append(o);
            // Remove from textbox
            $("#tbAddSelfRegistrationEmail").val('');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
        },
        complete: function () {
            // Set default cursor
            $('html,body').css('cursor', 'default');
        }
    });
}

function removeFromSelfRegistrationEmail() {

    // Build URL String
    var sb = tools.getStringBuilder();
    sb.append("../../Service/Administration.svc/Company/" + $("#tbCompany_Company_ID").val());
    sb.append("/SelfRegistration?strEmailSuffix=");
    var strBaseURL = sb.toString();

    // Set Wait Cursor
    $('html,body').css('cursor', 'wait');

    $("#selectSelfRegistrationAddresses option:selected").each(function () {
        // Loop through each selected email suffix and call service to delete
        // Set Wait Cursor
        var strOptionValue = $(this).val();
        $('html,body').css('cursor', 'wait');
        $.ajax({
            type: "DELETE",
            url: strBaseURL + strOptionValue,
            cache: false,
            async: true,
            success: function (data, textStatus, xmlhttprequest) {
                // Remove item from select list
                $("#selectSelfRegistrationAddresses option[value='" + strOptionValue + "']").remove();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            },
            complete: function () {
                // Set default cursor
                $('html,body').css('cursor', 'default');
            }
        });
    });
    $('html,body').css('cursor', 'default');
}
