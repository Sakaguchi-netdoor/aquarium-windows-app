namespace aquarium_windows_api.Response
{
    public class CancelCashierResponse
    {
        private readonly bool success;
        private readonly string? errorFunction;
        private readonly int? errorCode;
        private readonly int? errorCodeExtended;

        private CancelCashierResponse(bool success, string? errorFunction, int? errorCode, int? errorCodeExtended)
        {
            this.success = success;
            this.errorFunction = errorFunction;
            this.errorCode = errorCode;
            this.errorCodeExtended = errorCodeExtended;
        }


        public static CancelCashierResponse Success()
        {

            return new CancelCashierResponse(success: true, errorFunction: null, errorCode: null, errorCodeExtended: null);
        }

        public static CancelCashierResponse Error(string errorFunction, int errorCode, int? errorCodeExtended)
        {

            return new CancelCashierResponse(success: false, errorFunction: errorFunction, errorCode: errorCode, errorCodeExtended: errorCodeExtended);
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