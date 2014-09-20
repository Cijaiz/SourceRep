#region References
using System.Collections.Generic;
using C2C.BusinessEntities;
using C2C.BusinessEntities.C2CEntities;
using C2C.Core.Constants.C2CWeb;
using DAL = C2C.DataAccessLogic;
#endregion

namespace C2C.BusinessLogic
{
    /// <summary>
    /// Manages business logic for WelcomeNote module.
    /// </summary>
    public static class WelcomeNoteManager
    {

        /// <summary>
        /// Gets the WelcomeNote details for the id.
        /// </summary>
        /// <param name="id">WelcomeNote Id</param>
        /// <returns>welcomeNote entity</returns>
        public static WelcomeNote Get(int id)
        {
            WelcomeNote welcomeNote = null;
            if (id >= 0)
            {
                welcomeNote = DAL.WelcomeNoteWorker.GetInstance().Get(id);
            }

            return welcomeNote;
        }

        /// <summary>
        /// Updates the status of WelcomeNote to deleted
        /// </summary>
        /// <param name="id">WelcoemNote id</param>
        public static ProcessResponse Delete(int id)
        {
            WelcomeNote welcomeNote = DAL.WelcomeNoteWorker.GetInstance().Get(id);
            welcomeNote.Status = Status.Deleted;
            var response = DAL.WelcomeNoteWorker.GetInstance().Update(welcomeNote);

            return response;
        }

        /// <summary>
        /// Gets all the WelcomeNote list.
        /// </summary>
        /// <param name="pager">Page object which gives the page size and page count</param>
        /// <returns>WelcomeNote list</returns>
        public static List<WelcomeNote> Get(Pager pager)
        {
            return DAL.WelcomeNoteWorker.GetInstance().Get(pager);
        }

        /// <summary>
        /// Retrieves welcomeNote text to be displayed in Popup
        /// </summary>
        /// <param name="sourceID">Input to decide the source of request(First time login or from tile)</param>
        /// <returns>WelcomeNote text</returns>
        public static string GetWelcomeNoteToDisplay(int sourceID)
        {
            string welcomNote = string.Empty;
            welcomNote = DAL.WelcomeNoteWorker.GetInstance().GetWelcomeNoteToDisplay(sourceID);
            return welcomNote;
        }

        /// <summary>
        /// Edits and updates welcomeNote based on the parameter object
        /// </summary>
        /// <param name="welcomeNote">WelcomeNote object details which ahs to be updated</param>
        /// <returns>ProcessResponse</returns>
        public static ProcessResponse Update(WelcomeNote welcomeNote)
        {
            ProcessResponse response = null;
            response = DAL.WelcomeNoteWorker.GetInstance().Update(welcomeNote);

            return response;
        }

        /// <summary>
        /// Creates WelcomeNote based on the parameter WelcomeNote object
        /// </summary>
        /// <param name="welcomeNote">WelcomeNote object which ahs details about the Note</param>
        /// <returns>ProcessResponse with WelcomeNote entity</returns>
        public static ProcessResponse<WelcomeNote> Create(WelcomeNote welcomeNote)
        {
            ProcessResponse<WelcomeNote> response = null;
            response = DAL.WelcomeNoteWorker.GetInstance().Create(welcomeNote);

            return response;
        }

    }
}
