# Info object library

lightweight and powerful binary serialization library made with c#.

This library implements Reflection to convert public properties of model classes to and from binary data.
The Info class was originally meant to be used as a containing medium to hold and transfer model Information on disk or over network using TCP protocol.
The classes that inherit from Info class can be converted to byte array and transferred over network or saved to disk as a binary file.

## supported types:
1. all primitive types are supported.
2. supports serialization of nested Info objects.
3. supports serialization of nested enums and structs.
4. supports serialization of lists and Dictionaries of all primitive types as well as of structs, enums and info objects.
5. supports serialization of object type that can dynamically holds all other supported types. this object can then be casted to the original type after deserialization.

## Security features:
supports encryption of the byte array and the saved binary files with password using the AES256 algorithm

## How to use:
In order to use the Info class we have to include the InfoObject namespace

```c#
using InfoObject;
```

let's declare a simple user model:
```c#
public class User : Info
{
  public int Id {get; set;}
  public string Name {get; set;}  
  public string Email {get; set;}
  public DateTime Birthday {get; set;}  
}
```

let's instantiate our user

```c#
var user = new User{
  Id=1,
  Name= "Tom Adams",
  Email="toom.adams@gmx.com",
  Birthday = new DateTime(1980, 7, 21)
};
```

## Serialization

### Simple Serialization

```c#
byte[] serialized = user.Serialize();
```

### password encrypted Serialization

Through utilisation of AES256 algorithm a password with maximum length of 32 charachter can be used.
```c#
byte[] serializedEncrypted = user.Serialize("123ta");
```

the resulting byte array can now be simply ransmitted over network using TCP protocol.


## Deserialization

The received byte array can be easily deserialized on the other network side, given that the user class is also declared there.
When using deserialization we must know beforehand the type of the original InfoObject and pass it as a generic argument. This generic argument must always be of type Info(i.e. a class that is inheriting the abstract Info class). If we pass a wrong type, null will be returned.

### Simple deserialization

```c#
var userBack = Info.Deserialize<user>(receivedByteArray);
if (userback != null)
  Console.WriteLine( userBack.Name );
else
    Console.WriteLine( "invalid type!" );
```

### Deserialization of password encrypted binary array

Here we need to enter the correct password, otherwise null will be returned.

```c#
var userback = Info.Deserialize<User>(receivedByteArray, "123ta");
if (userback != null)
  Console.WriteLine( userBack.Name );
else
    Console.WriteLine( "wrong password!" );

```

## Exceptions interception

Depending solely on null checking will lead eventually to ambiguity. Sometimes we need to know exactly what's going on and what was the cause of the returned null. To achieve this we can get Exception out of the deserialization method with the type of the error.

```c#
var userback = Info.Deserialize<User>(receivedByteArray, out Exception ex ,"123ta");
if (ex != null)
  Console.WriteLine( "Error : " + ex );
else
    Console.WriteLine( userBack.Name );
```


## Saving the model to binary file

The resulting byte array can also be saved to file.

Example

```c#
//save our model without password 
user.Save("D:\BinModels\user.info");

// save with password
user.Save("D:\BinModels\userEncrypted.info", "strangeWorld313");

// loading the model back from non-encrypetd file
var userback = Info.Deserialize<User>("D:\BinModels\user.info");

// loading the model back from encrypetd file
var userbackFromEncrypted = Info.Deserialize<User>("D:\BinModels\userEncrypted.info", "strangeWorld313");

```

We can also intercept exceptions when loading model from file

```c#
var userback = Info.Deserialize<User>("D:\BinModels\userEncrypted.info", out Exception ex ,"strangeWorld313");
if (ex != null)
  Console.WriteLine( "Error : " + ex );
else
    Console.WriteLine( userback.Name );
```

## iterable InfoObject

The Info class implements also the IEnumerable interface and can thus be iterated over using foreach loop.


## asigning element type dynamically

By declaring an object type in properties of infoObject we can asign any type of element to this property. This feature comes particularly handy when we want to receive element of a type that is unknown at combile time.

Example of possible implementation of this feature:

let's assume we want to get some object from server, for example this Book class

```c#
 public class Book : Info
 {
	public int  Id {get; set;}
 	public string Name {get; set;}
 	public string Author {get; set;}	
 }
```

We use a response class to send the requested object back to client
```c#
 public class Response : Info
 {
	public int  Id {get; set;}
 	public object Data {get; set;}// this property can now hold any other type and it can be serializes and deserialized successfuly
 }
```


requesting some info type in client. Here we ask the server to send us an InfoObject of type Book.

```c#
Book book = Client.Request<Book>( args); //this sample method is not implemented in this library
```


The server can process the request that came from client and send back a response that holds the required type( here Book )

```c#
Response response = new Response{
	Id = 1,
	Data = new Book{ // the type object will hold an info object of type Book(it can also holds any other type, but here the client requested a book)
		Id = 12,
		Name = "Anna Karenina",
		Author = "Lew Tolstoi"
	}
}
```


example of sending the response back to the client.

```c#
Server.SendResponse( clientId, response ); //this sample method is not implemented in this library
```

in client we receive the response and cast the object type to Book, since we told the server to send us a Book

```c#
var bookFromServer = (Book)response.Data;
Console.WrieLine(bookFromServer.Name);
```



