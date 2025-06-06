namespace AssetManager;

public class AssetManager(string basePath) {
    public Stream LoadAssetStream(string filename) {
        return File.OpenRead(Path.Combine(basePath, filename));
    }

    public string LoadAssetText(string filename) {
        return File.ReadAllText(Path.Combine(basePath, filename));
    }

    public StreamReader LoadAssetTextStream(string filename) {
        return File.OpenText(Path.Combine(basePath, filename));
    }
}