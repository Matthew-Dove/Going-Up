using GoingUp.Models;
using GoingUp.Services;

namespace Tests.GoingUp.Services
{
    public class ElevatorValidatorTests
    {
        private readonly ElevatorValidator _validator = null;

        public ElevatorValidatorTests()
        {
            _validator = new ElevatorValidator();
        }

        [Fact]
        public void ValidTravelDestination()
        {
            var requestedLevel = Level.L10;
            var currentLevel = Level.L1;
            var intendedDirection = Direction.Up;

            var isValid = _validator.ValidateTravel(requestedLevel, currentLevel, intendedDirection);

            Assert.True(isValid);
        }

        [Fact]
        public void CannotTravelDown_WhenMovingUp()
        {
            var requestedLevel = Level.L2;
            var currentLevel = Level.L3;
            var intendedDirection = Direction.Up;

            var isValid = _validator.ValidateTravel(requestedLevel, currentLevel, intendedDirection);

            Assert.False(isValid);
        }

        [Fact]
        public void CannotTravelUp_WhenMovingDown()
        {
            var requestedLevel = Level.L3;
            var currentLevel = Level.L2;
            var intendedDirection = Direction.Down;

            var isValid = _validator.ValidateTravel(requestedLevel, currentLevel, intendedDirection);

            Assert.False(isValid);
        }
    }
}
