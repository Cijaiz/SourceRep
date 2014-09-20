
c2c.blogpost.postscrollpage = function (category, title, status, postCount) {
    $('#div_blogpost').ScrollPagination({
        contentPage: c2c.blogpost.postlistUrl,
        totalPageCount: postCount,
        heightOffset: 10,
        contentData: { category: category, postStatus: status, searchText: title },
        pagerFieldName: "page",
        OnLoad: function (data) {
            $('#div_blogpost').append(data);
        },
        OnEnd: function () {
            //alert('all content loaded');
        }
    });
}
