using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantClient
{
    public static class CurrentUser
    {
        public static int Id { get; set; }
        public static string Username { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string FullName { get; set; } = "";
        public static string Role { get; set; } = "";
    }
}
