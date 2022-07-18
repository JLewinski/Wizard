using System;

namespace Wizard.Models
{
    public sealed class GameSummary
    {
        public GameSummary() { }

        public GameSummary(int id, string name, DateTime dateCreated, DateTime lastUpdated, bool isFinished)
        {
            Id = id;
            Name = name;
            LastUpdated = lastUpdated;
            DateCreated = dateCreated;
            IsFinished = isFinished;
        }

        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }
}
