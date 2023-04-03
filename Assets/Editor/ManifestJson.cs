using System.Collections.Generic;

namespace Editor
{
    public class ManifestJson {
        public Dictionary<string,string> dependencies = new Dictionary<string, string>();
 
        public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
    }
}
