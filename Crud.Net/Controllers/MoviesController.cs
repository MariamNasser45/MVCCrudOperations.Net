//  all views defined in this form

using Crud.Net.Data;
using Crud.Net.Models;
using Crud.Net.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using NToastNotify.Helpers;

// if (condition) by default return True


namespace Crud.Net.Controllers

{
    // model is var of type MovieFormViewModel
    // _Context var of type DbContext
    //movie type of  _Context.Movies
    //viewmodel type of MovieFormViewModel

    public class MoviesController : Controller
    {
        // since we use two lines in two methods (creat , edit) then the best way to use
        // these without rewrit them again is write them in level of controler to can use any time
        // in all method in scop if controller

        private readonly IToastNotification _toastNotification; // To appear massage to user when edit , creat are compelet or field

        private long _PosterMaxLength = 1048576; //1048576 byt = 1 megabyt

        private List<string> _allowedExetintions = new List<string> { ".jpg", ".png" };

        private readonly ApplicationDbContext _Context;    // since we need to work with DB 
                                                           //Then we take an INSTANCE from DbContext

        public MoviesController(ApplicationDbContext Context ,IToastNotification toastNotification)  // and making constructor take instance of Dbcontext As prametere

        {
            _Context = Context; //now we can working with DB using the last instance
             _toastNotification = toastNotification;  

        }
        // comlete of async : Asynchronous
        //using async 'at the same time' and await  To (workin with other task without waiting this task to  complete
        //"compiler working to complete current statement to complet and at the same time working on the next staye")

        public async Task<IActionResult> Index()  //To make Index.cshtml able to access data from DB
                                                  // Index responsible for data appear in form
        {
            var  movies = await _Context.Movies.OrderByDescending(m => m.Rate).ToListAsync(); // movies variable linking with movie table in DB
                                                                                             // using OrderByDescending to arrange movies based on rate
            return View(movies); // return list of existing movies in DB
        }

        ////////////// Implemention of creat page  //////////////

        // in past we used in return only name of model without view name beacuse
        // name of view is the same name of index (create) but now
        // we chanage name of view from (Create) to (MovieForm)

        public async Task<IActionResult> Create()
        {
            var viewmodel = new MovieFormViewModel

            {
                //We need populate 'means giving values to its fields' only values of Genre Exixt in DB

                // There are two ways

                // 1- have view handling Genres and user can add or remove the movies
                //2- MAKING Data Seeding 'when app open view outomaticlly add data in Db'

                // but we add movies maniualy in DB

                Genres = await _Context.Genres.ToListAsync() // need only values which render in drobdownlist "all generes in Db appear in site"
                                                             // Generes defined in  class MovieFormViewModel.cs
            };

            return View("MovieForm", viewmodel); //return View model which create pages we work with it
        }
        // "MovieForm" == Create form but changed the name
        // code to validation action i.e "applies rules to inputted data" of type post 

        [HttpPost]   //attribute tells the routing engine to send any POST requests to that action method 
                     // HTTP command used to send text to a Web server for processing
                     // POST method is widely implemented in HTML files (Web pages) // for sending filled-in forms to the server

