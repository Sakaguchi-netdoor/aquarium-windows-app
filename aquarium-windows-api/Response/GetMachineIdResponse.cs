namespace aquarium_windows_api.Response
{
    class GetMachineIdResponse
    {
        private readonly bool success;
        private readonly string? macAddress;

        private GetMachineIdResponse(bool success, string? macAddress)
        {
            this.success = success;
            this.macAddress = macAddress;
        }

        public static GetMachineIdResponse Success(string macAddress)
        {

            return new GetMachineIdResponse(success: true, macAddress: macAddress);
        }

        public static GetMachineIdResponse Error()
        {

            return new GetMachineIdResponse(success: false, macAddress: null);
        }

        public object ToJson()
        {
            return new
            {
                this.success,
                this.macAddress
            };
        }
    }
}