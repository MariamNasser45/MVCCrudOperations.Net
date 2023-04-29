                                   // for all modules in project


namespace Crud.Net.Constant
{
    // since number of permissions in permission.cs 
    //then maxmum number of permission to specific role =12 '3 module * 4 permissions'
    public enum Modules
    {
        Products,
        Stock,
        Categrous

            // making loop on this modules to generate 4 permissions for each module
            // loop is in permissions.cs
    }
}
