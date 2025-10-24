using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace RestaurantClient
{
    public static class DatabaseConnection
    {
        // Thay đổi chuỗi kết nối này!
        private static readonly string ConnectionString =
            "Data Source=RestaurantServer;Initial Catalog=QLQuanAn";

        // Hàm tĩnh để tạo và trả về đối tượng kết nối
        public static SqlConnection GetConnection()
        {
            try
            {
                return new SqlConnection(ConnectionString);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi kết nối database ở đây
                MessageBox.Show("Không thể tạo kết nối Database: " + ex.Message, "Lỗi Kết Nối");
                return null;
            }
        }
    }
}