        [ValidateAntiForgeryToken]   //(for secure)attribute is to prevent cross-site request forgery attacks     
        public async Task<IActionResult> Create(MovieFormViewModel model)
        {
            if (!ModelState.IsValid) // in case if Model state not exsist returne the model
            {

                model.Genres = await _Context.Genres.ToListAsync(); // to solve problem of genres = null
                                                                    //since model is empty then generes returne with null value

                return View("MovieForm", model); // returne all attribut defined in (MovieFormViewModel)
            }

            // now after check validate of modelstate need to cheack : if any file attach the form or not 'user choos poster or not'

            var files = Request.Form.Files;

            if (!files.Any())  // Any : if finde any filein DB return True
                               // in this case using (!) to apply code if there is no file 


            {
                // since the return is error the must validate genres

                model.Genres = await _Context.Genres.OrderBy(m => m.Name).ToListAsync(); // validate to solve problem of genres = null

                ModelState.AddModelError("Poster", "Pleas select movie poster"); // AddModelError tazking two pramer (key well send error to it , alert massage)

                return View("MovieForm", model);

            }
            // to cheack which exetintion and the size of file are allow for user
            // 1- check exetinsion

            var poster = files.FirstOrDefault(); // this var to contain choosen file name 

            if (!_allowedExetintions.Contains(Path.GetExtension(poster.FileName).ToLower())) // to cheack if the exe of file one of png,jpg or not
            {
                // if exe is not png , jpg applyin next code "if : return false"                               // ToLower used to if name is capital leter conver to small aoutomatic

                model.Genres = await _Context.Genres.OrderBy(m => m.Name).ToListAsync(); // to solve problem of genres = null

                ModelState.AddModelError("Poster", "Only .png , .jpg are allowed "); // AddModelError : taking two pramer (key well send error to it , alert massage)

                return View("MovieForm", model);

            }
            // 2- cheack size

            if (poster.Length > _PosterMaxLength) // 
                                                  // if condition is true : meaning img size > defined limit exeute code
            {
                model.Genres = await _Context.Genres.OrderBy(m => m.Name).ToListAsync(); // to solve problem of genres = null

                ModelState.AddModelError("Poster", "Poster is large please select other in range 1 MB "); // AddModelError : taking two pramer (key well send error to it , alert massage)

                return View("MovieForm", model);
            }

            // To store data of form in Db

            using var dataStream = new MemoryStream();  // MemoryStream: is bult in fun used to define info about Datatype

            await poster.CopyToAsync(dataStream); // CopyToAsync : used to Opens the request stream for reading the uploaded file

            var movies = new Movie // take inistansce from Movie class to access this attribute

            {

                //mapping must reciev value from (form) with the same type of values in DB 
                //can use packge automapper for this but we mak it manually

                Name = model.Name,
                GenreId = model.GenreId,
                year = model.Year,       // model of type of MovieFormView
                History = model.History,
                Rate = model.Rate,
                Poster = dataStream.ToArray(),

            };

            _Context.Movies.Add(movies);  // Movies : name of class and defined in DbContexr , movies type of class Movie

            _Context.SaveChanges(); // to send changes in form to DB

            _toastNotification.AddSuccessToastMessage("Movie Created Successfully");

            /*return View(model);*/                           // this is mistake and error will occure because it cannot make population to dropdownlist
                                                              // becuas the accept model has null genres so it can not select drobdownlist from null
            return RedirectToAction(nameof(Index));         // now we need after add movie go to index page so this return

        } // end of method create

