// Global document ready scripts

$(document).ready(function () {
    // Set the focus to the desired field on every form in the app
    $(".initial-focus-control").focus();

    // Trap hover event for EMSdata.ca logo style changes
    $(".branddata").mouseover(function () { brandHover(true); });
    $(".branddata").mouseout(function () { brandHover(false); });
    $(".brandEMS").mouseover(function () { brandHover(true); });
    $(".brandEMS").mouseout(function () { brandHover(false); });


});

function brandHover(isHovering){
    //
    // Handle the mouseover/mouseout effects on the brand logo
    //
    if (isHovering) {
        $(".branddata").css("color", "#FFFFFF");
        $(".brandEMS").css("color", "#2060DF");
    }
    else {
        $(".branddata").css("color", "#797979");
        $(".brandEMS").css("color", "#143D8D");
    }
}
