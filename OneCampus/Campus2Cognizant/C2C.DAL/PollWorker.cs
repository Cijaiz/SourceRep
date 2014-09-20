using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;
using BE = C2C.BusinessEntities.C2CEntities;

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs Data manipulation operations on Poll Entity Libraries.
    /// </summary>
    public class PollWorker
    {
        /// <summary>
        /// Gets the Poll using pollId
        /// </summary>
        /// <param name="id">Poll Id</param>
        /// <returns>Business Entity Poll model</returns>
        public BE.Poll Get(int id)
        {
            BE.Poll poll = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var selectedPoll = dbContext.Polls.Where(p => p.Id == id &&
                                                              p.Status != (byte)Status.Deleted)
                                                  .FirstOrDefault();

                if (selectedPoll != null && selectedPoll.Id > 0)
                {
                    poll = new BE.Poll();
                    poll.PollAnswers = new List<BE.PollAnswer>();

                    poll.Id = selectedPoll.Id;
                    poll.Question = selectedPoll.Question;
                    poll.VisibleFrom = selectedPoll.VisibleFrom;
                    poll.VisibleTill = selectedPoll.VisibleTill;

                    poll.Status = (Status)selectedPoll.Status;
                    poll.Notify = selectedPoll.Notify;
                    poll.UpdatedBy = selectedPoll.UpdatedBy.HasValue ? selectedPoll.UpdatedBy.Value : selectedPoll.CreatedBy;
                    poll.UpdatedOn = selectedPoll.UpdatedOn.HasValue ? selectedPoll.UpdatedOn.Value : selectedPoll.CreatedOn;

                    poll.IsVotingStarted = selectedPoll.PollResults.Count() > 0;

                    poll.SelectedGroupIds = selectedPoll.PollGroups.Select(p => p.GroupId).ToList();

                    //Populating poll answers business entity
                    selectedPoll.PollAnswers.ToList().ForEach(p => poll.PollAnswers.Add(new BE.PollAnswer()
                    {
                        Id = p.Id,
                        Answer = p.Answer,
                        PollId = p.PollId
                    }));

                    //populating poll groups business entity
                    selectedPoll.PollGroups.ToList().ForEach(p => poll.GroupList.Add(new BE.Group()
                    {
                        Id = p.GroupId,
                        Title = p.Group.Name,
                    }));
                }
            }
            return poll;
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <returns>Returns new instance</returns>
        public static PollWorker GetInstance()
        {
            return new PollWorker();
        }

        /// <summary>
        /// Checks whether already poll exists in the entered date
        /// </summary>
        /// <param name="poll">Business entity Poll model</param>
        /// <returns>true/false</returns>
        public bool IsPollExists(BE.Poll poll)
        {
            bool isPollExists = false;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                isPollExists = dbContext.Polls
                                         .Where(p => p.Id != poll.Id &&
                                                      p.Status == (byte)Status.Active &&
                                                     ((p.VisibleFrom <= poll.VisibleFrom && p.VisibleTill >= poll.VisibleFrom) ||
                                                     (p.VisibleFrom <= poll.VisibleTill && p.VisibleTill >= poll.VisibleTill)))
                                         .Select(p => p.PollGroups
                                         .Any(a => poll.SelectedGroupIds
                                         .Contains(a.GroupId))).Any(p => p == true);
            }

            return isPollExists;
        }

        /// <summary>
        /// Changes the poll status to delete
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public ProcessResponse Delete(int pollId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var poll = dbContext.Polls.Where(p => p.Id == pollId && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (poll != null)
                {
                    poll.Status = (byte)Status.Deleted;

                    if (dbContext.SaveChanges() > 0)
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Success,
                            Message = Message.DELETED,
                        };
                    }
                    else
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Failed,
                            Message = Message.FAILED,
                        };
                    }
                }

                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.RECORED_NOT_FOUND,
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Changes the poll status to Active
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public ProcessResponse Publish(int pollId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var poll = dbContext.Polls.Where(p => p.Id == pollId && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (poll != null)
                {
                    poll.Status = (byte)Status.Active;

                    if (dbContext.SaveChanges() > 0)
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Success,
                            Message = Message.PUBLISHED,
                        };
                    }
                    else
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Failed,
                            Message = Message.FAILED,
                        };
                    }
                }

                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.RECORED_NOT_FOUND,
                    };
                }
            }
            return response;
        }

        ///<summary>
        ///To list the Poll
        ///</summary>
        ///<param name="pager"></param>
        ///<returns>returns the Poll list</returns>
        public List<BE.Poll> GetList(Pager pager)
        {
            List<BE.Poll> pollList = null;

            using (C2CStoreEntities dbcontext = RepositoryManager.GetStoreEntity())
            {
                //Transform the Business entity to Database Entity for Repository operations.
                var polls = dbcontext.Polls.Where(p => p.Status != (byte)Status.Deleted)
                                           .OrderBy(o => o.Id)
                                           .Skip(pager.SkipCount)
                                           .Take(pager.PageSize)
                                           .ToList();

                if (polls != null && polls.Count() > 0)
                {
                    pollList = new List<BE.Poll>();

                    polls.ForEach(p => pollList.Add(new BE.Poll()
                    {
                        Id = p.Id,
                        Question = p.Question,
                        VisibleFrom = p.VisibleFrom,
                        VisibleTill = p.VisibleTill,

                        Status = (Status)p.Status,
                        UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                        UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn,

                        PollAnswers = p.PollAnswers.Select(q => new BE.PollAnswer()
                        {
                            Id = q.Id,
                            PollId = q.PollId,
                            Answer = q.Answer
                        }).ToList()
                    }));

                }
            }
            return pollList;

        }

        /// <summary>
        /// Changes the poll status to InActive
        /// </summary>
        /// <param name="pollId">PollId</param>
        /// <returns>Success/Failure response</returns>
        public ProcessResponse Unpublish(int pollId)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var poll = dbContext.Polls.Where(p => p.Id == pollId && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (poll != null)
                {
                    poll.Status = (byte)Status.InActive;

                    if (dbContext.SaveChanges() > 0)
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Success,
                            Message = Message.UNPUBLISHED,
                        };
                    }
                    else
                    {
                        response = new ProcessResponse()
                        {
                            Status = ResponseStatus.Failed,
                            Message = Message.FAILED,
                        };
                    }
                }

                else
                {
                    response = new ProcessResponse()
                    {
                        Status = ResponseStatus.Failed,
                        Message = Message.RECORED_NOT_FOUND,
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Checks whether user has voted for the poll
        /// </summary>
        /// <param name="pollId">Poll Id</param>
        /// <param name="userId">User Id</param>
        /// <returns>True/False</returns>
        public bool IsUserVoted(int pollId, int userId)
        {
            bool isVoted = false;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                isVoted = dbContext.Polls.Where(p => p.Id == pollId &&
                                                     p.Status != (byte)Status.Deleted)
                                         .FirstOrDefault().PollResults
                                         .Where(p => p.AnsweredBy == userId)
                                         .Count() > 0;
            }

            return isVoted;
        }

        /// <summary>
        /// Updates the existing poll using updated poll model
        /// </summary>
        /// <param name="pollModel">Business entity polModel</param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse Update(BE.Poll pollModel)
        {
            ProcessResponse response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var poll = dbContext.Polls.Where(p => p.Id == pollModel.Id && p.Status != (byte)Status.Deleted).FirstOrDefault();

                if (poll != null)
                {
                    poll.Question = pollModel.Question;
                    poll.VisibleFrom = pollModel.VisibleFrom;
                    poll.VisibleTill = pollModel.VisibleTill;

                    poll.Status = (byte)pollModel.Status;
                    poll.Notify = pollModel.Notify;
                    poll.UpdatedBy = pollModel.UpdatedBy;
                    poll.UpdatedOn = DateTime.UtcNow;

                    //Removes all existing Answers in poll and updates new set of Answers for poll
                    var pollAnswers = dbContext.PollAnswers.Where(p => p.PollId == pollModel.Id).ToList();

                    foreach (var ans in pollAnswers)
                    {
                        dbContext.PollAnswers.Remove(ans);
                    }

                    pollModel.PollAnswers.ForEach(p => poll.PollAnswers.Add(new PollAnswer()
                    {
                        Answer = p.Answer,
                        PollId = poll.Id,
                    }));

                    //Removes all existing Groups in poll and updates new set of groups in poll
                    if (poll.PollGroups != null)
                    {
                        var pollGroups = dbContext.PollGroups.Where(p => p.PollId == pollModel.Id).ToList();

                        foreach (var group in pollGroups)
                        {
                            dbContext.PollGroups.Remove(group);
                        }

                        pollModel.SelectedGroupIds.ForEach(p => poll.PollGroups.Add(new PollGroup()
                        {
                            GroupId = p,
                            PollId = poll.Id,
                        }));

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
                            Message = Message.RECORED_NOT_FOUND
                        };
                    }

                }
            }
            return response;
        }        

        /// <summary>
        /// Creates new poll 
        /// </summary>
        /// <param name="pollModel">Business entity PollModel</param>
        /// <returns>Success/Failure response</returns>
        public ProcessResponse<BE.Poll> Create(BE.Poll pollModel)
        {
            ProcessResponse<BE.Poll> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {

                //Populating Data entity using business entity
                Poll poll = new Poll();
                poll.Question = pollModel.Question;
                poll.VisibleFrom = pollModel.VisibleFrom;
                poll.VisibleTill = pollModel.VisibleTill;

                poll.Status = (byte)Status.Pending;
                poll.Notify = pollModel.Notify;
                poll.CreatedBy = pollModel.UpdatedBy;
                poll.CreatedOn = DateTime.UtcNow;

                dbContext.Polls.Add(poll);

                //populating pollanswers data entity 
                pollModel.PollAnswers.ForEach(p => poll.PollAnswers.Add(new PollAnswer()
                                                                                        {
                                                                                            Answer = p.Answer,
                                                                                            PollId = p.Id
                                                                                        }));

                //populating pollgroups data entity
                pollModel.SelectedGroupIds.ForEach(p => poll.PollGroups.Add(new PollGroup()
                                                                                          {
                                                                                              GroupId = p,
                                                                                              PollId = poll.Id,
                                                                                          }));


                if (dbContext.SaveChanges() > 0)
                {
                    pollModel.Id = poll.Id;
                    response = new ProcessResponse<BE.Poll>()
                                                            {
                                                                Status = ResponseStatus.Success,
                                                                Message = Message.CREATED,
                                                                Object = pollModel
                                                            };

                }
                else
                {
                    response = new ProcessResponse<BE.Poll>()
                                                            {
                                                                Status = ResponseStatus.Failed,
                                                                Message = Message.FAILED,
                                                                Object = pollModel
                                                            };
                }
            }

            return response;
        }

        /// <summary>
        /// Gets the Current Active poll with poll answers and vote count
        /// </summary>
        /// <param name="activeDate">Active Date</param>
        /// <param name="userId">User Id</param>
        /// <returns>Business Entity Poll model</returns>
        public BE.Poll GetCurrentPoll(DateTime activeDate, int userId)
        {
            BE.Poll pollModel = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                List<int> ids = dbContext.UserGroups.Where(c => c.UserId == userId && c.Status == (byte)Status.Active).Select(c => c.GroupId).ToList();
                var poll = dbContext.Polls.Where(p => p.VisibleFrom <= activeDate.Date &&
                                                      p.VisibleTill >= activeDate.Date &&
                                                      p.Status == (byte)Status.Active &&
                                                      p.PollGroups.Any(q => q.Group.UserGroups.Any(r => r.UserId == userId && r.Status == (byte)Status.Active)))
                                          .FirstOrDefault();


                if (poll != null)
                {
                    pollModel = new BE.Poll()
                    {
                        Id = poll.Id,
                        Question = poll.Question,
                        Status = (Status)poll.Status,

                        VisibleFrom = poll.VisibleFrom,
                        VisibleTill = poll.VisibleTill,
                        UpdatedBy = poll.UpdatedBy.HasValue ? poll.UpdatedBy.Value : poll.CreatedBy,
                        UpdatedOn = poll.UpdatedOn.HasValue ? poll.UpdatedOn.Value : poll.CreatedOn,

                        PollAnswers = poll.PollAnswers.Select(q => new BE.PollAnswer()
                        {
                            Id = q.Id,
                            PollId = q.PollId,
                            Answer = q.Answer,
                            VoteCount = q.PollResults.Count()
                        }).ToList(),
                        IsVoted = poll.PollResults.Where(p => p.AnsweredBy == userId)
                                                  .Count() > 0,
                        IsVotingStarted = poll.PollResults.Count() > 0,
                    };

                }
            }
            return pollModel;
        }

        /// <summary>
        /// Returns the poll list with filters
        /// </summary>
        /// <param name="pager">Pager parameters for pagination</param>
        /// <param name="Title">Text to be searched in title of poll</param>
        /// <returns></returns>
        public List<BE.Poll> GetList(Pager pager, string Title = null)
        {
            List<BE.Poll> pollList = null;

            using (C2CStoreEntities dbcontext = RepositoryManager.GetStoreEntity())
            {
                if (Title != null)
                {
                    var polls = dbcontext.Polls.Where(p => p.Question.Contains(Title) && p.Status != (byte)Status.Deleted).OrderBy(o => o.Id).Take(pager.PageSize).ToList();
                    if (polls != null && polls.Count > 0)
                        polls.ForEach(p => pollList.Add(
                                      new BE.Poll
                                      {
                                          Id = p.Id,
                                          Question = p.Question,
                                          VisibleFrom = p.VisibleFrom,
                                          VisibleTill = p.VisibleTill,

                                          Status = (Status)p.Status,
                                          UpdatedBy = p.UpdatedBy.HasValue ? p.UpdatedBy.Value : p.CreatedBy,
                                          UpdatedOn = p.UpdatedOn.HasValue ? p.UpdatedOn.Value : p.CreatedOn,

                                          PollAnswers = p.PollAnswers.Select(q => new BE.PollAnswer()
                                          {
                                              Id = q.Id,
                                              PollId = q.PollId,
                                              Answer = q.Answer
                                          }).ToList(),
                                          PollResults = p.PollResults.Select(r => new BE.PollResult()
                                          {
                                              Id = r.Id,
                                              PollId = r.PollId,
                                              PollAnswerId = r.PollAnswerId,
                                              AnsweredBy = r.AnsweredBy,
                                              AnsweredOn = r.AnsweredOn
                                          }).ToList()
                                      }));
                }
                else
                {
                    pollList = GetList(pager);
                }
            }
            return pollList;
        }

        /// <summary>
        /// Vote for a poll based on user action
        /// </summary>
        /// <param name="pollResult">Business entity PollResult Model</param>
        /// <returns>Success/Failure Response</returns>
        public ProcessResponse<BE.PollResult> Vote(BE.PollResult pollResult)
        {
            ProcessResponse<BE.PollResult> response = null;

            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var hasVoted = dbContext.PollResults.Where(c => c.PollId == pollResult.PollId && c.AnsweredBy == pollResult.AnsweredBy).Count() > 0;

                if (!hasVoted)
                {
                    PollResult result = new PollResult();


                    result.PollId = pollResult.PollId;
                    result.PollAnswerId = pollResult.PollAnswerId;
                    result.AnsweredBy = pollResult.AnsweredBy;
                    result.AnsweredOn = DateTime.UtcNow;

                    dbContext.PollResults.Add(result);

                    pollResult.Id = result.Id;

                    if (dbContext.SaveChanges() > 0)
                    {
                        response = new ProcessResponse<BE.PollResult>()
                                                                        {
                                                                            Status = ResponseStatus.Success,
                                                                            Message = Message.VOTED,
                                                                            Object = pollResult
                                                                        };

                    }
                    else
                    {
                        response = new ProcessResponse<BE.PollResult>()
                                                                        {
                                                                            Status = ResponseStatus.Failed,
                                                                            Message = Message.FAILED,
                                                                        };
                    }
                }
                else
                {
                    response = new ProcessResponse<BE.PollResult>()
                                                                    {
                                                                        Status = ResponseStatus.Error,
                                                                        Message = Message.ALREADY_VOTED,
                                                                    };
                }
            }

            return response;
        }        
    }
}


