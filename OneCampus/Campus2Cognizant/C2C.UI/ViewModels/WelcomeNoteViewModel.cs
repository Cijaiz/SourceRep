#region References
using System.Collections.Generic;
using C2C.BusinessEntities.C2CEntities;
#endregion

namespace C2C.UI.ViewModels
{
    public class WelcomeNoteListViewModel
    {
        public List<WelcomeNote> WelcomeNotes{ get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }

    public class WelcomeNoteModel
    {
        public WelcomeNote WelcomeNote { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }
 }