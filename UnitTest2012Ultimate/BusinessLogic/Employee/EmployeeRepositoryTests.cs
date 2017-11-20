using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest2012Ultimate.BusinessLogic.Employee
{
    using global::BusinessLogic.Employees;

    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Summary description for EmployeeRepositoryTests
    /// </summary>
    [TestClass]
    public class EmployeeRepositoryTests
    {
        [TestMethod]
        public void EmployeeBaseTest()
        {

            var employeeRepo  = new TestEmployeeRepository();
            var result = employeeRepo[0];
            Assert.IsNull(result);

            var employee = new Employee();

        }


        internal class TestEmployeeRepository : EmployeeRepository
        {

        }
    }
}
