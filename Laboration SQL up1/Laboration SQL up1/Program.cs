
using Laboration_SQL_up1.Migrations;

namespace Laboration_SQL_up1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Välkommen till mitt biblioteksprogram!---");
            bool run = true;
            bool loggedIn = false;
            bool isAdmin = false;
            while (run)
            {
                if (!loggedIn)
                {
                    HeadMenu();
                    string choice = Console.ReadLine();
                    AddAdmin(); //username: admin, pw: admin //skapar ett adminkonto från start (om det inte redan finns)
                    switch (choice)
                    {
                        case "1":
                            AddUser();
                            break;
                        case "2":
                            Console.WriteLine("Ange användarnamn: ");
                            string username = Console.ReadLine();

                            Console.WriteLine("Ange lösenord: ");
                            string password = Console.ReadLine();
                            //hämta användaren från databas om den finns
                            // och jämför lösenordet
                            var user = GetUser(username);
                            if (user != null && user.Password == password)
                            {
                                Console.WriteLine($"Du loggades in, välkommen {username}!");
                                loggedIn = true;

                                if (username == "admin" && password == "admin")
                                {
                                    isAdmin = true;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Fel användarnamn eller lösenord.");
                            }
                            break;

                        case "3":
                            Console.WriteLine("Ange användarnamn för admin: "); //username = admin
                            string adminNamn = Console.ReadLine();
                            Console.WriteLine("Ange lösenord för admin: "); //lösen = admin
                            string adminPw = Console.ReadLine();

                            if (adminNamn == "admin" && adminPw == "admin")
                            {
                                Console.WriteLine("Inloggning lyckades");
                                loggedIn = true;
                                isAdmin = true;
                                continue;
                            }
                            break;
                        

                        case "4":
                            Console.WriteLine("Avslutar programmet...");
                            run = false;
                            break;
                        default:
                            Console.WriteLine("Något gick fel, försök igen...");
                            break;

                    }
                }
                else if (isAdmin)
                {
                    AdminMenu();
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            AddUser();
                            break;
                            

                        case "2":
                            Console.WriteLine("Skriv namnet på den du vill ta bort från databasen: ");
                            string name = Console.ReadLine();
                            RemoveUser(name);
                            break;

                        case "3":
                            AddBook();
                            break;

                        case "4":
                            RemoveBook();
                            break;

                        case "5":
                            Console.WriteLine("Skriv in ditt nuvarande användarnamn: ");
                            string currentUsername = Console.ReadLine();

                            Console.WriteLine("Vill du ändra användarnamn eller lösenord på ditt konto?(namn/pw)");
                            string answer = Console.ReadLine().ToLower();
                            if (answer == "namn")
                            {
                                Console.WriteLine("Skriv in ditt nya användarnamn: ");
                                string newName = Console.ReadLine();
                                UpdateUserName(currentUsername, newName);
                            }
                            else if (answer == "pw")
                            {
                                Console.WriteLine("Skriv in ditt nya lösenord: ");
                                string newPw = Console.ReadLine();
                                UpdateUserPw(currentUsername, newPw);
                            }
                            else
                            {
                                Console.WriteLine("Något gick fel, försök igen.");
                            }
                            break;
                        
                        case "6":
                            Console.WriteLine("Skriv in titeln på den bok som du vill ändra: ");
                            string title = Console.ReadLine();
                            UpdateBook(title);
                            break;

                        case "7":
                            Console.WriteLine("Lista på böcker: ");
                            BookList();
                            break;
                        case "8":
                            Console.WriteLine("Du loggades ut");
                            isAdmin = false;
                            loggedIn = false;
                            break;
                        default:
                            Console.WriteLine("Något gick fel, försök igen...");
                            break;
                    }
                }
                else
                {
                    
                    LoggedinMenu();
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            BorrowBook();
                            break;

                        case "2":
                            Console.WriteLine("Skriv titeln på boken du vill lämna tillbaka: ");
                            string answer = Console.ReadLine();
                            ReturnBook(answer);
                            break;

                        case "3":
                            Console.WriteLine("Lista på böcker: ");
                            BookList();
                            break;

                        case "4":
                            Console.WriteLine("Skriv titeln på boken du letar efter: ");
                            string title = Console.ReadLine();
                            SearchBookTitle(title);
                            break;

                        case "5":
                            LoanedBookList();
                            break;
                            
                        case "6":
                            Console.WriteLine("Du loggades ut.");
                            loggedIn = false;
                            break;
                        default:
                            Console.WriteLine("Något gick fel, försök igen");
                            break;
                    }
                }
                
            }
        }

        static void AddUser()
        {
            Console.WriteLine("Välj användarnamn: ");
            string newUsername = Console.ReadLine();
            while(newUsername == "")
            {
                Console.WriteLine("Du får inte ange ett tomt användarnamn, försök igen.");
                Console.WriteLine("Välj användarnamn: ");
                newUsername = Console.ReadLine();
            }
            Console.WriteLine("Välj lösenord: ");
            string newPassword = Console.ReadLine();
            //Skapa ett nytt User-obj o lägg till i databasen
            Console.WriteLine("Du har skapat ett konto, gå tillbaka till huvudmenyn för att logga in");
            using (var context = new UserContext())
            {
                var user = new User
                {
                    Username = newUsername,
                    Password = newPassword
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        static void AddAdmin()
        {
            using (var context = new UserContext())
            {
                var existingAdmin = context.Users.FirstOrDefault(u => u.Username == "admin");
                if (existingAdmin == null)
                {
                    var user = new User
                    {
                        Username = "admin",
                        Password = "admin"
                    };

                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Det finns redan ett admin konto skapat.");
                }
                
            }
        }
        static User GetUser(string username)
        {
            using (var context = new UserContext())
            {
                return context.Users.FirstOrDefault(u => u.Username == username);
            }
        }
        static User RemoveUser(string username)
        {
            using (var context = new UserContext()) //öppnar lådan kan man säga och context blir en instans av klassen usercontext
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    Console.WriteLine($"Användaren: {username}, togs bort från databasen.");
                    context.Users.Remove(user);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Hittade inte en användare med det namnet i databasen.");
                }
                return user;
            }
            
        }
        static void UpdateUserName(string currentUsername, string newName)
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == currentUsername);
                if (user != null)
                {
                    user.Username = newName;
                    context.SaveChanges();
                    Console.WriteLine("Användarnamn uppdaterat");
                }
                else
                {
                    Console.WriteLine("Ingen användare hittades.");
                }
            }
        }
        static void UpdateUserPw(string currentUsername, string newPw) //har mest kopierat och kollat hur du gjorde dina första metoder,
                                                                       //och sen efter lite googling klurade jag ut hur man gjorde detta, tycker fortfarande det är svårt!
        {
            using (var context = new UserContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == currentUsername);
                if (user != null)
                {
                    user.Password = newPw;
                    context.SaveChanges();
                    Console.WriteLine("Lösenordet uppdaterades");
                }
                else
                {
                    Console.WriteLine("Ingen användare hittades.");
                }
            }
        }

        //Book methods:

        static void LoanedBookList()
        {
            using (var context = new UserContext())
            {
                foreach (var book in context.Books)
                {
                    if (book.IsBorrowed == true)
                    {
                        Console.WriteLine($"Lista över lånade böcker:{book.Title}");
                    }
                }                        
            }
        }
        static void AddBook()
        {
            Console.WriteLine("Skriv titeln på boken du vill lägga till: ");
            string newTitle = Console.ReadLine();
   
            Console.WriteLine("Skriv författarens namn: ");
            string newAuthor = Console.ReadLine();
            Console.WriteLine($"Boken med titeln: {newTitle} och författaren: {newAuthor}, lades till.");
            using (var context = new UserContext())
            {
                var book = new Book
                {
                    Title = newTitle,
                    Author = newAuthor,
                    IsBorrowed = false
                };

                context.Books.Add(book);
                context.SaveChanges();
            }
        }

        static void UpdateBook(string title)
        {
            using (var context = new UserContext())
            {
                var book = context.Books.FirstOrDefault(b => b.Title == title);
                if (book != null)
                {
                    Console.WriteLine("Boken hittades, vill du ändra titel? (ja/nej)");
                    string confirm = Console.ReadLine().ToLower();
                    if(confirm == "ja")
                    {
                        Console.WriteLine("Ange den nya titeln: ");
                        string newTitle = Console.ReadLine();
                        book.Title = newTitle;
                        Console.WriteLine("Ange den nya författaren: ");
                        string newAuthor = Console.ReadLine();
                        book.Author = newAuthor;
                        context.SaveChanges();
                        Console.WriteLine($"Boken uppdaterades med titel:{newTitle}, och författaren uppdaterades till:{newAuthor}");
                    }
                    else
                    {
                        Console.WriteLine("Bekräftelsen avbröts, inga ändringar gjordes.");
                    }
                    
                }
                else
                {
                    Console.WriteLine("Ingen bok hittades.");
                }
            }
        }


        static void BorrowBook()
        {
            using (var context = new UserContext())
            {
                Console.WriteLine("Skriv titeln på den bok du vill låna: ");
                string title = Console.ReadLine();
                var book = context.Books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase)); //detta hämtar boken från databasen
                if (book != null && !book.IsBorrowed)
                {
                    Console.WriteLine("Boken hittades, vill du låna boken(ja/nej)");
                    string confirm = Console.ReadLine().ToLower();
                    if(confirm == "ja")
                    {
                        Console.WriteLine($"Du lånar nu boken med titeln: {title}");
                        book.IsBorrowed = true;
                        context.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("Bekräftelsen avbröts, du lånar inte boken.");
                    }
                }
                else if(book != null && book.IsBorrowed)
                {
                    Console.WriteLine("Denna bok lånas av någon annan.");
                }
                else
                {
                    Console.WriteLine("Ingen bok med den titeln hittades.");
                }
                
            }
        }

        static void ReturnBook(string title)
        {
            using (var context = new UserContext())
            {
                var book = context.Books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase)); //detta hämtar boken från databasen
                if (book != null && book.IsBorrowed)
                {
                    Console.WriteLine($"Vill du lämna tillbaka boken med titeln:{title}, (ja/nej)");
                    string answer = Console.ReadLine().ToLower();
                    if(answer == "ja")
                    {
                        book.IsBorrowed = false;
                        context.SaveChanges();
                        Console.WriteLine($"Boken med titeln: {title}, lämnades tillbaka");
                    }
                    else
                    {
                        Console.WriteLine("Bekräftelsen avbröts, inga ändringar gjordes.");
                    }
                }
                else
                {
                    Console.WriteLine($"Boken med titeln:{title}, hittades ej bland lånade böcker.");
                }
            }
        }

        static void BookList()
        {
            using (var context = new UserContext())
            {
                foreach(var book in context.Books)
                {
                    Console.WriteLine($"Böcker i databasen med title:{book.Title}, författare: {book.Author}");
                }
            }
        }
        static void SearchBookTitle(string title)  //saknar kunskap för att söka efter författare med böcker
        {
            using (var context = new UserContext())
            {
                var book = context.Books.FirstOrDefault(t => t.Title == title);
                if(book != null)
                {
                    Console.WriteLine($"Boken med titeln: {book.Title}, hittades.");
                }
                else
                {
                    Console.WriteLine("Boken hittades inte.");
                }
            }
        }


        static void RemoveBook()
        {
            Console.WriteLine("Skriv titeln på den boken du vill ta bort: ");
            string title = Console.ReadLine().ToLower();

            using (var context = new UserContext())
            {
                var book = context.Books.FirstOrDefault(u => u.Title == title);
                if(book != null)
                {
                    Console.WriteLine($"Boken med titeln: {title}, hittades vill du ta bort den(ja/nej)");
                    string confirm = Console.ReadLine().ToLower();
                    if(confirm == "ja")
                    {
                        Console.WriteLine($"Boken {title}, togs bort.");
                        context.Books.Remove(book);
                        context.SaveChanges();
                        
                    }
                    else
                    {
                        Console.WriteLine("Bekräftelsen avbröts, boken togs inte bort.");
                        
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Boken med titeln: {title}, hittades inte i databasen.");
                    
                }
                
            }
        }
        //static void PausProgramMessage()
        //{
        //    Console.WriteLine("Klicka på enter för att fortsätta: ");
        //    Console.ReadKey();
        //}


        static void HeadMenu()
        {
            Console.WriteLine("Välj ett alternativ (1-4)");
            Console.WriteLine("1. Registrering");
            Console.WriteLine("2. Logga in");
            Console.WriteLine("3. Logga in som admin");
            Console.WriteLine("4. Avsluta");
            Console.WriteLine("------------------");
        }

        static void AdminMenu()
        {
            Console.WriteLine("--Admin Meny--");
            Console.WriteLine("1. Lägg till användare");
            Console.WriteLine("2. Ta bort användare");
            Console.WriteLine("3. Lägg till bok"); 
            Console.WriteLine("4. Ta bort bok"); 
            Console.WriteLine("5. Uppdatera / ändra lösenord / användarnamn");
            Console.WriteLine("6. Uppdatera bok");
            Console.WriteLine("7. Lista på böcker");
            Console.WriteLine("8. Logga ut");
            Console.WriteLine("------------------");
        }

        static void LoggedinMenu()
        {
            Console.WriteLine("1. Låna bok"); 
            Console.WriteLine("2. Lämna tillbaka bok");  
            Console.WriteLine("3. Lista på böcker");
            Console.WriteLine("4. Sök bok efter titel");
            Console.WriteLine("5. Lista på lånade böcker");
            Console.WriteLine("6. Logga ut");
            Console.WriteLine("------------------");
        }

    }
}
