using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GameOfLifeTests
{
    public class Dimension
    {
        private static Dimension nullDimension = new Dimension();
        public Dimension _next = nullDimension;
        public Dimension _previous = nullDimension;

        public Dimension next()
        {
            if (_next == nullDimension)
            {
                _next = new Dimension();
                _next._previous = this;
            }
            return _next;
        }

        public Dimension previous()
        {
            if (_previous == nullDimension)
            {
                _previous = new Dimension();
                _previous._next = this;
            }
            return _previous;
        }
    }

    public class Position
    {
        private Dimension x;
        private Dimension y;

        public Position(Dimension x, Dimension y)
        {
            this.x = x;
            this.y = y;
        }

        public Position down()
        {
            return new Position(x, y.next());
        }

        public Position downleft()
        {
            return new Position(x.next(), y.next());
        }

        public Position downright()
        {
            return new Position(x.previous(), y.next());
        }

        public bool sameAs(object obj)
        {
            return x.Equals(((Position)obj).x) && y.Equals(((Position)obj).y);
        }


        public Position left()
        {
            return new Position(x.next(), y);
        }

        public Position right()
        {
            return new Position(x.previous(), y);
        }

        public Position up()
        {
            return new Position(x, y.previous());
        }
        public Position upleft()
        {
            return new Position(x.next(), y.previous());
        }

        public Position upright()
        {
            return new Position(x.previous(), y.previous());
        }

        public PositionSet neighbours()
        {
            return new PositionSet()
                .add(left())
                .add(right())
                .add(up())
                .add(down())
                .add(upleft())
                .add(upright())
                .add(downleft())
                .add(downright());
        }
    }

    public class PositionSet
    {
        private static Position nullPosition = new Position(new Dimension(), new Dimension());
        public PositionSet _next;
        public Position _value = nullPosition;
        public PositionSet add(Position position)
        {
            if (!_value.sameAs(position))
                if (_value == nullPosition)
                {
                    _value = position;
                    _next = new PositionSet();
                }
                else
                    _next.add(position);
            return this;
        }
        public bool include(Position position)
        {
            return (_value != nullPosition) &&
             (_value.sameAs(position) || _next.include(position));
        }

        internal bool exactlyThree()
        {
            return _value != nullPosition && _next.exactlyTwo();
        }

        internal bool exactlyTwo()
        {
            return _value != nullPosition && _next.exactlyOne();
        }

        private bool exactlyOne()
        {
            return _value != nullPosition && _next._value == nullPosition;
        }

        internal PositionSet intersect(PositionSet positionSet)
        {
            if (_value == nullPosition)
                return new PositionSet();
            PositionSet intersection = _next.intersect(positionSet);
            if (positionSet.include(_value))
                intersection.add(_value);
            return intersection;
        }

        public PositionSet filter(Func<Position, bool> condition)
        {
            PositionSet newlifes = new PositionSet();
            foreach(Position pos in this)
                if (condition(pos))
                    newlifes.add(pos);
            return newlifes;
        }

        public PositionSet append(PositionSet pset)
        {
            foreach(Position pos in pset)
                add(pos);
            return this;
        }

        public IEnumerator<Position> GetEnumerator()
        {
            PositionSet ps = this;
            while (ps._value != nullPosition)
            {
                Position pos = ps._value;
                yield return pos;
                ps = ps._next;
            }
        }

        public PositionSet concat(PositionSet other)
        {
            return new PositionSet().append(this).append(other);
        }

    }

    public class World
    {
        private PositionSet lives = new PositionSet();

        public World(PositionSet lives)
        {
            this.lives = lives;
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
            return new World(survivors().concat(reproductions()));
        }

        private PositionSet reproductions()
        {
            PositionSet positionSet = new PositionSet();
            foreach(Position pos in lives)
                positionSet.append(pos.neighbours());
            return positionSet.filter(pos => lives.intersect(pos.neighbours()).exactlyThree());
        }

        private PositionSet survivors()
        {
            return lives.filter(pos => lives.intersect(pos.neighbours()).exactlyTwo());
        }

        internal bool isAlive(Position position)
        {
            return !isDead(position);
        }
    }

    [TestFixture()]
    public class UnitTest1
    {
        private Position aPosition;
        private World _world;
        
        [SetUp]
        public void SetUp()
        {
            _world = new World(new PositionSet());
            aPosition = new Position(new Dimension(), new Dimension());
        }

        [Test()]
        public void SamePositionEqualToEachOther()
        {
            Assert.AreEqual(aPosition, aPosition);
        }

        [Test()]
        public void allTheNeighboursExist()
        {
            Assert.IsTrue(aPosition.left().sameAs(aPosition.left()));
            Assert.IsTrue(aPosition.right().sameAs(aPosition.right()));
            Assert.IsFalse(aPosition.left().sameAs(aPosition));
            Assert.IsTrue(aPosition.left().right().sameAs(aPosition));
            Assert.IsTrue(aPosition.right().left().sameAs(aPosition));
            Assert.IsTrue(aPosition.up().down().sameAs(aPosition));
            Assert.IsTrue(aPosition.down().up().sameAs(aPosition));
            Assert.IsTrue(aPosition.upright().down().left().sameAs(aPosition));
            Assert.IsTrue(aPosition.upleft().down().right().sameAs(aPosition));
            Assert.IsTrue(aPosition.downright().up().left().sameAs(aPosition));
            Assert.IsTrue(aPosition.downleft().up().right().sameAs(aPosition));
        }

        [Test()]
        public void shouldBeAliveIfSetAlive()
        {
            _world.alive(aPosition);
            _world.alive(aPosition.left());
            Assert.IsFalse(_world.isDead(aPosition));
            Assert.IsFalse(_world.isDead(aPosition.left()));
        }

        [Test()]
        public void shouldBeDeadIfNotSetAlive()
        {
            Assert.IsTrue(_world.isDead(aPosition));
        }

        [Test()]
        public void AnAlivePositionWithNoAliveNeighbourShouldBeDeadInNextGenerate()
        {
            _world.alive(aPosition);
            Assert.IsTrue(_world.nextGeneration().isDead(aPosition));
        }

        [Test()]
        public void AnAlivePositionWithTwoAliveNeighboursShouldSurviveInNextGenerate()
        {
            _world.alive(aPosition);
            _world.alive(aPosition.left());
            _world.alive(aPosition.right());
            Assert.IsTrue(_world.nextGeneration().isAlive(aPosition));
        }

        [Test()]
        public void ReproductionSaysADeadPositionWithThreeAliveNeighboursShouldComeToLifeInNextGenerate()
        {
            _world.alive(aPosition.up());
            _world.alive(aPosition.left());
            _world.alive(aPosition.upleft());
            Assert.IsTrue(_world.nextGeneration().isAlive(aPosition));
        }

        [Test()]
        public void AnAlivePositionWithFourAliveNeighboursShouldDieInNextGenerate()
        {
            _world.alive(aPosition);
            _world.alive(aPosition.up());
            _world.alive(aPosition.down());
            _world.alive(aPosition.left());
            _world.alive(aPosition.upleft());
            Assert.IsTrue(_world.nextGeneration().isDead(aPosition));
        }

    }
}
