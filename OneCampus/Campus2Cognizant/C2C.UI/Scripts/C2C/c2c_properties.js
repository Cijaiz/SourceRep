var c2c = c2c || {};

/**
 * c2c HTTP Request properties
 */
c2c.request = {};
c2c.request.typePost = "POST";
c2c.request.typeGet = "GET";
c2c.request.typePut = "PUT";
c2c.request.typeDelete = "DELETE";
c2c.request.dataTypeJson = "json";
c2c.request.dataTypeApplicationJson = "application/json";
c2c.request.timeout = 60000;

/**
 * c2c path
 */
c2c.path = {};
c2c.path.themesImageFolder = "/Content/themes/base/images/";

/**
 * c2c like
 */
c2c.like = {};
c2c.likeUrl = "/Like/Like";
c2c.unlikeUrl = "/Like/UnLike";
c2c.like.showLikedUsers = {};
c2c.like.showLikedUsersUrl = "/Like/LikedUsers";
c2c.like.usersLikedUrl = "/Like/LikedUserList";

/**
 * c2c share
 */
c2c.shareContent = {};
c2c.shareContent.Url = "/Share/ShareContent";
c2c.shareContent.saveUrl = "/Share/SubmitContent";
c2c.shareContent.sharedusersUrl = "/Share/SharedUsers";
c2c.shareContent.userstoshareUrl = "/Share/UserList";
c2c.shareContent.userssharedUrl = "/Share/SharedUsersList";
c2c.shareContent.PopUpTitle = "";

/**
 * c2c Welcome Note
 */
c2c.welcomeNote = {};
c2c.welcomeNote.Url = "/WelcomeNote/GetWelcomeNoteToDisplay?sourceID=";
c2c.welcomeNote.SourceLogin = 1;
c2c.welcomeNote.SourceTile = 2;

c2c.poll = {};
c2c.poll.submitUrl = "";

c2c.comment = {};
c2c.comment.submitUrl = "/Comment/SaveComment";
c2c.comment.listUrl = "/Comment/ListComments";
c2c.comment.deleteUrl = "/Comment/Delete";
c2c.comment.Url = "/Comment/Comments";
/**
 * c2c Notification Corner
 */
c2c.notificationcorner = {};
c2c.notificationcorner.GetUnReadFeedsUrl = "/NotificationCorner/GetUnReadFeeds";
c2c.notificationcorner.LoadFeedCountUrl = "/NotificationCorner/GetFeedCount";
c2c.notificationcorner.MarkAsReadFeedsUrl = "/NotificationCorner/MarkAsRead";
c2c.notificationcorner.RefreshHtml = "<a href='#' onclick='c2c.notificationcorner.loadfeedcount(); c2c.notificationcorner.getunreadfeeds();'><img height='32px' width='32px' src='/Content/themes/base/images/appbar.refresh.png' /></a>";
/**
* c2c SiteSettings
*/

c2c.siteSettings = {};
c2c.siteSettings.cacheUrl = "/Account/CacheSiteSettings";
c2c.poll.chartUrl = "/poll/Chart/";
c2c.poll.voteUrl = "/poll/Vote/";
c2c.notificationcorner.MarkAsReadFeedsUrl="/NotificationCorner/MarkAsRead";

/**
 * c2c User
 */

c2c.user = {};
c2c.user.UserListUrl = "/User/List";
c2c.user.UserGroupUrl = "/User/UserGroups";

/**
 * c2c Group
 */

c2c.group = {};
c2c.group.NonGroupMembersUrl = "/UserGroup/GetNonGroupMembers";
c2c.group.UserGroupMembersUrl = "/UserGroup/GetUserGroupMembers";
c2c.group.groupListUrl = "/Group/GroupList";

/**
* c2c Blog Post
*/
c2c.blogpost = {};
c2c.blogpost.postlistUrl = "/Blog/GetBlogPostList";

/**
* c2c Dashboard
*/
c2c.dashboard = {};
c2c.dashboard.activeUserStatUrl = "/Dashboard/ActiveUserStat";
c2c.dashboard.userBrowserStatUrl = "/Dashboard/UserBrowserStat";
c2c.dashboard.widgetErrorHtml = "Unable to load information, Please refresh after sometime.";