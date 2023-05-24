using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace XML
{
    [Serializable]
    public class XML
    {
        public String str;
        public string[] strs;
        public int[][] ints;
        public Sub subc;
        [XmlIgnore]
        public Bitmap pic;
        [XmlElement("Bitmap")]
        public byte[] bitmapFieldSerialized
        {
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pic.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
            set
            {
                MemoryStream ms = new MemoryStream(value);
                pic = new Bitmap(ms);
            }
        }
        public XML() { }
        public XML(String str, string[] strs, int[][] integer,
            Sub cl, Bitmap photo)
        {
            this.str = str;
            this.strs = strs;
            this.ints = integer;
            subc = cl;
            pic = photo;
        }
    }
    [Serializable]
    public class Sub
    {
        public int integer;
        public double doub;

        public Sub() { }
        public Sub(int integer, double doub)
        {
            this.integer = integer;
            this.doub = doub;
        }
    }
}
