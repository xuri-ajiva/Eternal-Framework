using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace System.Ftp {
    public class FtpClinet {

        public string _message;
        public string _Name;
        public string _Base64Password;
        public string _File;
        public string _FtpAddres;
        public int _BufferLength;


        public FtpClinet(string FtpAddres, string Name, string Base64Password, string file, int BufferLength) {
            _BufferLength = BufferLength;
            _Name = Name;
            _FtpAddres = FtpAddres;
            _File = file;
            _Base64Password = Base64Password;
        }
        public void upload() {
            upload( _FtpAddres, _Name, _Base64Password, _File, _BufferLength );
        }
        public void upload(string FtpAddres, string Name, string Base64Password, string file, int BufferLength) {
            var currentpos = Console.CursorLeft;
            _message = "Uploadeding... ";
            var dt = DateTime.Now;
            var request =
    (FtpWebRequest) WebRequest.Create( FtpAddres );
            request.Credentials = new NetworkCredential( Name, Encoding.UTF8.GetString( Convert.FromBase64String( Base64Password ) ) );
            request.Method = WebRequestMethods.Ftp.UploadFile;

            using (Stream fileStream = File.OpenRead( file ))
            using (var ftpStream = request.GetRequestStream()) {
                var buffer = new byte[BufferLength];
                int read;
                while (( read = fileStream.Read( buffer, 0, buffer.Length ) ) > 0) {
                    ftpStream.Write( buffer, 0, read );


                    _message = "Uploaded " + fileStream.Position + " bytes";
                }
            }
            _message = $"Finished in {( DateTime.Now - dt )}!";
        }
        public void download() {
            download( _FtpAddres, _Name, _Base64Password, _File, _BufferLength );
        }
        public void download(string FtpAddres, string Name, string Base64Password, string file, int BufferLength) {
            var currentpos = Console.CursorLeft;
            Console.Write( "Downloaded " );
            var dt = DateTime.Now;
            var request =
    (FtpWebRequest) WebRequest.Create( FtpAddres );
            request.Credentials = new NetworkCredential( Name, Encoding.UTF8.GetString( Convert.FromBase64String( Base64Password ) ) );
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (var ftpStream = request.GetResponse().GetResponseStream())
            using (Stream fileStream = File.Create( file )) {
                var buffer = new byte[BufferLength];
                int read;
                while (( read = ftpStream.Read( buffer, 0, buffer.Length ) ) > 0) {
                    fileStream.Write( buffer, 0, read );
                    Console.SetCursorPosition( currentpos + 11, Console.CursorTop );
                    Console.Write( "                                " );
                    Console.SetCursorPosition( currentpos + 11, Console.CursorTop );
                    Console.Write( "{0} bytes", fileStream.Position );
                }
            }
            Console.WriteLine( "\nFinished in {0}!", ( DateTime.Now - dt ) );
        }
    }
}
