using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using C2C.BusinessEntities.C2CEntities;

namespace C2C.UI.ViewModels
{
    public class MetaMasterViewModel
    {
        public MetaMasterViewModel()
        {
            MetaValueList = new List<MetaValueViewModel>();
        }

        public int Id { get; set; }
        public string KeyReference { get; set; }
        public List<MetaValueViewModel> MetaValueList { get; set; }
        public PageNotificationViewModel ResponseMessage { get; set; }
    }
}