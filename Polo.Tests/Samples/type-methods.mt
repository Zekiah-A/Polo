##
--- type-methods.mt ---
This file demonstrates the ability to define type-encapsulated
methods, and callable methods within type instances with access
to other instance locals.
##
@import("std")

type Book
    def title:Str
    def author:Str

type BookStore
    def books:Book[]

    def clearBooks:fn()* = fn()
        this.books = Book[]{}

    def printBooks:fn()* = fn()
        debug("This book store contains:")
        for let i:i32 = 0; i < this.books.length; i++
            let book:Book = this.books[i]
            debug(book.title, "by", book.author)

    fn createDefault():BookStore
        let lordOfTheRings:Book = Book{ title = "The lord of the rings", author = "J.R.R. Tolkien" }
        let kiteRunner:Book = Book{ title = "The kite runner", author = "Khaled Hosseini" }
        let chaosWalking:Book = Book{ title =  "Chaos walking", author = "Patrick Ness" }
        return BookStore{ books = Book[] { lordOfTheRings, kiteRunner, chaosWalking } }

let store:BookStore = BookStore.createDefault()
store.printBooks()
store.clearBooks()
store.printBooks()
