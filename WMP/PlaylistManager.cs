using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMP
{
    public class PlaylistManager
    {
        Playlist actual;
        List<Playlist> all;

        public void init(List<Playlist> content)
        {
            actual = null;
            all = new List<Playlist>();
            foreach (Playlist p in content)
                this.all.Add(p);
        }

        public void setActual(string name)
        {
            this.actual = null;
            foreach (Playlist p in all)
                if (p.getName().Equals(name))
                    this.actual = p;
        }

        public Playlist getActual()
        {
            return this.actual;
        }

        public List<Playlist> getContent()
        {
            return all;
        }

        public Playlist getPlaylist(string name)
        {
            foreach (Playlist p in all)
                if (p.getName().Equals(name))
                    return p;
            return null;
        }

        public void addPlaylist(Playlist p)
        {
            if (p.getName() != null && p.getName() != "")
                all.Add(p);
        }

        public void removePlaylist(string name)
        {
            foreach (Playlist p in this.all)
                if (p.getName().Equals(name))
                {
                    this.all.Remove(p);
                    return ;
                }
        }
    }
}
