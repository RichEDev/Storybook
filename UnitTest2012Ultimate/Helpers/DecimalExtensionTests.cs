using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpendManagementLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpendManagementLibrary.Helpers.Tests
{
    using SpendManagementLibrary.Enumerators;

    [TestClass()]
    public class DecimalExtensionTests
    {
        [TestMethod()]
        public void EsrRoundingTestWhenValueZero()
        {
            this.RoundDecimal(0, EsrRoundingType.Down, 0);
            this.RoundDecimal(0, EsrRoundingType.Up, 0);
            this.RoundDecimal(0, EsrRoundingType.ForceUp, 0);
        }

        [TestMethod()]
        public void EsrRoundingTestWhenValuePointFour()
        {
            this.RoundDecimal(0.4m, EsrRoundingType.Down, 0);
            this.RoundDecimal(0.4m, EsrRoundingType.Up, 0);
            this.RoundDecimal(0.4m, EsrRoundingType.ForceUp, 1);
        }

        [TestMethod()]
        public void EsrRoundingTestWhenValuePointFourNine()
        {
            this.RoundDecimal(0.49m, EsrRoundingType.Down, 0);
            this.RoundDecimal(0.49m, EsrRoundingType.Up, 0);
            this.RoundDecimal(0.49m, EsrRoundingType.ForceUp, 1);
        }

        [TestMethod()]
        public void EsrRoundingTestWhenValuePointFive()
        {
            this.RoundDecimal(0.5m, EsrRoundingType.Down, 0);
            this.RoundDecimal(0.5m, EsrRoundingType.Up, 1);
            this.RoundDecimal(0.5m, EsrRoundingType.ForceUp, 1);
        }

        [TestMethod()]
        public void EsrRoundingTestWhenValuePointFiveOne()
        {
            this.RoundDecimal(0.51m, EsrRoundingType.Down, 0);
            this.RoundDecimal(0.51m, EsrRoundingType.Up, 1);
            this.RoundDecimal(0.51m, EsrRoundingType.ForceUp, 1);
        }

        [TestMethod()]
        public void EsrRoundingTestWhenValueNegative()
        {
            this.RoundDecimal(-0.51m, EsrRoundingType.Down, 0);
            this.RoundDecimal(-0.51m, EsrRoundingType.Up, -1);
            this.RoundDecimal(-0.51m, EsrRoundingType.ForceUp, 0);
        }

        [TestMethod(), ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EsrRoundingTestWhenValueMax()
        {
            this.RoundDecimal(decimal.MaxValue, EsrRoundingType.Down, decimal.MaxValue);
            this.RoundDecimal(decimal.MaxValue, EsrRoundingType.Up, decimal.MaxValue);
            this.RoundDecimal(decimal.MaxValue, EsrRoundingType.ForceUp, decimal.MaxValue);
        }

        /// <summary>
        /// The round decimal.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="esrRoundingType">
        /// The esr rounding type.
        /// </param>
        /// <param name="expectedDecimal">
        /// The expecte decimal result.
        /// </param>
        private void RoundDecimal(decimal value, EsrRoundingType esrRoundingType, decimal expectedDecimal)
        {
            var result = value.EsrRounding(esrRoundingType);
            Assert.IsTrue(result == expectedDecimal, "Was expecting {0}, got {1}", expectedDecimal, result);
        }

    }
}