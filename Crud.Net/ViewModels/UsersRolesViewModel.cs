namespace Crud.Net.ViewModels
{
    public class UsersRolesViewModel
    {
        public string UserId { get; set; } // to define submit using which Id

        public string UserName { get; set; }  // hint to admin to know which user is managed

        public List<CheackBoxViewModel> Roles { get; set; }

        // now goto usercontroller to define action of manageroles
    }
}
