#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov&version=0.8.0

var target = Argument("target", "default");
var configuration = Argument("configuration", "Debug");
var rootPath     = "./";
var srcPath      = rootPath + "AElf.Client/";
var testPath     = rootPath + "AElf.Client.Test/";
var distPath     = rootPath + "aelf-node/";
var solution     = rootPath + "all.sln";
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
        Configuration = configuration,
        ArgumentCustomization = args => {
            return args.Append("/clp:ErrorsOnly")
                       .Append("--no-incremental")
                       .Append("-v quiet");}
    };
     
    DotNetCoreBuild(solution, buildSetting);
});
Task("test")
    .Description("operation test")
    .IsDependentOn("build")
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

Task("test-with-codecov")
    .Description("operation test")
    .IsDependentOn("Build")
    .Does(() =>
{
    var testSetting = new DotNetCoreTestSettings{
        Configuration = configuration,
        NoRestore = true,
        NoBuild = true,
        ArgumentCustomization = args => {
                    return args
                        .Append("--logger trx")
                        .Append("--settings CodeCoverage.runsettings")
                        .Append("--collect:\"XPlat Code Coverage\"");
                }                  
    };
    var testProjects = GetFiles("./test/*.Test/*.csproj");
    var testProjectList = testProjects.OrderBy(p=>p.FullPath).ToList();
    foreach(var testProject in testProjectList)
    {
        DotNetCoreTest(testProject.FullPath, testSetting);
    }
});

Task("upload-coverage-azure")
    .Does(() =>
{
    Codecov("./CodeCoverage/Cobertura.xml","$CODECOV_TOKEN");
});

Task("default")
    .Description("default run test(-target test)")
    .IsDependentOn("build");

RunTarget(target);
