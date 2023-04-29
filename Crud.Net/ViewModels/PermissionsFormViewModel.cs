namespace Crud.Net.ViewModels
{
    public class PermissionsFormViewModel
    {
        public string RoleId { get; set; } // to define submit using which Id

        public string RoleName { get; set; }  // hint to admin to know which user is managed

        public List<CheackBoxViewModel> RoleCalims { get; set; } 
        
        // CheackBoxViewModel using in thes also becaus its generic 'allow to reusable'

        // now goto usercontroller to define action of manageroles

    }
}
