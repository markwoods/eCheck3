$(document).ready(function () {
    $('#AllowLive').on("change", function () {
        // If allowlive is unchecked, then allow training must be checked,
        // and default live must not be checked
        if ($(this).is(":checked") == false) {
            $('#AllowTraining').prop("checked", true);
            $('#DefaultLive').prop("checked", false);
            $('#DefaultLive').prop("disabled", true);
        }
        else {
            if ($("#AllowTraining").is(":checked") == true) {
                // both checked, enable live vs training default selector
                $('#DefaultLive').prop("disabled", false);
            }
        }
    });

    $('#AllowTraining').on("change", function () {
        if ($(this).is(":checked") == false) {
            $('#AllowLive').prop("checked", true);
            $('#DefaultLive').prop("checked", true);
            $('#DefaultLive').prop("disabled", true);
        }
        else {
            if ($("#AllowLive").is(":checked") == true) {
                // both checked, enable live vs training default selector
                $('#DefaultLive').prop("disabled", false);
            }
        }
    });
});
