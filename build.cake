#l "common.cake"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var buildNumber=0;
var baseDir=System.IO.Directory.GetCurrentDirectory();
var buildDir=System.IO.Path.Combine(baseDir, "build");
var distDir=System.IO.Path.Combine(baseDir, "dist");
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
if(isRunningOnAppVeyor)
    buildNumber = AppVeyor.Environment.Build.Number;
System.Environment.SetEnvironmentVariable("DNX_BUILD_VERSION", buildNumber.ToString(), System.EnvironmentVariableTarget.Process);

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
});

Task("Build")
    .Does(() =>
{
    ExecuteCommand("dotnet restore");
    ExecuteCommand(string.Format("dotnet publish \"src/JavaScriptViewEngine/project.json\" --configuration \"{0}\" -o \"{1}\"", configuration, System.IO.Path.Combine(buildDir, "JavaScriptViewEngine")));
});

Task("Test")
    .WithCriteria(() => !isRunningOnAppVeyor)
    .Does(() =>
{
    // no tests
});

Task("Deploy")
    .Does(() =>
{
    if(!DirectoryExists(distDir))
        CreateDirectory(distDir);

    ExecuteCommand(string.Format("dotnet pack \"src/JavaScriptViewEngine/project.json\" --configuration \"{0}\" -o \"{1}\"", configuration, distDir));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("CI")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Deploy");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
