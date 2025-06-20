using Microsoft.AspNetCore.Mvc.Rendering;
using newportal.Models;

public class UserNameAndCompanyVM
{
    public ApplicationUser app { get; set; } = new ApplicationUser();

    public List<SelectListItem> UserList { get; set; } = new List<SelectListItem>();
}
