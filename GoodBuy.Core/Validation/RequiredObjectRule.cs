﻿namespace GoodBuy.Validation
{
    public class RequiredObjectRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; } = "Preenchimento é obrigatório!";

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(value.ToString());
        }
    }
}
