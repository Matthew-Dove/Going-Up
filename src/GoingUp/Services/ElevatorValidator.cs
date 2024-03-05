using GoingUp.Models;
using System.Reflection.Emit;

namespace GoingUp.Services
{
    /// <summary>
    /// Holds validation rules for elevator rides.
    /// </summary>
    internal sealed class ElevatorValidator
    {
        public bool ValidateTravel(Level requestedLevel, Level currentLevel, Direction intendedDirection)
        {
            if (intendedDirection == Direction.Up && requestedLevel <= currentLevel) return false; // Passenger must select a travel level higher than their current level.
            if (intendedDirection == Direction.Down && requestedLevel >= currentLevel) return false; // Passenger must select a travel level lower than their current level.

            return true;
        }
    }
}
