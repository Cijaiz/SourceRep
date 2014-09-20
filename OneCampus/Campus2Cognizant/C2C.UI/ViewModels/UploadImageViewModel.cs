using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace C2C.UI.ViewModels
{
    public class UploadImageViewModel
    {
        public string ModuleName { get; set; }
        public string Url { get; set; }
        public List<string> ImageUrls { get; set; }

        [Required(ErrorMessage="Please select file to upload.")]
        public HttpPostedFileBase ImageFile { get; set; }
        public bool IsuploadSuccess { get; set; }
    }
}