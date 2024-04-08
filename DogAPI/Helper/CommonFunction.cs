using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DogAPI.Helper
{
    public class CommonFunction
    {
        public string GenrateRandomNo()
        {
            try
            {
                string _allowedChars = "123456789";
                Random randNum = new Random();
                char[] chars = new char[4];
                int allowedCharCount = _allowedChars.Length;
                for (int i = 0; i < 4; i++)
                {
                    chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
                }
                return new string(chars);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}