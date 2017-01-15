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
            var data = Diagram.GetData(40, 0);
            Assert.AreEqual(7.4, data.Item1);
            Assert.AreEqual(24, data.Item2);
        }
    }
}
