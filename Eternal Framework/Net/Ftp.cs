using System;
using System.IO;
using System.Net;
using System.Text;

namespace Eternal.Net {
    public class FtpClinet : EternalFramework.EternalMain {
        public string Message;
        public string Name;
        public string Base64Password;
        public string File;
        public string FtpAddres;
        public int    BufferLength;

        public FtpClinet(string ftpAddres, string name, string base64Password, string file, int bufferLength) {
            this.BufferLength   = bufferLength;
            this.Name           = name;
            this.FtpAddres      = ftpAddres;
            this.File           = file;
            this.Base64Password = base64Password;
        }

        public void Upload() { Upload( this.FtpAddres, this.Name, this.Base64Password, this.File, this.BufferLength ); }

        public void Upload(string ftpAddres, string name, string base64Password, string file, int bufferLength) {
            this.Message = "Uploadeding... ";
            var dt      = DateTime.Now;
            var request = (FtpWebRequest) WebRequest.Create( ftpAddres );
            request.Credentials = new NetworkCredential( name, Encoding.UTF8.GetString( Convert.FromBase64String( base64Password ) ) );
            request.Method      = WebRequestMethods.Ftp.UploadFile;

            using ( Stream fileStream = System.IO.File.OpenRead( file ) )
                using ( var ftpStream = request.GetRequestStream() ) {
                    var buffer = new byte[bufferLength];
                    int read;
                    while ( ( read = fileStream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
                        ftpStream.Write( buffer, 0, read );

                        this.Message = "Uploaded " + fileStream.Position + " bytes";
                    }
                }

            this.Message = $"Finished in {( DateTime.Now - dt )}!";
        }

        public void Download() { Download( this.FtpAddres, this.Name, this.Base64Password, this.File, this.BufferLength ); }

        public void Download(string ftpAddres, string name, string base64Password, string file, int bufferLength) {
            var currentpos = Console.CursorLeft;
            this.Message = ( "Downloaded " );
            var dt      = DateTime.Now;
            var request = (FtpWebRequest) WebRequest.Create( ftpAddres );
            request.Credentials = new NetworkCredential( name, Encoding.UTF8.GetString( Convert.FromBase64String( base64Password ) ) );
            request.Method      = WebRequestMethods.Ftp.DownloadFile;

            using ( var ftpStream = request.GetResponse().GetResponseStream() )
                using ( Stream fileStream = System.IO.File.Create( file ) ) {
                    var buffer = new byte[bufferLength];
                    int read;
                    while ( ftpStream != null && ( read = ftpStream.Read( buffer, 0, buffer.Length ) ) > 0 ) {
                        fileStream.Write( buffer, 0, read );

                        this.Message = "Downloaded " + fileStream.Position + " bytes";
                    }
                }

            this.Message = "Finished in " + ( DateTime.Now - dt ) + "!";
        }
    }
}