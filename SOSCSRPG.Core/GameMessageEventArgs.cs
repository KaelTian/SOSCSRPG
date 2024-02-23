namespace SOSCSRPG.Core
{
    public class GameMessageEventArgs:System.EventArgs
    {
        public string Message { get; set; }
        public GameMessageEventArgs(string message)
        {
            Message = message;
        }
    }
}
