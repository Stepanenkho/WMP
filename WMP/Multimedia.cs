using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMP
{

    public class Multimedia
    {

        public List<Element> video;
        public List<Element> music;
        public List<Element> image;

        public void init(List<Element> all)
        {
            video = new List<Element>();
            music = new List<Element>();
            image = new List<Element>();

            foreach (Element element in all)
            {
                addElement(element);
            }
        }

        bool exist(string name)
        {
            foreach (Element e in video)
                if (e.getName().Equals(name))
                    return true;
            foreach (Element e in music)
                if (e.getName().Equals(name))
                    return true;
            foreach (Element e in image)
                if (e.getName().Equals(name))
                    return true;
            return false;
        }

        public List<Element> getAllElements()
        {
            List<Element> all = new List<Element>();

            foreach (Element e in video)
                all.Add(e);
            foreach (Element e in music)
                all.Add(e);
            foreach (Element e in image)
                all.Add(e);
            return all;
        }

        public Element getElement(string name)
        {
            foreach (Element e in video)
                if (e.getName().Equals(name))
                    return e;
            foreach (Element e in music)
                if (e.getName().Equals(name))
                    return e;
            foreach (Element e in image)
                if (e.getName().Equals(name))
                    return e;
            return null;
        }

        public bool addElement(Element e)
        {
            switch (e.getType())
            {
                case 0:
                    return addVideo(e);
                case 1:
                    return addMusic(e);
                case 2:
                    return addImage(e);
                default:
                    return false;
            }
        }

        public void removeElement(Element e)
        {
            switch (e.getType())
            {
                case 0:
                    removeVideo(e);
                    break ;
                case 1:
                    removeMusic(e);
                    break ;
                case 2:
                    removeImage(e);
                    break ;
            }
        }

        public List<Element> getVideo()
        {
            return this.video;
        }

        public bool addVideo(Element e)
        {
            if (exist(e.getName()) == true)
                return false;
            video.Add(e);
            return true;
        }

        public void removeVideo(Element e)
        {
            video.Remove(e);
        }

        public List<Element> getMusic()
        {
            return this.music;
        }

        public bool addMusic(Element e)
        {
            if (exist(e.getName()) == true)
                return false;
            music.Add(e);
            return true;
        }

        public void removeMusic(Element e)
        {
            music.Remove(e);
        }

        public List<Element> getImage()
        {
            return this.image;
        }

        public bool addImage(Element e)
        {
            if (exist(e.getName()) == true)
                return false;
            image.Add(e);
            return true;
        }

        public void removeImage(Element e)
        {
            image.Remove(e);
        }
    }
}
