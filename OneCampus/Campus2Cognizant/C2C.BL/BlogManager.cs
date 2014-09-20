using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using System;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;
using NE = C2C.BusinessEntities.NotificationEntities;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// BlogManager calls the blog worker to do blog specific operations
    /// </summary>
    public static class BlogManager
    {
        /// <summary>
        /// Delete the BlogPost by calling the Data Access Layer
        /// </summary>
        /// <param name="id"></param>
        public static void Delete(int id)
        {
            BlogPost blogPost = DAL.BlogWorker.GetInstance().Get(id);
            blogPost.Status = Status.Deleted;
            DAL.BlogWorker.GetInstance().Update(blogPost);
        }

        /// <summary>
        /// Get the particular BlogPost entries by calling the Data Access Layer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static BlogPost Get(int id)
        {
            BlogPost post = DAL.BlogWorker.GetInstance().Get(id);

            return post;
        }

        /// <summary>
        /// Publish the BlogPost by calling the Data Access Layer
        /// </summary>
        /// <param name="id"></param>
        public static ProcessResponse Activate(int id)
        {
            BlogPost blogPost = DAL.BlogWorker.GetInstance().Get(id);
            blogPost.Status = Status.Active;
            ProcessResponse response = DAL.BlogWorker.GetInstance().Update(blogPost);
            return response;
        }

        /// <summary>
        /// UnPublish the BlogPost by calling the Data Access Layer
        /// </summary>
        /// <param name="id"></param>
        public static void Deactivate(int id)
        {
            BlogPost blogPost = DAL.BlogWorker.GetInstance().Get(id);
            blogPost.Status = Status.InActive;
            DAL.BlogWorker.GetInstance().Update(blogPost);
        }

        /// <summary>
        /// Lists all the BlogPost by calling Data Access Layer
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static List<BlogPost> GetList(Pager pager)
        {
            List<BlogPost> posts = DAL.BlogWorker.GetInstance().GetList(pager);

            return posts;
        }

        /// <summary>
        /// Updates the BlogPost by calling the Data Access Layer
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static ProcessResponse Update(BlogPost post)
        {
            ProcessResponse responce = null;
            responce = DAL.BlogWorker.GetInstance().Update(post);

            return responce;
        }

        /// <summary>
        /// Creates a BlogPost by calling the Data Access Layer
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static ProcessResponse<BlogPost> Create(BlogPost post)
        {
            ProcessResponse<BlogPost> response = null;
            response = DAL.BlogWorker.GetInstance().Create(post);

            return response;
        }

        public static int GetCount(int blogCategoryId, string Title)
        {
            return DAL.BlogWorker.GetInstance().GetCount(blogCategoryId, Title);
        }

        public static int GetCount(int blogCategoryId, string Title, Status status)
        {
            return DAL.BlogWorker.GetInstance().GetCount(blogCategoryId, status, Title);
        }

        /// <summary>
        /// Lists all the BlogPost 
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="blogCategoryId"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static List<BlogPost> Get(Pager pager, int blogCategoryId, string Title)
        {
            List<BlogPost> posts = DAL.BlogWorker.GetInstance().Get(pager, blogCategoryId, Title);

            return posts;
        }

        /// <summary>
        /// Lists all the BlogPost based on their Categories by calling Data Access Layer
        /// </summary>
        /// <param name="blogCategoryId"></param>
        /// <param name="userId"></param>
        /// <param name="pager"></param>
        /// <returns></returns>
        public static List<BlogPost> GetList(int blogCategoryId, int userId, Pager pager)
        {
            List<BlogPost> posts = DAL.BlogWorker.GetInstance().GetList(blogCategoryId, userId, pager);

            return posts;
        }

        /// <summary>
        /// Lists all BlogPost based on filters
        /// </summary>
        /// <param name="pager"></param>
        /// <param name="blogCategoryId"></param>
        /// <param name="Title"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<BlogPost> Get(Pager pager, int blogCategoryId, string Title, Status status)
        {
            List<BlogPost> posts = DAL.BlogWorker.GetInstance().Get(pager, blogCategoryId, status, Title);

            return posts;
        }

        /// <summary>
        /// Gets all the BlogPost within a week
        /// </summary>
        /// <param name="publishedStartDate"></param>
        /// <param name="publishedEndDate"></param>
        /// <returns></returns>
        public static List<NE.BlogPost> GetWeeklyBlogs(DateTime publishedStartDate, DateTime publishedEndDate)
        {
            List<NE.BlogPost> posts = DAL.BlogWorker.GetInstance().GetWeeklyBlogs(publishedStartDate, publishedEndDate);

            return posts;
        }
    }
}
