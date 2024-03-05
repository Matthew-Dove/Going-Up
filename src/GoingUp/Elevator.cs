using GoingUp.Models;
using GoingUp.Services;
using Microsoft.Extensions.Logging;

namespace GoingUp
{
    /// <summary>
    /// A passenger can summon the elevator to go up or down from any floor, once in the lift they can choose the floor they would like to travel to.
    /// </summary>
    public sealed class Elevator
    {
        private Level _currentLevel; // The elevator's current level.
        private Direction _intendedDirection; // The elevator's intended direction.

        private readonly ILogger<Elevator> _log;
        private readonly Queue<Summon> _summons = null; // Queued FIFO summons made since the last check.
        private readonly ElevatorValidator _validator = null;

        public Elevator(ILogger<Elevator> log)
        {
            _currentLevel = Level.L1;
            _intendedDirection = Direction.Up; // Assume the elevator "starts" on the ground floor (L1).

            _log = log;
            _summons = new Queue<Summon>();
            _validator = new ElevatorValidator();
        }

        /// <summary>User has requested the elevator to their level, they have selected the direction they intend to travel to.</summary>
        /// <param name="level">The level the request came from.</param>
        /// <param name="direction">The user's wish to travel either up, or down.</param>
        public void Summon(Level level, Direction direction)
        {
            // Assume the level, and direction are always correct, as only valid summon buttons exist on each floor.

            _log.LogInformation("Elevator summoned to level {level}, going {direction}.", level, direction);
            _summons.Enqueue(new Summon(level, direction));
        }

        public bool Travel(Level requestedLevel)
        {
            // Once in the elevator, a user *can* select an incorrect level for the current journey.
            // For example, if they selected "up" on the summons, and then selected a lower level once on the elevator.

            if (!_validator.ValidateTravel(requestedLevel, _currentLevel, _intendedDirection))
            {
                _log.LogInformation(
                    "The passenger has selected an incorrect travel destination ({level}), based on the current level ({currentLevel}), and direction ({intendedDirection}).",
                    requestedLevel, _currentLevel, _intendedDirection
                );
                return false;
            }



            return true;
        }
    }
}
