using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;


namespace Lab5.Controllers
{
    public class BookController : Controller
       
    {
        public IActionResult Index()
        {
            IList<Models.Book> bookList = new List<Models.Book>(); //list of book obj
                                                                         //load people from people.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            
            if (System.IO.File.Exists(path))
                {
                //only load if the file exists
                doc.Load(path); //exists, so load
                XmlNodeList book = doc.GetElementsByTagName("book"); //load book elements
                
                foreach (XmlElement b in book)
                {
                    //load book data from each <book> and bind to a model
                    Models.Book books = new Models.Book();
                    books.ID = b.GetElementsByTagName("id")[0].InnerText;
                    books.bookTitle = b.GetElementsByTagName("title")[0].InnerText;

                    XmlElement author = (XmlElement) b.SelectSingleNode("author");
                       

                    
                    //TROUBLE WITH OBTAINING ATTRIBUTE
                    books.authorTitle = author.GetAttribute("title");


                    books.firstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    // books.middleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    XmlNodeList middle__name = b.GetElementsByTagName("middlename");
                    
                    if (middle__name != null && middle__name.Count > 0)
                    {
                        books.middleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    }
                    else
                    {
                        books.middleName = " ";
                    }
                    books.lastName = b.GetElementsByTagName("lastname")[0].InnerText;
                    bookList.Add(books);
                }
            }
            return View(bookList); //pass the bookList to view
        }

        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();
            return View(book);
        }

        [HttpPost]
        public IActionResult Create(Models.Book b)
        {
            //load book.xml
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();

            if (System.IO.File.Exists(path))
            {
                //if file exists, just load it and create new book
                doc.Load(path);

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //get the root element
                doc.DocumentElement.AppendChild(book);

            }
            else
            {
                //file doesn't exist, so create and create new book
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "utf-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("book");

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);
            }
            doc.Save(Request.PathBase + "App_Data/books.xml");

            return RedirectToAction("Index");
        }

        private XmlElement _CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
            //BUILDING THE XML NODES
            XmlElement book = doc.CreateElement("book");
            
            
            //ID
            XmlNode id = doc.CreateElement("id");
            id.InnerText = newBook.ID;
            
            //Title of Book
            XmlNode title = doc.CreateElement("title");
            title.InnerText = newBook.bookTitle;

            
            //Author
            XmlNode author = doc.CreateElement("author");
            //ATTRIBUTES
            XmlAttribute authorTitle = doc.CreateAttribute("title");
            authorTitle.Value = newBook.authorTitle;
            author.Attributes.Append(authorTitle);

            XmlNode firstName = doc.CreateElement("firstname");
            firstName.InnerText = newBook.firstName;

            XmlNode middleName = doc.CreateElement("middlename");
            middleName.InnerText = newBook.middleName;

            XmlNode lastName = doc.CreateElement("lastname");
            lastName.InnerText = newBook.lastName;


            //APPENDING
            author.AppendChild(firstName);
            author.AppendChild(middleName);
            author.AppendChild(lastName);


            book.AppendChild(title);
            book.AppendChild(author);
            book.AppendChild(id);


            return book;
        }
    }
}
