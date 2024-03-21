using aquarium_windows_api.Request;
using aquarium_windows_api.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using NPrinterCLib;
using OposCashChanger_CCO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Text;
using Image = System.Drawing.Image;

namespace aquarium_windows_api.Controllers
{

    [ApiController]
    public class AquariumWindowsApiController : ControllerBase
    {
        private static readonly OPOSCashChanger opos = new();
        private static int totalAmount = 0;
        private static int depositAmount = 0;


        [Route("api/checkAmount")]
        [HttpGet]
        public object CheckAmount()
        {
            try
            {
                int ret = 0;
                if (opos == null)
                {
                    return CheckAmountResponse.Error(errorFunction: "SCAN_NOT_STARTED", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                return CheckAmountResponse.Success(amount: opos.DepositAmount).ToJson();
            }
            catch (Exception)
            {
                return CheckAmountResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
            }
        }


        [Route("api/getComputerMAC")]
        [HttpGet]
        public object GetComputerMAC()
        {
            String? macAddress = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(
                nic => nic.OperationalStatus == OperationalStatus.Up &&
                nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)?.GetPhysicalAddress().ToString();

            return new { computerMAC = macAddress };
        }

        [Route("api/startScan")]
        [HttpPost]
        public object StartScan()
        {
            try
            {
                int ret = 0;
                int resultCodeExtended = 0;

                opos.DeviceEnabled = false;
                opos.ReleaseDevice();
                opos.Close();

                ret = opos.Open("CashChanger");
                if (ret != 0)
                {
                    return StartScanResponse.Error(errorFunction: "Open", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }
                ret = opos.ClaimDevice(10000);
                if (ret != 0)
                {
                    return StartScanResponse.Error(errorFunction: "ClaimDevice", errorCode: ret, errorCodeExtended: resultCodeExtended).ToJson();
                }
                opos.DeviceEnabled = true;

                ret = opos.BeginDeposit();
                if (ret != 0)
                {
                    return StartScanResponse.Error(errorFunction: "BeginDeposit", errorCode: ret, errorCodeExtended: resultCodeExtended).ToJson();
                }
                return StartScanResponse.Success().ToJson();
            }
            catch (Exception)
            {
                return StartScanResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: null).ToJson();
            }
        }

        [Route("api/cancelCashier")]
        [HttpPost]
        public object CancelCashier()
        {
            try
            {
                int ret = 0;

                int ChanDepositrepay = 3;


                if (opos == null)
                {
                    return CancelCashierResponse.Error(errorFunction: "SCAN_NOT_STARTED", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                for (int i = 0; i < 5; i++)
                {
                    if (opos.ResultCodeExtended == 0)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }

                if (opos.ResultCodeExtended != 0)
                {
                    return CancelCashierResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                opos.CurrentExit = 1;

                ret = opos.FixDeposit();
                if (ret != 0)
                {
                    return CancelCashierResponse.Error(errorFunction: "FixDeposit", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }


                ret = opos.EndDeposit(ChanDepositrepay);
                if (ret != 0)
                {
                    return CancelCashierResponse.Error(errorFunction: "EndDeposit", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }


                opos.DeviceEnabled = false;

                ret = opos.ReleaseDevice();
                if (ret != 0)
                {
                    return CancelCashierResponse.Error(errorFunction: "ReleaseDevice", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                ret = opos.Close();
                if (ret != 0)
                {
                    return CancelCashierResponse.Error(errorFunction: "Close", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                return CancelCashierResponse.Success().ToJson();
            }
            catch (Exception)
            {
                return CancelCashierResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
            }
        }

        [Route("api/payByCashier")]
        [HttpPost]
        public object PayByCashier([FromBody] PayByCashierRequest Request)
        {
            try
            {
                int ret = 0;

                int ChanDepositChange = 1;

                if (opos == null)
                {
                    return PayByCashierResponse.Error(errorFunction: "SCAN_NOT_STARTED", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                ret = opos.FixDeposit();
                if (ret != 0)
                {
                    return PayByCashierResponse.Error(errorFunction: "FixDeposit", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                depositAmount = opos.DepositAmount;
                totalAmount = Request.TotalAmount;

                ret = opos.EndDeposit(ChanDepositChange);
                if (ret != 0)
                {
                    return PayByCashierResponse.Error(errorFunction: "EndDeposit", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                opos.CurrentExit = 1;

                ret = opos.DispenseChange(depositAmount - totalAmount);
                if (ret != 0)
                {
                    int resultCodeExtended = opos.ResultCodeExtended;
                    opos.DeviceEnabled = false;
                    opos.ReleaseDevice();
                    opos.Close();
                    return PayByCashierResponse.Error(errorFunction: "DispenseChange", errorCode: ret, errorCodeExtended: resultCodeExtended).ToJson();
                }

                opos.DeviceEnabled = false;

                ret = opos.ReleaseDevice();
                if (ret != 0)
                {
                    return PayByCashierResponse.Error(errorFunction: "ReleaseDevice", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                ret = opos.Close();
                if (ret != 0)
                {
                    return PayByCashierResponse.Error(errorFunction: "Close", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                depositAmount = 0;
                totalAmount = 0;

                return PayByCashierResponse.Success().ToJson();
            }
            catch (Exception)
            {
                return PayByCashierResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
            }
        }


        [Route("api/dispenseChange")]
        [HttpPost]
        public object DispenseChange()
        {
            try
            {
                int ret = 0;

                int ChanDepositChange = 1;

                ret = opos.DispenseChange(depositAmount - totalAmount);
                if (ret != 0)
                {
                    return DispenseChangeResponse.Error(errorFunction: "DispenseChange", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }


                opos.DeviceEnabled = false;

                ret = opos.ReleaseDevice();
                if (ret != 0)
                {
                    return DispenseChangeResponse.Error(errorFunction: "ReleaseDevice", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                ret = opos.Close();
                if (ret != 0)
                {
                    return DispenseChangeResponse.Error(errorFunction: "Close", errorCode: ret, errorCodeExtended: opos.ResultCodeExtended).ToJson();
                }

                return DispenseChangeResponse.Success().ToJson();
            }
            catch (Exception)
            {
                return DispenseChangeResponse.Error(errorFunction: "APPLICATION_ERROR", errorCode: 1000, errorCodeExtended: opos.ResultCodeExtended).ToJson();
            }
        }

        [Route("api/printReceipt")]
        [HttpPost]
        [SupportedOSPlatform("windows")]
        public object PrintReceipt([FromBody] PrintReceiptRequest Request)
        {
            try
            {
                int ret = 0;
                string directoryPath = @".\image";
                string filePathPng = Path.Combine(directoryPath, "image.png");
                string filePathBmp = Path.Combine(directoryPath, "image.bmp");
                Directory.CreateDirectory(directoryPath);
                using (var fs = new FileStream(filePathPng, FileMode.Create, FileAccess.Write))
                {
                    var bs = Convert.FromBase64String(Request.EncodedImage);
                    fs.Write(bs, 0, bs.Length);
                    fs.Close();
                }

                using (Image pngImage = Image.FromFile(filePathPng))
                {
                    using (Bitmap bmpImage = new Bitmap(pngImage.Width, pngImage.Height, PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphics = Graphics.FromImage(bmpImage))
                        {
                            graphics.DrawImage(pngImage, 0, 0);
                        }
                        bmpImage.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                        bmpImage.Save(filePathBmp, ImageFormat.Bmp);

                    }
                }

                NClassLib nei;
                nei = new NClassLib();
                string printerName = "NPI Integration Driver";
                ret = nei.NGetStatus(printerName, out long status);
                if (ret < 0)
                {
                    return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.sdk, errorCode: ret, errorMessage: null).ToJson();
                }

                if ((status & 0x7F) > 0 && (status & 0x7F) != 1)
                {
                    return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.status, errorCode: Convert.ToInt32(status & 0x7F), errorMessage: null).ToJson();
                }

                ret = nei.NStartDoc(printerName, out int aaa);
                if (ret != 0)
                {
                    nei.NCancelDoc(printerName);
                    return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.sdk, errorCode: ret, errorMessage: null).ToJson();
                }

                ret = nei.NImagePrintF(printerName, filePathBmp, 0x01, out int v);
                if (ret != 0)
                {
                    nei.NCancelDoc(printerName);
                    return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.sdk, errorCode: ret, errorMessage: null).ToJson();
                }

                if (Request.IsCut)
                {
                    byte[] byteArray = Encoding.ASCII.GetBytes("0a0a0a1b6e");
                    ret = nei.NPrint(printerName, byteArray, (uint)byteArray.Length, out int v2);
                    if (ret != 0)
                    {
                        nei.NCancelDoc(printerName);
                        return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.sdk, errorCode: ret, errorMessage: null).ToJson();
                    }
                }


                ret = nei.NEndDoc(printerName);
                if (ret != 0)
                {
                    nei.NCancelDoc(printerName);
                    return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.sdk, errorCode: ret, errorMessage: null).ToJson();
                }


                return PrintReceiptResponse.Success().ToJson();
            }
            catch (Exception e)
            {
                return PrintReceiptResponse.Error(errorType: PrintReceiptResponse.ErrorType.app, errorCode: 1, errorMessage: e.ToString()).ToJson();
            }

        }

    }
}
