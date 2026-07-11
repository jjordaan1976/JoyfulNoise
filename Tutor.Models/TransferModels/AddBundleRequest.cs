using Tutor.Data.Models;

namespace Tutor.Models.TransferModels
{
    public class AddBundleRequest
    {
        /// <summary>
        /// Bundle to create. TotalLessons is the full-year size selected (e.g. 32, 36);
        /// StartDate is the first lesson date. The server derives the quarter layout,
        /// end date, and (for Prorata) the reduced TotalLessons.
        /// </summary>
        public LessonBundle Bundle { get; set; } = new();

        public BundleCreationMode Mode { get; set; } = BundleCreationMode.Prorata;
    }
}
