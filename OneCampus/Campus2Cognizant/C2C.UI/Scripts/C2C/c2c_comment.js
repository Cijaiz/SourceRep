
c2c.comment.save = function (contentItemTypeId, contentItemId) {
    var commentDescription = $("#commentText").val().trim();
    if (commentDescription.length == 0)
        alert("Please text in your Comment");
    if (commentDescription.length > 0) {
        $.ajax({
            url: c2c.comment.submitUrl,
            data: { contentTypeId: contentItemTypeId, contentId: contentItemId, newcomment: commentDescription },
            cache: false,
            success: function (response) {
                if (response.UserName.length > 0) {
                    strAppendText = '<div class="comment-notf" id="comment-'+ response.CommentId +'"><img src="' + response.PhotoPath + '" height="32px" width="32px">';
                    strAppendText += '<p class="time">' + response.UserName + ' said a while ago</p><p class="commentDesc">' + commentDescription + '</p>';
                    if (response.CanDelete) {
                        strAppendText += '<a href="#" onclick = "c2c.comment.deleteComment(' + response.CommentId + ')">Delete</a></div>';
                    }
                    if (response.CommentCount == 1) {
                        $("#commentList").html(strAppendText);
                        $("#commentList").mCustomScrollbar();
                    }
                    else {

                        $("#commentList .mCSB_container").append(strAppendText); //append new content inside .mCSB_container
                        $("#commentList").mCustomScrollbar("update");
                    }
                    $("#commCount").text(response.CommentCount);
                    $("#commentsCount").text(response.CommentCount);
                    $("#commentText").val('');
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                if (xhr.status == 401) {
                    alert("You are not authorized to comment.");
                }
            }
        });
    }
}


c2c.comment.deleteComment = function (commentId) {
    var divId = "#comment-" + commentId;

    $.ajax({
        url: c2c.comment.deleteUrl,
        data: { id: commentId },
        cache: false,
        success: function (response) {
            $(divId).remove();
            $("#commCount").text(response.CommentCount);
            $("#commentsCount").text(response.CommentCount);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            if (xhr.status == 401) {
                alert("You are not authorized to delete.");
            }
        }
    });
}


c2c.comment.getList = function (contentItemTypeId, contentItemId) {
    var count = 1;
    $.ajax({
        url: c2c.comment.Url,
        cache: false,
        data: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        cache: false,
        success: function (response) {
            $("#commentsList").html(response);
            $("#commentList").mCustomScrollbar({
                scrollButtons: {
                    enable: true
                },
                callbacks: {
                    onTotalScroll: function () {
                        count = count + 1;
                        //debugger
                        if (count <= commentPageCount)
                            c2c.comment.sharedCommentScroll(count, contentItemTypeId, contentItemId);
                    }
                },

                theme: "dark-thick"

            });

            c2c.comment.sharedCommentScroll(1, contentItemTypeId, contentItemId);
            //c2c.comment.Scrollbar();
        },
        error: function (data) {
        }
    });
}

c2c.comment.sharedCommentScrollpage = function (commentCount, contentItemTypeId, contentItemId) {

    $('#commentList').ScrollPagination({
        contentPage: c2c.comment.listUrl,
        totalPageCount: commentCount,
        heightOffset: 10,
        contentData: { contentTypeId: contentItemTypeId, contentId: contentItemId },
        pagerFieldName: "page",
        OnLoad: function (data) {

            $("#commentList .mCSB_container").append(data); //append new content inside .mCSB_container
            $("#commentList").mCustomScrollbar("update"); //update scrollbar according to newly appended content
            $("#commentList").mCustomScrollbar("scrollTo", "h2:last", { scrollInertia: 2500, scrollEasing: "easeInOutQuad" }); //scroll to appended content
        },
        OnEnd: function () {
        }
    });
}

$(document).ready(function () {
    $(".comment-close").live("click", function () {
        if ($(".connect-content").hasClass("move-left")) {
            $(".connect-content").removeClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment.png");
            $(".comment-count").css('color', '#133d74');
            $("#like-box").css('margin-left', '795px');
        } else {
            $(".connect-content").addClass('move-left');
            $("img.comment-box").attr("src", c2c.path.themesImageFolder + "comment-hide.png");
            $(".comment-count").css('color', '#b5b5b5');
        }
        $(".comment-toggle").toggle();
    });
});


c2c.comment.sharedCommentScroll = function (commentCount, contentItemTypeId, contentItemId) {
    $.ajax({
        url: c2c.comment.listUrl,
        cache: false,
        data: { page: commentCount, contentTypeId: contentItemTypeId, contentId: contentItemId },
        cache: false,
        success: function (data) {

            $("#commentList .mCSB_container").append(data); //append new content inside .mCSB_container
            $("#commentList").mCustomScrollbar("update"); //update scrollbar according to newly appended content
            $("#commentList").mCustomScrollbar("scrollTo", "h2:last", { scrollInertia: 2500, scrollEasing: "easeInOutQuad" }); //scroll to appended content
        },
        error: function (data) {
        }
    });
}

   
   