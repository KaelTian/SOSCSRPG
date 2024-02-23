using System.ComponentModel;

namespace SOSCSRPG.Models
{
    public class QuestStatus : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public Quest PlayerQuest { get; set; }
        public bool IsCompleted { get; set; }
        public QuestStatus(Quest quest)
        {
            PlayerQuest = quest;
            IsCompleted = false;
        }
    }
}
