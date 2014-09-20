//--------------------------------------- script Browser Cookie Detection
//play on hover

$(document).ready(function () {
    //string urlLogOn = Url.Action("LogOn", "Account", new { Area = "Orchard.Users", ReturnUrl = (Request.QueryString["ReturnUrl"] ?? Request.RawUrl) });    

    function cookiedetection() {
        // Create cookie 
        var cookieEnabled = (navigator.cookieEnabled) ? true : false;
        document.cookie = "testcookie";
        //if navigator,cookieEnabled is not supported
        if (typeof navigator.cookieEnabled == "undefined" || !cookieEnabled) {
            cookieEnabled = (document.cookie.indexOf("testcookie") != -1) ? true : false;
        }
        if (!cookieEnabled) {
            alert("COOKIES need to be enabled!");
        }
    }
});