using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodBuy.Validation
{
    class ValidatableObject<T> where T : IComparable
    {
        private List<IValidationRule<T>> Validations { get; set; } = new List<IValidationRule<T>>();
        public List<string> Errors { get; private set; } = new List<string>();
        public bool IsValid { get; private set; }

        public T Value { get; }

        public ValidatableObject(T value)
        {
            Value = value;
        }

        public void AddValidations(params IValidationRule<T>[] validations)
        {
            foreach (var validation in validations)
            {
                Validations.Add(validation);
            }
        }

        public bool Validate()
        {
            Errors.Clear();

            IEnumerable<string> errors = Validations
                .Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            IsValid = !Errors.Any();

            return this.IsValid;
        }

    }
}
