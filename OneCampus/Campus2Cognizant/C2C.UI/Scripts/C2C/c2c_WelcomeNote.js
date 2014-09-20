//Date picker for Welcomenote//
$(document).ready(function () {


    $("#OfferExtendedStartDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });
    $("#OfferExtendedEndDate").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });

    tinymce.init({
        selector: "textarea",
        plugins: [
        "advlist autolink lists link image charmap print preview anchor",
        "searchreplace visualblocks code fullscreen",
        "insertdatetime media table contextmenu paste"
        ],
        toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",

        file_browser_callback: function (field_name, url, type, win) {
            window.open('/FileManager/UploadImage?name=WelcomeNote', 'ImageBrowser', 'width=800,height=600,scrollbars=yes,status=yes,location=no,resizable=yes,dependent');
        }
    });

});


/**Sets Image Source to the tiny mice window source textbox **/
function SetImageSource(arg) {
    if (arg != null) {
        $('.mce-placeholder').val(arg);
    }
    else {
        $('#mce_57-inp').val("");
    }
}
//------End---------//
