
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;

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
        public static string ver = "v0.0.1";
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="irminsul">输入的irminsul文件（文件路径）</param>
        /// <returns>所有的内部文件列表，IDic为：临时ID，IrminsulFile</returns>
        //public static Dictionary<long, IrminsulFile> GetList(string irminsul)
        //{

        //}
        /// <summary>
        /// 从文件列表里 获取一个或多个文件的基本信息
        /// </summary>
        /// <param name="irminsuldic">GetList()获取的列表</param>
        /// <param name="idlist">需要截取的id列表</param>
        /// <returns>获取到的一个或多个文件的基本信息，IDic为：临时id，IrminsulFile</returns>
        public static Dictionary<long, IrminsulFile> GetProfile(Dictionary<long, IrminsulFile> irminsuldic,List<long> idlist)//从文件列表里 获取一个或多个文件的基本信息，IDic为：id，IrminsulFile
        {
            Dictionary<long, IrminsulFile> ret = new Dictionary<long, IrminsulFile>();
            try
            {
                for (int i = 0; i < idlist.Count; i++) {
                    ret.Add(i,irminsuldic[idlist[i]]);
                }
            }
            catch
            {
                return null;
            }
            return ret;
        }
        /// <summary>
        /// 写入已格式化的Irminsul文件
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
                    fs.Write(IrminsulInfo.WriteBytes(ir));
                    using (FileStream rd = new FileStream(rawfile, FileMode.Open, FileAccess.Read))
                    {
                        int splitsize = 1024 * 1024 * 1024;
                        long splitcount = rd.Length / splitsize;
                        for (int i = 0; i < splitcount; i++)
                        {
                            //read
                            rd.Seek(i * splitsize, SeekOrigin.Begin);// 定位
                            int remainsize = (int)fs.Length - (i * splitsize);
                            if (splitsize <= remainsize)
                            {

                                rd.CopyTo(fs, splitsize);

                            }
                            else
                            {
                                rd.CopyTo(fs, remainsize);
                            }
                        }
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
        /// 创建一个新的空白irminsul文件
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
0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,0x01,
0x49,0x44,0x4C,0x0D,0x0A,0x0D,0x0A,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
0x20,0x20,0x32,0x2E,0x00,0x07,0x21,0x07,0x21,0x00,0x11,0x00,0x01,0x01,0x01,0x01,
0x49,0x44,0x61,0x74,0x61,0x00,0x1F,0x8B,0x08,0x00,0x00,0x00,0x00,0x00,0x00,0x0A,
0x55,0x8E,0x4D,0x0A,0xC2,0x40,0x0C,0x85,0xAF,0xF2,0x76,0x6E,0xD4,0x3B,0x08,0x6E,
0x04,0x97,0x5E,0xA0,0xCC,0x44,0x27,0x30,0x4E,0x4A,0x92,0x5A,0x7A,0x7B,0x93,0xE2,
0x46,0x08,0xE4,0xEF,0xE5,0x7B,0xB9,0xE9,0x9B,0x87,0x2D,0x1D,0x6C,0x78,0xD0,0xF6,
0x99,0xFC,0x60,0x58,0x45,0x7B,0x85,0x2B,0xD1,0x11,0x13,0x7C,0x9B,0x09,0xF2,0x84,
0x71,0xFF,0x90,0x9E,0xD6,0xC6,0x4E,0xFB,0x12,0x51,0x96,0x86,0x97,0xCA,0x6A,0xA8,
0x44,0x33,0x96,0x51,0x49,0xA3,0x8F,0x1C,0x97,0xA3,0x26,0xB5,0xC8,0x18,0x54,0x9C,
0x02,0x28,0xF0,0x46,0xB8,0xD3,0x86,0x3B,0x0F,0xB2,0x84,0xE6,0x60,0xB7,0x3B,0xE3,
0xE6,0x29,0xB7,0x89,0x43,0xD9,0x26,0xDF,0x57,0x57,0x1A,0x55,0x05,0x17,0x2D,0x4D,
0x46,0x7C,0x16,0x30,0x2B,0x2C,0x8B,0xC5,0xB9,0xA5,0xBC,0xB2,0x06,0xBC,0x6F,0xFF,
0x36,0xEC,0xC7,0x74,0xDF,0x31,0x9C,0x61,0xD4,0x9F,0x29,0x4F,0xA6,0x8A,0x78,0x5A,
0xFF,0xD8,0xB3,0xAC,0xA4,0xE7,0x2F,0x08,0xFC,0xA6,0xAE,0x08,0x01,0x00,0x00,0x00,
0x00,0x0F,0x0F,0x49,0x56,0x65,0x72,0x73,0x69,0x6F,0x6E,0x00 });
                    fs.Write(System.Text.Encoding.Default.GetBytes(Irminsul.ver));
                    fs.Write(new byte[] {0x00,0x49,0x4E,0x61,0x68,0x69,0x64,0x61,0x0D,0x00,0x07,0x21,0x0D,0x00,0x07,0x21,
0x0D,0x00,0x07,0x21,0x0D,0x00,0x07,0x21,0x49,0x53,0x74,0x72,0x65,0x61,0x6D,0x00
 });
                    return true;
                }
            }
            catch { return false; }
        }
        /// <summary>
        /// 获取一个文件流（不包含irminsul元数据）的offset
        /// </summary>
        /// <param name="file">irminsul流</param>
        /// <param name="offset">iminsulfile的offset</param>
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


    }
    public class IrminsulInfo//操作文件基本信息
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
                List<string> path1 = path.Split('\\').ToList();//注意斜杠
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

                irminsul.path = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 4)..(21 + typel + 4 + pathl)]).Split('\\').ToList();

                int namel = BitConverter.ToInt32(bytes[(21 + typel + 4 + pathl)..(21 + typel + 8 + pathl)], 0);
                irminsul.name = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 8 + pathl)..(21 + typel + 8 + pathl + namel)]);
                int descl = BitConverter.ToInt32(bytes[(21 + typel + 8 + pathl + namel)..(21 + typel + 12 + pathl + namel)], 0);
                irminsul.description = System.Text.Encoding.Default.GetString(bytes[(20 + typel + 12 + pathl + namel)..(21 + typel + 12 + pathl + namel + descl)]);
                int tagl = BitConverter.ToInt32(bytes[(21 + typel + 12 + pathl + namel + descl)..(21 + typel + 16 + pathl + namel + descl)], 0);
                irminsul.tag = System.Text.Encoding.Default.GetString(bytes[(21 + typel + 16 + pathl + namel + descl)..(21 + typel + 16 + pathl + namel + descl + tagl)]);
                
                irminsul.length = BitConverter.ToInt64(bytes[(21 + typel + 16 + pathl + namel + descl + tagl)..], 0);
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

                string path = String.Join("\\", irminsul.path);
                byte[] bpath = System.Text.Encoding.Default.GetBytes(path);
                int pathl = bpath.Length;
                byte[] bpathl = BitConverter.GetBytes(pathl);

                //
                long alllength = 17 + btypel.Length + typel + bnamel.Length + namel + descriptionl + bdescriptionl.Length + tagl + btagl.Length + bpathl.Length + pathl;
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
}
