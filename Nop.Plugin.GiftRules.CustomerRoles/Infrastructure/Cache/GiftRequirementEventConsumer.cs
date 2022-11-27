using System.Threading.Tasks;
using Nop.Core.Events;
using Nop.Plugin.Misc.GiftProvider.Domain;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.GiftRules.CustomerRoles.Infrastructure.Cache
{
    /// <summary>
    /// Discount requirement rule event consumer (used for removing unused settings)
    /// </summary>
    public partial class GiftRequirementEventConsumer : IConsumer<EntityDeletedEvent<GiftRequirement>>
    {
        #region Fields
        
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public GiftRequirementEventConsumer(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle discount requirement deleted event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<GiftRequirement> eventMessage)
        {
            var giftRequirement = eventMessage?.Entity;
            if (giftRequirement == null)
                return;

            //delete saved restricted customer role identifier if exists
            var setting = await _settingService.GetSettingAsync(string.Format(GiftRequirementDefaults.SettingsKey, giftRequirement.Id));
            if (setting != null)
                await _settingService.DeleteSettingAsync(setting);
        }

        #endregion
    }
}