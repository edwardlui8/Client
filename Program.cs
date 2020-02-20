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

                Console.WriteLine("Connecting to server...");

                tcpClient.Connect("192.168.1.46", 8001);

                Console.WriteLine("Client running on: " + tcpClient.Client.LocalEndPoint);
                Console.WriteLine("Connected to: " + tcpClient.Client.RemoteEndPoint);

                string filePath = @"C:\Users\Edward\Desktop\alfie.jpg";

                // Filename [1]
                string fileName = Path.GetFileName(filePath);
                byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName);
                Console.WriteLine("\nFile name {0}", fileName);

                // Filename length [0]
                int fileNameLength = fileName.Length;
                byte fileNameLengthByte = Convert.ToByte(fileNameLength);

                // File [n]
                byte[] fileBytes = File.ReadAllBytes(filePath);
                Console.WriteLine("\nFile size {0}", fileBytes.Length);

                // Combined bytes
                byte[] combinedBytes = AddByteToArray(Combine(fileNameBytes, fileBytes), fileNameLengthByte);

                // Send combined bytes
                NetworkStream networkStream = tcpClient.GetStream();
                networkStream.Write(combinedBytes, 0, combinedBytes.Length);
                Console.WriteLine("\nTotal bytes sent {0}", combinedBytes.Length);

                tcpClient.Close();
                networkStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
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
                Console.WriteLine("Error: " + e.StackTrace);
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

        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        private static byte[] AddByteToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
    }
}
