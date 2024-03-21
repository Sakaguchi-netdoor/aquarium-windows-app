namespace aquarium_windows_api.Request
{
    public class PrintReceiptRequest
    {
        public string EncodedImage { get; set; }
        public bool IsCut { get; set; }
    }
}