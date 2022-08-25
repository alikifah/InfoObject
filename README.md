# Info object library

lightweight and powerful binary serialization library made with c#.

This library implements Reflection to convert public properties to and from binary data.
The Info class was meant to be used as a containing medium to hold and transfer Information on disk or over network.
The classes that inherit from Info class can be converted to byte array and transferred over network or saved to disk as a binary file.

## supported types:
1. all primitive types are supported.
2. supports serialization of nested Info objects.
3. supports serialization of nested enums and structs.
4. supports serialization of lists and Dictionaris.
5. supports lists of structs and info objects.

## Security features:
supports encryption of the byte array and the saved binary files with password using the AES256 algorithm

