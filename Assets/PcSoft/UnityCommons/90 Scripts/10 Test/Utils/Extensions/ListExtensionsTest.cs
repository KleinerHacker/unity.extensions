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
    }
}
