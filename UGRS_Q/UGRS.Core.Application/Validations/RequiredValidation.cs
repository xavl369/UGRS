using System.Globalization;
using System.Windows.Controls;

namespace UGRS.Core.Application.Validations
{
    public class RequiredValidation : ValidationRule
    {
        public override ValidationResult Validate(object pObjValue, CultureInfo pObjCultureInfo)
        {
            if (!string.IsNullOrWhiteSpace((pObjValue ?? "").ToString()))
            {
                return ValidationResult.ValidResult;
            }
            else
            {
                return new ValidationResult(false, "Favor de capturar.");
            }
        }
    }
}
