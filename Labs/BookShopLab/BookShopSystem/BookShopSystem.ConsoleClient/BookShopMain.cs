﻿namespace BookShopSystem.ConsoleClient
{
    using System;
    using System.Globalization;
    using System.Data.Entity;
    using System.Linq;
    using Data;
    using BookShopSystem.Models;

    public class BookShopMain
    {
        public static void Main()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InstalledUICulture;
            BookShopContext context = new BookShopContext();

            //FistDataBaseInitialisation(context);

            GetAllBooksAfter2000(context);
            GetAllAuthoursWithBookBefore1990(context);
            GetAllAuthoursOrderderByNumberOfTheirBooksDescending(context);
            GetAllBooksFromAuthorGeorgePowell(context);
            GetCategoriesAnd3BooksOfEach(context);
        }

        private static void GetCategoriesAnd3BooksOfEach(BookShopContext context)
        {
            var wantedCategories = context.Categories
                .OrderBy(category => category.Books.Count)
                .Select(category => new
                {
                    category.Name,
                    BooksCount = category.Books.Count,
                    Books = category.Books
                    .OrderByDescending(book => book.ReleaseDate)
                    .ThenBy(book => book.Title)
                    .Take(3)
                    .Select(book => new
                    {
                        book.Title,
                        book.ReleaseDate
                    })
                });

            foreach (var category in wantedCategories)
            {
                Console.WriteLine("--{0}: {1} books", category.Name, category.BooksCount);

                foreach (var book in category.Books)
                {
                    Console.WriteLine("{0} ({1})", book.Title, book.ReleaseDate.Value.Year);
                }
            }
        }

        private static void GetAllBooksFromAuthorGeorgePowell(BookShopContext context)
        {
            var wantedBooks = context.Books
                .Where(book => book.Author.FirstName == "George" && book.Author.LastName == "Powell")
                .OrderByDescending(book => book.ReleaseDate)
                .ThenBy(book => book.Title);

            foreach (Book book in wantedBooks)
            {
                Console.WriteLine("{0} {1} {2}", book.Title, book.ReleaseDate, book.Copies);
            }
        }

        private static void GetAllAuthoursOrderderByNumberOfTheirBooksDescending(BookShopContext context)
        {
            var wantedAuthours = context.Authors
                .OrderByDescending(author => author.Books.Count);

            foreach (Author author in wantedAuthours)
            {
                Console.WriteLine($"{author.FirstName} {author.LastName} - {author.Books.Count}");
            }
        }

        private static void GetAllAuthoursWithBookBefore1990(BookShopContext context)
        {
            var wantedAuthors = context.Authors
                .Where(author => author.Books
                .Count(book => book.ReleaseDate.HasValue && book.ReleaseDate.Value.Year < 1990) > 0);

            foreach (Author author in wantedAuthors)
            {
                Console.WriteLine("{0} {1}", author.FirstName, author.LastName);
            }
        }

        private static void GetAllBooksAfter2000(BookShopContext context)
        {
            var wantedBooks = context.Books
                .Where(book => book.ReleaseDate.HasValue && book.ReleaseDate.Value.Year > 2000);

            foreach (Book wantedBook in wantedBooks)
            {
                Console.WriteLine(wantedBook.Title);
            }
        }

        private static void FistDataBaseInitialisation(BookShopContext context)
        {
            var migrationStrategy = new DropCreateDatabaseAlways<BookShopContext>();
            Database.SetInitializer<BookShopContext>(migrationStrategy);

            context.Database.Initialize(true);

            var bookCount = context.Books.Count();
        }
    }
}
