namespace AElf.Client.Abp.Infrastructure;

public interface IKeyDirectoryProvider
{
    string GetAppDataPath();
}