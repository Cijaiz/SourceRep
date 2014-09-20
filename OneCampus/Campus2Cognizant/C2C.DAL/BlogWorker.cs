using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BE = C2C.BusinessEntities.C2CEntities;
using NE = C2C.BusinessEntities.NotificationEntities;
using C2C.Core.Logger;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Blog Worker hits the database to do crud operations & retrieve other blog specific data 
    /// </summary>
    public class BlogWorker
    {
        /// <summary>
        /// Creates an instance for BlogWorker class.
        /// </summary>
        /// <returns>an instance of BlogWorker</returns>
        public static BlogWorker GetInstance()
        {
            return new BlogWorker();
        }

        /// <summary>
        /// Gets the blog with the given id
        /// </summary>
        /// <param name="id">id to get the blog</param>
        /// <returns>Entity defining the BlogPost</returns>
        public BE.BlogPost Get(int id)
        {
            BE.BlogPost post = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var blogPost = dbContext.BlogPosts.Where(p => p.Id == id && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (blogPost != null && blogPost.Id > 0)
                {
                    post = new BE.BlogPost()
                    {
                        Author = blogPost.Author,
                        BlogCategory = blogPost.BlogCategoryId,
                        Description = blogPost.Description,
                        Id = blogPost.Id,
                        IsArchived = blogPost.IsArchived,
                        PostContent = blogPost.PostContent,
                        Status = (Status)blogPost.Status,
                        Notify = blogPost.Notify,
                        Title = blogPost.Title,
                        UpdatedBy = blogPost.UpdatedBy.HasValue ? blogPost.UpdatedBy.Value : blogPost.CreatedBy,
                        UpdatedOn = blogPost.UpdatedOn.HasValue ? blogPost.UpdatedOn.Value : blogPost.CreatedOn,
                        VisibleFrom = blogPost.VisibleFrom,
                        VisibleTill = blogPost.VisibleTill,
                        GroupIDs = blogPost.BlogPostGroups.Select(p => p.GroupId).ToList()
                    };
                }
            }

            return post;
        }

        /// <summary>
        /// Get list of blogs
        /// </summary>
        /// <param name="pager"></param>
        /// <returns>List of blogpost entity</returns>
        public List<BE.BlogPost> GetList(Pager pager)
        {
            List<BE.BlogPost> blogposts = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var posts = dbContext.BlogPosts.Where(p => p.Status != (byte)Status.Deleted).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                if (posts != null && posts.Count() > 0)
                {
                    blogposts = new List<BE.BlogPost>();
                    posts.ForEach(p => blogposts.Add(
                        new BE.BlogPost()
                        {
                            Author = p.Author,
                            BlogCategory = p.BlogCategoryId,
                            Description = p.Description,
                            Id = p.Id,
                            IsArchived = p.IsArchived,
                            PostContent = p.PostContent,
                            Status = (Status)p.Status,
                            Title = p.Title,
                            VisibleFrom = p.VisibleFrom,
                            VisibleTill = p.VisibleTill,
                            UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                            UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn
                            //TODO need to get groups
                        }));
                }
            }
            return blogposts;
        }

        /// <summary>
        /// Updates the blog with the given BlogPost entity
        /// </summary>
        /// <param name="post">Entity defining the BlogPost to be updated</param>
        /// <returns>Response defining the Success or Failure of the operation</returns>
        public ProcessResponse Update(BE.BlogPost post)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var blogPost = dbContext.BlogPosts.Where(p => p.Id == post.Id).FirstOrDefault();

                if (blogPost != null)
                {
                    blogPost.Author = post.Author;
                    blogPost.BlogCategoryId = post.BlogCategory;
                    blogPost.Description = post.Description;
                    blogPost.Title = post.Title;
                    blogPost.PostContent = post.PostContent;
                    blogPost.IsArchived = post.IsArchived;
                    blogPost.VisibleFrom = post.VisibleFrom;
                    blogPost.VisibleTill = post.VisibleTill;
                    blogPost.Status = (byte)post.Status;
                    blogPost.Notify = post.Notify;
                    blogPost.UpdatedBy = post.UpdatedBy;
                    blogPost.UpdatedOn = DateTime.UtcNow;

                    if (post.GroupIDs != null)
                    {
                        //Delete existing groups
                        var blogGroups = dbContext.BlogPostGroups.Where(p => p.BlogPostId == post.Id).ToList();
                        foreach (var groupId in blogGroups)
                        {
                            dbContext.BlogPostGroups.Remove(groupId);
                        }

                        //Update with New Groups
                        foreach (var Id in post.GroupIDs)
                        {
                            var blogPostGroup = new BlogPostGroup
                            {
                                GroupId = Id,
                                BlogPostId = blogPost.Id
                            };
                            dbContext.BlogPostGroups.Add(blogPostGroup);
                        }

                        int count = dbContext.SaveChanges();
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Success,
                            Message = string.Format(Message.UPDATED_COUNT, count)
                        };
                    }
                    else
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Failed,
                            //Object = post,
                            Message = Message.GROUPSELECTION
                        };
                    }
                }
                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.RECORED_NOT_FOUND
                    };
                }
            }

            return response;
        }

        /// <summary>
        /// Creates a new Blog with the BlogPost entity.
        /// </summary>
        /// <param name="post">Entity defining the BlogPost</param>
        /// <returns>Response defining the Success or Failure of the operation</returns>
        public ProcessResponse<BE.BlogPost> Create(BE.BlogPost post)
        {
            ProcessResponse<BE.BlogPost> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var blogPost = new BlogPost()
                {
                    Author = post.Author,
                    BlogCategoryId = post.BlogCategory,
                    Description = post.Description,
                    Title = post.Title,
                    PostContent = post.PostContent,
                    VisibleFrom = post.VisibleFrom,
                    VisibleTill = post.VisibleTill,
                    IsArchived = post.IsArchived,
                    Status = (int)Status.Pending,
                    Notify = post.Notify,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = post.UpdatedBy,
                    UpdatedBy = post.UpdatedBy,
                    UpdatedOn = DateTime.UtcNow
                    //TODO need to add Groups
                };

                blogPost.BlogPostGroups = new List<BlogPostGroup>();
                if (post.GroupIDs != null)
                {
                    post.GroupIDs.ForEach(p => blogPost.BlogPostGroups.Add(
                        new BlogPostGroup()
                        {
                            GroupId = p,
                            BlogPostId = blogPost.Id
                        }));
                    dbContext.BlogPosts.Add(blogPost);

                    if (dbContext.SaveChanges() > 0)
                    {
                        post.Id = blogPost.Id;
                        response = new ProcessResponse<BE.BlogPost>()
                        {
                            Status = ResponseStatus.Success,
                            Object = post,
                            Message = C2C.Core.Constants.C2CWeb.Message.CREATED
                        };
                    }
                    else
                    {
                        response = new ProcessResponse<BE.BlogPost>()
                        {
                            Status = ResponseStatus.Failed,
                            Object = post,
                            Message = Message.FAILED
                        };
                    }
                }
                else
                {
                    response = new ProcessResponse<BE.BlogPost>()
                    {
                        Status = ResponseStatus.Failed,
                        Object = post,
                        Message = Message.GROUPSELECTION
                    };

                }
            }
            return response;
        }

        /// <summary>
        /// Get list of blogs with the given category
        /// </summary>
        /// <param name="blogCategoryId">CategoryId</param>
        /// <param name="pager">Pager parameters defining the size and its count</param>
        /// <returns>List of blogpost entity</returns>
        public List<BE.BlogPost> GetList(int blogCategoryId, int userId, Pager pager)
        {
            List<BE.BlogPost> posts = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                posts = dbContext.BlogPosts.
                        Where(
                            p => p.BlogCategoryId == blogCategoryId &&
                                p.Status == (byte)Status.Active &&
                                p.VisibleFrom <= DateTime.UtcNow &&
                                (p.VisibleTill >= DateTime.UtcNow || p.VisibleTill == null) &&
                                p.BlogPostGroups.Count(q => q.Group.UserGroups.Count(r => r.UserId == userId && r.Status == (byte)Status.Active) > 0) > 0).
                        OrderBy(c => c.Id).Skip(pager.SkipCount).Take(pager.PageSize).
                        Select(s => new BE.BlogPost()
                        {
                            Author = s.Author,
                            Description = s.Description,

                            Id = s.Id,
                            IsArchived = s.IsArchived,
                            PostContent = s.PostContent,

                            Status = (Status)s.Status,
                            Title = s.Title,
                            VisibleFrom = s.VisibleFrom,
                            VisibleTill = s.VisibleTill,

                            UpdatedBy = s.UpdatedBy.HasValue ? s.UpdatedBy.Value : s.CreatedBy,
                            UpdatedOn = s.UpdatedOn.HasValue ? s.UpdatedOn.Value : s.CreatedOn
                            //TODO : if needed in future fill groups
                        }).ToList();
            }
            return posts;
        }

        /// <summary>
        /// Get list of blogPost with the given title and category
        /// </summary>
        /// <param name="pager">Pager parameters defining the size and its count</param>
        /// <param name="blogCategoryId">CategoryId to filter the blogs</param>
        /// <param name="Title">Title to filter the blogs</param>
        /// <returns>List of blogPost entity</returns>
        public List<BE.BlogPost> Get(Pager pager, int blogCategoryId, string Title = null)
        {
            List<BE.BlogPost> blogposts = new List<BE.BlogPost>();
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (Title != null)
                {
                    blogposts = (from blogPosts in dbContext.BlogPosts
                                 where blogPosts.BlogCategoryId == blogCategoryId && blogPosts.Title.Contains(Title)
                                 join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                                 from profile in blogProfile.DefaultIfEmpty()
                                 select new BE.BlogPost()
                                                 {
                                                     Id = blogPosts.Id,
                                                     Status = (Status)blogPosts.Status,
                                                     UpdatedBy = (blogPosts.UpdatedBy.Value != null) ? blogPosts.UpdatedBy.Value : blogPosts.CreatedBy,
                                                     UpdatedOn = (blogPosts.UpdatedOn.Value != null) ? blogPosts.UpdatedOn.Value : blogPosts.CreatedOn,
                                                     Title = blogPosts.Title,
                                                     Author = profile.FirstName
                                                 }).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();

                }
                else
                {

                    blogposts = (from blogPosts in dbContext.BlogPosts
                                 where blogPosts.BlogCategoryId == blogCategoryId
                                 join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                                 from profile in blogProfile.DefaultIfEmpty()
                                 select new BE.BlogPost()
                                                {
                                                    Id = blogPosts.Id,
                                                    Status = (Status)blogPosts.Status,
                                                    UpdatedBy = (blogPosts.UpdatedBy.Value != null) ? blogPosts.UpdatedBy.Value : blogPosts.CreatedBy,
                                                    UpdatedOn = (blogPosts.UpdatedOn.Value != null) ? blogPosts.UpdatedOn.Value : blogPosts.CreatedOn,
                                                    Title = blogPosts.Title,
                                                    Author = profile.FirstName
                                                }).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();

                }

            }
            return blogposts;
        }

        /// <summary>
        /// List the BlogPost that has been published within a week
        /// </summary>
        /// <param name="publishedStartDate"></param>
        /// <param name="publishedEndDate"></param>
        /// <returns></returns>
        public List<NE.BlogPost> GetWeeklyBlogs(DateTime publishedStartDate, DateTime publishedEndDate)
        {
            DayOfWeek weekStart = DayOfWeek.Sunday;
            DayOfWeek weekEnd = DayOfWeek.Saturday;
            DateTime startingDate = DateTime.Today;
            DateTime endingDate = DateTime.Today;


            while (startingDate.DayOfWeek != weekStart)
                startingDate = startingDate.AddDays(-1);

            while (endingDate.DayOfWeek != weekEnd)
                endingDate = endingDate.AddDays(+1);

            List<NE.BlogPost> blogposts = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var posts = dbContext.BlogPosts
                    .Where(p => (p.Status != (byte)Status.Deleted) &&
                        (p.VisibleFrom <= DateTime.UtcNow || p.VisibleFrom == null)
                        && (p.VisibleTill >= DateTime.UtcNow || p.VisibleTill == null)
                        && (p.CreatedOn >= startingDate) && (p.CreatedOn <= endingDate)).ToList();


                if (posts != null && posts.Count() > 0)
                {
                    blogposts = new List<NE.BlogPost>();

                    posts.ForEach(p => blogposts.Add(
                        new NE.BlogPost()
                        {
                            Author = p.Author,
                            Description = p.Description,
                            Id = p.Id,
                            PostContent = p.PostContent,
                            Title = p.Title,
                            VisibleFrom = p.VisibleFrom,
                            VisibleTill = p.VisibleTill,
                            PublishedOn = p.CreatedOn

                            //TODO:Need to add groups when mail has to be sent only to particular group.
                        }));
                }

                return blogposts;
            }
        }

        /// <summary>
        /// Gets List of Blog post with the given categoryId,status and title
        /// </summary>
        /// <param name="pager">Pager parameters defining the size and its count</param>
        /// <param name="blogCategoryId">CategoryId to filter the blogs</param>
        /// <param name="status">status to filter the blogs</param>
        /// <param name="Title">Title to filter the blogs</param>
        /// <returns>st of blogPost entity</returns>
        public List<BE.BlogPost> Get(Pager pager, int blogCategoryId, Status status, string Title = null)
        {
            //int s = (status == null) ? 10 : (int)status;
            int s = (int)status;
            List<BE.BlogPost> blogposts = new List<BE.BlogPost>();
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //List<BlogPost> posts = new List<BlogPost>();
                if (Title != null)
                {

                    blogposts = (from blogPosts in dbContext.BlogPosts
                                 where blogPosts.BlogCategoryId == blogCategoryId && blogPosts.Title.Contains(Title) && blogPosts.Status == s
                                 join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                                 from profile in blogProfile.DefaultIfEmpty()
                                 select new BE.BlogPost()
                                 {
                                     Id = blogPosts.Id,
                                     Status = (Status)blogPosts.Status,
                                     UpdatedBy = (blogPosts.UpdatedBy.Value != null) ? blogPosts.UpdatedBy.Value : blogPosts.CreatedBy,
                                     UpdatedOn = (blogPosts.UpdatedOn.Value != null) ? blogPosts.UpdatedOn.Value : blogPosts.CreatedOn,
                                     Title = blogPosts.Title,
                                     Author = profile.FirstName
                                 }).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();


                }
                else
                {
                    blogposts = (from blogPosts in dbContext.BlogPosts
                                 where blogPosts.BlogCategoryId == blogCategoryId && blogPosts.Status == s
                                 join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                                 from profile in blogProfile.DefaultIfEmpty()
                                 select new BE.BlogPost()
                                 {
                                     Id = blogPosts.Id,
                                     Status = (Status)blogPosts.Status,
                                     UpdatedBy = (blogPosts.UpdatedBy.Value != null) ? blogPosts.UpdatedBy.Value : blogPosts.CreatedBy,
                                     UpdatedOn = (blogPosts.UpdatedOn.Value != null) ? blogPosts.UpdatedOn.Value : blogPosts.CreatedOn,
                                     Title = blogPosts.Title,
                                     Author = profile.FirstName
                                 }).OrderBy(o => o.Id).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
                }
            }
            return blogposts;
        }

        public int GetCount(int blogCategoryId, Status status, string Title = null)
        {
            int count = 0;
            int s = (int)status;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                //List<BlogPost> posts = new List<BlogPost>();
                if (Title != null)
                {

                    count = (from blogPosts in dbContext.BlogPosts
                             where blogPosts.BlogCategoryId == blogCategoryId && blogPosts.Title.Contains(Title) && blogPosts.Status == s
                             join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                             from profile in blogProfile.DefaultIfEmpty()
                             select new BE.BlogPost() { }).Count();
                }
                else
                {
                    count = (from blogPosts in dbContext.BlogPosts
                             where blogPosts.BlogCategoryId == blogCategoryId && blogPosts.Status == s
                             join profile in dbContext.UserProfiles on blogPosts.UpdatedBy equals profile.Id into blogProfile
                             from profile in blogProfile.DefaultIfEmpty()
                             select new BE.BlogPost() { }).Count();
                }
            }

            return count;
        }

        public int GetCount(int blogCategoryId, string Title = null)
        {
            int count = 0;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                if (!string.IsNullOrEmpty(Title))
                {
                    count = dbContext.BlogPosts.Where(p => p.BlogCategoryId == blogCategoryId && p.Title== Title).Count();
                }
                else
                {
                    count = dbContext.BlogPosts.Where(p=>p.BlogCategoryId== blogCategoryId).Count();                    
                }
            }
            return count;
        }
    }
}
