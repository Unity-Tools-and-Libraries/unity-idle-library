using System.Collections.Generic;
using BreakInfinity;
namespace IdleFramework
{
    public class Product: NumberContainer
    {
        private NumberContainer left;
        private NumberContainer right;

        private Product(NumberContainer left, NumberContainer right)
        {
            this.left = left;
            this.right = right;
        }

        public BigDouble Get(IdleEngine engine)
        {
            var leftValue = left.Get(engine);
            var rightValue = right.Get(engine);
            return  leftValue * rightValue;
        }

        public static Product Of(NumberContainer left, NumberContainer right)
        {
            return new Product(left, right);
        }
    }
}