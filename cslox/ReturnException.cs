namespace cslox
{
    class ReturnException : System.Exception
    {
        public readonly object? value;

        public ReturnException(object? value)
        {
            this.value = value;
        }
    }


}