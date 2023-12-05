using NeaProject.Engine;

namespace NeaProject.Test
{
    public class UnitTest1
    {
        [Fact]
        public void MixesWell()
        {
            var result = Renderer.OpacityCalc(0x00, 0xff, 0x80);
            Assert.Equal((uint)0x7f, result);
        }

        [Theory]
        [InlineData(0x00, 0xff, 0x80, 0x7f)]
        [InlineData(0xff, 0x00, 0xff, 0xff)]
        [InlineData(0xff, 0x00, 0x00, 0x00)]
        [InlineData(0x00, 0xff, 0x40, 0xbf)]
        public void MixesGood(byte colour, byte baseColour, byte alpha, byte expectedResult)
        {
            var result = Renderer.OpacityCalc(colour, baseColour, alpha);
            Assert.Equal(expectedResult, result);
        }
    }
}