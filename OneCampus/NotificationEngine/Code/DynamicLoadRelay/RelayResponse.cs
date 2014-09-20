namespace DynamicLoadRelay
{
    public class RelayResponse
    {
        public string PushUrl { get; set; }
        public string GetUrl { get; set; }
        public string PushBaseUrl { get; set; }
        public string GetBaseUrl { get; set; }
    }

    public enum BaseUrl
    {
        C2C,
        Hub
    }

}
