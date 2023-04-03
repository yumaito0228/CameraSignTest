using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Editor
{
    public static class ScopedRegistriesHandler
    {
        private static string _manifestPath = string.Empty;
        private static string ManifestPath
        {
            get
            {
                if(_manifestPath == string.Empty)
                    _manifestPath = Path.Combine(Application.dataPath, "..", "Packages/manifest.json");
                return _manifestPath;
            }
        }

        public static void AddScopedRegistry(ScopedRegistry scopedRegistry)
        {
            var manifest = GetScopedRegistry();

            if (manifest.scopedRegistries.Any(registry => registry.url.TrimEnd('/') == scopedRegistry.url))
                return;
            
            manifest.scopedRegistries.Add(scopedRegistry);
            File.WriteAllText(_manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        private static ManifestJson GetScopedRegistry()
        {
            var manifestJson = File.ReadAllText(ManifestPath);
 
            return JsonConvert.DeserializeObject<ManifestJson>(manifestJson);
        }
    }
}
