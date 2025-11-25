namespace DomainLayer
{
    /// <summary>
    /// Represents the outcome of an operation that does not return a value.
    /// Contains information about success, status code, and an optional message.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets the HTTP-style status code representing the result.
        /// Default is 200 for success, 400+ for errors.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets an optional message describing the result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <param name="message">An optional success message.</param>
        /// <returns>A Result instance marked as successful.</returns>
        public static Result Succeed(string message = "") 
            => new Result() {Message = message, Success = true, StatusCode = 200};

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">An optional error message.</param>
        /// <param name="statusCode">Optional status code (default is 400).</param>
        /// <returns>A Result instance marked as failed.</returns>
        public static Result Fail(string? message = null, int statusCode = 400) 
            => new Result() { Success = false, Message = message, StatusCode = statusCode };

    }

    /// <summary>
    /// Represents the outcome of an operation that returns a value of type <typeparamref name="T"/>.
    /// Contains success info, status code, optional message, and returned value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>

    public class Result<T>
    {
        /// <summary>
        /// Gets or sets the HTTP-style status code representing the result.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets an optional message describing the result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the value returned from the operation, if any.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// Creates a successful result with a value.
        /// </summary>
        /// <param name="value">The result value to return.</param>
        /// <returns>A Result&lt;T&gt; marked as successful with the specified value.</returns>
        public static Result<T> Succeed(T value, string? message = null) =>
            new Result<T> {Success = true, Value = value, StatusCode = 200, Message = message };

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">An optional error message.</param>
        /// <param name="statusCode">Optional status code (default is 400).</param>
        /// <param name="value">Optional value to return, typically null or default.</param>
        /// <returns>A Result&lt;T&gt; marked as failed.</returns>
        public static Result<T> Fail(string? message = null, int statusCode = 400, T? value = default)  =>
            new Result<T> { Success = false, Value = value, StatusCode = statusCode, Message = message};
    }
}
