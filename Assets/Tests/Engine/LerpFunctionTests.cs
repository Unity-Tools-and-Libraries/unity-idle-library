using io.github.thisisnozaku.idle.framework.Engine;
using NUnit.Framework;
namespace io.github.thisisnozaku.idle.framework.Engine
{
    public class LerpFunctionTests
    {
        [Test]
        public void Linear()
        {
            for (float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(x, Lerp.Linear(x));
            }
        }

        [Test]
        public void EaseInSquare()
        {
            for(float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(x * x, Lerp.EaseInSquare(x));
            }
        }

        [Test]
        public void EaseInCube()
        {
            for(float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(x * x * x, Lerp.EaseInCube(x));
            }
        }

        [Test]
        public void EaseInQuad()
        {
            for (float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(x * x * x * x, Lerp.EaseInQuad(x));
            }
        }

        [Test]
        public void EaseOutSquare()
        {
            for (float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(1 - (1 - x) * (1 - x), Lerp.EaseOutSquare(x));
            }
        }

        [Test]
        public void EaseOutCube()
        {
            for (float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(1 - (1 - x) * (1 - x) * (1 - x), Lerp.EaseOutCube(x));
            }
        }

        [Test]
        public void EaseOutQuad()
        {
            for (float x = 0; x < 1f; x++)
            {
                Assert.AreEqual(1 - (1 - x) * (1 - x) * (1 - x) * (1 - x), Lerp.EaseOutQuad(x));
            }
        }
    }
}