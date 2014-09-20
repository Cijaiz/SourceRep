
c2c.like.save = function (userActionType, contentItemTypeId, contentItemId) {    
    var _divId = "Like-" + contentItemTypeId + "-" + contentItemId;   
    var likeUrl;
    if (userActionType == "like") {       
        likeUrl = c2c.likeUrl;
    }
    else {
        if (userActionType == "unlike") {           
            likeUrl = c2c.unlikeUrl;
        }
    }
   
    $.ajax({
        url: likeUrl,
        cache:false,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        success: function (data) {
            var totalCount = data.LikedUsersCount;
            if(totalCount > 0)
            {              
                c2c.like.showLikedUsers(contentItemTypeId, contentItemId);
                c2c.like.likescrollpage()
            }           
            if (data.IsLiked) {
                strLikeText = '<img src="/Content/Themes/base/images/like.png" id="likimg" title="UnLike"onclick="c2c.like.save(' + "'unlike'," + contentItemTypeId + "," + contentItemId + ')"/>'
                if (totalCount > 0) {
                    strLikeText += '<span class="count " id="clike-all" title="Number of Users Liked">&nbsp' + totalCount + '</span>'
                }
                else {
                    strLikeText += '<span class="count " title="Number of Users Liked">&nbsp' + totalCount + '</span>'
                }
                $('#' + _divId).html(''); $('#' + _divId).html(strLikeText);
                $(".like-bx img").addClass('like-background');
            }
            else {
                strLikeText = ' <img src="/Content/Themes/base/images/like.png" id="likimg" title="Like" onclick="c2c.like.save(' + "'like'," + contentItemTypeId + "," + contentItemId + ')"/>'
                if (totalCount > 0) {
                    strLikeText += '<span class="count " id="clike-all" title="Number of Users Liked">&nbsp' + totalCount + '</span>'
                }
                else {
                    strLikeText += '<span class="count " title="Number of Users Liked">&nbsp' + totalCount + '</span>'
                }
                $('#' + _divId).html(''); $('#' + _divId).html(strLikeText);
                $(".like-bx img").removeClass('like-background');
            }
        },     
        error: function (xhr, ajaxOptions, thrownError) {
            if (xhr.status == 401) {
                alert("You are not authorized to like/Unlike");
            }
        },
        async: false
    });
}

c2c.like.showLikedUsers = function (contentItemTypeId, contentItemId) {   
    var getActionUrl;
    var divId = "Like-" + contentItemId;   
    $.ajax({
        url: c2c.like.showLikedUsersUrl,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        cache: false,
        success: function (response) {
            $('#show-all-content').html(response);
            c2c.like.likescrollpage(likePageCount, contentItemTypeId, contentItemId);
        },
        error: function (data) {
        }
    });
}

c2c.like.likescrollpage = function (LikedUserPagesCount, contentItemTypeId, contentItemId) {
    $('#div_LikedUsers').ScrollPagination({
        contentPage: c2c.like.usersLikedUrl,
        totalPageCount: LikedUserPagesCount,
        heightOffset: 10,
        contentData: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_LikedUsers').append(data);
        },
        OnEnd: function () {
            //alert('all content loaded');
        }
    });
}