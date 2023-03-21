using System;
using System.Globalization;
using System.Numerics;

namespace Etherscan.WorkerService.Utils
{
    public class Helper
    {
        public static decimal HexadecimalToBigNumber(string hex)
        {
            try
            {
                hex = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? hex.Substring(2) : hex;
                BigInteger number = BigInteger.Parse($"0{hex}", NumberStyles.AllowHexSpecifier);
                return (decimal)number;
            }
            catch (Exception)
            {

                throw;
            }
            
            //return long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public static int HexadecimalToInt(string hex)
        {
            return Convert.ToInt32(hex, 16);
            //return int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
