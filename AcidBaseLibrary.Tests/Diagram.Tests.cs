using System;
using NUnit.Framework;

namespace AcidBaseLibrary.Tests
{
    [TestFixture]
    public class Diagram_Tests
    {
        public void InRange(double min, double max, double value)
        {
            Assert.Greater(value, min);
            Assert.Less(value, max);
        }

        public void AreApproxEqual(double expected, double actual, double precision = 1.0)
        {
            InRange(expected - precision, expected + precision, actual);
        }

        [Test]
        public void Diagram_Center()
        {
            var data = Diagram.GetData(7.40, 40);
            AreApproxEqual(0.0, data.Item1);
            AreApproxEqual(24.0, data.Item2);
        }

        [Test]
        public void Diagram_UnknownRange()
        {
            Assert.Throws<ArgumentException>(() => Diagram.GetData(10.0, 40));
            Assert.Throws<ArgumentException>(() => Diagram.GetData(0.0, 40));
            Assert.Throws<ArgumentException>(() => Diagram.GetData(7.40, 0));
            Assert.Throws<ArgumentException>(() => Diagram.GetData(7.40, 200));
        }
    }
}
