$userList = $("tbody.user-list");
$userTemplate = $("tr.user-template");
$refresh = $(".fa-refresh");
$errorMessage = $(".generic-error");
$authErrorMessage = $(".auth-error");
isRefreshing = false;

$("button.btn-refresh").click(function (e) {
    if (!isRefreshing) {
        $userList.empty();
        $refresh.slideDown("slow");
        isRefreshing = true;
        $.getJSON("/api/admin/users", function (json) {
            isRefreshing = false;
            $refresh.hide();
            json.map(appendUser);
        }).fail(function (xhr) {
            if (xhr.status == 401) {
                $authErrorMessage.show();
            } else {
                $errorMessage.show();
            }
        });
    }
});

function appendUser(obj) {
    $template = $userTemplate.clone();
    $template.removeAttr("hidden");
    $template.children(".displayName").html(obj.DisplayName);
    $template.children(".givenName").html(obj.GivenName);
    $template.children(".surname").html(obj.Surname);
    $template.children(".displayableID").html(obj.UserPrincipalName);
    $template.children(".objectID").html(obj.ObjectId);
    if (obj.assignmentStatus == "New" || obj.assignmentStatus == "Enabled") {
        $template.children(".disabledIcon").empty();
    } else {
        $template.addClass("removed");
    }
    $userList.append($template);
    if (obj.assignmentStatus == "New") {
        console.log('Turning Green....' + obj.DisplayName);
        $template.addClass("new-user").delay(500).queue(function (next) {
            $(this).removeClass("new-user");
            next();
        });
    }
}

$("button.btn-refresh").trigger("click");