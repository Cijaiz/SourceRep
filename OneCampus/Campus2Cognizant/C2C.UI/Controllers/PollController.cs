#region References
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
#endregion

namespace C2C.UI.Controllers
{
    [Authorize]
    public class PollController : BaseController
    {
        /// <summary>
        /// Shows the poll Tile in Indexpage
        /// </summary>
        /// <returns>Renders partial View of polllTile</returns>
        public ActionResult Tile()
        {
            //var poll = PollManager.GetCurrentPoll(DateTime.UtcNow, User.UserId);
            //if(poll != null && User.HasPermission(ApplicationPermission.CanVote))
            //{
            //    poll.HasPermission = true;
            //}
            //return PartialView(poll);
            return PartialView();
        }
                
        /// <summary>
        /// Provides the vote count for the poll to draw chart
        /// </summary>
        /// <returns>Json Value</returns>
        public JsonResult Chart()
        {
            var pollResult = PollManager.GetCurrentPoll(DateTime.UtcNow, User.UserId);
            if (pollResult != null && User.HasPermission(ApplicationPermission.CanVote))
            {
                pollResult.HasPermission = true;
            }
            return Json(new { result = pollResult }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays the index page as list of Polls.
        /// </summary>
        /// <returns>An Index page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Index()
        {
            Pager pager = new Pager();

            ViewData["PollResponse"] = (PageNotificationViewModel)TempData["PollResponse"];

            var polls = PollManager.GetList(pager);

            return View(polls);
        }

        /// <summary>
        /// Vote the poll with selected answer Id
        /// </summary>
        /// <param name="pollId">Poll Id</param>
        /// <param name="ansId">Answer Id</param>
        /// <returns></returns>
        [CheckPermission(ApplicationPermission.CanVote)]
        public ActionResult Vote(int pollId, int ansId)
        {
            PollResult pollResult = new PollResult();

            pollResult.PollId = pollId;
            pollResult.PollAnswerId = ansId;
            pollResult.AnsweredBy = User.UserId;

            var response = PollManager.Vote(pollResult);
            var poll = PollManager.GetCurrentPoll(DateTime.UtcNow, User.UserId);

            return Json(new { result = poll, status = response.Status.ToString() }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Shows poll creation page with groups 
        /// </summary>
        /// <returns>Create Poll page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Create()
        {
            Poll poll = new Poll();

            poll.GroupList = GroupManager.GetList();

            return View(poll);
        }

        /// <summary>
        /// Shows the Edit  page of selected poll
        /// </summary>
        /// <param name="id">Poll id</param>
        /// <returns>Edit Page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Edit(int id)
        {
            var poll = PollManager.Get(id);
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (poll != null)
            {
                if (poll.GroupList != null && poll.GroupList.Count() > 0)
                {
                    poll.GroupList.ForEach(c => poll.SelectedGroupIds.Add(c.Id));
                }

                poll.GroupList = GroupManager.GetList();

                return View(poll);
            }

            responseMessage.AddError("Invalid PollId");

            TempData["PollResponse"] = responseMessage;

            return RedirectToAction("Index");
            
        }

        /// <summary>
        /// Deletes the Selected Poll
        /// </summary>
        /// <param name="id">PollId</param>
        /// <returns>Redirects to PollList Page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Delete(int id)
        {
            ProcessResponse response = null;

            response = PollManager.Delete(id);
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["PollResponse"] = responseMessage;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Publishes the Selected Poll
        /// </summary>
        /// <param name="id">PollId</param>
        /// <returns>Redirects to PollList Page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Publish(int id)
        {
            ProcessResponse response = null;

            response = PollManager.Publish(id);
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            if (response.Status == ResponseStatus.Success)
            {
                Poll poll = PollManager.Get(id);
                if (poll.Notify)
                {
                    NotificationContent notificationContent = new NotificationContent()
                    {
                        Id = id,
                        ContentType = Module.Poll,
                        ContentTitle = WebUtility.HtmlEncode(poll.Question),
                        Description = WebUtility.HtmlEncode(C2C.Core.Constants.C2CWeb.DefaultValue.NEW_POLL_CREATED),
                        //URL = Services.ContentManager.GetItemMetadata(post.ContentItem).DisplayRouteValues.ToString(),
                        URL = GetUrl("Home", "Index"),
                        ValidFrom = poll.VisibleFrom,
                        ValidTo = poll.VisibleTill,
                        Groups = poll.SelectedGroupIds,
                        SharedBy = User.UserId,
                        Subject = "New Poll is published"
                    };

                    PublisherEvents publisherEvents = new PublisherEvents()
                    {
                        EventId = id.ToString(),
                        EventCode = EventCodes.POLL_PUBLISH,
                        NotificationContent = SerializationHelper.JsonSerialize<NotificationContent>(notificationContent)
                    };

                    NotifyPublisher.Notify(publisherEvents, NotificationPriority.High);
                }
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["PollResponse"] = responseMessage;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Unpublishes the Selected Poll
        /// </summary>
        /// <param name="id">PollId</param>
        /// <returns>Redirects to PollList Page</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult UnPublish(int id)
        {
            ProcessResponse response = null;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            response = PollManager.Unpublish(id);

            if (response.Status == ResponseStatus.Success)
            {
                responseMessage.AddSuccess(response.Message);
            }
            else
            {
                responseMessage.AddError(response.Message);
            }
            TempData["PollResponse"] = responseMessage;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// List polls based on filter
        /// </summary>
        /// <param name="filter">Filter conditions</param>
        /// <returns>List of polls</returns>
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult List(PollListFilter filter = null)
        {
            Pager pager = new Pager();

            var pollList = PollManager.GetList(pager, filter.SearchTitle);

            return View(pollList);
        }

        /// <summary>
        /// Validates the poll answers and poll groups
        /// </summary>
        /// <param name="poll">poll</param>
        /// <param name="pollAnswers">Poll Answers list</param>
        /// <returns></returns>
        private void ValidatePoll(Poll poll, List<PollAnswer> pollAnswers)
        {
            //Poll Answers validation
            if (pollAnswers != null && pollAnswers.Count() > 1)
            {
                foreach (var ans in pollAnswers)
                {
                    //checks whether user deleted in view page
                    if (ans != null && ans.Answer != null && !ans.IsDeleted)
                    {
                        //checks whether answer length exceeds 50 characters
                        if (ans.Answer.Length > 50)
                        {
                            ModelState.AddModelError("PollAnswers", "Answer Should not exceed 50 characters");
                        }
                        poll.PollAnswers.Add(ans);
                    }
                }
            }

            if(poll.PollAnswers.Count() < 2)
            {
                ModelState.AddModelError("PollAnswers", "Minimum 2 answers are required");
            }

            //Poll Groups validation
            if (poll.SelectedGroupIds == null || poll.SelectedGroupIds.Count() <= 0)
            {
                ModelState.AddModelError("GroupList", "Select One Group to proceed");
            }
        }

        /// <summary>
        /// Updates the selected poll using updates data from view page
        /// </summary>
        /// <param name="poll">Poll</param>
        /// <param name="pollAnswers">Poll Answers</param>
        /// <param name="groupList">GRoup list</param>
        /// <returns>Poll list Page</returns>
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Edit(Poll poll, List<PollAnswer> pollAnswers)
        {
            ProcessResponse response = null;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            ValidatePoll(poll, pollAnswers);

            if (poll.IsVotingStarted)
            {
                ModelState.AddModelError("IsVotingStarted", "Already Voting Started for this Poll. Editing Prohibited");
            }
            else
            {
                if (PollManager.IsPollExists(poll))
                {
                    ModelState.AddModelError("Id", "Already Poll Exists Within the specified Date");
                }
                else
                {
                    int validDate = DateTime.Compare(poll.VisibleFrom.Date, DateTime.UtcNow.Date);

                    if (validDate < 0)
                        ModelState.AddModelError("VisibleFrom", "VisibleFrom Date must be Greaterthan SystemDate");
                }               
            }

            if (ModelState.IsValid)
            {
                poll.Status = Status.Pending;
                poll.UpdatedBy = User.UserId;
                response = PollManager.Update(poll);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
                TempData["PollResponse"] = responseMessage;
            }

            if (response != null && response.Status == ResponseStatus.Success)
                return RedirectToAction("Index");
            else
            {
                poll.GroupList = GroupManager.GetList();
                return View(poll);
            }
        }

        /// <summary>
        /// Creates a new poll using pollModel
        /// </summary>
        /// <param name="poll">PollModel</param>
        /// <param name="pollAnswers">Entered PollAnswers</param>
        /// <param name="groupList">Group List</param>
        /// <returns>Redirects to PollList Page</returns>
        [HttpPost]
        [CheckPermission(ApplicationPermission.ManagePoll)]
        public ActionResult Create(Poll poll, List<PollAnswer> pollAnswers)
        {
            ProcessResponse<Poll> response = null;
            PageNotificationViewModel responseMessage = new PageNotificationViewModel();

            ValidatePoll(poll, pollAnswers);

            int validDate = DateTime.Compare(poll.VisibleFrom.Date, DateTime.UtcNow.Date);

            if (validDate < 0)
                ModelState.AddModelError("VisibleFrom", "StartDate must be Greaterthan SystemDate");

            if (PollManager.IsPollExists(poll))
            {
                ModelState.AddModelError("Id", "Already Active Poll Exists Within the specified Date. Make that Inactive");
            }

            if (ModelState.IsValid)
            {
                poll.Status = Status.Pending;                
                poll.UpdatedBy = User.UserId;
                response = PollManager.Create(poll);

                if (response.Status == ResponseStatus.Success)
                {
                    responseMessage.AddSuccess(response.Message);
                }
                else
                {
                    responseMessage.AddError(response.Message);
                }
                TempData["PollResponse"] = responseMessage;
            }

            if (response != null && response.Status == ResponseStatus.Success)
            {
                return RedirectToAction("Index");
            }

            else
            {
                poll.GroupList = GroupManager.GetList();
                return View(poll);
            }
        }

        private string GetUrl(string controller, string action)
        {
            string domainUrl = string.Empty;
            string itemUrl = string.Empty;
            Uri uri = Request.Url;
            domainUrl = String.Format("{0}{1}{2}", (Request.IsSecureConnection) ? "https://" : "http://",
                                              uri.Host, (uri.IsDefaultPort) ? "" : String.Format(":{0}", uri.Port));
            itemUrl = string.Format("{0}/{1}/{2}", domainUrl,
                controller, action);
            return itemUrl;
        }

    }
}
