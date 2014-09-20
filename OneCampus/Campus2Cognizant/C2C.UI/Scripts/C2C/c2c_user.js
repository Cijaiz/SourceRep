$(document).ready(function () {

    $("#AccountValidity").datepicker({
        changeYear: true,
        changeMonth: true,
        dateFormat: "mm/dd/yy",
        minDate: new Date()
    });

    //if ($("#AccountValidity").val() == '' || $("#AccountValidity").val() == '01/01/0001') {
    //    $("#AccountValidity").datepicker("setDate", '+0');
    //} 
});



c2c.user.scrollpage = function (userPageCount, name, status) {
    var dataFilter = { Name: name, Status: status };
    $('#div_userlist').ScrollPagination({
        contentPage: c2c.user.UserListUrl,
        totalPageCount: userPageCount,
        heightOffset: 10,
        contentData: dataFilter,
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_userlist').append(data);
        },
        OnEnd: function () {
        }
    });
}

c2c.user.grouplist = function (userId) {
    if ($("#userGrupList-" + userId).attr('title') == "") {
        $.ajax({
            cache: false,
            url: c2c.user.UserGroupUrl,
            data: { userId: userId },
            html: true,
            success: function (response) {
                $("#userGrupList-" + userId).attr('title', response.userGroups);
            },
            error: function (data) {
            }
        });
    }
}