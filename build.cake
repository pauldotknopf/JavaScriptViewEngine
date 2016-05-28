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
var srcDir = System.IO.Path.Combine(baseDir, "src");
var srcProjectDir = System.IO.Path.Combine(srcDir, "JavaScriptViewEngine");
var srcProjectMvc6Dir = System.IO.Path.Combine(srcDir, "JavaScriptViewEngine.Mvc6");
var srcProjectMvc5Dir = System.IO.Path.Combine(srcDir, "JavaScriptViewEngine.Mvc5");
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
    CleanDirectory(srcProjectMvc6Dir);
    CleanDirectory(srcProjectMvc5Dir);
});

Task("PrepareMvc")
    .Does(() =>
{
    // We are going to copy src/JavaScriptViewEngine to src/JavaScriptViewEngine.MvcX/.
    CopyDirectory(srcProjectDir, srcProjectMvc6Dir);
    CopyDirectory(srcProjectDir, srcProjectMvc5Dir);
    CopyFile(System.IO.Path.Combine(srcProjectDir, "project.mvc6.json"), System.IO.Path.Combine(srcProjectMvc6Dir, "project.json"));
    CopyFile(System.IO.Path.Combine(srcProjectDir, "project.mvc5.json"), System.IO.Path.Combine(srcProjectMvc5Dir, "project.json"));
});

Task("Build")
    .Does(() =>
{
    ExecuteCommand("dotnet restore src");
    ExecuteCommand(string.Format("dotnet build \"src/JavaScriptViewEngine.Mvc6/project.json\" --configuration \"{0}\"", configuration));
    ExecuteCommand(string.Format("dotnet build \"src/JavaScriptViewEngine.Mvc5/project.json\" --configuration \"{0}\"", configuration));
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

    ExecuteCommand(string.Format("dotnet pack \"src/JavaScriptViewEngine.Mvc6/project.json\" --configuration \"{0}\" -o \"{1}\"", configuration, distDir));
    ExecuteCommand(string.Format("dotnet pack \"src/JavaScriptViewEngine.Mvc5/project.json\" --configuration \"{0}\" -o \"{1}\"", configuration, distDir));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("PrepareMvc")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("CI")
    .IsDependentOn("EnsureDependencies")
    .IsDependentOn("Clean")
    .IsDependentOn("PrepareMvc")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Deploy");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
