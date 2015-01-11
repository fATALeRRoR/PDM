using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PDM.Client.Windows;

namespace PDM.Tests
{
    [TestClass]
    public class AddDocumentTests
    {
        /*
        '+1 functionality does not work correctly in case of leading zeros or in case no number exists yet:
        - ABC.DOC: +1 does not work
        - CDEFv1.4.DOC: +1 leads to CDEFv2.4.DOC. Best is to always +1 the digit most to the right just before the file extension
        - RPT0021.DOC: +1 leads to RPT22.DOC. I would like to keep the leading zero's         
         */

        [TestMethod]
        public void NameIncreaseTest()
        {
            //Test 1
            ExecuteAssert("ABC1.DOC", "ABC.DOC");
            ExecuteAssert("ABC10.DOC", "ABC9.DOC");
            ExecuteAssert("ABC11.DOC", "ABC10.DOC");

            //Test 2
            ExecuteAssert("CDEFv1.5.DOC", "CDEFv1.4.DOC");
            ExecuteAssert("CDEFv1.10.DOC", "CDEFv1.9.DOC");

            //Test 3
            ExecuteAssert("RPT0022.DOC", "RPT0021.DOC");
            ExecuteAssert("RPT0030.DOC", "RPT0029.DOC");

            //Test 4
            ExecuteAssert("RPT001.23.DOC", "RPT001.22.DOC");
            ExecuteAssert("RPT001.023.DOC", "RPT001.022.DOC");
        }

        private void ExecuteAssert(string expected, string input)
        {
            Assert.AreEqual(expected, AddDocumentWindow.IncreaseName(input));
        }
    }
}
