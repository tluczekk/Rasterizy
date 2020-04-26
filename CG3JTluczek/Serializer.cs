using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CG3JTluczek
{
    public class Serializer
    {
        public RasterGraphicsWrapper Load(string filename)
        {
            RasterGraphicsWrapper temp = new RasterGraphicsWrapper();
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            temp = (RasterGraphicsWrapper)formatter.Deserialize(stream);
            stream.Close();
            return temp;
        }
        public void Save(string filename, RasterGraphicsWrapper ras)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, ras);
            stream.Close();
        }

    }
}
