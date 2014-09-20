$(document).ready(function () {
/*
        Panorama - Im still tweaking this for android
*/
var $panos = $(".panorama");
    stgs = {
        size: 'auto',
        padRight: 48,
        showScroll: false,
        contentSel: ".content-wrapper",
        contentWidthSel: typeof (window.contentWidthSel) !== "undefined" ? window.contentWidthSel : ".content-wrapper>section,.content-wrapper>div,.content-wrapper>.tile-group"
    },
    metrojs = $.fn.metrojs;
    if (metrojs.capabilities === null)
        metrojs.checkCapabilities();
    // debug
    // $("h2").first().append(' ' + metrojs.capabilities.canTouch + ' - ' + $("html").hasClass("no-overflowscrolling"));
    if ($panos.length > 0){        
        $panos.each(function (idx, ele) {
            var $pano = $(ele);
            var panoHeight = $pano.height();
            var panoWidth = $pano.width();
            if (!metrojs.capabilities.canTouch) {
                if (!stgs.showScroll)
                    $pano.css({ 'overflow-x': 'hidden' });
                setPanoWidth($pano, stgs);
                $pano.bind("mousewheel.metrojs", function (event, delta) {
                    if (event.ctrlKey)
                        return;
                    event.preventDefault();
                    var amount;
                    if (delta < 0) {
                        amount = 90;
                    } else {
                        amount = -90;
                    }
                    $pano.scrollLeft($pano.scrollLeft() + amount)
                });
            }else{
                setPanoWidth($pano, stgs);
            }
        });         
    }
    function setPanoWidth($pano, stgs) {
        if (stgs.size === 'auto') {
            var width = stgs.padRight;
            $pano.find(stgs.contentWidthSel).each(function () {
                width += $(this).outerWidth(true);
            });
            // debug
            // $(".site-title").append(width + '-');
            $pano.find(stgs.contentSel).css({ width: width + 'px' });
        } else if (stgs.size.length > 0) {
            // debug
            // $(".site-title").append(stgs.size + '+');
            $pano.find(stgs.contentSel).css({ width: stgs.size });
        }
    }
    var viewport = (function viewport() {
        var e = window,
            a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        return { width: e[a + 'Width'], height: e[a + 'Height'] }
    })();
    // debug
    //$(".site-title").append(viewport.width + 'x' + viewport.height);
    // console.log("complete");
});
 