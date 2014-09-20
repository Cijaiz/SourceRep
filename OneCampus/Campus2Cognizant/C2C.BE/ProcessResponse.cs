namespace C2C.BusinessEntities
{
    public class ProcessResponse
    {
        public int RefId { get; set; }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }

    public class ProcessResponse<T>
    {
        public T Object { get; set; }
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
    }

    public enum ResponseStatus
    {
        Success,
        Failed,
        Error
    }
}
