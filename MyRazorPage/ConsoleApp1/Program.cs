// See https://aka.ms/new-console-template for more information


string a = "123";

string enscript = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(a));

Console.WriteLine("encrypt");
Console.WriteLine(enscript);

Console.WriteLine("decrypt");