        public async Task<IActionResult> Edit(int? id)  // to provide user ability for editing the movies in site
        {
            // ((id) choose by user cheack with (Id) is key of table Movies in db)

            if (id == null)
                return BadRequest();

            var movie = await _Context.Movies.FindAsync(id); // cheack if Id exist in Db or not 
            {
                if (movie == null)
                    return NotFound();

                var viewmodel = new MovieFormViewModel

                {
                    //need all values appear in  form inorder to when user open movie show all data about this movie

                    Id = movie.Id,
                    Name = movie.Name,
                    GenreId = movie.GenreId,
                    Year = movie.year,
                    History = movie.History,
                    Rate = movie.Rate,
                    Poster = movie.Poster,

                    Genres = await _Context.Genres.ToListAsync()
                };

                return View("MovieForm", viewmodel); // using the view of create because( edit , create) forms are identicle with slightly diff

            }

        } // End of method

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(MovieFormViewModel model)
        {
            if (!ModelState.IsValid) // in case if Model state not exsist returne the model
            {

                model.Genres = await _Context.Genres.ToListAsync(); // to solve problem of genres = null

                return View("MovieForm", model);
            }

            var movie = await _Context.Movies.FindAsync(model.Id);
            {
                // if user choose ID to edit and this ID not exist in DB then return error 

                if (movie == null)
                    return NotFound();

                var files = Request.Form.Files;  // files defined to use it in the next function

                if (files.Any())

                {
                    // if  there are files found then files.any is tru an applied next code

                    var poster = files.FirstOrDefault();

                    using var datastream = new MemoryStream(); // initialize new instance  

                    await poster.CopyToAsync(datastream); // allow user to  Edit img of movie

                    model.Poster = datastream.ToArray(); // data stored in DB 

                    // cheack exetintion

                    if (!_allowedExetintions.Contains(Path.GetExtension(poster.FileName).ToLower())) // to cheack if the exe of file one of png,jpg or not
                    {
                        // if exe is not png , jpg applyin next code "if : return false"            // ToLower used to if name is capital leter conver to small aoutomatic

                        model.Genres = await _Context.Genres.OrderBy(m => m.Name).ToListAsync(); // to solve problem of genres = null

                        ModelState.AddModelError("Poster", "Only .png , .jpg are allowed "); // AddModelError : taking two pramer (key well send error to it , alert massage)

                        return View("MovieForm", model);

                    }
                    // 2- cheack size

                    if (poster.Length > _PosterMaxLength) // 
                                                          // if condition is true : meaning img size > defined limit exeute code
                    {
                        model.Genres = await _Context.Genres.OrderBy(m => m.Name).ToListAsync(); // to solve problem of genres = null

                        ModelState.AddModelError("Poster", "Poster is large please select other in range 1 MB "); // AddModelError : taking two pramer (key well send error to it , alert massage)

                        return View("MovieForm", model);
                    }

                    movie.Poster = model.Poster; // this is applied only in case of user hange the poster
                }                               // in any state if img satisfy all condition or not
                                                // the new img choosen by user eppear in edit form but
                                                // if not satisfy codition alert will appear

                movie.Id = model.Id;
                movie.Name = model.Name;
                movie.GenreId = model.GenreId;             //model type of MovieFormViewModel data entered from user
                movie.year = model.Year;                //movie type of Movies table in DB replace with data entered by user
                movie.History = model.History;
                movie.Rate = model.Rate;
                _Context.SaveChanges();

                _toastNotification.AddSuccessToastMessage("Movie Updated Successfully");

                return RedirectToAction(nameof(Index));
            };
        }
        // Create action of button Detalies
        public async Task<IActionResult> Details(int? id) // after spicify acction now create its view
        {
            // cheack user choose exist id or note

            if (id == null)
                return BadRequest();
            var movie = await _Context.Movies.Include(m => m.Genre).SingleOrDefaultAsync(m => m.Id == id); // cheack if Id in DB contain movie  or not
            if (movie == null)                                                  // Include(m=>m.Genre) : to returne genres from DB to can access it in details form                                                                     
                return NotFound();                                             // if we not use Include(m=>m.Genre) the gener will be null and error will occure in details form

            return View(movie); // in case user choose id exist and contain movies in DB

            //Not that : findAsync not work with Include(m => m.Genre) so we using other cretria
        }

        public async Task<IActionResult> Delete(int? id) // after spicify acction now create its view
        {
            // cheack user choose exist id or note

            if (id == null)
                return BadRequest();
           
            var movie = await _Context.Movies.FindAsync(id); // cheack if Id in DB contain movie  or not

            if (movie == null)                                                  // Include(m=>m.Genre) : to returne genres from DB to can access it in details form                                                                     
                return NotFound();                                             // if we not use Include(m=>m.Genre) the gener will be null and error will occure in details form

            _Context.Movies.Remove(movie);

            _Context.SaveChanges();

             _toastNotification.AddWarningToastMessage("Movie Deleted Successfuly");
            return Ok();   //appear to user when movie delet successful
        }
        // creat action of read more

        //public async Task<IActionResult>ReadMore(int? id )
        //{
        //    if (id == null)
        //        return BadRequest();
        //   var hist = await _Context.Movies.FindAsync(id); // cheack if Id in DB contain movie  or not
        //    if (hist == null)                                                  // Include(m=>m.Genre) : to returne genres from DB to can access it in details form                                                                     
        //        return NotFound();                                             // if we not use Include(m=>m.Genre) the gener will be null and error will occure in details form
        //    int Histlenght = hist.History.Length;

        //    return hist.History.Substring(501,"Histlenght")    // in case user choose id exist and contain movies in DB

        //}


    }

}

              