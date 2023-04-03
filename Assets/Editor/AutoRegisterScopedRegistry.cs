using System;
using UnityEditor;

namespace Editor
{
    public static class AutoRegisterScopedRegistry
    {
        public static void AddScopedRegistry()
        {
            // バッチの引数にname, url, scopesの順で入れる。
            ScopedRegistriesHandler.AddScopedRegistry(
                new ScopedRegistry("Activ8", "https://upm.activ8dev.com", new []{"jp.co.activ8"}));
            EditorApplication.Exit(0);
        }
    }
}