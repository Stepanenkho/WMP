using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMP
{
    public class Playlist
    {
        string name;
        List<string> content = new List<String>();

        public Playlist(string name)
        {
            this.name = name;
        }

        public Playlist(string name, List<string> content)
        {
            this.name = name;
            this.content = content;
        }

        public Playlist(string name, string content)
        {
            this.name = name;
            addContent(content);
        }

        public List<string> getContent()
        {
            return content;
        }

        public string getName()
        {
            return this.name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void addObject(string name)
        {
            content.Add(name);
        }

        public void removeObject(string name)
        {
            foreach (string s in content)
                if (s.Equals(name))
                {
                    content.Remove(s);
                    return;
                }
        }

        public void addContent(string str)
        {
            string[] split = str.Split(new Char[] { ';' });

            foreach (string s in split)
                if (s != "")
                    content.Add(s);
        }

        public string getContentString()
        {
            string str = "";

            foreach (string p in content)
                if (p != "")
                    str += p + ";";
            return str;
        }
    }
}
