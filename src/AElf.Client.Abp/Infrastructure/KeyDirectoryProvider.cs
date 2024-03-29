using Volo.Abp.DependencyInjection;

namespace AElf.Client.Abp.Infrastructure;

public class KeyDirectoryProvider : IKeyDirectoryProvider, ISingletonDependency
{
    private const string ApplicationFolderName = "aelf";
    private string _appDataPath;

    public string GetAppDataPath()
    {
        if (string.IsNullOrWhiteSpace(_appDataPath))
        {
            _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationFolderName);

            if (!Directory.Exists(_appDataPath))
            {
                Directory.CreateDirectory(_appDataPath);
            }
        }

        return _appDataPath;
    }
}