using System;

namespace IBISutilities
{
    class IBISutils
    {
        #region ReplaceIbisSC
        /// <summary>
        /// Replace IBIS special characters
        /// replaces german oumlauts in an IBIS message with special characters
        /// </summary>
        /// <param name="input">IBIS message string</param>
        public string ReplaceIbisSC(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == 'ä') chars[i] = '{';
                else if (chars[i] == 'ö') chars[i] = '|';
                else if (chars[i] == 'ü') chars[i] = '}';
                else if (chars[i] == 'Ä') chars[i] = '[';
                else if (chars[i] == 'Ö') chars[i] = '\\';
                else if (chars[i] == 'Ü') chars[i] = ']';
                else if (chars[i] == 'ß') chars[i] = '~';
            }
            return new string(chars);
        }
        #endregion

        #region GetIbisParity
        /// <summary>
        /// Calculates the IBIS parity byte 
        /// initiatilize parity byte with 0x7F and 
        /// make an exor operation with every single IBIS message byte
        /// </summary>
        /// <param name="input">IBIS message string</param>
        public char GetIbisParity(string input)
        {
            byte p = 0x7F;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(input);

            for (int i = 0; i < bytes.Length; i++)
            {
                p ^= bytes[i];
            }
            return (char)p;
        }
        #endregion

        #region EmulateEvenParity
        /// <summary>
        /// Emulates 7 Bit Even Parity in an IBIS message
        /// by calulatig parity bit
        /// and shifting it into the 8th databit
        /// </summary>
        /// <param name="input">IBIS message string</param>
        public byte[] EmulateEvenParity(string input)
        {
            int cnt;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(input);

            for (int i = 0; i < bytes.Length; i++)
            {
                cnt = GetParity(bytes[i]);
                if (cnt % 2 == 1)  // even parity
                {
                    bytes[i] |= (byte)0x80;
                }
                else              // odd parity
                {
                    bytes[i] &= (byte)0x7F;
                }
            }
            return bytes;
        }
        #endregion

        #region GetParity
        /// <summary>
        /// Calulatig parity bit in a singe byte
        /// </summary>
        /// <param name="input">single 7-Bit bata byte</param>
        public int GetParity(byte input)
        {
            int cnt = 0;
            for (int i = 0; i < 7; i++)
            {
                if ((input & (1 << i)) > 0)
                    cnt++;
            }
            return cnt;
        }
        #endregion

        #region dec2hexString
        /// <summary>
        /// Convert a decimal string into a special IBIS hex string
        /// by calulatig parity bit
        /// and shifting it into the 8th databit
        /// </summary>
        /// <param name="s">decimal string</param>
        public string dec2hexString(string s)
        {
            int x = 0;
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            Int32.TryParse(s, out x);
            string xstr = x.ToString("X4");
            char[] chars = xstr.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == 'A') chars[i] = ':';
                if (chars[i] == 'B') chars[i] = ';';
                if (chars[i] == 'C') chars[i] = '<';
                if (chars[i] == 'D') chars[i] = '=';
                if (chars[i] == 'E') chars[i] = '>';
                if (chars[i] == 'F') chars[i] = '?';
            }
            return new string(chars);
        }
        #endregion

        #region fillDecimalString
        /// <summary>
        /// Fill a decimal string with leading '0's
        /// </summary>
        /// <param name="s">decimal string</param>
        /// <param name="desiredLength">desired string length</param>
        public string fillDecimalString(string s, int desiredLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.Length >= desiredLength) return s;
            return s.PadLeft(desiredLength, '0');
        }
        #endregion

        #region leftalignedString
        /// <summary>
        /// left align a string with leading spaces
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="desiredLength">desired string length</param>
        public string leftalignedString(string s, int desiredLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.Length >= desiredLength) return s;
            return s.PadRight(desiredLength);
        }
        #endregion

        #region centerString
        /// <summary>
        /// center a string with leading and preceding spaces 
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="desiredLength">desired string length</param>
        public string centerString(string s, int desiredLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (s.Length >= desiredLength) return s;
            int firstpad = (s.Length + desiredLength) / 2;
            return s.PadLeft(firstpad).PadRight(desiredLength);
        }
        #endregion

        #region replaceString
        /// <summary>
        /// Replace a character in a string with a different character
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="csrc">character to replace</param>
        /// <param name="dsrc">replacement character</param>
        public string replaceString(string input, char csrc, char cdst)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == csrc) chars[i] = cdst;
            }
            return new string(chars);
        }
        #endregion
    }
}
