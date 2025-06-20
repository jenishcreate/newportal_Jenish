using newportal.Models.ViewModel;

namespace newportal.DataAccess.Repository.IRepository
{
    public interface IListRepository
    {
        Task<List<ListVM>> ListByParentID(String parentId, string roleName);
    }
}
