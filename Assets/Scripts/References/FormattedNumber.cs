namespace IdleFramework
{
    public class FormattedNumber : StringContainer
    {
        private readonly NumberContainer number;
        private readonly string formatSpecifier;

        public FormattedNumber(NumberContainer number, string formatSpecifier)
        {
            this.number = number;
            this.formatSpecifier = formatSpecifier;
        }

        public string Get(IdleEngine engine)
        {
            return number.Get(engine).ToString(formatSpecifier);
        }
    }
}