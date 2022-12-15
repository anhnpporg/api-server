using Spire.Barcode;
using System;
using System.Drawing;

namespace UtNhanDrug_BE.Hepper.GenaralBarcode
{
    public class GenaralBarcode
    {
        public static string CreateEan13Product(string barcode)
        {
            string temp = $"{barcode}";
            do
            {
                temp = "0" + temp;
            } while (temp.Length < 9);
            int sum = 0;
            // Calculate the checksum digit here.
            for (int i = temp.Length; i >= 1; i--)
            {
                int digit = Convert.ToInt32(temp.Substring(i - 1, 1));
                // This appears to be backwards but the 
                // EAN-13 checksum must be calculated
                // this way to be compatible with UPC-A.
                if (i % 2 == 0)
                { // odd  
                    sum += digit * 3;
                }
                else
                { // even
                    sum += digit * 1;
                }
            }
            int checkSum = (10 - (sum % 10)) % 10;
            return $"PRO{temp}{checkSum}";
        }
        public static string CreateEan13Batch(string barcode)
        {
            string temp = $"{barcode}";
            do
            {
                temp = "0" + temp;
            } while (temp.Length < 9);
            int sum = 0;
            // Calculate the checksum digit here.
            for (int i = temp.Length; i >= 1; i--)
            {
                int digit = Convert.ToInt32(temp.Substring(i - 1, 1));
                // This appears to be backwards but the 
                // EAN-13 checksum must be calculated
                // this way to be compatible with UPC-A.
                if (i % 2 == 0)
                { // odd  
                    sum += digit * 3;
                }
                else
                { // even
                    sum += digit * 1;
                }
            }
            int checkSum = (10 - (sum % 10)) % 10;
            return $"BAT{temp}{checkSum}";
        }

        public static string CreateEan13Invoice(string barcode)
        {
            string temp = $"{barcode}";
            do
            {
                temp = "0" + temp;
            } while (temp.Length < 9);
            int sum = 0;
            // Calculate the checksum digit here.
            for (int i = temp.Length; i >= 1; i--)
            {
                int digit = Convert.ToInt32(temp.Substring(i - 1, 1));
                // This appears to be backwards but the 
                // EAN-13 checksum must be calculated
                // this way to be compatible with UPC-A.
                if (i % 2 == 0)
                { // odd  
                    sum += digit * 3;
                }
                else
                { // even
                    sum += digit * 1;
                }
            }
            int checkSum = (10 - (sum % 10)) % 10;
            return $"INV{temp}{checkSum}";
        }

        public static string CreateEan13GIN(string barcode)
        {
            string temp = $"{barcode}";
            do
            {
                temp = "0" + temp;
            } while (temp.Length < 9);
            int sum = 0;
            // Calculate the checksum digit here.
            for (int i = temp.Length; i >= 1; i--)
            {
                int digit = Convert.ToInt32(temp.Substring(i - 1, 1));
                // This appears to be backwards but the 
                // EAN-13 checksum must be calculated
                // this way to be compatible with UPC-A.
                if (i % 2 == 0)
                { // odd  
                    sum += digit * 3;
                }
                else
                { // even
                    sum += digit * 1;
                }
            }
            int checkSum = (10 - (sum % 10)) % 10;
            return $"GIN{temp}{checkSum}";
        }

    }
}
