
$(document).ready(function () {
    $("#selectall").click(function () {
        $('.case').attr('checked', this.checked);
    });

    $(".case").click(function () {
        if ($(".case").length == $(".case:checked").length) {
            $("#selectall").attr("checked", "checked");
        } else {
            $("#selectall").removeAttr("checked");
        }

    });
});

c2c.group.scrollpage = function (userPageCount, groupId, searchText) {
    var dataFilter = { groupId: groupId, searchText: searchText };
    $('#div_membersList').ScrollPagination({
        contentPage: c2c.group.NonGroupMembersUrl,
        totalPageCount: userPageCount,
        heightOffset: 10,
        contentData: dataFilter,
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_membersList').append(data);
        },
        OnEnd: function () {
        }
    });
}

c2c.group.groupmemberlistScroll = function (userPageCount, groupId, memberStatus) {
    var dataFilter = { groupId: groupId, memberStatus: memberStatus };
    $('#div_userGroupMembers').ScrollPagination({
        contentPage: c2c.group.UserGroupMembersUrl,
        totalPageCount: userPageCount,
        heightOffset: 10,
        contentData: dataFilter,
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_userGroupMembers').append(data);
        },
        OnEnd: function () {
        }
    });
}

c2c.group.grouplistScroll = function (pageCount, searchText) {
    $('#div_groupList').ScrollPagination({
        contentPage: c2c.group.groupListUrl,
        totalPageCount: pageCount,
        heightOffset: 10,
        contentData: { searchText: searchText },
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_groupList').append(data);
        },
        OnEnd: function () {
        }
    });
}