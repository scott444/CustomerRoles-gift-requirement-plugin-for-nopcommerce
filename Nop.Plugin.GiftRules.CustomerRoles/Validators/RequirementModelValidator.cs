using FluentValidation;
using Nop.Plugin.GiftRules.CustomerRoles.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.GiftRules.CustomerRoles.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseNopValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.GiftId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.GiftRules.CustomerRoles.Fields.GiftId.Required"));
            RuleFor(model => model.CustomerRoleId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.GiftRules.CustomerRoles.Fields.CustomerRoleId.Required"));
        }
    }
}
