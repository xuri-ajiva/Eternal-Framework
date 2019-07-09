using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eternal.Settings {
    public class XMLOPERATION {
        public const string PathBase = ".\\DataSave\\";

        public static void SAVE(object dataLayout, string path) {
            System.IO.FileInfo file = new System.IO.FileInfo( path );
            file.Directory.Create();

            var        sr     = new XmlSerializer( dataLayout.GetType() );
            TextWriter writer = new StreamWriter( path );
            sr.Serialize( writer, dataLayout );
            writer.Close();
        }

        public static T LOAD <T>(string path) {
            if ( !File.Exists( path ) ) throw new TypeAccessException();

            var xs = new XmlSerializer( typeof(T) );
            var fs = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
            var r  = (T) xs.Deserialize( fs );
            fs.Close();
            return r;
        }
    }
}