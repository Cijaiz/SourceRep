
c2c.shareContent.showpopup = function (contentItemTypeId, contentItemId)
{
    $.ajax({
        cache: false,
        url: c2c.shareContent.Url,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId, contentTitle: c2c.shareContent.PopUpTitle },
        success: function (response) {
            $('#basic-modal-content').html(response);
            c2c.shareContent.scrollpage(shareUserCount);
        },
        error: function (data) {
        },
        async: false
    });
}

c2c.shareContent.scrollpage = function (shareUserCountPages) {
    $('#div_userlist').ScrollPagination({
        contentPage: c2c.shareContent.userstoshareUrl,
        totalPageCount: shareUserCountPages,
        heightOffset: 10,
        contentData: { totalPages: shareUserCountPages },
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_userlist').append(data);
        },
        OnEnd: function () {
            //alert('all content loaded');
        }
    });
}

c2c.shareContent.save = function (contentItemTypeId, contentItemId)
{
    if ($('.friend-info-share').hasClass('add-background')) {
        var values = $('.add-background .sharecheck').map(function () {
            return $(this).val();
        }).get().join(',');
        $('#hid_sharedto').val(values);
    }
    var sharedTo = $("#hid_sharedto").val();
    var commentText = $("#sharecomment").val();

    $.ajax({
        cache: false,
        url: c2c.shareContent.saveUrl,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId, sharedTo: sharedTo, description: commentText },

        success: function (response) {
            if (response.ShareContent == false) {
                alert("You don't have permission to share this content!");
                $.modal.close();
            }
            else {
                if (response.Status == true) {
                    $('#share-all').html(response.SharedUsersCount);
                    c2c.shareContent.showSharedUsers(contentItemTypeId, contentItemId);
                    alert(response.Message);
                    $.modal.close();
                }
                else if (response.Status == false) {
                    alert(response.Message);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        if (xhr.status == 401) {
            alert("You are not authorized to share this content.");
        }
    },
    });
}

c2c.shareContent.showSharedUsers = function (contentItemTypeId, contentItemId) {
    
    $.ajax({
        url: c2c.shareContent.sharedusersUrl,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        cache: false,
        success: function (response) {
            $('#share-people').html(response);
            c2c.shareContent.sharedUserScrollpage(shareUserCount, contentItemTypeId, contentItemId);
        },
        error: function (data) {
        }
    });
}

c2c.shareContent.sharedUserScrollpage = function (sharedUserPagesCount, contentItemTypeId, contentItemId) {
    $('#div_SharedUsers').ScrollPagination({
        contentPage: c2c.shareContent.userssharedUrl,
        totalPageCount: sharedUserPagesCount,
        heightOffset: 10,
        contentData: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_SharedUsers').append(data);
        },
        OnEnd: function () {
            //alert('all content loaded');
        }
    });
}