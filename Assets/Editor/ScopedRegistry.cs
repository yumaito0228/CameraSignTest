namespace Editor
{
    public class ScopedRegistry
    {
        public string name;
        public string url;
        public string[] scopes;

        public ScopedRegistry(string name, string url, string[] scopes)
        {
            this.name = name;
            this.url = url;
            this.scopes = scopes;
        }
    }
}