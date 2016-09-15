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
function isValidEmailAddress(emailAddress) {
    //
    // Evaluates sent string against pattern for compliance with standard email formats
    //

    var pattern = new RegExp(/^[+a-zA-Z0-9.'\_-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
    return pattern.test(emailAddress);
};

var tools = {
    getStringBuilder: function () {
        var data = [];
        var counter = 0;
        return {
            // adds string s to the stringbuilder
            append: function (s) { data[counter++] = s; return this; },
            // removes j elements starting at i, or 1 if j is omitted
            remove: function (i, j) { data.splice(i, j || 1); return this; },
            // inserts string s at i
            insert: function (i, s) { data.splice(i, 0, s); return this; },
            // builds the string
            toString: function (s) { return data.join(s || ""); return this; },
            // clears the string
            clear: function () { data.length = 0; return this; }
        };
    }
};
