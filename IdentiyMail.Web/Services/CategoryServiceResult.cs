namespace IdentiyMail.Web.Services
{
    public class CategoryServiceResult
    {
        public bool Success { get; init; }

        public string? ErrorMessage { get; init; }

        public static CategoryServiceResult Successful()
        {
            return new CategoryServiceResult
            {
                Success = true
            };
        }

        public static CategoryServiceResult Failure(string errorMessage)
        {
            return new CategoryServiceResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
