# Introduction #

This tutorial explain how to deploy and test RunSA server


# Clone project #

First of all clone the repository to obtain latest version of RunSA.
The clone will contain RunSA source core and RunSA Distro scripts

# Compile core server #

Open RunSA VS solution "RunSA.sln" and rebuild it.

# Start the server #

Ultima Online SA Classic must be installed on your computer.
Execute RunSA.exe in bin/Release/ and create admin account

# Configure clients #

## Ultima Online Stygian Abyss Classic ##

Use Razor to connect to the shard with these parameters:
  * IP : 127.0.0.1
  * Port : 2593
  * Encryption options : we don't care our server support encryption.

Type your admin login and password. Enjoy :)

## Ultima Online Stygian Abyss Enhanced client ##

The server do not permit to connect with first admin account yet.
You have to create an In Game Account and set admin rights with Classic client.

When account is created you can download [UOSALoader](http://code.google.com/p/runsa/downloads/detail?name=UoSALoader.exe&can=2) and path your UOSA Enhanced client with it. UOSALoader fix IP adress and port which are hardcoded in UOSA.exe.
  * IP : 127.0.0.1
  * Port : 2593

# Issues #

## Encryption not supported ##

Encryption not supported means that your version of UO is too recent.
In order to fix it you have to generate [Login keys](http://code.google.com/p/runsa/downloads/detail?name=UOKeyLogin%20Calculator.exe&can=2&q=) of your version of UO and add them to the file : bin/Release/Scripts/Encryption/Configuration.cs
Restart the server. Enjoy :)