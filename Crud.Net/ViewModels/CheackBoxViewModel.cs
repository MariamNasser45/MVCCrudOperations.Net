
// using only to creat check box in roles view

// create new roleview not using UsersRolesViewModel becaus we need using
// list to store all roles in DB With Check boxes to them

namespace Crud.Net.ViewModels
{
    // chsnge name of model from Roles view to CheackBox to making it Geniric ' to create cheackbox whith name of roles
    // in multiple views instead of write same code more than one'
    public class CheackBoxViewModel
    {
        public string RoleName { get; set; }

        public bool IsSelected { get; set; }

        // now go to define list of RolesViewModel in user roles viewmodel

    }
}
