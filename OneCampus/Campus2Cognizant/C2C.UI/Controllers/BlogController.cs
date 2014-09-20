using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.BusinessEntities.NotificationEntities;
using C2C.BusinessLogic;
using C2C.Core.Constants.C2CWeb;
using C2C.Core.Constants.Engine;
using C2C.Core.Helper;
using C2C.UI.Filters;
using C2C.UI.Publisher;
using C2C.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace C2C.UI.Controllers
{
    /// <summary>
    ///  Blog Controller calls the business layer to perform Blog specific operations
    /// </summary>
    [Authorize]
    public class BlogController : BaseController
    {
        /// <summary>
        /// Lists the Categories in BlogTile Index Page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Tile()
        {
            BlogViewModel model = new BlogViewModel();
            model = BuildBlogViewModel();            
            return PartialView("_BlogTileIndex", model);
        }

        /// <summary>
        /// Create the Blog Post
        /// </summary>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Create()
        {
            if (User.HasPermission(ApplicationPermission.ManageBlogs))
            {
                BlogViewModel blogViewModel = new BlogViewModel();
                blogViewModel = BuildBlogViewModel(); // Building the BlogViewModel 
                blogViewModel.SelectedGroupIds = new List<int>();
               
                return View(blogViewModel);
            }
            else
            {
                //CustomLogger.WriteLine(Constants.MSG_NOT_AUTHORIZED);
                throw new UnauthorizedAccessException(Message.MSG_NOT_AUTHORIZED);
            }
        }

        /// <summary>
        /// Edit the Blog Post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Edit(int id)
        {
            var post = BlogManager.Get(id);          
            BlogViewModel blogViewModel = new BlogViewModel();

            blogViewModel = BuildBlogViewModel();           
            blogViewModel.BlogPost = post;
            blogViewModel.SelectedGroupIds = post.GroupIDs;
            blogViewModel.Category = post.BlogCategory.ToString();

            return View(blogViewModel);
        }

        /// <summary>
        /// Delete the Blog Post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Delete(int id)
        {
            BlogManager.Delete(id);
            return RedirectToAction("List");
        }

        /// <summary>
        /// Display the Particular Blog Post in detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Article(int id)
        {
            var post = BlogManager.Get(id);

            bool isLiked = LikeManager.IsLiked(User.UserId, (short)Module.Blog, id);
            int likeCount = LikeManager.GetLikedUsersCount((short)Module.Blog, id);

            int shareCount = ShareManager.GetSharedUsersCount((short)Module.Blog, id);
            int commentCount = CommentManager.GetCommentsCountForContent((short)Module.Blog, id);

            BlogArticleViewModel blogArticleViewModel = new BlogArticleViewModel();
            blogArticleViewModel.Post = post;
            blogArticleViewModel.IsLiked = isLiked;
            blogArticleViewModel.LikeCount = likeCount;
            blogArticleViewModel.shareCount = shareCount;
            blogArticleViewModel.commentCount = commentCount;

            return View(blogArticleViewModel);
        }

        /// <summary>
        /// Publish the Blog Post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Activate(int id)
        {
            ProcessResponse response = BlogManager.Activate(id);
        
            if (response.Status == ResponseStatus.Success)
            {
                BusinessEntities.C2CEntities.BlogPost blog = BlogManager.Get(id);
                if (blog.Notify)
                {
                    NotificationContent notificationContent = new NotificationContent()
                    {
                        Id = id,
                        ContentType = Module.Blog,
                        ContentTitle = WebUtility.HtmlEncode(blog.Title),
                        Description = WebUtility.HtmlEncode("New Blog post '" + blog.Title + "' is published."),
                        //URL = Services.ContentManager.GetItemMetadata(post.ContentItem).DisplayRouteValues.ToString(),
                        URL = GetUrl(id, "Blog", "Article"),
                        ValidFrom = blog.VisibleFrom,
                        ValidTo = blog.VisibleTill != null ? blog.VisibleTill.Value : DateTime.UtcNow.AddMonths(1).Date,
                        Groups = blog.GroupIDs,
                        SharedBy = User.UserId,
                        Subject = "New BlogPost is published"
                    };

                    PublisherEvents publisherEvents = new PublisherEvents()
                    {
                        EventId = id.ToString(),
                        EventCode = EventCodes.BLOG_POST,
                        NotificationContent = SerializationHelper.JsonSerialize<NotificationContent>(notificationContent)
                    };

                    NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);
                }
            }
            return RedirectToAction("List");
        }

        /// <summary>
        /// List the Categories in Blog Index Page as Sub Menu
        /// </summary>
        /// <returns></returns>
        public ActionResult SubMenuCategory()
        {
            BlogViewModel blogViewModel = new BlogViewModel();
            blogViewModel = BuildBlogViewModel();
            return PartialView("_BlogCategorySubMenu", blogViewModel);
        }

        /// <summary>
        /// Unpublish the Blog Post
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Deactivate(int id)
        {
            BlogManager.Deactivate(id);
            return RedirectToAction("List");
        }
     
        /// <summary>
        /// Method for building the BlogViewModel
        /// </summary>
        /// <returns></returns>
        private BlogViewModel BuildBlogViewModel(BlogViewModel blogViewModel = null)
        {
            if(blogViewModel == null)
            {
                blogViewModel = new BlogViewModel();
            }
            MetaMaster metamaster = MetaDataManager.GetMetaValueList((int)MetaMasterKey.BlogCategory);
            List<BlogCategory> blogCategoryList = new List<BlogCategory>();
            List<Group> groupList = new List<Group>();
            List<Group> group = GroupManager.GetList();
            BlogCategory blogCategory;

            foreach (var value in metamaster.MetaValues)
            {
                blogCategory = new BlogCategory();
                blogCategory.CategoryName = value.Value;
                blogCategory.CategoryId = value.Id;
                blogCategoryList.Add(blogCategory);
            }
                    
            blogViewModel.CategoryList = blogCategoryList.Select(v => new SelectListItem
            {
                Text = v.CategoryName,
                Value = v.CategoryId.ToString()
            }).ToList();

         
            foreach (var item in group)
            {
                Group groups = new Group();
                groups.Id = item.Id;
                groups.Title = item.Title;
                groupList.Add(groups);
            }

            blogViewModel.GroupList = groupList;

            return blogViewModel;
        }

        /// <summary>
        /// Listing the BlogPost Based on their Categories
        /// </summary>
        /// <param name="blogCategoryId"></param>
        /// <returns></returns>
        public ActionResult Index(int blogCategoryId = 0)
        {
            List<C2C.BusinessEntities.C2CEntities.BlogPost> list = new List<BusinessEntities.C2CEntities.BlogPost>();
            int userId = User.UserId;
            if (blogCategoryId == 0)
            {
                BlogViewModel blogViewModel = BuildBlogViewModel();
                blogCategoryId = blogViewModel != null && blogViewModel.CategoryList != null ? Convert.ToInt32(blogViewModel.CategoryList.FirstOrDefault().Value) : 0;
            }

            if (blogCategoryId > 0)
            {
                list = BlogManager.GetList(blogCategoryId, userId, new Pager());
            }

            return View(list);
        }

        /// <summary>
        /// Save the updated blog post in DB
        /// </summary>
        /// <param name="blogViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Edit(BlogViewModel blogViewModel)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            int validDate = DateTime.Compare(blogViewModel.BlogPost.VisibleFrom, DateTime.UtcNow.Date);

            if (validDate < 0)
                ModelState.AddModelError("VisibleFrom", "VisibleFrom Date must be Greater than SystemDate");

            blogViewModel.BlogPost.GroupIDs = blogViewModel.SelectedGroupIds;
            blogViewModel.BlogPost.UpdatedBy = User.UserId;
            blogViewModel.BlogPost.BlogCategory = Convert.ToInt32(blogViewModel.Category);

            ProcessResponse response = BlogManager.Update(blogViewModel.BlogPost);
            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
                return RedirectToAction("List", "Blog");
            }
            else
            {
                blogViewModel.SelectedGroupIds = new List<int>();
                responseMessage.AddError(response.Message);
            }
            blogViewModel.ResponseMessage = responseMessage;           
            blogViewModel = BuildBlogViewModel(blogViewModel);

            return View(blogViewModel);
        }

        /// <summary>
        /// Listing all the Blog Post using Filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ActionResult List(BlogListFilter filter = null)
        {
            Pager pager = new Pager();
            int postCount = 0;
            BlogListViewModel blogPostListViewModel = new BlogListViewModel();
            List<BlogCategory> blogCategoryList = new List<BlogCategory>();

            MetaMaster metamaster = MetaDataManager.GetMetaValueList((int)MetaMasterKey.BlogCategory);
            BlogListFilter blogListFilter = new BlogListFilter();
            BlogCategory blogCategory;

            List<C2C.BusinessEntities.C2CEntities.BlogPost> blogPostList = new List<C2C.BusinessEntities.C2CEntities.BlogPost>();
            if (string.IsNullOrEmpty(filter.Category))
            {
               filter.Category = metamaster.MetaValues.FirstOrDefault().Id.ToString();
            }
            if (string.IsNullOrEmpty(filter.Status))
            {
                filter.Status = "All";
            }
            if (filter.Status != "All")
            {
                Status status = (Status)Enum.Parse(typeof(Status), filter.Status, true);
                postCount = BlogManager.GetCount(Convert.ToInt32(filter.Category), filter.SearchTitle, status);
            }
            else
            {
                postCount = BlogManager.GetCount(Convert.ToInt32(filter.Category), filter.SearchTitle);
            }

            blogPostListViewModel.PageCount = (int)Math.Ceiling((double)postCount / (double)pager.PageSize);
            blogPostListViewModel.BlogPostList = blogPostList;
            blogListFilter.Category = filter.Category;
            blogListFilter.Status = filter.Status;
            //blogPostListViewModel.Filter.Category = filter.Category;
            //blogPostListViewModel.Filter.Status = filter.Status;

            blogListFilter.StatusList = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString(),

            }).ToList();
            blogListFilter.StatusList.Add(new SelectListItem { Text = "All", Selected = true, });

            foreach (var value in metamaster.MetaValues)
            {
                blogCategory = new BlogCategory();
                blogCategory.CategoryName = value.Value;
                blogCategory.CategoryId = value.Id;
                blogCategoryList.Add(blogCategory);
            }

            blogListFilter.CategoryList = blogCategoryList.Select(v => new SelectListItem
            {
                Text = v.CategoryName,
                Value = v.CategoryId.ToString()
            }).ToList();

            blogPostListViewModel.Filter = blogListFilter;

            return View(blogPostListViewModel);
        }

        public ActionResult GetBlogPostList(string category, string postStatus, string searchText,int page) 
        {
            BlogListViewModel blogPostListViewModel = new BlogListViewModel();
            Pager pager = new Pager { PageNo = page };
            MetaMaster metamaster = MetaDataManager.GetMetaValueList((int)MetaMasterKey.BlogCategory);
            List<C2C.BusinessEntities.C2CEntities.BlogPost> blogPostList = new List<C2C.BusinessEntities.C2CEntities.BlogPost>();
            if (string.IsNullOrEmpty(postStatus))
            {
                postStatus = "All";
            }

            if (string.IsNullOrEmpty(category))
            {
                category = metamaster.MetaValues.FirstOrDefault().Id.ToString();
            }

            if (postStatus != "All")
            {
                Status status = (Status)Enum.Parse(typeof(Status), postStatus, true);
                blogPostList = BlogManager.Get(pager,Convert.ToInt32(category), searchText,status);
            }
            else
            {
                blogPostList = BlogManager.Get(pager, Convert.ToInt32(category), searchText);
            }
            blogPostListViewModel.BlogPostList = blogPostList;

            return View("_BlogPostList", blogPostListViewModel);
        }

        /// <summary>
        /// Save the created Blog Post in DB
        /// </summary>
        /// <param name="blogViewModel"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        [CheckPermission(ApplicationPermission.ManageBlogs)]
        public ActionResult Create(BlogViewModel blogViewModel)
        {
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            int validDate = DateTime.Compare(blogViewModel.BlogPost.VisibleFrom, DateTime.UtcNow.Date);

            if (validDate < 0)
                ModelState.AddModelError("VisibleFrom", "VisibleFrom must be Greaterthan SystemDate");

            if (ModelState.IsValid)
            {
                blogViewModel.BlogPost.BlogCategory = Convert.ToInt32(blogViewModel.Category);
                blogViewModel.BlogPost.GroupIDs = blogViewModel.SelectedGroupIds;
                blogViewModel.BlogPost.UpdatedBy = User.UserId;

                var response = BlogManager.Create(blogViewModel.BlogPost);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);                 
                    return RedirectToAction("List", "Blog");
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
            }
                blogViewModel.SelectedGroupIds = new List<int>();
                blogViewModel.ResponseMessage = responseMessage;

                blogViewModel = BuildBlogViewModel(blogViewModel);  

                return View(blogViewModel);
        }

        /// <summary>
        /// Gets the Url of the given blog
        /// </summary>
        /// <param name="itemId">BlogId</param>
        /// <param name="controller">Controller Name</param>
        /// <param name="action">Action Name</param>
        /// <returns>String</returns>
        private string GetUrl(int itemId, string controller, string action)
        {
            string domainUrl = string.Empty;
            string itemUrl = string.Empty;
            Uri uri = Request.Url;
            domainUrl = String.Format("{0}{1}{2}", (Request.IsSecureConnection) ? "https://" : "http://",
                                              uri.Host, (uri.IsDefaultPort) ? "" : String.Format(":{0}", uri.Port));
            itemUrl = string.Format("{0}/{1}/{2}/{3}/", domainUrl,
                controller, action, itemId.ToString());
            return itemUrl;
        }

    }

}

