using AS_ASSN2_Rachel.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using QRCoder;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OtpSharp;
using AS_ASSN2_Rachel.Helpers;
using System.Diagnostics;

namespace AS_ASSN2_Rachel.Pages
{
    public class Enable2FAModel : PageModel
    {
        public string QrCodeImage { get; set; } = string.Empty;

        public void OnGet()
        {
            var key = OtpSharp.KeyGeneration.GenerateRandomKey(20);
            string secretKey = Base32Encoding.ToBase32(key);
            string secretUri = $"otpauth://totp/AS_ASSN2_Rachel:missrach.lim@gmail.com?secret={secretKey}&issuer=AS_ASSN2_Rachel";

            QrCodeImage = GenerateQrCodeImage(secretUri);
        }

        private string GenerateQrCodeImage(string uri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeAsPngBytes = qrCode.GetGraphic(20);

            if (qrCodeAsPngBytes.Length == 0)
            {
                Debug.WriteLine("QR Code generation failed. No bytes generated.");
                return string.Empty;
            }

            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeAsPngBytes)}";
        }
    }
}
