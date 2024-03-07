using GoingUp;
using GoingUp.Models;
using Microsoft.Extensions.Logging;
using Moq;

/**
 * [level]
 * index button
 * 
 * [1]
 * 0 down
 * 1 up
 * 
 * [2]
 * 2 down
 * 3 up
 * 
 * [3]
 * 4 down
 * 5 up
 * 
 * [4]
 * 6 down
 * 7 up
 * 
 * [5]
 * 8 down
 * 9 up
**/

namespace Tests.GoingUp
{
    public class ElevatorTests
    {
        private readonly Elevator _elevator = null;

        public ElevatorTests()
        {
            var log = new Mock<ILogger<Elevator>>();
            _elevator = new Elevator(log.Object);
        }

        [Fact]
        public void Travel_From_Level1_To_Level2()
        {
            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.Travel(Level.L2);

            while (_elevator.MoveNext());

            Assert.Equal(Level.L2, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level2_To_Level1()
        {
            _elevator.Summon(Level.L2, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level3_To_Level2_To_Level4()
        {
            _elevator.Summon(Level.L3, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L2);
            _elevator.MoveNext();

            _elevator.Summon(Level.L4, Direction.Up);
            _elevator.MoveNext();

            Assert.Equal(Level.L4, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level1_To_Level10()
        {
            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.MoveNext();

            _elevator.Travel(Level.L10);
            _elevator.MoveNext();


            Assert.Equal(Level.L10, _elevator.Level);
            Assert.Equal(Direction.Down, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level10_To_Level1()
        {
            _elevator.Summon(Level.L10, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();


            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level1_To_Level10_To_Level1()
        {
            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.MoveNext();

            _elevator.Travel(Level.L10);
            _elevator.MoveNext();

            _elevator.Summon(Level.L10, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        [Fact]
        public void Travel_From_Level10_To_Level1_To_Level10()
        {
            _elevator.Summon(Level.L10, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.MoveNext();

            _elevator.Travel(Level.L10);
            _elevator.MoveNext();

            Assert.Equal(Level.L10, _elevator.Level);
            Assert.Equal(Direction.Down, _elevator.Direction);
        }

        // Passenger summons lift on the ground floor. Once in chooses to go to level 5.
        [Fact]
        public void Summon_Level1_To_Level5()
        {
            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.Travel(Level.L5);

            while (_elevator.MoveNext()) ;

            Assert.Equal(Level.L5, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        // Passenger summons lift on level 6 to go down. Passenger on level 4 summons the lift to go down. They both choose L1.
        [Fact]
        public void Summon_Level6Down_SummonLevel4Down_TravelLevel1()
        {
            _elevator.Summon(Level.L6, Direction.Down);
            _elevator.MoveNext();

            _elevator.Summon(Level.L4, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }


        // Passenger 1 summons lift to go up from L2. Passenger 2 summons lift to go down from L4. Passenger 1 chooses to go to L6. Passenger 2 chooses to go to Ground Floor.
        [Fact]
        public void Summon_Level2Up_Summon_Level4Down_Level6_Level1()
        {
            _elevator.Summon(Level.L2, Direction.Up);
            _elevator.MoveNext();

            _elevator.Summon(Level.L4, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L6);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }

        // Passenger 1 summons lift to go up from Ground. They choose L5. Passenger 2 summons lift to go down from L4. Passenger 3 summons lift to go down from L10. Passengers 2 and 3 choose to travel to Ground.
        [Fact]
        public void SummonLevel1_ToLevel5_SummonL4_SummonL10_TravelLevel1()
        {
            _elevator.Summon(Level.L1, Direction.Up);
            _elevator.MoveNext();

            _elevator.Travel(Level.L5);
            _elevator.MoveNext();

            _elevator.Summon(Level.L4, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            _elevator.Summon(Level.L10, Direction.Down);
            _elevator.MoveNext();

            _elevator.Travel(Level.L1);
            _elevator.MoveNext();

            Assert.Equal(Level.L1, _elevator.Level);
            Assert.Equal(Direction.Up, _elevator.Direction);
        }
    }
}