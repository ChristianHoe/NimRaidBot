using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace EventBot.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Regex regex = new Regex(@"'(.+?)'");


            string text = "'gruppe' 'n1' 'desc1' 1 'n2' 'desc2' 2 'n3' 'desc3' 3";

            var matche = regex.Matches(text);
            for (int i = 1; i < matche.Count; i++)
            {
            }
        }
    }
}
