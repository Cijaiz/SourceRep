

jQuery(function ($) {
    // Load dialog on page load
    //$('#basic-modal-content').modal();

    // Load dialog on click
    $('#basic-modal .basic').click(function (e) {
        $('#basic-modal-content').modal();

        return false;
    });

    //$('#clike-all').click(function(){
    //			//alert("clicked");
    //			$('#show-all-content').modal();

    //			return false;
    //});

    //for showing liked users list pop up.
    $("#clike-all").live("click", function (ev) {
        ev.preventDefault();
        if ($.trim(this.innerText) != "0") {
            $('#show-all-content').modal();
        }
        return false;
    });


    $('#share-all').live("click", function (ev) {
        ev.preventDefault();
        if ($.trim(this.innerText) != "0") {
            $('#share-people').modal();
        }
        return false;
    });

    $('#submitQuiz').click(function () {
        //alert("clicked");
        $('#alert').modal();

        return false;
    });



});