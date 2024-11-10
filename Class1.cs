
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Nahida.LibIrminsul
{
    /// <summary>
    /// IrminsulFile
    /// </summary>
    public struct IrminsulFile
    {
        public string type;//file type
        public List<string> path;//path,without filename
        public string name;//file name
        public string description;//description
        public string tag;//tag
        public long length;//file len,8bytes
    };
    
    public class Irminsul//主要函数，主要操作（读写）
    {
        public static string ver = "v0.1.1";
        
        /// <summary>
        /// 写入已格式化的Irminsul区块
        /// </summary>
        /// <param name="ir">IrminsulFile元数据</param>
        /// <param name="offset">目标文件的offset，允许任意值</param>
        /// <param name="rawfile">原始文件</param>
        /// <param name="target">目标irminsul文件流</param>
        /// <returns>bool：true为成功，false为失败</returns>
        public static bool WriteStream(IrminsulFile ir,long offset,string rawfile,string target)
        {
            try
            {
                using (FileStream fs = new FileStream(target, FileMode.Append, FileAccess.Write))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Write(Irminsul.Info.WriteBytes(ir));

                int splitcount = (int)(new FileInfo(rawfile).Length / 1048576);
                for (long j = 0; j <= splitcount; j++)
                {
                    fs.Write(FileSplit(rawfile, j * 1048576, 1048576));
                }
                    
                        
                    
                }
                return true;
            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="rawfile"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ReadStream(long offset, long length, string rawfile, string target)
        {
            try
            {

                int splitcount = (int)(length / 1048576);
                using (FileStream fs = new FileStream(target, FileMode.Create, FileAccess.Write))
                {
                    FileStream fs1 = new FileStream(rawfile, FileMode.Open, FileAccess.Read);

                    long Startseek = offset;
                    int SplitSize = 1048576;

                    fs1.Seek(Startseek, SeekOrigin.Begin);// 定位
                    for (long j = 0; j <= splitcount; j++)
                    {
                        long remainsize = length - (j* SplitSize);
                        if (SplitSize <= remainsize)
                        {
                            byte[] datab = new byte[SplitSize];
                            fs1.Read(datab);
                            fs.Write(datab);
                        }
                        else
                        {
                            byte[] datac = new byte[remainsize];
                            fs1.Read(datac);
                            fs.Write(datac);
                        }
                    }
                    fs1.Close();

                }
                return true;
            }

            catch
            {

            return false; }

        }
        public static byte[] FileSplit(string FileName, Int64 Startseek, Int32 SplitSize)//Finish!
        {
            //read
            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            fs.Seek(Startseek, SeekOrigin.Begin);// 定位
            long remainsize = fs.Length - Startseek;
            if (SplitSize <= remainsize)
            {
                byte[] datab = new byte[SplitSize];
                fs.Read(datab, 0, datab.Length);
                fs.Close();
                return datab;
            }
            else
            {
                byte[] datac = new byte[remainsize];
                fs.Read(datac, 0, datac.Length);
                fs.Close();
                return datac;
            }

        }
        /// <summary>
        /// 创建一个新的空白irminsul文件（有功能缺陷，等待完善）
        /// </summary>
        /// <param name="target">目标路径</param>
        /// <returns>bool：true为成功，false为失败</returns>
        public static bool CreateStream(string target)
        {
            try
            {

                using (FileStream fs = new FileStream(target, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(new byte[] {0x49,0x72,0x6D,0x69,0x6E,0x73,0x75,0x6C,0x53,0x74,0x72,0x65,0x61,0x6D,0x00,0xFF,
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x0d,0x00,0x07,0x21,
0x49,0x56,0x65,0x72,0x73,0x69,0x6F,0x6E,0x00,0x00 });
                    fs.Write(System.Text.Encoding.Default.GetBytes(Irminsul.ver));
                    return true;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 创建整个Irminsul流的元数据xml。
        /// </summary>
        /// <param name="author">作者</param>
        /// <param name="time">发布日期</param>
        /// <param name="filecount">文件总数</param>
        /// <param name="desc">描述</param>
        /// <returns>info的byte数组</returns>
        public static byte[] CreateInfo(string author,string time,string filecount,string desc)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclar;
                xmlDeclar = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDeclar);

                XmlElement xmlElement = xmlDoc.CreateElement("", "Data", "");
                xmlDoc.AppendChild(xmlElement);


                XmlNode root = xmlDoc.SelectSingleNode("Data");
                XmlElement xe1 = xmlDoc.CreateElement("Author");
                xe1.InnerText = author;
                XmlElement xe11 = xmlDoc.CreateElement("Time");
                xe11.InnerText = time;
                XmlElement xe111 = xmlDoc.CreateElement("Count");
                xe111.InnerText = filecount;
                XmlElement xe1111 = xmlDoc.CreateElement("Description");
                xe1111.InnerText = desc;

                root.AppendChild(xe1);
                root.AppendChild(xe11);
                root.AppendChild(xe111);
                root.AppendChild(xe1111);
                xmlDoc.Save(memoryStream);
                long len = memoryStream.Length;
                byte[] buffer = memoryStream.ToArray();
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.Write(new byte[] {0x00,0x49,0x6e,0x66,0x6f,0x00,0x00});
                memoryStream.Write(BitConverter.GetBytes(len));
                memoryStream.Write(buffer);

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                return System.Text.Encoding.Default.GetBytes(ex.Message);
            }
            
        }
        /// <summary>
        /// 读取info
        /// </summary>
        /// <param name="xmls">xml的数组</param>
        /// <returns></returns>
        public static Dictionary<string,string> ReadInfo(byte[] xmls)
        {
            try
            {
                Dictionary<string, string> a = new Dictionary<string, string>();
                var doc = new XmlDocument();
                string s = System.Text.Encoding.Default.GetString(xmls);
                doc.LoadXml(s);
                XmlNodeList nodes = doc.DocumentElement.ChildNodes;
                foreach (XmlNode node in nodes)
                {
                    a.Add(node.Name, node.InnerText);
                }
                return a;

            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取一个文件流（不包含irminsul元数据）的offset
        /// </summary>
        /// <param name="file">irminsul流</param>
        /// <param name="offset">irminsulfile的offset</param>
        /// <returns>long（如果失败则-1）</returns>
        public static long GetFileOffset(string file,long offset)
        {
            try
            {

                long irs = offset + 9;
                using (FileStream rd = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    rd.Seek(irs, SeekOrigin.Begin);
                    byte[] len = new byte[8];
                    rd.Read(len, 0, len.Length);
                    long len1 = BitConverter.ToInt64(len, 0);
                    return offset + len1;
                }
            }
            catch
            {
                return -1;
            }
        }
        public class Info//操作文件基本信息
        {
            /// <summary>
            /// 创建一个IrminsulFile
            /// </summary>
            /// <param name="type">文件类型</param>
            /// <param name="name">文件名</param>
            /// <param name="path">文件路径</param>
            /// <param name="length">长度（大小）</param>
            /// <param name="description">描述</param>
            /// <param name="tag">标签</param>
            /// <returns>IrminsulFile，如果输入有误则全Null</returns>
            public static IrminsulFile Create(string type, string name, string path, long length, string description = "Null", string tag = "Null")
            {
                IrminsulFile irminsulFile = new IrminsulFile();
                try
                {
                    irminsulFile.name = name;
                    irminsulFile.length = length;
                    irminsulFile.type = type;
                    irminsulFile.description = description;
                    irminsulFile.tag = tag;
                    List<string> path1 = path.Split('/').ToList();//注意斜杠
                    irminsulFile.path = path1;
                }
                catch
                {
                    irminsulFile.name = irminsulFile.type = irminsulFile.description = irminsulFile.tag = String.Empty;
                    irminsulFile.path = null;
                }
                return irminsulFile;
            }
            /// <summary>
            /// 解析IrminsulFile
            /// </summary>
            /// <param name="file">IrminsulFile</param>
            /// <param name="what">具体类型，0=type，1=path，2=name，3=desc，4=tag，5=length</param>
            /// <returns>object，注意类型</returns>
            public static object Parse(IrminsulFile file, int what)
            {
                switch (what)
                {
                    case 0:
                        return file.type;
                    case 1:
                        return file.path;
                    case 2:
                        return file.name;
                    case 3:
                        return file.description;
                    case 4:
                        return file.tag;
                    case 5:
                        return file.length;
                    default:
                        return -1;
                }
            }
            /// <summary>
            /// 从二进制中读取IrminsulFile
            /// </summary>
            /// <param name="bytes">输入的二进制块（注意必须是单独的info而不是整个文件）</param>
            /// <returns>IrminsulFile</returns>
            public static IrminsulFile ReadBytes(byte[] bytes)
            {
                IrminsulFile irminsul = new IrminsulFile();
                try
                {
                    long hlen = BitConverter.ToInt64(bytes[9..17], 0);
                    int typel = BitConverter.ToInt32(bytes[17..21], 0);

                    irminsul.type = System.Text.Encoding.Default.GetString(bytes[21..(21 + typel)]);

                    int pathl = BitConverter.ToInt32(bytes[(21 + typel)..(21 + typel + 4)], 0);

                    irminsul.path = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 4)..(21 + typel + 4 + pathl)]).Split('/').ToList();

                    int namel = BitConverter.ToInt32(bytes[(21 + typel + 4 + pathl)..(21 + typel + 8 + pathl)], 0);
                    irminsul.name = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 8 + pathl)..(21 + typel + 8 + pathl + namel)]);
                    int descl = BitConverter.ToInt32(bytes[(21 + typel + 8 + pathl + namel)..(21 + typel + 12 + pathl + namel)], 0);
                    irminsul.description = System.Text.Encoding.Default.GetString(bytes[(20 + typel + 12 + pathl + namel)..(21 + typel + 12 + pathl + namel + descl)]);
                    int tagl = BitConverter.ToInt32(bytes[(21 + typel + 12 + pathl + namel + descl)..(21 + typel + 16 + pathl + namel + descl)], 0);
                    irminsul.tag = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 16 + pathl + namel + descl)..(21 + typel + 16 + pathl + namel + descl + tagl)]);
                    irminsul.length = BitConverter.ToInt64(bytes[(21 + typel + 16 + pathl + namel + descl + tagl)..(29 + typel + 16 + pathl + namel + descl + tagl)], 0);
                    return irminsul;
                }
                catch
                {
                    return irminsul;
                }
            }
            /// <summary>
            /// IrminsulFile写入bytes
            /// </summary>
            /// <param name="irminsul">IrminsulFile</param>
            /// <returns>二进制块（只包含info数据）</returns>
            public static byte[] WriteBytes(IrminsulFile irminsul)
            {
                try
                {
                    byte[] type = System.Text.Encoding.Default.GetBytes(irminsul.type);
                    int typel = type.Length;
                    byte[] btypel = BitConverter.GetBytes(typel);

                    byte[] name = System.Text.Encoding.Default.GetBytes(irminsul.name);
                    int namel = name.Length;
                    byte[] bnamel = BitConverter.GetBytes(namel);

                    byte[] description = System.Text.Encoding.Default.GetBytes(irminsul.description);
                    int descriptionl = description.Length;
                    byte[] bdescriptionl = BitConverter.GetBytes(descriptionl);

                    byte[] tag = System.Text.Encoding.Default.GetBytes(irminsul.tag);
                    int tagl = tag.Length;
                    byte[] btagl = BitConverter.GetBytes(tagl);

                    string path = String.Join("/", irminsul.path);
                    byte[] bpath = System.Text.Encoding.Default.GetBytes(path);
                    int pathl = bpath.Length;
                    byte[] bpathl = BitConverter.GetBytes(pathl);

                    //
                    long alllength = 17 + btypel.Length + typel + bnamel.Length + namel + descriptionl + bdescriptionl.Length + tagl + btagl.Length + bpathl.Length + pathl + 8;
                    byte[] balllength = BitConverter.GetBytes(alllength);
                    //
                    byte[] h = { 0x40, 0x49, 0x72, 0x6d, 0x69, 0x6e, 0x73, 0x75, 0x6c };
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.SetLength(alllength);
                    memoryStream.Write(h);
                    memoryStream.Write(balllength);
                    memoryStream.Write(btypel);
                    memoryStream.Write(type);
                    memoryStream.Write(bpathl);
                    memoryStream.Write(bpath);


                    memoryStream.Write(bnamel);
                    memoryStream.Write(name);


                    memoryStream.Write(bdescriptionl);
                    memoryStream.Write(description);

                    memoryStream.Write(btagl);
                    memoryStream.Write(tag);
                    memoryStream.Write(BitConverter.GetBytes(irminsul.length));

                    return memoryStream.ToArray();

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public class Index//索引操作
        {
            /// <summary>
            /// 创建索引块
            /// </summary>
            /// <param name="offsets">每个IrminsulFile的offset</param>
            /// <returns>索引列表的byte[]，注意开头第一个8字节是long类型的文件总数而不是长度</returns>
            public static byte[] Create(List<long> offsets)
            {
                try
                {
                    MemoryStream memoryStream = new MemoryStream();
                    memoryStream.SetLength(18 + (offsets.Count * 8));
                    memoryStream.Write(new byte[] { 0x49, 0x46, 0x69, 0x6c, 0x65, 0x49, 0x6e, 0x64, 0x65, 0x78 });
                    memoryStream.Write(BitConverter.GetBytes((long)offsets.Count));
                    foreach (long offset in offsets)
                    {
                        memoryStream.Write(BitConverter.GetBytes(offset));
                    }
                    memoryStream.Flush();
                    return memoryStream.ToArray();
                }
                catch
                {
                    return new byte[] { 0x49, 0x46, 0x69, 0x6c, 0x65, 0x49, 0x6e, 0x64, 0x65, 0x78 };
                }
                
            }
            /// <summary>
            /// 获取索引块的内容
            /// </summary>
            /// <param name="indexbyte">索引块bytes</param>
            /// <returns>list+long的索引列表（IrminsulFile的offset）</returns>
            public static List<long> Get(byte[] indexbyte)
            {
                try
                {
                    long filecount = BitConverter.ToInt64(indexbyte[10..18], 0);
                    byte[] offsetbs = indexbyte[18..];
                    if (filecount * 8 != offsetbs.Length)
                    {
                        return [-32];//-32代表文件的块不完整。
                    }
                    else
                    {
                        List<long> list = new List<long>();
                        for (int i = 0; i < filecount; i++)
                        {
                            list.Add(BitConverter.ToInt64(offsetbs[(i*8)..((i*8)+8)], 0));
                        }
                        return list;
                    }
                }
                catch
                {
                    return [-1];
                }
            }

        }
        
    }
    
}
