using System;
using System.Collections.Generic;

namespace Wizard.Models
{
    public interface IGame
    {
        // Game ToPoco();
        int Id { get; set; }

        string Name { get; set; }

        List<IPlayer> Players { get; set; }

        List<Suit> Suits { get; set; }

        DateTime DateCreated { get; }

        DateTime LastUpdated { get; set; }
    }
}