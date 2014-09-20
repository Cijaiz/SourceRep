
$(document).ready(function () {
    $("#VisibleFrom").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });

    $("#VisibleTill").datepicker({
        changeMonth: true,
        changeYear: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });

    if ($("#VisibleFrom").val() == '' || $("#VisibleFrom").val() == '01/01/0001 12:00:00 AM' || $("#VisibleFrom").val() == '1/1/0001 12:00:00 AM') {
        $("#VisibleFrom").datepicker("setDate", '+0');
    }
    if ($("#VisibleTill").val() == '' || $("#VisibleTill").val() == '01/01/0001 12:00:00 AM' || $("#VisibleTill").val() == '1/1/0001 12:00:00 AM') {
        $("#VisibleTill").datepicker("setDate", '+0');
    }

    $('.addItem').click(function () {
        c2c.poll.addtextbox(counter);
        counter++;
    });
});

c2c.poll.removetextbox = function (counter) {
    $("#PollAnswer-" + counter).hide();
    $("input[name='[" + counter + "].IsDeleted']:hidden").val(true);
    return false;
}

c2c.poll.addtextbox = function (counter) {
    var txtAns = "<div id='PollAnswer-" + counter + "'><input type='text' name='[" + counter + "].Answer' maxlength='50' id='textbox" + counter + "' /> <input type='hidden' value='false' name='[" + counter + "].IsDeleted' id='IsDeleted" + counter + "' /> <input type='button' value='X' id='remove" + counter + "' onclick='return c2c.poll.removetextbox(" + counter + ");' /></div>";
    $('#PollAnswers').append(txtAns);
    var txtBox = document.getElementById("textbox" + counter + "").focus();
    return false;
}

