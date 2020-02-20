using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpClient.Connect("192.168.1.46", 8001);

                Console.WriteLine("Connected");

                string fileName = @"C:\Users\Edward\Desktop\alfie.jpg";
                tcpClient.Client.SendFile(fileName);

                NetworkStream networkStream = tcpClient.GetStream();

                networkStream = tcpClient.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();

                byte[] response = new byte[tcpClient.ReceiveBufferSize];
                networkStream.Read(response, 0, tcpClient.ReceiveBufferSize);

               String returnData = Encoding.UTF8.GetString(response);
              Console.WriteLine("Server Response " + returnData);

                tcpClient.Close();
                networkStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }

            Thread thread = new Thread(new ThreadStart(WorkThreadFunction));
            thread.Start();

            Console.ReadKey();
        }

        private static void WorkThreadFunction()
        {
            try
            {
                Console.WriteLine("File watcher thread started");
                string directory = @"C:\Users\Edward\Desktop\client";
                MonitorDirectory(directory);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        private static void MonitorDirectory(string path)
        {

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = path;

            fileSystemWatcher.Created += FileSystemWatcher_Created;

            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;

            fileSystemWatcher.EnableRaisingEvents = true;

        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File created: {0}", e.Name);
        }

        private static void FileSystemWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File renamed: {0}", e.Name);
        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("File deleted: {0}", e.Name);
        }
    }
}
