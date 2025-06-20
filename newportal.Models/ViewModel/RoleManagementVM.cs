

using Microsoft.AspNetCore.Mvc.Rendering;

namespace newportal.Models.ViewModel
{
    public class RoleManagementVM
    {
        public List<string?> Rolelist { get; set; }

        public IEnumerable<SelectListItem> RoleItem { get; set; }
    }
}
