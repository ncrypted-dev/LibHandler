# LibHandler

This is a simple **WIP** C# LibGen Library that uses Web scraping and the "official" API to search and download books. <br>
Since this Library is still in early development I cannot guarantee a bug-free experience. <br>
Please note that if the LibGen API or Website changes at any point, this Library might break. <br>
The maintainers of the different LibGen Mirrors might limit your traffic or block you altogether if you misuse this Library. <br>
**USE AT YOUR OWN RISK!** <br>

## Supported Features

| Feature | Description | Status |
| --- | --- | --- |
| Books JSON | Get JSON Book Data of the provided IDs.  | 🟢 Works
| Multiple Mirrors | Use the optimal Mirror for searching and downloading. | 🟢 Works
| Search (Title, Author, ...) | Search for books with certain filters | 🟡 Works, but hasn't been tested with all filters yet
| Downloads | Download books | 🟡 Works, but only with the MD5 Hash of the book

## Usage / Example

### Get IDs of the first 25 Books that have ".NET" in their title

```cs
string request = LibraryHandler.GetRequestFormat(SearchField.Title, ".NET", 25);
List<string> ids = LibraryHandler.GetIDs(request);
```

### Get JSON Data of books with the provided IDs

```cs
string request = LibraryHandler.GetRequestFormat(SearchField.Title, ".NET", 25);
List<string> ids = LibraryHandler.GetIDs(request);
string json = LibraryHandler.GetJSONData(ids);
// List<Book> books = JsonConvert.DeserializeObject<List<Book>>(json);
```

### Combined Version (This is the recommended way)

```cs
string request = LibraryHandler.GetRequestFormat(SearchField.Title, ".NET", 25);
List<Book> books = LibraryHandler.GetBooks(request);
```

### Download Book
```cs
string download = LibraryHandler.GetDownloadLinkByMD5(books[0].md5);
// OR
string download = LibraryHandler.GetDownloadLink(books[0]);
```

### Additional Functions
```cs
// Gets the current Mirror (The Search and Download Mirror are seperate)
Mirror mirror = LibraryHandler.GetCurrentMirror(MirrorType.SearchMirror);
// Sets the current Mirror
Mirror mirror = new Mirror()
{
    Url = "libgen.is",
    FullUrl = "https://libgen.is",
    MirrorType = MirrorType.SearchMirror,
};

Mirror downloadMirror = new Mirror()
{
    Url = "library.lol",
    FullUrl = "http://library.lol",
    MirrorType = MirrorType.DownloadMirror,
    Path = "/main/"
};

LibraryHandler.SetCurrentMirror(mirror);
LibraryHandler.SetCurrentMirror(downloadMirror);
```
> Mirrors can also be added in the mirrors.json file

## License
Copyright (C) 2024 ncrypted-dev

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.
