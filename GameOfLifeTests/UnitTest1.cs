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

        [TestMethod]
        public void LeftIsSameAsLeftAndRightIsRight()
        {
            Assert.AreEqual(this._startPosition.left(), this._startPosition.left());
            Assert.AreEqual(this._startPosition.right(), this._startPosition.right());
        }

        [TestMethod]
        public void UpDown()
        {
            Assert.AreEqual(this._startPosition.up().down(), this._startPosition);
            Assert.AreEqual(this._startPosition.down().up(), this._startPosition);
        }

        [TestInitialize]
        public void SetUp()
        {
            _world = new World();
            _startPosition = _world.startPosition();
        }
    }

    internal class Dimension
    {
        public Position _previous;
        public Position _next;

        public override bool Equals(object obj)
        {
            return _previous == ((Dimension)obj)._previous && _next == ((Dimension)obj)._next;
        }
    }

    public class Position
    {
        private Dimension x;
        private Dimension y;
        private Position _left;
        private Position _right;
        private Position _up;
        private Position _down;

        public override bool Equals(object obj)
        {
            return x.Equals(((Position)obj).x) && y.Equals(((Position)obj).y);
        }

        public Position(Position right, Position left, Position down, Position up)
        {
            this.x = new Dimension();
            this.y = new Dimension();
            this.x._previous = right;
            this.x._next = left;
            this.y._previous = up;
            this.y._next = down;
        }

        public Position left()
        {
            if (this.x._next == null)
                this.x._next = new Position(this, null, null, null);
            var left_x = this.x._next;
            return new Position(left_x.x._previous, left_x.x._next, this.y._next, this.y._previous);
        }

        public Position right()
        {
            if (this.x._previous == null)
                this.x._previous = new Position(null, this, null, null);
            var right_x = this.x._previous;
            return new Position(right_x.x._previous, right_x.x._next, this.y._next, this.y._previous);
        }

        public Position up()
        {
            if (this.y._previous == null)
            {
                this.y._previous = new Position(null, null, this, null);
            }

            return this.y._previous;
        }

        public Position down()
        {
            if (this.y._next == null)
            {
                this.y._next = new Position(null, null, null, this);
            }

            return this.y._next;
        }
    }

    public class World
    {
        public Position startPosition()
        {
            return new Position(null, null, null, null);
        }
    }
}