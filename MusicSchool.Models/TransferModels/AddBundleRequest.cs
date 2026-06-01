using MusicSchool.Data.Models;

namespace MusicSchool.Models.TransferModels
{
    public class AddBundleRequest
    {
        public LessonBundle Bundle { get; set; }
        public IEnumerable<BundleQuarter> Quarters { get; set; }

        /// <summary>
        /// The full-year bundle size the student selected (e.g. 32, 48).
        /// The system will prorate this to the number of months remaining in the year.
        /// </summary>
        public int SelectedBundleLessons { get; set; }
    }
}