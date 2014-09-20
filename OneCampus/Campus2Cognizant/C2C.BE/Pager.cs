namespace C2C.BusinessEntities
{
    public class Pager
    {
        public Pager()
        {
            PageSize = C2C.Core.Constants.C2CWeb.DefaultValue.PAGE_SIZE;
            PageNo = 1;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int SkipCount { get { return PageNo > 0 ? ((PageNo - 1) * PageSize) : 0; } }
    }
}
