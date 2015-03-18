using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HighFlyers.Communication
{
    enum DataFormat { Ascii, Hex, Dec }

    class StringParser
    {
        static readonly Dictionary<char, byte> SpChArray = new Dictionary<char, byte>();

        static StringParser()
        {
            SpChArray.Add('n', 10);
            SpChArray.Add('r', 13);
            SpChArray.Add('0', 9);
            SpChArray.Add('\\', 92);
        }

        public static byte? SpecialCharacter(char sign)
        {
            if (SpChArray.ContainsKey(sign))
                return SpChArray[sign];
            
            return null;
        }

        private static string GetFormattedValue(DataFormat p, byte b)
        {
            switch (p)
            {
                case DataFormat.Hex:
                    return "{x" + b.ToString("X2") + "}";

                case DataFormat.Dec:
                    return "{d" + b.ToString("D3") + "}";

                case DataFormat.Ascii:
                    return Convert.ToChar(b).ToString(CultureInfo.InvariantCulture);

                default:
                    return String.Empty;
            }
        }


        public static string ByteToDisplay(byte[] byteArray, DataFormat p = DataFormat.Ascii)
        {
            var text = new StringBuilder(byteArray.Length);

            foreach (byte t in byteArray)
            {
                text.Append(GetFormattedValue(p, t));
            }

            return text.ToString();
        }

        public static byte[] StrToByteArray(string str)
        {
            var conBytes = new List<byte>();
            int i;

            for (i = 0; i < str.Length; i++)
            {
                if (str[i] == '\\')
                {
                    if (i + 1 == str.Length)
                        break;

                    switch (str[i + 1])
                    {
                        case 'x':
                            if (i + 3 == str.Length)
                                break;

                            conBytes.Add(Convert.ToByte(str.Substring(i + 2, 2), 16));
                            i += 3;
                            break;
                        case 'd':
                            if (i + 4 == str.Length)
                                break;

                            conBytes.Add(Convert.ToByte(str.Substring(i + 2, 3), 10));
                            i += 4;
                            break;

                        default:
                            byte? spec = SpecialCharacter(str[++i]);

                            if (spec != null)
                                conBytes.Add(spec.Value);

                            break;
                    }
                }
                else
                    conBytes.Add(Convert.ToByte(str[i]));
            }

            if (i != str.Length)
                throw new Exception("Nie można sparsować stringa.");

            return conBytes.ToArray();
        }

    }
}
