namespace Crud.Net.ViewModels
{                               
                                     // create propirties of usere
    public class UsersViewModel
    {
        public string? Id { get; set; }
        public string ?UserName { get; set; }
        public string ?Email { get; set; }
        
        public IEnumerable<String>? Roles { get; set; }


    }
}
