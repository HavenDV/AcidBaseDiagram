using System;
using NUnit.Framework;

namespace AcidBaseLibrary.Tests
{
    [TestFixture]
    public class Diagram_Tests
    {
        public void InRange(double min, double max, double value, string message)
        {
            Assert.Greater(value, min, message);
            Assert.Less(value, max, message);
        }

        public void AreApproxEqual(double expected, double actual, double precision, string message)
        {
            InRange(expected - precision, expected + precision, actual, message);
        }

        public void Diagram_Center(double precision)
        {
            var parameters = Parameters.FromPCO2AndPH(40, 7.40);
            AreApproxEqual(0.0, parameters.SBE, precision, nameof(Diagram_Center) + ": SBEs not equals");
            AreApproxEqual(24.0, parameters.Bic, precision, nameof(Diagram_Center) + ": Bics not equals");
        }

        [Test]
        public void Parameters_Precision_1()
        {
            Diagram_Center(1.0);
        }

        [Test]
        public void Parameters_Precision_0_1()
        {
            Diagram_Center(0.1);
        }

        [Test]
        public void Diagram_UnknownRange()
        {
            //Assert.Throws<ArgumentException>(() => Diagram.GetData(10.0, 40));
            //Assert.Throws<ArgumentException>(() => Diagram.GetData(0.0, 40));
            //Assert.Throws<ArgumentException>(() => Diagram.GetData(7.40, 0));
            //Assert.Throws<ArgumentException>(() => Diagram.GetData(7.40, 200));
        }
    }
}
