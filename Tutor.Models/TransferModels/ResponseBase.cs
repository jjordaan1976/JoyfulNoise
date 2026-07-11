namespace Tutor.Models.TransferModels
{
    public class ResponseBase<T>
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ResponseBase<T> Success(T? data, string message = "Success") =>
            new() { ReturnCode = 0, ReturnMessage = message, Data = data };

        public static ResponseBase<T> Failure(string message = "An error occurred") =>
            new() { ReturnCode = -1, ReturnMessage = message };
    }
}
