var IsLoadUserStat = false;

$(document).ready(function () {
    c2c.dashboard.loadActiveUserStat();
    c2c.dashboard.loadUserBrowserStat();

    setInterval(function () {
        if (IsLoadUserStat == false) {
            c2c.dashboard.loadActiveUserStat();
        }
    }, 15000);
});

c2c.dashboard.loadActiveUserStat = function () {
    IsLoadUserStat = true;

    $.ajax({
        url: c2c.dashboard.activeUserStatUrl,
        async:true,
        cache: false,
        timeout: 30000,
        success: function (response) {
            $('#divOnlineUserStat > .widget-content').html(response);
        },
        error: function (data) {
            if ($('#divOnlineUserStat > .widget-content').html() == "Loading...") {
                $('#divOnlineUserStat > .widget-content').html(c2c.dashboard.widgetErrorHtml);
            }
        }
    });

    IsLoadUserStat = false;
}

c2c.dashboard.loadUserBrowserStat = function () {
    $.ajax({
        url: c2c.dashboard.userBrowserStatUrl,
        async: true,
        cache: false,
        success: function (response) {
            $('#divUserBrowserStat > .widget-content').html(response);
        },
        error: function (data) {
            $('#divUserBrowserStat > .widget-content').html(c2c.dashboard.widgetErrorHtml);
        }
    });
}