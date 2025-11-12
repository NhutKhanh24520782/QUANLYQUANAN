using System.Text;

namespace RestaurantServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Server server = new Server();
            server.Start(5000); // Cổng TCP
            Console.WriteLine("✅ Server đang chạy tại cổng 5000...");
            //Console.WriteLine("Nhấn Enter để dừng server...");
            Console.ReadLine();
        }
    }
}
