namespace BLL.DTOs
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
