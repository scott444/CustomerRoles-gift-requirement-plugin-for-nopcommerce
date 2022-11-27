using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.GiftRules.CustomerRoles.Models;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Plugin.Misc.GiftProvider.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.GiftRules.CustomerRoles.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class GiftRulesCustomerRolesController : BasePluginController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGiftService _giftService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public GiftRulesCustomerRolesController(ICustomerService customerService,
            IGiftService giftService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _customerService = customerService;
            _giftService = giftService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(int giftId, int? giftRequirementId)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return Content("Access denied");

            //load the gift
            var gift = await _giftService.GetGiftRequirementByIdAsync(giftId);
            if (gift == null)
                throw new ArgumentException("Gift could not be loaded");

            //check whether the gift requirement exists
            if (giftRequirementId.HasValue && await _giftService.GetGiftRequirementByIdAsync(giftRequirementId.Value) is null)
                return Content("Failed to load requirement.");

            //try to get previously saved restricted customer role identifier
            var restrictedRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(GiftRequirementDefaults.SettingsKey, giftRequirementId ?? 0));

            var model = new RequirementModel
            {
                RequirementId = giftRequirementId ?? 0,
                GiftId = giftId,
                CustomerRoleId = restrictedRoleId
            };

            //set available customer roles
            model.AvailableCustomerRoles = (await _customerService.GetAllCustomerRolesAsync(true)).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = role.Id == restrictedRoleId
            }).ToList();
            model.AvailableCustomerRoles.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Plugins.GiftRules.CustomerRoles.Fields.CustomerRole.Select"),
                Value = "0"
            });

            //set the HTML field prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(GiftRequirementDefaults.HtmlFieldPrefix, giftRequirementId ?? 0);

            return View("~/Plugins/GiftRules.CustomerRoles/Views/Configure.cshtml", model);
        }

        [HttpPost]        
        public async Task<IActionResult> Configure(RequirementModel model)
        {
            if (!await _permissionService.AuthorizeAsync(GiftPermissionProvider.ManageGifts))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                //load the gift
                var gift = await _giftService.GetGiftByIdAsync(model.GiftId);
                if (gift == null)
                    return NotFound(new { Errors = new[] { "Gift could not be loaded" } });

                //get the gift requirement
                var giftRequirement = await _giftService.GetGiftRequirementByIdAsync(model.RequirementId);
                
                //the gift requirement does not exist, so create a new one
                if (giftRequirement == null)
                {
                    giftRequirement = new GiftRequirement
                    {
                        GiftId = gift.Id,
                        GiftRequirementRuleSystemName = GiftRequirementDefaults.SystemName
                    };

                    await _giftService.InsertGiftRequirementAsync(giftRequirement);
                }

                //save restricted customer role identifier
                await _settingService.SetSettingAsync(string.Format(GiftRequirementDefaults.SettingsKey, giftRequirement.Id), model.CustomerRoleId);

                return Ok(new { NewRequirementId = giftRequirement.Id });
            }

            return Ok(new { Errors = GetErrorsFromModelState(ModelState) });
        }

        #endregion

        #region Utilities

        private IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }

        #endregion
    }
}


