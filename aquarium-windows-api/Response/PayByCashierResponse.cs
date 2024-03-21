namespace aquarium_windows_api.Response
{
    public class PayByCashierResponse
    {
        private readonly bool success;
        private readonly string? errorFunction;
        private readonly int? errorCode;
        private readonly int? errorCodeExtended;

        private PayByCashierResponse(bool success, string? errorFunction, int? errorCode, int? errorCodeExtended)
        {
            this.success = success;
            this.errorFunction = errorFunction;
            this.errorCode = errorCode;
            this.errorCodeExtended = errorCodeExtended;
        }


        public static PayByCashierResponse Success()
        {

            return new PayByCashierResponse(success: true, errorFunction: null, errorCode: null, errorCodeExtended: null);
        }

        public static PayByCashierResponse Error(string errorFunction, int errorCode, int? errorCodeExtended)
        {

            return new PayByCashierResponse(success: false, errorFunction: errorFunction, errorCode: errorCode, errorCodeExtended: errorCodeExtended);
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