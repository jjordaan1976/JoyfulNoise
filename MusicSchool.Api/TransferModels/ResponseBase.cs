namespace MusicSchool.Api.TransferModels
{
    public class ResponseBase<T>
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;        
        public T Data { get; set; }
    }
}
