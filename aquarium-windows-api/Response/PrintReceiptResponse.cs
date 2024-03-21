namespace aquarium_windows_api.Response
{
    public class PrintReceiptResponse
    {
        private readonly bool success;
        private readonly ErrorType? errorType;
        private readonly int? errorCode;
        private readonly string? errorMessage;
        public enum ErrorType
        {
            status,
            app,
            sdk,
        }

        private PrintReceiptResponse(bool success, ErrorType? errorType, int? errorCode, string? errorMessage)
        {
            this.success = success;
            this.errorType = errorType;
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
        }


        public static PrintReceiptResponse Success()
        {
            return new PrintReceiptResponse(success: true, errorType: null, errorCode: null, errorMessage: null);
        }

        public static PrintReceiptResponse Error(ErrorType errorType, int errorCode, string? errorMessage)
        {

            return new PrintReceiptResponse(success: false, errorType: errorType, errorCode: errorCode, errorMessage: errorMessage);
        }

        public object ToJson()
        {
            return new
            {
                this.success,
                errorType = this.errorType?.ToString(),
                this.errorCode,
                this.errorMessage,
            };
        }
    }
}