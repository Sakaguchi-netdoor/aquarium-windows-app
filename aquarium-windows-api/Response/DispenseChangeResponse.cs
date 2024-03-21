namespace aquarium_windows_api.Response
{
    public class DispenseChangeResponse
    {
        private readonly bool success;
        private readonly string? errorFunction;
        private readonly int? errorCode;
        private readonly int? errorCodeExtended;

        private DispenseChangeResponse(bool success, string? errorFunction, int? errorCode, int? errorCodeExtended)
        {
            this.success = success;
            this.errorFunction = errorFunction;
            this.errorCode = errorCode;
            this.errorCodeExtended = errorCodeExtended;
        }


        public static DispenseChangeResponse Success()
        {

            return new DispenseChangeResponse(success: true, errorFunction: null, errorCode: null, errorCodeExtended: null);
        }

        public static DispenseChangeResponse Error(string errorFunction, int errorCode, int? errorCodeExtended)
        {

            return new DispenseChangeResponse(success: false, errorFunction: errorFunction, errorCode: errorCode, errorCodeExtended: errorCodeExtended);
        }

        public object ToJson()
        {
            return new
            {
                this.success,
                this.errorFunction,
                this.errorCode,
                this.errorCodeExtended
            };
        }
    }
}