
namespace Nop.Plugin.GiftRules.CustomerRoles
{
    /// <summary>
    /// Represents defaults for the discount requirement rule
    /// </summary>
    public static class GiftRequirementDefaults
    {
        /// <summary>
        /// The system name of the discount requirement rule
        /// </summary>
        public static string SystemName => "GiftRequirement.MustBeAssignedToCustomerRole";

        /// <summary>
        /// The key of the settings to save restricted customer roles
        /// </summary>
        public static string SettingsKey => "GiftRequirement.MustBeAssignedToCustomerRole-{0}";

        /// <summary>
        /// The HTML field prefix for discount requirements
        /// </summary>
        public static string HtmlFieldPrefix => "GiftRulesCustomerRoles{0}";
    }
}
