using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostUserActivity.HW.WIALib;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class WIAScanners_UnitTests
    {
        [TestMethod]
        public void FindWIAScanners_TestMethod()
        {
            try
            {
                var manager = new WiaManager();
                var divces = manager.Devices.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

		[TestMethod]
        public void ScanUsedWIA_TestMethod()
        {

        }
    }
}
