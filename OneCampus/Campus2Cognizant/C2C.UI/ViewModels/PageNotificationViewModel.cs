using System.Collections.Generic;

namespace C2C.UI.ViewModels
{
    public class PageNotificationViewModel
    {
        public PageNotificationViewModel()
        {
            this.Errors = new List<string>();
            this.Warning = new List<string>();
            this.Success = new List<string>();
        }

        public List<string> Errors { get; private set; }
        public List<string> Warning { get; private set; }
        public List<string> Success { get; private set; }

        public void AddError(string message)
        {
            this.Errors.Add(message);
        }

        public void AddWarning(string message)
        {
            this.Warning.Add(message);
        }

        public void AddSuccess(string message)
        {
            this.Success.Add(message);
        }
    }
}