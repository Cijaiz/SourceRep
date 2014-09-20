var IsLoadFeedCountProcessing = false;
var IsGetUnReadFeedProcessing = false;

$(document).ready(function () {

    // First call to load feed count.
    c2c.notificationcorner.loadfeedcount();
    c2c.notificationcorner.getunreadfeeds();

    //Refresh the feed count & getunreadfeeds  for every 15 seconds
    setInterval(function () {
        if (IsLoadFeedCountProcessing == false && IsGetUnReadFeedProcessing == false) {
            c2c.notificationcorner.loadfeedcount();
            c2c.notificationcorner.getunreadfeeds();
        }
    }, 15000);

    $(".slidingDiv").hide();
    $(".show_hide").show();
    $('.show_hide').click(function () {
        c2c.notificationcorner.MarkAsReadFeeds();
        $(".slidingDiv").toggle();
        $(".slidingSettings").hide();
        $(".overlayTooltip").show();
    });

    $(".slidingSettings").hide();
    $(".show_settings").show();
    $('.show_settings').click(function () {
        $(".slidingSettings").toggle();
        $(".slidingDiv").hide();
        $(".overlayTooltip").show();
    });

    $(".overlayTooltip").click(function () {
        $(".slidingDiv").hide();
        $(".slidingSettings").hide();
        $(".overlayTooltip").hide();
    });
});
c2c.notificationcorner.getunreadfeeds = function () {
    IsGetUnReadFeedProcessing = true;

    $.ajax({
        cache: false,
        async: true,
        timeout: 30000,
        url: c2c.notificationcorner.GetUnReadFeedsUrl,
        success: function (response) {
            $('#divUnReadFeedList').html(response);
        },
        error: function (data) {
            var html = c2c.notificationcorner.RefreshHtml + "<p>Problem in retriving feeds </p>";
            $('#divUnReadFeedList').html(html);
        }
    });

    IsGetUnReadFeedProcessing = false;
}

c2c.notificationcorner.loadfeedcount = function () {
    IsLoadFeedCountProcessing = true;

    $.ajax({
        cache: false,
        async: true,
        timeout: 30000,
        url: c2c.notificationcorner.LoadFeedCountUrl,
        success: function (response) {
            $('#divFeedCount').html(response);
        },
        error: function (data) {
            $('#divFeedCount').html(c2c.notificationcorner.RefreshHtml);
        }
    });

    IsLoadFeedCountProcessing = false;
}

var isFeedRead = false;
c2c.notificationcorner.MarkAsReadFeeds = function () {
    var value = $('#divFeedCount').text();
    if (value > 0) {
        if (!isFeedRead) {
                $.ajax({
                    type: c2c.request.typePost,
                    async: true,
                    url: c2c.notificationcorner.MarkAsReadFeedsUrl,
                    success: function (response) {
                        if (response.Error == false)
                            isFeedRead = true;
                    }
                })
        }
    }
}