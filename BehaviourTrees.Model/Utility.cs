using System;
using System.Text;

namespace BehaviourTrees.Model
{
    internal static class Utility
    {
        private static readonly Random Random = new Random();
        internal static string CreateShortId()
        {
            while (true)
            {
                var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
                var sb = new StringBuilder(6);

                for (var i = 0; i < 6; i++) sb.Append(chars[Random.Next(62)]);

                return sb.ToString();
            }
        }
    }
}