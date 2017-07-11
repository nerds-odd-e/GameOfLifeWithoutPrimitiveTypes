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

        [TestMethod]
        public void UpRight_UpLeft_DownRight_DownLeft()
        {
            Assert.AreEqual(this._startPosition.upright().down().left(), this._startPosition);
            Assert.AreEqual(this._startPosition.upleft().down().right(), this._startPosition);
            Assert.AreEqual(this._startPosition.downright().up().left(), this._startPosition);
            Assert.AreEqual(this._startPosition.downleft().up().right(), this._startPosition);
        }

        [TestMethod]
        public void shouldBeDeadIfNotSetAlive()
        {
            Assert.IsTrue(this._world.isDead(this._startPosition));
        }

        [TestMethod]
        public void shouldBeAliveIfSetAlive()
        {
            this._world.alive(this._startPosition);
            this._world.alive(this._startPosition.left());
            Assert.IsFalse(this._world.isDead(this._startPosition));
            Assert.IsFalse(this._world.isDead(this._startPosition.left()));
        }

        
        [TestMethod]
        public void AnAlivePositionWithNoAliveNeighbourShouldBeDeadInNextGenerate()
        {
            this._world.alive(this._startPosition);
            Assert.IsTrue(this._world.nextGeneration().isDead(this._startPosition));
        }

        [TestMethod]
        public void AnAlivePositionWithTwoAliveNeighboursShouldSurviveInNextGenerate()
        {
            this._world.alive(this._startPosition);
            this._world.alive(this._startPosition.left());
            this._world.alive(this._startPosition.right());
            Assert.IsFalse(this._world.nextGeneration().isDead(this._startPosition));
        }


        [TestInitialize]
        public void SetUp()
        {
            _world = new World();
            _startPosition = _world.startPosition();
        }
    }

    public class Dimension
    {
        public Dimension _previous;
        public Dimension _next;

        public Dimension(Dimension previous, Dimension next)
        {
            this._previous = previous;
            this._next = next;
        }

        public Dimension previous()
        {
            if (this._previous == null)
                this._previous = new Dimension(null, this);
            return this._previous;
        }

        public Dimension next()
        {
            if (this._next == null)
                this._next = new Dimension(this, null);
            return this._next;
        }
    }

    public class Position
    {
        private Dimension x;
        private Dimension y;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return x.Equals(((Position)obj).x) && y.Equals(((Position)obj).y);
        }

        public Position(Dimension x, Dimension y)
        {
            this.x = x;
            this.y = y;
        }

        public Position left()
        {
            return new Position(x.next(), this.y);
        }

        public Position right()
        {
            return new Position(x.previous(), this.y);
        }

        public Position up()
        {
            return new Position(this.x, this.y.previous());
        }

        public Position down()
        {
            return new Position(this.x, this.y.next());
        }

        public Position upright()
        {
            return new Position(this.x.previous(), this.y.previous());
        }

        public Position upleft()
        {
            return new Position(this.x.next(), this.y.previous());
        }

        public Position downright()
        {
            return new Position(this.x.previous(), this.y.next());
        }

        public Position downleft()
        {
            return new Position(this.x.next(), this.y.next());
        }
    }

    public class PositionList
    {
        private Position _p;
        private PositionList _next;
        public void add(Position position)
        {
            if (this._p != null)
            {
                this._next = new PositionList();
                this._next.add(position);
            }
            else
                this._p = position;
        }
        public bool include(Position position)
        {
            if (this._p == null) return false;
            return (this._p.Equals(position) || this._next.include(position));
        }

    }
    public class World
    {
        private PositionList lives = new PositionList();
        public Position startPosition()
        {
            return new Position(new Dimension(null, null), new Dimension(null, null));
        }

        public void alive(Position position)
        {
            lives.add(position);
        }

        public bool isDead(Position position)
        {
            return !lives.include(position);
        }

        public World nextGeneration()
        {
            return new World();
        }
    }
}