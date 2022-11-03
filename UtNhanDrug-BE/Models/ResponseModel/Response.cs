namespace UtNhanDrug_BE.Models.ResponseModel
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            StatusCode = 200;
            Data = data;
        }
        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
