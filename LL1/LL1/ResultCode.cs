using System;
namespace Executable
{
static class Execute
{
static int input_index = 0;
static string[] input_string;
public static void Main(string[] args)
{
while(true)
{
input_index = 0;
Console.WriteLine("Introduceti propozitia pentru verificare sau exit pentru inchidere!");
string sentence = Console.ReadLine().Trim();
if( sentence == "exit") break;sentence = sentence + " $ $";
input_string = sentence.Split(' ');
try
{
E();
if(input_string[input_index] == "$")
Console.WriteLine("Propozitie Corecta!");
else
throw new Exception();
}
catch(Exception e)
{
Console.WriteLine("Propozitie Incorecta");
}
}
return;
}
static void E()
{
if ( input_string[input_index] == "(" || input_string[input_index] == "a" )
{
R2();
}
else
{
throw new Exception();
}
}
static void T()
{
if ( input_string[input_index] == "(" || input_string[input_index] == "a" )
{
R6();
}
else
{
throw new Exception();
}
}
static void F()
{
if ( input_string[input_index] == "(" )
{
R1();
}
else if ( input_string[input_index] == "a" )
{
R10();
}
else
{
throw new Exception();
}
}
static void L()
{
if ( input_string[input_index] == "(" || input_string[input_index] == "a" )
{
R13();
}
else
{
throw new Exception();
}
}
static void E1()
{
if ( input_string[input_index] == "+" )
{
R3();
}
else if ( input_string[input_index] == "-" )
{
R4();
}
else if ( input_string[input_index] == ")" || input_string[input_index] == "," || input_string[input_index] == "$" )
{
R5();
}
else
{
throw new Exception();
}
}
static void T1()
{
if ( input_string[input_index] == "+" || input_string[input_index] == "-" || input_string[input_index] == ")" || input_string[input_index] == "," || input_string[input_index] == "$" )
{
R9();
}
else if ( input_string[input_index] == "*" )
{
R7();
}
else if ( input_string[input_index] == "/" )
{
R8();
}
else
{
throw new Exception();
}
}
static void F2()
{
if ( input_string[input_index] == "+" || input_string[input_index] == "-" || input_string[input_index] == ")" || input_string[input_index] == "," || input_string[input_index] == "*" || input_string[input_index] == "/" || input_string[input_index] == "$" )
{
R12();
}
else if ( input_string[input_index] == "(" )
{
R11();
}
else
{
throw new Exception();
}
}
static void L2()
{
if ( input_string[input_index] == ")" )
{
R15();
}
else if ( input_string[input_index] == "," )
{
R14();
}
else
{
throw new Exception();
}
}
static void R2()
{
T();
}

static void R6()
{
F();
}

static void R1()
{
if ( input_string[input_index] == "(" )
{
input_index++;
}
else
{
throw new Exception();
}
E();
}

static void R10()
{
if ( input_string[input_index] == "a" )
{
input_index++;
}
else
{
throw new Exception();
}
}

static void R13()
{
E();
}

static void R3()
{
if ( input_string[input_index] == "+" )
{
input_index++;
}
else
{
throw new Exception();
}
T();
}

static void R4()
{
if ( input_string[input_index] == "-" )
{
input_index++;
}
else
{
throw new Exception();
}
T();
}

static void R5()
{
}

static void R9()
{
}

static void R7()
{
if ( input_string[input_index] == "*" )
{
input_index++;
}
else
{
throw new Exception();
}
F();
}

static void R8()
{
if ( input_string[input_index] == "/" )
{
input_index++;
}
else
{
throw new Exception();
}
F();
}

static void R12()
{
}

static void R11()
{
if ( input_string[input_index] == "(" )
{
input_index++;
}
else
{
throw new Exception();
}
L();
}

static void R15()
{
}

static void R14()
{
if ( input_string[input_index] == "," )
{
input_index++;
}
else
{
throw new Exception();
}
}

}
}