#region References
using System;
using System.Collections.Generic;
using System.Linq;
using C2C.BusinessEntities;
using C2C.Core.Constants.C2CWeb;
using C2C.DataStore;
using BE = C2C.BusinessEntities.C2CEntities;
#endregion

namespace C2C.DataAccessLogic
{
    /// <summary>
    /// Performs actions on Welcome Note entities.
    /// </summary>
    public class WelcomeNoteWorker
    {

        /// <summary>
        /// Retrieves the details of WelcomeNote
        /// </summary>
        /// <param name="welcomeNoteId">WelcomeNote id</param>
        /// <returns>Returns WelcomeNote for the NoteID passed.</returns>
        public BE.WelcomeNote Get(int welcomeNoteId)
        {
            BE.WelcomeNote welcomeNote = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var selectedNote = dbContext.WelcomeNotes.Where(p => p.Id == welcomeNoteId && p.Status != (byte)Status.Deleted).FirstOrDefault();
                if (selectedNote != null && selectedNote.Id > 0)
                {
                    welcomeNote = new BE.WelcomeNote()
                    {
                        Id = selectedNote.Id,
                        Note = selectedNote.Note,
                        OfferExtendedStartDate = selectedNote.OfferExtendedStartDate,
                        OfferExtendedEndDate = selectedNote.OfferExtendedEndDate,
                        Status = (Status)selectedNote.Status,
                        UpdatedBy = selectedNote.UpdatedBy.HasValue ? selectedNote.UpdatedBy.Value : selectedNote.CreatedBy,
                        UpdatedOn = selectedNote.UpdatedOn.HasValue ? selectedNote.UpdatedOn.Value : selectedNote.CreatedOn
                    };
                }
            }

            return welcomeNote;
        }

        /// <summary>
        /// Gets all the WelcomeNote list.
        /// </summary>
        /// <param name="pager">Page object which gives the page size and page count</param>
        /// <returns>WelcomeNote list</returns>
        public List<BE.WelcomeNote> Get(Pager pager)
        {
            List<BE.WelcomeNote> notes = new List<BE.WelcomeNote>();
            List<WelcomeNote> selectedNotes = null;

            using (C2CStoreEntities dbcontext = RepositoryManager.GetStoreEntity())
            {
                selectedNotes = dbcontext.WelcomeNotes.Where(w=>w.Status != (byte)Status.Deleted).OrderBy(o => o.CreatedBy).Skip(pager.SkipCount).Take(pager.PageSize).ToList();
            }
            foreach (var welcomeNote in selectedNotes)
            {
                notes.Add(new BE.WelcomeNote
                {
                    Id = welcomeNote.Id,
                    Note = welcomeNote.Note,
                    OfferExtendedStartDate = welcomeNote.OfferExtendedStartDate,
                    OfferExtendedEndDate = welcomeNote.OfferExtendedEndDate,
                    Status = (Status)welcomeNote.Status
                });
            }
            return notes;
        }

        /// <summary>
        /// Creates an instance for WelcomeNoteWorker
        /// </summary>
        /// <returns>Returns new instance of WelcomeNoteWorker</returns>
        public static WelcomeNoteWorker GetInstance()
        {
            return new WelcomeNoteWorker();
        }

        /// <summary>
        /// Retrieves welcomeNote text to be displayed in Popup
        /// </summary>
        /// <param name="sourceID">Input to decide the source of request(First time login or from tile)</param>
        /// <returns>WelcomeNote text</returns>
        public string GetWelcomeNoteToDisplay(int sourceID)
        {
            string welcomeNote = string.Empty;
            string firstName = string.Empty;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var selectedNote = dbContext.WelcomeNotes.Where(w => w.Status != (byte)Status.Deleted
                                                                      && w.OfferExtendedStartDate <= DateTime.UtcNow
                                                                      && w.OfferExtendedEndDate >= DateTime.UtcNow).FirstOrDefault();
                if (selectedNote != null && selectedNote.Id > 0)
                {
                    welcomeNote = selectedNote.Note;
                }
            }
            return welcomeNote;
        }

        /// <summary>
        /// Edits and updates the welcomeNote 
        /// </summary>
        /// <param name="newWelcomeNote">WelcomeaNote to be updated</param>
        /// <returns>ProcessResponse with response</returns>
        public ProcessResponse Update(BE.WelcomeNote newWelcomeNote)
        {
            ProcessResponse response = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var welcomeNote = dbContext.WelcomeNotes.Where(p => p.Id == newWelcomeNote.Id).FirstOrDefault();

                if (welcomeNote != null)
                {
                    welcomeNote.Note = newWelcomeNote.Note;
                    welcomeNote.OfferExtendedStartDate = newWelcomeNote.OfferExtendedStartDate;
                    welcomeNote.OfferExtendedEndDate = newWelcomeNote.OfferExtendedEndDate;
                    welcomeNote.Status = (byte)newWelcomeNote.Status;
                    welcomeNote.UpdatedBy = newWelcomeNote.UpdatedBy;
                    welcomeNote.UpdatedOn = DateTime.UtcNow;

                    int count = dbContext.SaveChanges();
                    response = new ProcessResponse() { Status = ResponseStatus.Success, Message = string.Format(Message.UPDATED_COUNT, count) };
                }
                else
                {
                    response = new ProcessResponse() { Status = ResponseStatus.Failed, Message = Message.RECORED_NOT_FOUND };
                }
            }
            return response;
        }

        /// <summary>
        /// Creates the WelcomeNote based on the supplied parameter.
        /// </summary>
        /// <param name="newWelcomeNote">WelcomeNote object which has the details of the note to be created.</param>
        /// <returns>ProcessResponse with response and welcomeNote entity.</returns>
        public ProcessResponse<BE.WelcomeNote> Create(BE.WelcomeNote newWelcomeNote)
        {
            ProcessResponse<BE.WelcomeNote> response = null;
            using (C2CStoreEntities dbContext = RepositoryManager.GetStoreEntity())
            {
                var welcomeNote = new WelcomeNote()
                {
                    Note = newWelcomeNote.Note,
                    OfferExtendedStartDate = newWelcomeNote.OfferExtendedStartDate,
                    OfferExtendedEndDate = newWelcomeNote.OfferExtendedEndDate,
                    Status = (byte)newWelcomeNote.Status,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = newWelcomeNote.UpdatedBy
                };

                dbContext.WelcomeNotes.Add(welcomeNote);

                if (dbContext.SaveChanges() > 0)
                {
                    newWelcomeNote.Id = welcomeNote.Id;
                    response = new ProcessResponse<BE.WelcomeNote>() { Status = ResponseStatus.Success, Message = Message.CREATED, Object = newWelcomeNote };
                }
                else
                {
                    response = new ProcessResponse<BE.WelcomeNote>() { Status = ResponseStatus.Failed, Message = Message.FAILED };
                }

            }

            return response;
        }

    }
}
