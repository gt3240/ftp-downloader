using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AdminImporter
{
    class Program
    {
        static void Main()
        {
            string filename = "INSERT FTP ADDRESS HERE";
            string username = "USERNAME";
            string password = "PASSWORD";
            String downloadDate = DateTime.Now.ToString("dd.MM.yyyy");
            string destinationFolder = @"C:\downloads\" + downloadDate ;
            string destination = destinationFolder + "/FileName.zip";
     
            int progress = 0;
            double progressP = 0;

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            FtpWebRequest frequest = (FtpWebRequest)WebRequest.Create(filename);
            frequest.Method = WebRequestMethods.Ftp.GetFileSize;
            frequest.Credentials = new NetworkCredential(username, password);
            frequest.UsePassive = true;
            frequest.UseBinary = true;
            frequest.KeepAlive = true;
            int dataLength = (int)frequest.GetResponse().ContentLength;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filename);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);
            request.UseBinary = true;

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream rs = response.GetResponseStream())
                {
                    using (FileStream ws = new FileStream(destination, FileMode.Create))
                    {
                        byte[] buffer = new byte[2048];
                        int bytesRead = rs.Read(buffer, 0, buffer.Length);

                        Console.WriteLine("Downloading {0} ", filename);
                        while (bytesRead > 0)
                        {
                            ws.Write(buffer, 0, bytesRead);
                            bytesRead = rs.Read(buffer, 0, buffer.Length);
                            progress += bytesRead;
                            progressP = (double)progress / dataLength * 100;
                         
                            Console.Write("\r{0:N2}%   ", progressP);
                        }
                    }
                }
            }

            Console.WriteLine("Download completed on {0:HH:mm:ss tt}", DateTime.Now);
            Console.WriteLine("Extracting file...");

            ZipFile.ExtractToDirectory(destination, destinationFolder);

            Console.WriteLine("File extracted!  =)");
            Console.ReadLine();

        }
    }
}
