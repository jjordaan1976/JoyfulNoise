using MusicSchool.Data.Models;

namespace MusicSchool.Controllers
{
    public class AddBundleRequest
    {
        public LessonBundle Bundle { get; internal set; }
        public IEnumerable<BundleQuarter> Quarters { get; internal set; }
    }
}