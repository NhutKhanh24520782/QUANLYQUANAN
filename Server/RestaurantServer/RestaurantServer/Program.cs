namespace RestaurantServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start(5000); // Cổng TCP
            Console.WriteLine("✅ Server đang chạy tại cổng 5000...");
            Console.WriteLine("Nhấn Enter để dừng server...");
            Console.ReadLine();
        }
    }
}
