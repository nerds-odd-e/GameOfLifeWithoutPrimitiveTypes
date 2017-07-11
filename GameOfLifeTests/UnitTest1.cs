using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameOfLifeTests
{
    [TestClass]
    public class UnitTest1
    {
        private Position _startPosition;
        private World _world;
        //ctrl+R, A => run all tests

        [TestMethod]
        public void SamePositionEqualToEachOther()
        {
            Assert.AreEqual(this._startPosition, this._startPosition);
        }

        [TestMethod]
        public void LeftSideOfAPositionIsNotTheSameAsTheStartPosition()
        {
            Assert.AreNotEqual(this._startPosition.left(), this._startPosition);
        }

        [TestMethod]
        public void leftThenRigthShouldReturnToTheStart()
        {
            Assert.AreEqual(this._startPosition.left().right(), this._startPosition);
        }

        [TestMethod]
        public void RightThenLeftShouldReturnToTheStart()
        {
            Assert.AreEqual(this._startPosition.right().left(), this._startPosition);
        }
        
        [TestInitialize]
        public void SetUp()
        {
            _world = new World();
            _startPosition = _world.startPosition();
        }
    }

    public class Position
    {
        private Position position;
        private Position _right = null;

        public Position(Position position)
        {
            this._right = position;
        }

        public Position left()
        {
            return new Position(this);
        }

        public Position right()
        {
            return this._right;
        }
    }

    public class World
    {
        public Position startPosition()
        {
            return new Position(null);
        }
    }
}