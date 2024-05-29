namespace Utility
{
    public static class ErrorResponse
    {
        public static object NotFound()
        {
            return new
            {
                error = "Not Found",
                message = "The resource was not found"
            };
        }

        public static object Unauthorized()
        {
            return new
            {
                error = "Unauthorized",
                message = "You are not authorized to access this resource"
            };
        }

        public static object InternalServerError()
        {
            return new
            {
                error = "Internal Server Error",
                message = "An unexpected error occurred while processing the request"
            };
        }
        public static object ErrorCustom(string error, string message)
        {
            return new
            {
                error = $"{error}",
                message = $"{message}"
            };
        }
    }
}
