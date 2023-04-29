                                //generate name of this permision

namespace Crud.Net.Constant
{
    public static class Permisions  // take module name and returne 4 defined permission to this module
    {
        public static List<string> GeneratePermisionsList(string module) // (string module) : name of module
        {
            return new List<string>()
            {
                // to collect group of string togther there are many ways in c# (string bilder, concatonationn,..)
               // now using stringenterpolation

                $"Permisions.{module}.View"  ,     // {} using to write c# code
                $"Permisions.{module}.Create",    // thes permisions(create , view ,...) changed according to required  
                $"Permisions.{module}.Edit",     // permission.module.name of permision
                $"Permisions.{module}.Delete",

                // this 4 permissions apply for each module 
            };
        }

        public static List<string> GenerateAllPermisions() // to generateall permission to each module
        {
            var allpermission = new List<string>();  // defination of list to reciev perrmission

            var modules =Enum.GetValues(typeof(Modules));  // 1- using Enum  beacouse Modules typr 'Enum'
                                                           // returne array then using it to looping in all modules
             foreach(var module in modules)
            {
                // for each module in modules class  returne list of all permissions so 
                //using get method which recieve name of modul

                allpermission.AddRange(GeneratePermisionsList(module.ToString()));
            }
            return allpermission;
            }

        public static class Products
        {
            public const string View = "Permissions.Products.View";
            public const string Create = "Permissions.Products.Create";
            public const string Edit = "Permissions.Products.Edit";
            public const string Delete = "Permissions.Products.Delete";
        }
    }
    }

