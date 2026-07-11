namespace Tutor.Data.Models
{
    /// <summary>
    /// How a new bundle's quarters are laid out relative to the start date.
    /// </summary>
    public enum BundleCreationMode
    {
        /// <summary>All four quarters of the year. Only valid for January/February starts.</summary>
        Full,

        /// <summary>Only the calendar quarters remaining in the year, each at the bundle's normal per-quarter rate.</summary>
        Prorata
    }
}
