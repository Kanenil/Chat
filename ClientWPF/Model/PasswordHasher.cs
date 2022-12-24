﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Model
{
    public class PasswordHasher
    {
        public static string Hash(string password)
        {
            MD5 mD5 = MD5.Create();

            byte[] b = Encoding.UTF8.GetBytes(password);
            byte[] hash = mD5.ComputeHash(b);

            StringBuilder sb = new StringBuilder();
            foreach (var a in hash)
                sb.Append(a.ToString("X2"));

            return Convert.ToString(sb);
        }
    }
}
