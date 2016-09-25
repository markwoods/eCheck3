$(document).ready(function () {
    // Trap doubleclick event on lists to trigger add/delete functions
    $("#selectAllUsers").on("dblclick", function (e) { addUserToGroup(); });
    $("#selectGroupMembers").on("dblclick", function (e) { removeUserFromGroup(); });

});

function addUserToGroup() {
    //
    // add selected user(s) to group membership
    //

    // Ensure something is selected
    if ($("#selectAllUsers").val() == null) { return false; }

    // Get Comma Delimited list of Users to add to list
    var strUserList = buildCommaDelimitedStringFromSelectList($("#selectAllUsers"));

    // Build URL String
    var sb = tools.getStringBuilder();
    sb.append("../../Service/Administration.svc/Group/" + $("#GroupID").val());
    sb.append("/Membership?strUserIDList=" + strUserList);

    // Set Wait Cursor
    $('html,body').css('cursor', 'wait');

    // Call DataService to add to database
    $.ajax({
        type: "POST",
        url: sb.toString(),
        cache: false,
        async: true,
        success: function (data, textStatus, xmlhttprequest) {
            // add to select list on screen
            $("#selectAllUsers option:selected").each(function () {
                var o = new Option($(this).val(), $(this).val());
                $(o).html($(this).text());
                $("#selectGroupMembers").append(o);
            });
            // remove from user list
            $("#selectAllUsers option:selected").remove();
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


function removeUserFromGroup() {
    //
    // Remove selected user(s) from group membership
    //

    // Ensure something is selected
    if ($("#selectGroupMembers").val() == null) { return false; }

    // Get Comma Delimited list of Users to add to list
    var strUserList = buildCommaDelimitedStringFromSelectList($("#selectGroupMembers"));

    // Build URL String
    var sb = tools.getStringBuilder();
    sb.append("../../Service/Administration.svc/Group/" + $("#GroupID").val());
    sb.append("/Membership?strUserIDList=" + strUserList);

    // Set Wait Cursor
    $('html,body').css('cursor', 'wait');

    // Call DataService to remove from database
    $.ajax({
        type: "DELETE",
        url: sb.toString(),
        cache: false,
        async: true,
        success: function (data, textStatus, xmlhttprequest) {
            // add to select list on screen
            $("#selectGroupMembers option:selected").each(function () {
                var o = new Option($(this).val(), $(this).val());
                $(o).html($(this).text());
                $("#selectAllUsers").append(o);
            });
            // remove from user list
            $("#selectGroupMembers option:selected").remove();
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


