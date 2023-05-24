using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace XML
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public XML xml;

        private void button1_Click(object sender, EventArgs e) //write
        {
            
            String str = textBox1.Text;
            string[] strs = { textBox2.Text, textBox3.Text };
            int[][] ints = new int[2][];
            ints[0] = new int[2] { (int)num1.Value, (int)num2.Value };
            ints[1] = new int[2] { (int)num3.Value, (int)num4.Value };
            Sub subc = new Sub((int)num5.Value, (double)num6.Value);
            Bitmap pic = (Bitmap)pictureBox1.Image;
            xml = new XML(str, strs, ints, subc, pic);

            if (rbSerial.Checked)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(XML));
                using (var fs = new FileStream("XMLSerializer.xml", FileMode.Create))
                    xmlSerializer.Serialize(fs, xml);
                MessageBox.Show("Данные успешно записаны в файл \"XMLSerializer.xml\".", "XMLSerializer.xml", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                using (XmlWriter writer = XmlWriter.Create("XmlWriterReader.xml", new XmlWriterSettings() { Indent = true }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("XmlWriterReader");
                    writer.WriteAttributeString("writer", "true");

                    writer.WriteElementString("String", xml.str);

                    writer.WriteStartElement("Strings");
                    for(int i =0;i<2;i++)
                        writer.WriteElementString("String", xml.strs[i]);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Intss");
                    for (int i = 0; i < 2; i++)
                    {
                        writer.WriteStartElement("Ints");

                        for (int j = 0; j < 2; j++)
                            writer.WriteElementString("Int", xml.ints[i][j].ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("Sub");
                    writer.WriteElementString("Int", xml.subc.integer.ToString());
                    writer.WriteElementString("Double", xml.subc.doub.ToString());
                    writer.WriteEndElement();

                    writer.WriteElementString("Bitmap", Convert.ToBase64String(xml.bitmapFieldSerialized));
                    
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                MessageBox.Show("Данные успешно записаны в файл \"XmlWriterReader.xml\".", "XmlWriterReader.xml", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e) //read
        {
            if (rbSerial.Checked)
            {
                XmlSerializer serial = new XmlSerializer(typeof(XML));
                using (FileStream fs = new FileStream("XMLSerializer.xml", FileMode.OpenOrCreate))
                    xml = (XML)serial.Deserialize(fs);
            }
            else
            {
                using (XmlReader reader = XmlReader.Create("XmlWriterReader.xml"))
                {
                    /*xml = new XML();
                    xml.strs = new string[2];
                    xml.ints = new int[2][];
                    xml.ints[0] = new int[2];
                    xml.ints[1] = new int[2];*/

                    String str = textBox1.Text;
                    string[] strs = { textBox2.Text, textBox3.Text };
                    int[][] ints = new int[2][];
                    ints[0] = new int[2] { (int)num1.Value, (int)num2.Value };
                    ints[1] = new int[2] { (int)num3.Value, (int)num4.Value };
                    Sub subc = new Sub((int)num5.Value, (double)num6.Value);
                    Bitmap pic = (Bitmap)pictureBox1.Image;
                    xml = new XML(str, strs, ints, subc, pic);

                    reader.MoveToContent();
                    while (reader.Read())
                    {
                        if (reader.Name == "String")
                            xml.str = reader.ReadInnerXml();
                        else if (reader.Name == "Strings")
                        {
                            using (XmlReader subreader = reader.ReadSubtree())
                            {
                                if (subreader.ReadToDescendant("String"))
                                    xml.strs[0] = subreader.ReadInnerXml();
                                subreader.Read();
                                if (subreader.Name == "String")
                                    xml.strs[1] = subreader.ReadInnerXml();
                            }
                        }
                        else if (reader.Name == "Intss")
                        {
                            using (XmlReader subreader = reader.ReadSubtree())
                            {
                                if (subreader.ReadToDescendant("Ints"))
                                {
                                    using (XmlReader ssubreader = reader.ReadSubtree())
                                    {
                                        if (ssubreader.ReadToDescendant("Int"))
                                        {
                                            xml.ints[0][0] = Convert.ToInt32(ssubreader.ReadInnerXml());
                                            ssubreader.Read();
                                            xml.ints[0][1] = Convert.ToInt32(ssubreader.ReadInnerXml());
                                        }
                                    }
                                }
                                subreader.Read();
                                subreader.Read();
                                if (subreader.Name == "Ints")
                                {
                                    using (XmlReader ssubreader = reader.ReadSubtree())
                                    {
                                        if (ssubreader.ReadToDescendant("Int"))
                                        {
                                            xml.ints[1][0] = Convert.ToInt32(ssubreader.ReadInnerXml());
                                            ssubreader.Read();
                                            xml.ints[1][1] = Convert.ToInt32(ssubreader.ReadInnerXml());
                                        }
                                    }
                                }
                            }
                        }
                        else if (reader.Name == "Sub")
                        {
                            using (XmlReader subreader = reader.ReadSubtree())
                            {
                                if (subreader.ReadToDescendant("Int"))
                                    xml.subc.integer = Convert.ToInt32(subreader.ReadInnerXml());
                                subreader.Read();
                                if (subreader.Name == "Double")
                                    xml.subc.doub = Convert.ToDouble(subreader.ReadInnerXml());
                            }
                        }
                        else if (reader.Name == "Bitmap")
                            xml.bitmapFieldSerialized = Convert.FromBase64String(reader.ReadInnerXml());
                    }
                }
            }

            textBox1.Text = xml.str;
            textBox2.Text = xml.strs[0];
            textBox3.Text = xml.strs[1];
            num1.Value = (decimal)xml.ints[0][0];
            num2.Value = (decimal)xml.ints[0][1];
            num3.Value = (decimal)xml.ints[1][0];
            num4.Value = (decimal)xml.ints[1][1];
            num5.Value = (decimal)xml.subc.integer;
            num6.Value = (decimal)xml.subc.doub;
            pictureBox1.Image = xml.pic;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Изображкние (*.png;*.jpg)|*.png;*.jpg",
                Title = "Выберите изображение"
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            else
                pictureBox1.Image = new Bitmap(ofd.FileName);
        }
    }
}
