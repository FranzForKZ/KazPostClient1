using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Principal;

namespace PostUserActivity.UnitTests
{
    [TestClass]
    public class SecurityCheck
    {
        [TestMethod]
        public void CheckRoll()
        {

            WindowsPrincipal myPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var IsAdmin = myPrincipal.IsInRole("BUILTIN\\Administrators");
            Console.WriteLine(IsAdmin);
        }
    }
}
