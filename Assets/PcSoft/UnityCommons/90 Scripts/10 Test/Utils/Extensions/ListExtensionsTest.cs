using System.Linq;
using NUnit.Framework;
using PcSoft.UnityCommons._90_Scripts._00_Runtime.Utils.Extensions;

namespace PcSoft.UnityCommons._90_Scripts._10_Test.Utils.Extensions
{
    public class ListExtensionsTest
    {
        [Test]
        public void GetRandom()
        {
            var array = new [] {"Hello", "World", "Unity"};

            for (var i = 0; i < 100; i++)
            {
                array.GetRandom();
            }

            for (var i = 0; i < 100; i++)
            {
                var random = array.GetRandom("Unity");
                Assert.AreNotEqual("Unity", random);
            }
        }

        [Test]
        public void Remove()
        {
            var array = new [] {"Hello", "World", "Unity"};
            array = array.Remove("World").ToArray();
            
            Assert.AreEqual(2, array.Length);
            Assert.AreEqual("Hello", array[0]);
            Assert.AreEqual("Unity", array[1]);
        }
    }
}
