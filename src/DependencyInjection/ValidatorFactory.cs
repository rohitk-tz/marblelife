using Core.Application.Attribute;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjection
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        public override IValidator CreateInstance(Type validatorType)
        {
            if (validatorType.IsGenericType)
            {
                var args = validatorType.GetGenericArguments();

                if (args.Any(type => type.GetCustomAttributes(typeof(NoValidationResolveAtStartAttribute), false).Any() || type.GetCustomAttributes(typeof(NoValidatorRequiredAttribute), false).Any() ||
                    type.IsPrimitive || type.IsValueType || type == typeof(string) || (type == typeof(Int64[]))))
                {
                    return null;
                }
            }

            var validator = IoC.Resolve(validatorType) as IValidator;
            return validator;
        }
    }
}
