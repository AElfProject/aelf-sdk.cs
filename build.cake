var target = Argument("target", "default");
var rootPath     = "./";
var srcPath      = rootPath + "AElf.Client/";
var testPath     = rootPath + "AElf.Client.Test/";
var distPath     = rootPath + "aelf-node/";
var solution     = rootPath + "AElf.Client.sln";
var srcProjects  = GetFiles(srcPath + "AElf.Client.csproj");

Task("clean")
    .Description("clean up project cache")
    .Does(() =>
{
    DeleteFiles(distPath + "*.nupkg");
    CleanDirectories(srcPath + "bin");
    CleanDirectories(srcPath + "obj");
    CleanDirectories(testPath + "bin");
    CleanDirectories(testPath + "obj");
});

Task("restore")
    .Description("restore project dependencies")
    .Does(() =>
{
    var restoreSettings = new DotNetCoreRestoreSettings{
        ArgumentCustomization = args => {
            return args.Append("-v quiet");}
};
    DotNetCoreRestore(solution,restoreSettings);
});

Task("build")
    .Description("Compilation project")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
{
    var buildSetting = new DotNetCoreBuildSettings{
        NoRestore = true,
        Configuration = "Debug",
        ArgumentCustomization = args => {
            return args.Append("/clp:ErrorsOnly")
                       .Append("--no-incremental")
                       .Append("-v quiet");}
    };
     
    DotNetCoreBuild(solution, buildSetting);
});
Task("test")
    .Description("operation test")
    .Does(() =>
{
    var testSetting = new DotNetCoreTestSettings{
        NoRestore = true,
        NoBuild = true
};
    var testProjects = GetFiles("./*.Test/*.csproj");

    foreach(var testProject in testProjects)
    {
        DotNetCoreTest(testProject.FullPath, testSetting);
    }
});

Task("default")
    .Description("default run test(-target test)")
    .IsDependentOn("build");

RunTarget(target);
