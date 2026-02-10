namespace V_Quiz_Backend.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; }

        public static ServiceResponse<T> Fail(string message)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message
            };
        }

        public static ServiceResponse<T> Ok(T data, string? message = null)
        {
            return new ServiceResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

    }
    public class ServiceResponse
    {

        public bool Success { get; set; } = true;
        public string? Message { get; set; }

        public static ServiceResponse Ok(string? message = null)
        {
            return new ServiceResponse
            {
                Success = true,

                Message = message
            };
        }

        public static ServiceResponse Fail(string message)
        {
            return new ServiceResponse 
            { 
                Message = message, 
                Success = false 
            };
        }
    }
}
