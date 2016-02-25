//////////////////////////////////////////////////////////////////////
// HELPERS
//////////////////////////////////////////////////////////////////////

void EnsureTool(string tool, string arguements)
{
    try
    {
        ExecuteCommand(tool + (!string.IsNullOrEmpty(arguements) ? " " + arguements : null));
        Information("The tool \"" + tool + "\" is present...");
    }
    catch(Exception ex)
    {
        Error("The tool \"" + tool + "\" is not present...");
        throw;
    }
}

void ExecuteCommand(string command, string workingDir = null)
{
    if (string.IsNullOrEmpty(workingDir))
        workingDir = System.IO.Directory.GetCurrentDirectory();

    System.Diagnostics.ProcessStartInfo processStartInfo;

    if (IsRunningOnWindows())
    {
        processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            UseShellExecute = false,
            WorkingDirectory = workingDir,
            FileName = "cmd",
            Arguments = "/C " + command,
        };
    }
    else
    {
        processStartInfo = new System.Diagnostics.ProcessStartInfo
        {
            UseShellExecute = false,
            WorkingDirectory = workingDir,
            Arguments = command,
        };
    }

    using (var process = System.Diagnostics.Process.Start(processStartInfo))
    {
        process.WaitForExit();

        if (process.ExitCode != 0)
            throw new Exception(string.Format("Exit code {0} from {1}", process.ExitCode, command));
    }
}
