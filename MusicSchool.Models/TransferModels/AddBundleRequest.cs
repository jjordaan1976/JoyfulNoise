using MusicSchool.Data.Models;

namespace MusicSchool.Models.TransferModels
{
    public class AddBundleRequest
    {
        public LessonBundle Bundle { get; set; }
        public IEnumerable<BundleQuarter> Quarters { get; set; }
    }
}