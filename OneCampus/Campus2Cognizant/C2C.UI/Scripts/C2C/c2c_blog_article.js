$(document).ready(function () {

    $(".comment-toggle").hide();
    $(".comment-box").show();
    $(".comment-box").click(function () {
        if ($(".connect-content").hasClass("move-left")) {
            $(".connect-content").removeClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment.png");
            $(".comment-count").css('color', '#133d74');
            $("#like-box").css('margin-left', '795px');

        } else {
            $(".connect-content").addClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment-hide.png");
            $(".comment-count").css('color', '#b5b5b5');
            $("#like-box").css('margin-left', '557px');
        }
        $(".comment-toggle").toggle();
    });

    $(".comment-close").click(function () {
        if ($(".connect-content").hasClass("move-left")) {
            $(".connect-content").removeClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment.png");
            $(".comment-count").css('color', '#133d74');
            $("#like-box").css('margin-left', '795px');
        } else {
            $(".connect-content").addClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment-hide.png");
            $(".comment-count").css('color', '#b5b5b5');
        }
        $(".comment-toggle").toggle();
    });

    /**ISSUE: .like-bx  class also includes the popup span ,hence image changes when clicked on like popup**/
    //$(".like-bx").click(function () {
    //    if ($(".like-bx img").hasClass("like-background")) {
    //        $(".like-bx img").removeClass('like-background');
    //    } else {
    //        $(".like-bx img").addClass('like-background');
    //    }

    //});

    /**FIX:Click to change background written only for image**/
    $(".likimg").click(function () {
        if ($(".like-bx img").hasClass("like-background")) {
            $(".like-bx img").removeClass('like-background');
        } else {
            $(".like-bx img").addClass('like-background');
        }

    });



    $('.like-btn').mouseover(function () {
        //alert("over");
        // $(".like-bx").css('background-color','#b5b5b5');
        $("#like-box").fadeIn();
        $(".overlayTooltip").show();
    });

    $(".friend-info-share").live("click", function (ev) {
        if ($(this).hasClass("add-background")) {
            $(this).removeClass('add-background');
        } else {
            $(this).addClass('add-background');
        }
    });

    $("#cancel").live("click", function (ev) {
        ev.preventDefault();
        $.ajaxSetup({ cache: false });
        $('#sharecomment').val('');
        $('.friend-info-share').removeClass('add-background');
    });

    $("#divArticleContent").mCustomScrollbar({
        scrollButtons: {
            enable: true
        },
        theme: "dark-thick"
    });

    
});
