//--------------------------------------- script Tile FLIP EFFECTS
//play on hover

$(document).ready(function () {

    var $tiles = $("#tile1, #tile2, #tile3, #tileWelcomeNote").liveTile({
        playOnHover: true,
        repeatCount: 0,
        delay: 0,
        startNow: false
    });

    //animate on click
    var $tile4 = $("#tile4").liveTile({
        playOnHover: false,
        repeatCount: 0,
        delay: 0,
        startNow: false
    });

    //$("#poll-submit").on("click", function () {
    //    var hasPermission = $("#hasPermission").val();
    //    if (hasPermission) {
    //        if ($("input[name='poll-vote']:checked").length > 0) {
    //            // one ore more checkboxes are checked
    //            //alert("Thanks");
    //            $("#tile4").liveTile("play", 0);
    //        }
    //        else {
    //            // no checkboxes are checked
    //            alert("Please select any 1 option");
    //        }
    //    }
    //    else {
    //        alert('You dont have permission to vote');
    //    }

    //});
});

//SCroll function by arrow keys
$(document).keydown(function (evt) {
    k = evt.keyCode;
    if (k == 37) {
        $('.content-wrapper').css('-webkit-transform', 'translateX(0px)');
        $('.right-arrow').hide();
        $('.left-arrow').hide();
    }
    else if (k == 39) {
        $('.content-wrapper').css('-webkit-transform', 'translateX(-400px)');
        $('.left-arrow').hide();
        $('.right-arrow').hide();

    }
});


// arrows 

$(document).ready(function () {
    var leftMargin = 600;
    var width = $(document).width();
    //alert(width);
    var windowWidth = 470;
    //alert(windowWidth);

    $('.right-arrow').mouseover(function () {
        $('.content-wrapper').animate({
            marginLeft: "-=" + windowWidth
        }, "medium");

        $('.left-arrow').show();
        leftMargin = (leftMargin + windowWidth);
        if (leftMargin > width - windowWidth) {
            $('.right-arrow').hide();
        }
    });

    $('.left-arrow').mouseover(function () {
        $('.content-wrapper').animate({
            marginLeft: "+=" + windowWidth
        }, "medium");

        $('.right-arrow').show();
        leftMargin = (leftMargin - windowWidth)
        if (leftMargin == 600) {
            $('.left-arrow').hide();
        }
    });


    $('.left-arrow').click(function () {
        $('.content-wrapper').animate({
            marginLeft: "+=" + windowWidth
        }, "medium");

        $('.right-arrow').show();
        //alert(leftMargin);
        leftMargin = (leftMargin - windowWidth)
        //alert(leftMargin);
        if (leftMargin == 600) {
            $('.left-arrow').hide();
        }
    });
    $('.right-arrow').click(function () {
        $('.content-wrapper').animate({
            marginLeft: "-=" + windowWidth
        }, "medium");

        $('.left-arrow').show();
        leftMargin = (leftMargin + windowWidth);
        if (leftMargin > width - windowWidth) {
            //alert(leftMargin);
            $('.right-arrow').hide();
        }
    });

    $('#tileWelcomeNote .re-assurance').click(function () {
        var location = $(this).children('a').attr('href');
        window.location = location;
    });
    //---------------youTube,twitter,linkedin & facebook Starts -----------------------//
    $(".newwindow").click(function () {
        window.open(this.id, '_blank', 'fullscreen=no,status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes,titlebar=no');
    });
    //---------------youTube,twitter,linkedin & facebook Ends-------------------------//

});


//-------------WelcomeNote script starts here--------------------------//


$(document).ready(function () {
    LoadWelcomeNote(c2c.welcomeNote.SourceLogin);
    function LoadWelcomeNote(sourceID) {
        var actionUrl = c2c.welcomeNote.Url + sourceID;
        $.ajax({
            cache: false,
            url: actionUrl,
            async: true,
            success: function (data) {
                if (!data.errorCode && data.length > 0) {
                    if (sourceID == c2c.welcomeNote.SourceLogin) {
                        $('#get-started').fadeIn("slow");
                        $('.overlayBg').fadeIn("slow");
                    }
                    if (sourceID == c2c.welcomeNote.SourceTile) {
                        $('#popup_box').fadeIn("slow");
                        $("#container").css({
                            "opacity": "0.3"
                        });
                        $('.overlayBg').fadeIn("slow");
                    }
                    $('.note-content').html('');
                    $('.note-content').html(data);
                }
            }
        });
    }


    $('#getstarted-btn').click(function () {
        $('#get-started').fadeOut("slow");
        $('.overlayBg').fadeOut("slow");
    });

    $("#btn-WelcomeNotePopup").click(function () {
        LoadWelcomeNote(c2c.welcomeNote.SourceTile)
    });

    $('#popupBoxClose').click(function () {
        $('#popup_box').fadeOut("slow");
        $("#container").css({
            "opacity": "1"
        });
        $('.overlayBg').fadeOut("slow");
    });
});


//-------------WelcomeNote script ends here--------------------------//

//Navigation of Blog Tile to Blog-Index Page starts here

$('.blogfirstdiv').click(function () {
    var location = $(this).children('a').attr('href');
    window.location = location;
});

$('.blogseconddiv').click(function () {
    var location = $(this).children('span').children('a').attr('href');
    window.location = location;
});
