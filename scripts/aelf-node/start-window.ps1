wget https://github.com/microsoftarchive/redis/releases/download/win-3.2.100/Redis-x64-3.2.100.zip -OutFile  redis.zip
Expand-Archive -Path redis.zip -DestinationPath redis ;
$job1 = Start-Job -ScriptBlock { cd D:\a\1\s\redis; .\redis-server.exe; }
sleep 30
mkdir -p C:\AppData\Local\aelf\keys
cp -r scripts\aelf-node\keys\* C:\AppData\Local\aelf\keys;
wget https://github.com/AElfProject/AElf/releases/download/v1.2.0/aelf.zip -OutFile  aelf.zip ;
Expand-Archive -Path aelf.zip -DestinationPath aelf ;
cp scripts\aelf-node\appsettings.json  aelf\AElf\appsettings.json ;
cp scripts\aelf-node\appsettings.MainChain.TestNet.json  aelf\AElf\appsettings.MainChain.TestNet.json ;
cd aelf/AElf
$job2 = Start-Job -ScriptBlock { cd D:\a\1\s\aelf\AElf; dotnet AElf.Launcher.dll; } 
sleep 60
cd D:\a\1\s
PowerShell.exe -file build.ps1 --target=test-with-codecov
