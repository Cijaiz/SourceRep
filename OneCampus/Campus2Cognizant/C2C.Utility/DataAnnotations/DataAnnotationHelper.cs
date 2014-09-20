using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace C2C.Core.DataAnnotations
{
    /// <summary>
    /// This decorator attribute used for comparing date field with another property of the same class or value passed.
    /// </summary>
    public class CompareDate : ValidationAttribute
    {
        public string OtherProperty { get; private set; }
        public bool MandateComparison { get; private set; }
        public bool CompareByValue { get; private set; }
        public DateTime CompareValue { get; private set; }
        public Type CompareType { get; private set; }

        public enum Type
        {
            Greater,
            Equals,
            Lesser,
            GreaterOrEqual,
            LesserOrEqual
        }

        public CompareDate(string otherProperty, bool mandateComparison, Type type)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
            MandateComparison = mandateComparison;
            CompareByValue = false;
            CompareType = type;
        }

        public CompareDate(DateTime value, Type type)
        {
            CompareType = type;
            CompareValue = value;
            CompareByValue = true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime _date;
            DateTime valueToCompare;

            if (value!= null && DateTime.TryParse(value.ToString(), out _date))
            {
                if (CompareByValue)
                {
                    valueToCompare = CompareValue;
                }
                else
                {
                    PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
                    object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

                    if (otherPropertyInfo == null || otherPropertyValue == null || (DateTime)otherPropertyValue == DateTime.MinValue)
                    {
                        if (MandateComparison)
                        {
                            return new ValidationResult(string.Format("{0} is null or invalid.", OtherProperty));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (!DateTime.TryParse(otherPropertyValue.ToString(), out valueToCompare))
                        {
                            return new ValidationResult(string.Format("{0} is null or invalid format.", OtherProperty));
                        }
                    }
                }


                switch (CompareType)
                {
                    case Type.Greater:
                        if (!(DateTime.Compare(_date.Date, valueToCompare.Date) > 0))
                        {
                            return new ValidationResult(string.Format("{0} needs to be greater than {1}.", validationContext.DisplayName, OtherProperty));
                        }
                        break;
                    case Type.Equals:
                        if (DateTime.Compare(_date.Date, valueToCompare.Date) != 0)
                        {
                            return new ValidationResult(string.Format("{0} is not equal to {1}.", validationContext.DisplayName, OtherProperty));
                        }
                        break;
                    case Type.Lesser:
                        if (!(DateTime.Compare(_date.Date, valueToCompare.Date) < 0))
                        {
                            return new ValidationResult(string.Format("{0} needs to be lesser than {1}.", validationContext.DisplayName, OtherProperty));
                        }
                        break;
                    case Type.GreaterOrEqual:
                         if (!(DateTime.Compare(_date.Date, valueToCompare.Date) >= 0))
                        {
                            return new ValidationResult(string.Format("{0} needs to be greater or equal to {1}.", validationContext.DisplayName, OtherProperty));
                        }
                        break;
                    case Type.LesserOrEqual:
                         if (!(DateTime.Compare(_date.Date, valueToCompare.Date) <= 0))
                        {
                            return new ValidationResult(string.Format("{0} needs to be lesser or equal to {1}.", validationContext.DisplayName, OtherProperty));
                        }
                        break;
                }
            }
            else //if (!string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(string.Format("Enter valid date format in {0}.", validationContext.DisplayName, OtherProperty));
            }

            return null;
        }
    }

    /// <summary>
    /// This class is a decorator attribute used to validate date field for valid date input.
    /// </summary>
    public class IsValidDate : ValidationAttribute
    {
        public IsValidDate()
        {

        }

        public override bool IsValid(object value)
        {
            DateTime temp;
            if (DateTime.TryParse(value.ToString(), out temp))
                return true;
            else
                return false;
        }
    }
}