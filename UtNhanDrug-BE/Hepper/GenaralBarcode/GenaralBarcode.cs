using System;

namespace UtNhanDrug_BE.Hepper.GenaralBarcode
{
    public class GenaralBarcode
    {
        public static string CreateBarcode()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}
