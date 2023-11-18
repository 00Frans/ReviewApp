﻿using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public List<BookViewModel> GetBooks()
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.DateAdded).ToList();
        return data;
    }

    public List<BookViewModel> TopBooks()
    {
        var data = _bookRepository.GetBooks().Select(x => new BookViewModel
        {
            BookId = x.BookId,
            BookImage = x.BookImage,
            Isbn = x.Isbn,
            Title = x.Title,
            Description = x.Description,
            Genre = x.Genre,
            Author = x.Author,
            TotalRating = x.TotalRating,
            CreatedBy = x.CreatedBy,
            DateAdded = x.DateAdded,
            UpdatedBy = x.UpdatedBy,
            UpdatedDate = x.UpdatedDate,
        }).OrderByDescending(x => x.TotalRating).ToList();
        return data;
    }

    public BookViewModel GetBook(int id)
    {
        var model = _bookRepository.GetBook(id);
        if (model != null)
        {
            BookViewModel book = new()
            {
                BookId = model.BookId,
                BookImage = model.BookImage,
                Isbn = model.Isbn,
                Title = model.Title,
                Description = model.Description,
                Genre = model.Genre,
                Author = model.Author,
                TotalRating = model.TotalRating,
                CreatedBy = model.CreatedBy,
                DateAdded = model.DateAdded,
                UpdatedBy = model.UpdatedBy,
            };
            return book;
        }
        return null;
    }

    public void AddBook(BookViewModel model, string name)
    {
        var url = "https://127.0.0.1:8080";
        var book = new Book();
        var bookName = Guid.NewGuid().ToString();
        var sharedImagesPath = PathManager.DirectoryPath.SharedImagesDirectory;
        book.Isbn = model.Isbn;
        book.BookImage = Path.Combine(url, bookName + ".png");
        book.Title = model.Title;
        book.Description = model.Description;
        book.Genre = model.Genre;
        book.Author = model.Author;
        book.TotalRating = 0;
        book.CreatedBy = name;
        book.DateAdded = DateTime.Now;
        book.UpdatedBy = name;
        book.UpdatedDate = DateTime.Now;
        var sharedImageFileName = Path.Combine(sharedImagesPath, bookName) + ".png";
        using(var fileStream = new FileStream(sharedImageFileName, FileMode.Create))
        {
            model.ImageFile.CopyTo(fileStream);
        }
        _bookRepository.AddBook(book);
    }

    //Validate Isbn
    public bool CheckIsbn(string isbn)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Isbn == isbn).Any();
        return isExist;
    }

    //Validate Title
    public bool CheckTitle(string title)
    {
        var isExist = _bookRepository.GetBooks().Where(x => x.Title == title).Any();
        return isExist;
    }

    public void UpdateBook(BookViewModel model, string name)
    {
        var url = "https://127.0.0.1:8080";
        Book book = _bookRepository.GetBook(model.BookId);
        if(book != null)
        {
            var bookName = Guid.NewGuid().ToString();
            var sharedImagesPath = PathManager.DirectoryPath.SharedImagesDirectory;
            book.Isbn = model.Isbn;
            book.Title = model.Title;
            book.Author = model.Author;
            book.Genre = model.Genre;
            book.Description = model.Description;
            book.UpdatedBy = name;
            book.UpdatedDate = DateTime.Now;
            if (model.ImageFile != null)
            {
                book.BookImage = Path.Combine(url, bookName + ".png");
                var sharedImageFileName = Path.Combine(sharedImagesPath, bookName) + ".png";
                using (var fileStream = new FileStream(sharedImageFileName, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);

                }
            }
            _bookRepository.EditBook(book);
        }
    }

    public void DeleteBook(BookViewModel model)
    {

        Book book = _bookRepository.GetBook(model.BookId);
        if (book != null)
        {
            _bookRepository.DeleteBook(book);
        }
    }
}