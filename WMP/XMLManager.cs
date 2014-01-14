using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace WMP
{
    public class XMLManager
    {
        public List<Element> objects;
        public List<Playlist> playlists;

        public string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Base.xml";

        public void loadAllElements()
        {
            objects = new List<Element>();
            playlists = new List<Playlist>();

            if (File.Exists(path) == false)
                return ;

            XDocument doc = XDocument.Load(path);

            IEnumerable<XNode> allObj = doc.Root.DescendantNodes();

            foreach (XNode node in allObj)
            {
                if (node is XElement && (node as XElement).Name == "object")
                    objects.Add(new Element(
                        (node as XElement).Element("name").Value,
                        (node as XElement).Element("type").Value,
                        (node as XElement).Element("path").Value));
                else if (node is XElement && (node as XElement).Name == "playlist")
                {
                    playlists.Add(new Playlist(
                        (node as XElement).Element("name").Value,
                        (node as XElement).Element("content").Value));
                }
            }
            loadElementFromFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 0);
            loadElementFromFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), 1);
            loadElementFromFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), 2);
        }

        public void saveAllElements(List<Element> pObjects, List<Playlist> pPlaylists)
        {
            XmlTextWriter XMLWriter = new XmlTextWriter(path, null);

            XMLWriter.Formatting = Formatting.Indented;
            XMLWriter.WriteStartDocument(false);

            XMLWriter.WriteStartElement("element");

            foreach (Element e in pObjects)
                addObject(e.getTypeString(), e.getName(), e.getPath(), XMLWriter);
            foreach (Playlist e in pPlaylists)
                addPlaylist(e.getName(), e.getContentString(), XMLWriter);

            XMLWriter.Close();
        }

        public void loadElementFromFolder(string pathFolder, int type)
        {
            DirectoryInfo files = new DirectoryInfo(pathFolder);

            foreach (FileInfo file in files.GetFiles())
            {
                if (GetMIMEType(file.FullName) != "unknown")
                    objects.Add(new Element(System.IO.Path.GetFileNameWithoutExtension(file.Name), type, file.FullName));
            }

        }

        private void addObject(string type, string name, string path, XmlTextWriter XMLWriter)
        {
            XMLWriter.WriteStartElement("object");

            XMLWriter.WriteElementString("name", name);
            XMLWriter.WriteElementString("type", type);
            XMLWriter.WriteElementString("path", path);

            XMLWriter.WriteEndElement();
            XMLWriter.Flush();
        }

        private void addPlaylist(string name, string content, XmlTextWriter XMLWriter)
        {
            XMLWriter.WriteStartElement("playlist");

            XMLWriter.WriteElementString("name", name);
            XMLWriter.WriteElementString("content", content);

            XMLWriter.WriteEndElement();
            XMLWriter.Flush();
        }

        public List<Element> getObjects()
        {
            return this.objects;
        }

        public List<Playlist> getPlaylists()
        {
            return this.playlists;
        }
        #region
        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
  {
    {"aif", "audio"},
    {"aifc", "audio"},
    {"aiff", "audio"},
    {"au", "audio"},
    {"avi", "video"},
    {"bmp", "image"},
    {"cgm", "image"},
    {"dif", "video"},
    {"djv", "image"},
    {"djvu", "image"},
    {"dv", "video"},
    {"gif", "image"},
    {"ico", "image"},
    {"ief", "image"},
    {"jp2", "image"},
    {"jpe", "image"},
    {"jpeg", "image"},
    {"jpg", "image"},
    {"kar", "audio"},
    {"m3u", "audio"},
    {"m4a", "audio"},
    {"m4b", "audio"},
    {"m4p", "audio"},
    {"m4u", "video"},
    {"m4v", "video"},
    {"mac", "image"},
    {"mid", "audio"},
    {"midi", "audio"},
    {"mov", "video"},
    {"movie", "video"},
    {"mp2", "audio"},
    {"mp3", "audio"},
    {"mp4", "video"},
    {"mpe", "video"},
    {"mpeg", "video"},
    {"mpg", "video"},
    {"mpga", "audio"},
    {"mxu", "video"},
    {"ogg", "audio"},
    {"pbm", "image"},
    {"pct", "image"},
    {"pgm", "image"},
    {"pic", "image"},
    {"pict", "image"},
    {"png", "image"}, 
    {"pnm", "image"},
    {"pnt", "image"},
    {"pntg", "image"},
    {"ppm", "image"},
    {"qt", "video"},
    {"qti", "image"},
    {"qtif", "image"},
    {"ra", "audio"},
    {"ram", "audio"},
    {"ras", "image"},
    {"rgb", "image"},
    {"snd", "audio"},
    {"svg", "image"},
    {"tif", "image"},
    {"tiff", "image"},
    {"wav", "audio"},
    {"wbmp", "image"},
    {"xbm", "image"},
    {"xpm", "image"},
    {"xwd", "image"},
    {"wmv", "video"}
  };
        #endregion
        public string GetMIMEType(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            if (extension.Length > 0 &&
                MIMETypesDictionary.ContainsKey(extension.Remove(0, 1)))
            {
                return MIMETypesDictionary[extension.Remove(0, 1)];
            }
            return "unknown";
        }

    }
}
