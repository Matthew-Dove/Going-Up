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
        private const int FLOORS = 10; // The number of floors in a building.
        private const int SUMMON_BUTTONS = 2; // The number of buttons on the outside of an elevator.

        public Level Level { get { return _currentLevel; } }
        public Direction Direction { get { return _intendedDirection; } }

        private Level _currentLevel; // The elevator's current level.
        private Direction _intendedDirection; // The elevator's intended direction.

        private readonly ILogger<Elevator> _log;
        private readonly ElevatorValidator _validator = null;
        private readonly bool[] _summons = null;

        public Elevator(ILogger<Elevator> log)
        {
            _currentLevel = Level.L1;
            _intendedDirection = Direction.Up; // Assume the elevator "starts" on the ground floor (L1).

            _log = log;
            _validator = new ElevatorValidator();
            _summons = new bool[FLOORS * SUMMON_BUTTONS];
        }

        /// <summary>User has requested the elevator to their level, they have selected the direction they intend to travel to.</summary>
        /// <param name="level">The level the request came from.</param>
        /// <param name="direction">The user's wish to travel either up, or down.</param>
        public void Summon(Level level, Direction direction)
        {
            // Assume the level, and direction are always correct, as only valid summon buttons exist on each floor.

            _log.LogInformation("Elevator summoned to level {level}, going {direction}.", level, direction);
            _summons[((int)level * SUMMON_BUTTONS) + (int)direction] = true; // Idempotent operation (summon button may be pressed many times).
        }

        public bool Travel(Level requestedLevel)
        {
            // Once in the elevator, a user *can* select an incorrect level for the current journey (as all buttons exist inside the elevator).
            // For example, if they selected "up" on the summons, and then selected a lower level once on the elevator.

            if (!_validator.ValidateTravel(requestedLevel, _currentLevel, _intendedDirection))
            {
                _log.LogInformation(
                    "The passenger has selected an incorrect travel destination ({level}), based on the current level ({currentLevel}), and direction ({intendedDirection}).",
                    requestedLevel, _currentLevel, _intendedDirection
                );
                return false; // Do not accept the travel command.
            }

            _summons[((int)requestedLevel * SUMMON_BUTTONS) + (int)_intendedDirection] = true; // Summons has the same effect as a travel request (as passengers may never get on the elevator).

            return true;
        }

        /// <summary>
        /// Executes the next travel destination / summons.
        /// </summary>
        /// <returns>True when there is another pending stop, otherwise false.</returns>
        public bool MoveNext()
        {
            _log.LogInformation("Received request to move to the next stop. There are a total of {summons} pending stops.", _summons.Count(x => x));

            if (TryGetDestination(out var destination))
            {
                // Move the elevator.
                _currentLevel = destination.level;
                _intendedDirection = destination.direction;
                _summons[((int)_currentLevel * SUMMON_BUTTONS) + (int)_intendedDirection] = false; // Mark travel / summon as completed.

                // When at the top, or bottom floors, then we can force the next "intended direction".
                if (_currentLevel == Level.L1) _intendedDirection = Direction.Up;
                if (_currentLevel == Level.L10) _intendedDirection = Direction.Down;

                _log.LogInformation("Elevator moved {direction} to level {level}.", destination.direction, destination.level);
            }

            return _summons.Any(x => x);
        }

        private bool TryGetDestination(out (Level level, Direction direction) destination)
        {
            destination = (Level.L1, Direction.Up);
            if (!_summons.Any(x => x)) return false;

            // Special case: elevator is on the first floor, and it has been summoned there.
            if (_currentLevel == Level.L1 && _summons[((int)_currentLevel * SUMMON_BUTTONS) + (int)Direction.Up])
            {
                destination = (Level.L1, Direction.Up);
            }

            // Special case: elevator is on the top floor, and it has been summoned there.
            else if (_currentLevel == Level.L10 && _summons[((int)_currentLevel * SUMMON_BUTTONS) + (int)Direction.Down])
            {
                destination = (Level.L10, Direction.Down);
            }

            // Find the next stop in the up direction. If no match is found, then look for a stop in the down direction (from the top floor down).
            else if (_intendedDirection == Direction.Up)
            {
                // Look for "up" summons above our current floor.
                var floor = _summons.Skip((int)_currentLevel * SUMMON_BUTTONS).Select((x, i) => (x && i % 2 != 0, i)).FirstOrDefault(x => x.Item1);
                var direction = Direction.Up;

                // If there is no "up" destination above the current floor, then look for the next "down" target.
                if (floor == default)
                {
                    floor = _summons.Select((x, i) => (x && i % 2 == 0, i)).Last(x => x.Item1);
                    direction = Direction.Down;
                }

                destination = ((Level)(floor.i / SUMMON_BUTTONS), direction);
            }

            // Find the next stop in the down direction. If no match is found, then look for a stop in the up direction (from the bottom floor up).
            else if (_intendedDirection == Direction.Down)
            {
                // Look for "down" summons below our current floor.
                var floor = _summons.Where((x, i) => i < (int)_currentLevel * SUMMON_BUTTONS).Select((x, i) => (x && i % 2 == 0, i)).LastOrDefault(x => x.Item1);
                var direction = Direction.Down;

                // If there is no "down" destination below the current floor, then look for the next "up" target.
                if (floor == default)
                {
                    floor = _summons.Select((x, i) => (x && i % 2 != 0, i)).First(x => x.Item1);
                    direction = Direction.Up;
                }

                destination = ((Level)(floor.i / SUMMON_BUTTONS), direction);

            }

            return true;
        }
    }
}
