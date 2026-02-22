using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkUp.Core.Application.Helpers
{
    public static class PasswordEncryptation
    {
        public static string Hash(string input) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));
        public static string Unhash(string input) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }
}
