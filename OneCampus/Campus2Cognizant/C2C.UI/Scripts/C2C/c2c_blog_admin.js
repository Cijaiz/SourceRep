//Date picker for Blog Post//
$(document).ready(function () {


    $("#BlogPost_VisibleFrom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });
    $("#BlogPost_VisibleTill").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });

    if ($("#BlogPost_VisibleFrom").val() == '' || $("#BlogPost_VisibleFrom").val() == '01/01/0001 12:00:00 AM' || $("#BlogPost_VisibleFrom").val() == '1/1/0001 12:00:00 AM') {
        $("#BlogPost_VisibleFrom").datepicker("setDate", '+0');
    }
    if ($("#BlogPost_VisibleTill").val() == '' || $("#BlogPost_VisibleTill").val() == '01/01/0001 12:00:00 AM' || $("#BlogPost_VisibleTill").val() == '1/1/0001 12:00:00 AM') {
        $("#BlogPost_VisibleTill").datepicker("setDate", '+0');
    }

    tinymce.init({
        selector: "textarea",
        plugins: [
        "advlist autolink lists link image charmap print preview anchor",
        "searchreplace visualblocks code fullscreen",
        "insertdatetime media table contextmenu paste"
        ],
        toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image",

        file_browser_callback: function (field_name, url, type, win) {
            window.open('/FileManager/UploadImage?name=BlogPost', 'ImageBrowser', 'width=800,height=600,scrollbars=yes,status=yes,location=no,resizable=yes,dependent');
        },
        resize: false
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

