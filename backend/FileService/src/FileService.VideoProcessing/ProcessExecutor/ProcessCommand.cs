using System.Text.RegularExpressions;

namespace FileService.VideoProcessing.ProcessExecutor;

public partial record ProcessCommand(string ExecutableFile, string Arguments)
{
    public string NormalizedArguments => NormalizeWhitespace(Arguments);

    private static string NormalizeWhitespace(string arguments) =>
        WhitespaceRegex().Replace(arguments.Trim(), " ");

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    private static partial Regex WhitespaceRegex();
}