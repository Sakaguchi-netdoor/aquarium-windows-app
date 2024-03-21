namespace aquarium_windows_api.Response
{
    public class CheckAmountResponse
    {
        private readonly bool success;
        private readonly int? amount;
        private readonly string? errorFunction;
        private readonly int? errorCode;
        private readonly int? errorCodeExtended;

        private CheckAmountResponse(bool success, int? amount, string? errorFunction, int? errorCode, int? errorCodeExtended)
        {
            this.success = success;
            this.amount = amount;
            this.errorFunction = errorFunction;
            this.errorCode = errorCode;
            this.errorCodeExtended = errorCodeExtended;
        }


        public static CheckAmountResponse Success(int amount)
        {

            return new CheckAmountResponse(success: true, amount: amount, errorFunction: null, errorCode: null, errorCodeExtended: null);
        }

        public static CheckAmountResponse Error(string errorFunction, int errorCode, int? errorCodeExtended)
        {

            return new CheckAmountResponse(success: false, amount: null, errorFunction: errorFunction, errorCode: errorCode, errorCodeExtended: errorCodeExtended);
        }

        public object ToJson()
        {
            return new
            {
                this.success,
                this.amount,
                this.errorFunction,
                this.errorCode,
                this.errorCodeExtended
            };
        }
    }
}