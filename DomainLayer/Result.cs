namespace DomainLayer
{
    public class Result
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static Result Succeed(string message = "") 
            => new Result() {Message = message, Success = true, StatusCode = 200};
        public static Result Fail(string? message = null, int statusCode = 400) 
            => new Result() { Success = false, Message = message, StatusCode = statusCode };

    }

    public class Result<T>
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Value { get; set; }

        public static Result<T> Succeed(T value) =>
            new Result<T> {Success = true, Value = value, StatusCode = 200 };

        public static Result<T> Fail(string? message = null, int statusCode = 400, T? value = default)  =>
            new Result<T> { Success = false, Value = value, StatusCode = statusCode };
    }
}
