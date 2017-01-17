using System;
using NUnit.Framework;

namespace AcidBaseLibrary.Tests
{
    [TestFixture]
    public class Diagram_Tests
    {
        [Test]
        public void Diagram_Center()
        {
            var data = Diagram.GetData(7.40, 40);
            Assert.AreEqual(0.0, data.Item1);
            Assert.AreEqual(24.0, data.Item2);
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
