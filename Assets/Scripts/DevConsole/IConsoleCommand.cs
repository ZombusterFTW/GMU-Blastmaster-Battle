namespace GMUBMB.Utilities.DevConsole.Commands
{
    public interface IConsoleCommand
    {
        string CommandWord { get; }
        bool Process(string[] args);
    }
}