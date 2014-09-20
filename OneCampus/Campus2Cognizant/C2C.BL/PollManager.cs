using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Helper;
using System;
using System.Collections.Generic;
using DAL = C2C.DataAccessLogic;

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manipulates the Business logic for the Poll and Uses PollWorker to do Data manipulation
    /// </summary>
    public static class PollManager
    {
        /// <summary>
        /// Gets the Poll using pollId
        /// </summary>
        /// <param name="id">Poll Id</param>
        /// <returns>Business Entity Poll model</returns>
        public static Poll Get(int id)
        {
            Guard.IsNotZero(id, "Poll Id");

            var poll = DAL.PollWorker.GetInstance().Get(id);
            return poll;
        }

        /// <summary>
        /// Checks whether already poll exists in the entered date
        /// </summary>
        /// <param name="poll">Business entity Poll model</param>
        /// <returns>true/false</returns>
        public static bool IsPollExists(Poll poll)
        {
            Guard.IsNotNull(poll, "Poll");
            Guard.IsNotNull(poll.SelectedGroupIds, "Group");

            var isPollExists = DAL.PollWorker.GetInstance().IsPollExists(poll);
            return isPollExists;
        }

        /// <summary>
        /// Changes the poll status to delete
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public static ProcessResponse Delete(int id)
        {
            Guard.IsNotZero(id, "Poll Id");

            var response = DAL.PollWorker.GetInstance().Delete(id);
            return response;
        }

        /// <summary>
        /// Gets the List of Poll
        /// </summary>
        /// <param name="pager">PagerParameters</param>
        /// <returns>Poll List</returns>
        public static List<Poll> GetList(Pager pager)
        {
            var response = DAL.PollWorker.GetInstance().GetList(pager);
            return response;
        }

        /// <summary>
        /// Changes the poll status to Active
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public static ProcessResponse Publish(int id)
        {
            Guard.IsNotZero(id, "Poll Id");

            var response = DAL.PollWorker.GetInstance().Publish(id);
            return response;
        }

        /// <summary>
        /// Changes the poll status to InActive
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public static ProcessResponse Unpublish(int id)
        {
            Guard.IsNotZero(id, "Poll Id");

            var response = DAL.PollWorker.GetInstance().Unpublish(id);
            return response;
        }

        /// <summary>
        /// Updates the existing poll using updated poll model
        /// </summary>
        /// <param name="pollModel">Business entity polModel</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse Update(Poll pollModel)
        {
            Guard.IsNotNull(pollModel, "Poll");

            var response = DAL.PollWorker.GetInstance().Update(pollModel);
            return response;
        }

        /// <summary>
        /// Creates new poll 
        /// </summary>
        /// <param name="pollModel">Business entity PollModel</param>
        /// <returns>Success/Failure response</returns>
        public static ProcessResponse<Poll> Create(Poll poll)
        {
            Guard.IsNotNull(poll, "Poll");

            var response = DAL.PollWorker.GetInstance().Create(poll);
            return response;
        }

        /// <summary>
        /// Checks whether user has voted for the poll
        /// </summary>
        /// <param name="pollId">Poll Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        public static bool IsUserVoted(int pollId, int userId)
        {
            Guard.IsNotZero(pollId, "Poll Id");
            Guard.IsNotZero(userId, "User Id");

            var isVoted = DAL.PollWorker.GetInstance().IsUserVoted(pollId, userId);
            return isVoted;
        }

        /// <summary>
        /// Returns the poll list with filters
        /// </summary>
        /// <param name="pager">Pager parameters for pagination</param>
        /// <param name="Title">Text to be searched in title of poll</param>
        /// <returns></returns>
        public static List<Poll> GetList(Pager pager, string title)
        {
            Guard.IsNotBlank(title, "Poll Title");

            var response = DAL.PollWorker.GetInstance().GetList(pager, title);
            return response;
        }        

        /// <summary>
        /// Gets the Current Active poll with poll answers and vote count
        /// </summary>
        /// <param name="activeDate">Active Date</param>
        /// <param name="userId">User Id</param>
        /// <returns>Business Entity Poll model</returns>
        public static Poll GetCurrentPoll(DateTime activeDate, int userId)
        {
            Guard.IsNotZero(userId, "User Id");

            var poll = DAL.PollWorker.GetInstance().GetCurrentPoll(activeDate, userId);
            return poll;
        }

        /// <summary>
        /// Vote for a poll based on user action
        /// </summary>
        /// <param name="pollResult">Business entity PollResult Model</param>
        /// <returns>Success/Failure Response</returns>
        public static ProcessResponse<PollResult> Vote(PollResult pollResult)
        {
            Guard.IsNotNull(pollResult, "Poll");

            var response = DAL.PollWorker.GetInstance().Vote(pollResult);
            return response;
        }
    }
}
