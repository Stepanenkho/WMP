using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMP
{

    public class Element
    {
        string name;
        int type;
        string path;

        public Element(string name, int type, string path)
        {
            this.name = name;
            this.type = type;
            this.path = path;
        }

        public Element(string name, string type, string path)
        {
            this.name = name;
            this.path = path;

            if (type.Equals("video"))
                this.type = 0;
            else if (type.Equals("audio"))
                this.type = 1;
            else
                this.type = 2;
        }

        public string getName()
        {
            return this.name;
        }

        public int getType()
        {
            return this.type;
        }

        public string getTypeString()
        {
            if (type == 0)
                return "video";
            else if (type == 1)
                return "audio";
            else
                return "image";
        }

        public string getPath()
        {
            return this.path;
        }
    }
}
