#l "common.cake"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildNumber="0";
var versionNumber="1.4.0";
var baseDir=System.IO.Directory.GetCurrentDirectory();
var buildDir=System.IO.Path.Combine(baseDir, "build");
var distDir=System.IO.Path.Combine(baseDir, "dist");
var srcDir = System.IO.Path.Combine(baseDir, "src");
var srcProjectDir = System.IO.Path.Combine(srcDir, "JavaScriptViewEngine");
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
if(isRunningOnAppVeyor)
    buildNumber = AppVeyor.Environment.Build.Number.ToString();
var version = versionNumber + "." + buildNumber;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("EnsureDependencies")
    .Does(() =>
{
    EnsureTool("dotnet", "--version");
});

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(distDir);
    foreach(var directory in GetDirectories("src/**/bin"))
    {
        CleanDirectory(directory.FullPath);
    }
    foreach(var directory in GetDirectories("src/**/obj"))
    {
        CleanDirectory(directory.FullPath);
    }
});

Task("Build")
    .Does(() =>
{
    ExecuteCommand("dotnet restore JavaScriptViewEngine.sln");
    ExecuteCommand("dotnet build --configuration " + configuration + " --output " + buildDir + " /p:Version=" + versionNumber + " /p:FileVersion=" + version + " /p:AssemblyVersion=" + version + " JavaScriptViewEngine.sln");
});

Task("Deploy")
    .Does(() =>
{
    ExecuteCommand("dotnet publish --configuration " + configuration + " /p:Version=" + versionNumber + " /p:FileVersion=" + version + " /p:AssemblyVersion=" + version + " /t:pack src/JavaScriptViewEngine/JavaScriptViewEngine.csproj");
    ExecuteCommand("dotnet publish --configuration " + configuration + " /p:Version=" + versionNumber + " /p:FileVersion=" + version + " /p:AssemblyVersion=" + version + " /t:pack src/JavaScriptViewEngine.Mvc5/JavaScriptViewEngine.Mvc5.csproj");
    foreach(var package in GetFiles("src/JavaScriptViewEngine/bin/" + configuration + "/*.nupkg"))
    {
        CopyFile(package.FullPath, System.IO.Path.Combine(distDir, System.IO.Path.GetFileName(package.FullPath)));
    }
    foreach(var package in GetFiles("src/JavaScriptViewEngine.Mvc5/bin/" + configuration + "/*.nupkg"))
    {
        CopyFile(package.FullPath, System.IO.Path.Combine(distDir, System.IO.Path.GetFileName(package.FullPath)));
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("Build");

Task("CI")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Deploy");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
