
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace Project_LL1
{
    public partial class MainWindow : Window
    {
        public List<List<string>> _Grammar = new List<List<string>>();
        List<List<string>> _Alphabet = new List<List<string>>();
        List<List<string>> _Symbols = new List<List<string>>();
        string PathGrammar, ExecCode;
        string[,] _Matrix;
        public MainWindow()
        {

            InitializeComponent();

            
            ModifyGrammarB.Visibility       =
            CreateSymbolsB.Visibility       = 
            CreateMatrixB.Visibility        = 
            GenerateCodeB.Visibility        = Visibility.Hidden;               
        }

        private void ModifyGrammar_Click(object sender, RoutedEventArgs e)
        {
            //modific regulile ce contin recursivitate stanga
            LeftRecursivity();
            
            // verific daca exista o situatie unde pentru acelasi Net doua reguli de productie incep cu acelasi Ter
            RightTerminal();

            verify.Text = "";
            Print(_Alphabet, "Alphabet\n");
            Print(_Grammar, "\nGrammar\n");
            CreateSymbolsB.Visibility = Visibility.Visible;

        }

        private void CreateSymbols_CLick(object sender, RoutedEventArgs e)
        {
            _Symbols = CreateDirectorySymbols();
            verify1.Text = "";
            if (Check_LL1() == true)
            {
                Print1(_Symbols, "Grammar is LL1!\nSymbols:\n");
                CreateMatrixB.Visibility = Visibility.Visible;
            }
            else
                verify1.AppendText("Grammar in not LL1 !!!");
            
        }

        private void CreateMatrix_Click(object sender, RoutedEventArgs e)
        {
            verify2.Text = "";
            string[,] _matrix = new string[_Alphabet[1].Count() + _Alphabet[2].Count() + 2, _Alphabet[2].Count() + 2]; // ma folosesc de indicii din afabet[1] si _Alphabet[2]
            // _Alphabet[1] - neterminale
            // _Alphabet[2] - terminale
            // POP - POP
            // Error - !
            _matrix = CreateMatrix();
            _Matrix = _matrix;
            GenerateCodeB.Visibility = Visibility.Visible;
        }

        private void GenerateCode_Click(object sender, RoutedEventArgs e)
        {
            CreateSymbolsB.Visibility       = 
            CreateMatrixB.Visibility        = 
            ModifyGrammarB.Visibility       = 
            GenerateCodeB.Visibility        = Visibility.Hidden;


          
            CreateCode();



        }

        public void CreateCode()
        {
            ExecCode =  "using System;\n" +
                        "namespace Executable\n" + 
                        "{\n" + 
                        "static class Execute\n" + 
                        "{\n" +
                        "static int input_index = 0;\n" +
                        "static string[] input_string;\n" +                        
                        "public static void Main(string[] args)\n" +
                        "{\n" + 
                        "while(true)\n" +
                        "{\n" +
                        "input_index = 0;\n" +
                        "Console.WriteLine(\"Introduceti propozitia pentru verificare sau exit pentru inchidere!\");\n" +
                        "string sentence = Console.ReadLine().Trim();\n" +
                        "if( sentence == \"exit\") break;" +
                        "sentence = sentence + \" $ $\";\n" +
                        "input_string = sentence.Split(' ');\n" +
                        "try\n" +
                        "{\n" +
                        $"{_Alphabet[0][0]}();\n" +
                        $"if(input_string[input_index] == \"$\")\n" +
                        "Console.WriteLine(\"Propozitie Corecta!\");\n" +
                        "else\n" +
                        "throw new Exception();\n" +
                        "}\n" +
                        "catch(Exception e)\n" +
                        "{\n" +
                        "Console.WriteLine(\"Propozitie Incorecta\");\n" +
                        "}\n" +
                        "}\n" +
                        "return;\n" +
                        "}\n";

            List<string> rules = new List<string>();
            for (int line = 1; line < _Alphabet[1].Count() + 1; line++)
            {
                ExecCode += $"static void {_Matrix[line, 0]}()\n" + 
                            "{\n";
                List<string> Terminals = new List<string>();
                 
                int x = 0;
                while (true)
                {
                    Terminals.Clear();
                    string rule = "!";
                    for (int column = 1; column < _Alphabet[2].Count() + 2; column++)
                    {
                        if (_Matrix[line, column] != "!" && (rule == _Matrix[line, column] || rule == "!"))
                        {
                            rule = _Matrix[line, column];
                            rules.Add(rule);
                            _Matrix[line, column] = "!";
                            Terminals.Add(_Matrix[0, column]);
                        }
                    }
                    
                    if (Terminals.Count == 0)
                        break;

                    if (x == 1)
                        ExecCode += "else ";
                    x = 1;
                    ExecCode += $"if ( input_string[input_index] == \"{Terminals.First()}\"";
                    Terminals.RemoveAt(0);
                    while (Terminals.Count() != 0)
                    {
                        ExecCode += $" || input_string[input_index] == \"{Terminals.First()}\"";
                        Terminals.RemoveAt(0);
                    }
                    ExecCode += " )\n" +
                                "{\n" +
                                $"{rule}();\n" +
                                "}\n";
                }
                ExecCode += "else\n" +
                            "{\n" +
                            "throw new Exception();\n" +
                            "}\n" +
                            "}\n";
            }
            List<string> rulas = new List<string>(rules);
            rules.Clear();
            rules.AddRange(rulas.Distinct());
            foreach (string rule in rules)
            {
                ExecCode += $"static void {rule}()\n" +
                            "{\n";
                int numVal = int.Parse(new string((rule.ToArray()).Where(char.IsDigit).ToArray()));
                int index_g = 2;
                string parameter = _Grammar[numVal - 1][index_g];
                while (parameter != _Grammar[numVal - 1].Last())
                {
                    if (_IsNonTer(parameter) == true)
                    {
                        ExecCode += $"{parameter}();\n";
                    }
                    else
                    {
                        ExecCode += $"if ( input_string[input_index] == \"{parameter}\" )\n" +
                                    "{\n" +
                                    "input_index++;\n" +
                                    "}\n";
                        if (parameter != _Grammar[numVal - 1].Last())
                            if (_IsNonTer(_Grammar[numVal - 1][index_g + 1]) == false)
                            {
                                ExecCode += "else ";
                            }
                            else
                            {
                                ExecCode += "else\n" +
                                            "{\n" +
                                            "throw new Exception();\n" +
                                            "}\n";
                            }
                        else
                        {
                            if (_IsNonTer(_Grammar[numVal - 1][index_g]) == false)
                            {
                                ExecCode += "else\n" +
                                            "{\n" +
                                            "throw new Exception();\n" +
                                            "}\n";
                            }
                        }
                    }
                    index_g++;
                    parameter = _Grammar[numVal - 1][index_g];
                }
                ExecCode += "}\n\n";
            }
            ExecCode += "}\n}";


            Begin();
        }

        

        public bool _IsNonTer(string paramater)
        {
            for (int i = 0; i < _Alphabet[1].Count(); i++)
            {
                if (_Alphabet[1][i].Equals(paramater))
                    return true;
            }
            return false;
        }

      /*  public int VerifySentence(List<string> atoms, List<string> stack, int X)
        {
            if (X == 1000)
                return X;

            if (atoms[0] == "$")
            {
                if (stack.Count() == 0)
                {
                    W2_Response.Text = "Propozitia este Corecta ! :) ";
                    X = 1000;
                    return X;
                }
            }

            if (_IsNonTer(stack[0]) == true)
            {
                for (int i = 1; i <= _Alphabet[1].Count() && X != 1000; i++)
                {
                    if (_Matrix[i, 0] == stack[0])
                        for (int j = 1; j <= _Alphabet[2].Count() + 1 && X != 1000; j++)
                        {
                            if (_Matrix[0, j] == atoms[0] || _Matrix[0,j]== "$")
                                if (_Matrix[i, j] == "!")
                                {
                                    if (_Matrix[i, _Alphabet[2].Count() + 1] != "!")
                                    {
                                        int numVal = Int32.Parse(new String(_Matrix[i, _Alphabet[2].Count() + 1].Where(Char.IsDigit).ToArray()));
                                        List<string> aux = new List<string>();
                                        aux.AddRange(_Grammar[numVal - 1]); // adaug regula de productie in aux
                                        aux.RemoveAt(0);                    // elimin net si ->
                                        aux.RemoveAt(0);
                                        stack.RemoveAt(0);                  // elimin net pe care il inlocuiesc
                                        aux.AddRange(stack);                // adau la aux restul elementelor din stack
                                        stack = aux;                        // finalizez stack
                                        if (stack[0] == "Epsilon")
                                            stack.RemoveAt(0);
                                        if (stack[stack.Count() - 1] == "")
                                            stack.RemoveAt(stack.Count() - 1);
                                        X = VerifySentence(atoms, stack, X);
                                    }
                                    else
                                    {
                                        W2_Response.Text = "Propozitia NU este corecta!";
                                        X = 1000;
                                        return X;
                                    }
                                }
                                else
                                {
                                    int numVal = Int32.Parse(new String(_Matrix[i, j].Where(Char.IsDigit).ToArray()));
                                    List<string> aux = new List<string>();
                                    aux.AddRange(_Grammar[numVal - 1]); // adaug regula de productie in aux
                                    aux.RemoveAt(0);                    // elimin net si ->
                                    aux.RemoveAt(0);
                                    stack.RemoveAt(0);                  // elimin net pe care il inlocuiesc
                                    aux.AddRange(stack);                // adau la aux restul elementelor din stack
                                    stack = aux;                        // finalizez stack
                                    if (stack[0] == "Epsilon")
                                        stack.RemoveAt(0);
                                    if(stack.Count() - 1 >= 0)
                                        if (stack[stack.Count()-1] == "")
                                            stack.RemoveAt(stack.Count()-1);
                                    X = VerifySentence(atoms, stack, X);


                                    //trebuie sa fac expandare la regula
                                }

                        }
                }
            }
            else
            {
                if (stack[0] == atoms[0])
                {
                    stack.RemoveAt(0);
                    atoms.RemoveAt(0);
                    if (atoms.Count() == 0)
                        atoms.Add("$");
                    X = VerifySentence(atoms, stack, X);
                }
                else
                {
                    W2_Response.Text = "Propozitia NU este corecta!";
                    X = 1000;
                    return X;
                }
            }

            return X;
        } */

        public string[,] CreateMatrix()
        {
            int line_size = _Alphabet[1].Count() + _Alphabet[2].Count() + 2;
            int column_size = _Alphabet[2].Count() + 2;
            string[,] _matrix = new string[line_size, column_size];
            for (int i = 1; i < column_size - 1; i++)
            {
                _matrix[0, i] = _Alphabet[2][i - 1];
                _matrix[_Alphabet[1].Count() + i, 0] = _Alphabet[2][i - 1];
            }
            for (int i = 1; i < _Alphabet[1].Count() + 1; i++)
            {
                _matrix[i, 0] = _Alphabet[1][i - 1];
            }
            _matrix[line_size - 1, 0] = "$";
            _matrix[0, column_size - 1] = "$";

            for (int i = 1; i < line_size; i++)
                for (int j = 1; j < column_size; j++)
                    _matrix[i, j] = "0";

            for (int nr_prod_rule = 0; nr_prod_rule < _Grammar.Count(); nr_prod_rule++)
                for (int i = 1; i < line_size; i++)
                    if (_Grammar[nr_prod_rule][0].Equals(_matrix[i, 0]))
                    {
                        for (int j = 0; j < column_size; j++)
                            for (int k = 0; k < _Symbols[nr_prod_rule].Count(); k++)
                                if (_Symbols[nr_prod_rule][k].Equals(_matrix[0, j]))
                                    _matrix[i, j] = $"R{nr_prod_rule + 1}";
                    }
            for (int i = 1; i < line_size; i++)
                for (int j = 1; j < column_size; j++)
                    if (j + _Alphabet[1].Count() == i)
                        if (_matrix[i, 0].Equals("$"))
                            _matrix[i, j] = "ACCEPT";
                        else
                            _matrix[i, j] = "POP";
                    else if (_matrix[i, j].Equals("0"))
                        _matrix[i, j] = "!";
            for (int i = 0; i < line_size; i++)
            {
                for (int j = 0; j < column_size; j++)
                    verify2.AppendText(_matrix[i, j] + "\t");
                verify2.AppendText("\n");
            }
            return _matrix;
        }

        public List<string> First(string parameter, string origine, List<List<string>> _FirstNet, List<string> _Passed)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < _Grammar.Count(); i++)
            {
                if (_Grammar[i][0].Equals(parameter))
                {
                    if (_Grammar[i][2].Equals("Epsilon"))
                    {
                        vs.AddRange(Follow(origine, _FirstNet, _Passed));
                    }
                    else if (_IsNonTer(_Grammar[i][2]).Equals(true))
                    {
                        vs.AddRange(First(_Grammar[i][2], _Grammar[i][0], _FirstNet, _Passed)); // TODO ??? 
                    }
                    else
                        vs.Add(_Grammar[i][2]);
                }
            }
            return vs;
        }

        public List<string> Follow(string parameter, List<List<string>> _FirstNet, List<string> _Passed)
        {
            _Passed.Add(parameter);
            List<string> vs = new List<string>();
            if (parameter.Equals(_Alphabet[0][0]))
                vs.Add("$");
            for (int i = 0; i < _Grammar.Count(); i++)
                for (int j = 2; j < _Grammar[i].Count(); j++)
                {
                    if (_Grammar[i][j].Equals(parameter))
                    {
                        if (parameter.Equals(_Grammar[i].Last())) // daca parameter e ultimul element
                        {
                            if (_Grammar[i][j] != _Grammar[i][0] && NotPassed(_Passed, _Grammar[i][0]).Equals(true)) // altfel intra intr-o bucla infinita
                                vs.AddRange(Follow(_Grammar[i][0], _FirstNet, _Passed));
                        }
                        else if (_IsNonTer(_Grammar[i][j + 1]).Equals(true))
                        {
                            vs.AddRange(First(_Grammar[i][j + 1], _Grammar[i][0], _FirstNet, _Passed)); // Aplic First de element, Neterminalul de pe pozitia 0 in caz ca noul element merge in Epsilon si lista _FirstNet
                        }
                        else
                            vs.Add(_Grammar[i][j + 1]);
                    }

                }
            return vs;
        }

        public List<List<string>> CreateDirectorySymbols()
        {
            List<List<string>> _FirstNet = new List<List<string>>();
            for (int i = 0; i < _Alphabet[1].Count(); i++)
            {
                List<string> aux = new List<string>();
                aux.Add(_Alphabet[1][i]);
                _FirstNet.Add(aux);
            }

            for (int i = 0; i < _FirstNet.Count(); i++)
            {
                for (int j = 0; j < _Grammar.Count(); j++)
                {
                    if (_Grammar[j][0] == _FirstNet[i][0] && _Grammar[j][2] != "Epsilon")
                    {
                        _FirstNet[i].Add(_Grammar[j][2]);
                        // adaug in firstNet in lista
                        // verific daca e terminal
                        // daca nu caut in first net si inlocui cu ce am acolo
                    }
                }
            }
            while (true)
            {
                int _newModify = 0;
                for (int i = 0; i < _FirstNet.Count(); i++)
                {
                    for (int j = 1; j < _FirstNet[i].Count(); j++)
                    {
                        if (_IsNonTer(_FirstNet[i][j]) == true)
                        {
                            _newModify = 1;
                            int position = ReturnNonTerPos(_FirstNet, _FirstNet[i][j]); // pozitia neterminalului din _FirstNet
                            _FirstNet[i].RemoveAt(j);
                            for (int k = 1; k < _FirstNet[position].Count(); k++)
                            {
                                _FirstNet[i].Add(_FirstNet[position][k]);
                            }
                            // TODO eliminarea duplicatelor din _FirstNet[i]
                        }
                    }
                }
                if (_newModify == 0)
                    break;
            }
            //Print(_FirstNet, "\nFirst de NonTer\n");

            for (int i = 0; i < _Grammar.Count(); i++)
            {
                List<string> vs = new List<string>();
                if (_Grammar[i][2].Equals("Epsilon"))
                {
                    // APLICAM FOLLOW
                    vs = Follow(_Grammar[i][0], _FirstNet, new List<string>());
                }
                else if (_IsNonTer(_Grammar[i][2]) == true)
                {
                    int position = ReturnNonTerPos(_FirstNet, _Grammar[i][2]);
                    for (int j = 1; j < _FirstNet[position].Count(); j++)
                    {
                        vs.Add(_FirstNet[position][j]);
                    }
                }
                else
                    vs.Add(_Grammar[i][2]);
                vs = vs.Distinct().ToList();
                _Symbols.Add(vs);
            }
            return _Symbols;
        }

        public bool Check_LL1()
        {
            for (int i = 0; i < _Grammar.Count(); i++)
            {
                for (int j = i + 1; j < _Grammar.Count(); j++)
                {
                    if (_Grammar[i][0] == _Grammar[j][0])
                    {
                        for (int v = 0; v < _Symbols[i].Count(); v++)
                        {
                            for (int w = 0; w < _Symbols[j].Count(); w++)
                            {
                                if (_Symbols[i][v] == _Symbols[j][w])
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        
        public int ReturnNonTerPos(List<List<string>> _FirstNet, string parameter)
        {
            int position;
            for (int i = 0; i < _FirstNet.Count(); i++)
            {
                if (_FirstNet[i][0].Equals(parameter))
                {
                    position = i;
                    return position;
                }
            }
            return -1;
        }

        public void Print(List<List<string>> parameter, string details)
        {
            verify.AppendText(details);
            int nr = 1;
            foreach (var x in parameter)
            {
                verify.AppendText($"{ nr}) ");
                foreach (var y in x)
                    verify.AppendText($"{y.Trim()} ");
                verify.AppendText("\n");
                nr++;
            }
        }

        public void Print1(List<List<string>> parameter, string details)
        {
            verify1.AppendText(details);
            int i = 1;
            foreach (var x in parameter)
            {
                verify1.AppendText($"D{i} = {{ ");
                foreach (var y in x)
                    verify1.AppendText(y.Trim() + " ");
                verify1.AppendText("}\n");
                i++;

            }
        }

        public void Print2(List<List<string>> parameter, string details)
        {
            verify2.AppendText(details);
            foreach (var x in parameter)
            {
                foreach (var y in x)
                    verify2.AppendText(y.Trim() + " ");
                verify2.AppendText("\n");
            }
        }

        public void Create()
        {
            string textFile;
            // Open the text file using a stream reader.
            using (StreamReader sr = new StreamReader(PathGrammar))
            {
                textFile = sr.ReadToEnd();
            }
           
            //string[] lines = File.ReadAllLines(PathGrammar);
            //string textFile = ReadFile.Main();
            string[] lines = textFile.Split('\n');


            int count = 0; // il folosesc pentru a contoriza daca sunt la a 4-a linie in fisier adica in regulile de productie

            foreach (var x in lines)
            {

                List<string> regula = new List<string>();
                string[] words = x.Split(' ');
                foreach (var y in words)
                {
                    regula.Add(y);
                }
                count++;
                if (count > 4)
                    _Grammar.Add(regula);
                else
                    _Alphabet.Add(regula);
            }

            if (_Grammar.Count() > Int32.Parse(_Alphabet[3][0]))
            {
                int size = _Grammar.Count() - Int32.Parse(_Alphabet[3][0]);
                _Grammar.RemoveRange(_Grammar.Count() - size, size);
            }

            //Print(_Grammar, "Check");


            // scot \r din toata _Grammar
            for (int i = 0; i < _Grammar.Count(); i++)
            {
                for (int j = 0; j < _Grammar[i].Count(); j++)
                    _Grammar[i][j] = _Grammar[i][j].Replace("\r", "");
            }
            for (int i = 0; i < _Alphabet.Count(); i++)
            {
                for (int j = 0; j < _Alphabet[i].Count(); j++)
                    _Alphabet[i][j] = _Alphabet[i][j].Replace("\r", "");
            }
        }

        public void LeftRecursivity()
        {
            for (int i = 0; i < _Grammar.Count(); i++)
            {
                List<List<string>> NetNet = new List<List<string>>(); // regulile de productie ce contin Net : Net
                List<List<string>> NetTer = new List<List<string>>(); // regulile de productie ce contin Net : Ter
                string Net1 = _Grammar[i][0];
                string Net2 = _Grammar[i][2];
                if (Net1 == Net2)
                {
                    _Alphabet[1].Add(Net1 + "1");
                    NetNet.Add(_Grammar[i]);
                    _Grammar.Remove(_Grammar[i]);
                    i--;

                    for (int k = 0; k < _Grammar.Count(); k++)
                        if (_Grammar[k][0] == Net1)
                        {
                            if (_Grammar[k][2] == Net1)
                                NetNet.Add(_Grammar[k]);
                            else
                                NetTer.Add(_Grammar[k]);
                            _Grammar.RemoveAt(k);
                            k--;
                        }

                    // MODIFIC REGULILE DE PRODUCTIE
                    List<string> modified = new List<string>();
                    for (int k = 0; k < NetTer.Count(); k++)
                    {
                        modified = NetTer[k];
                        modified.Add(Net1 + "1");
                        _Grammar.Add(modified);
                    }
                    for (int k = 0; k < NetNet.Count(); k++)
                    {
                        modified = NetNet[k];
                        modified[0] = Net1 + "1";
                        if (modified.Count().Equals(3))
                            modified[2] = "Epsilon";
                        else
                            modified.RemoveAt(2);
                        modified.Add(Net1 + "1");
                        _Grammar.Add(modified);
                    }
                    modified = new List<string>();
                    modified.Add(Net1 + "1");
                    modified.Add("->");
                    modified.Add("Epsilon");
                    _Grammar.Add(modified);
                }
            }
        }

        public void RightTerminal()
        {
            int count = _Grammar.Count(); // scot o linie din _Grammar si trebuie sa raman pe aceasi pozitie dar sa schimb pana unde merg cu iteratiile
            while (true)
            {
                int verify = 0; // tin cont daca mai fac vreo verificare sau nu
                for (int w = 0; w < count; w++)
                {
                    for (int v = 0; v < count; v++)
                    {
                        if (v != w && _Grammar[v][0] == _Grammar[w][0] && _Grammar[v][2] == _Grammar[w][2])
                        {
                            verify = 1;
                            break;
                        }
                    }

                    if (verify == 1)
                        break;
                }
                if (verify == 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        List<List<string>> NetTer = new List<List<string>>();
                        int ok = 0;

                        for (int j = 0; j < count; j++)
                        {
                            if (j == i)
                                continue;
                            if (_Grammar[i][0] == _Grammar[j][0] && _Grammar[i][2] == _Grammar[j][2])
                            {

                                NetTer.Add(_Grammar[j]);
                                _Grammar.RemoveAt(j);
                                j--;
                                count = _Grammar.Count();
                                ok = 1;
                            }
                        }
                        if (ok == 1)
                        {
                            // TODO Net1 si Net2 sunt liste de elemente

                            string Net1 = _Grammar[i][0] + "2";
                            string Net2 = _Grammar[i][2]; // este ce au in comun

                            _Alphabet[1].Add(Net1);
                            NetTer.Add(_Grammar[i]);
                            _Grammar.RemoveAt(i);
                            i--;

                            List<string> modified = new List<string>(NetTer[0]);
                            int max_size = 3;
                            if (max_size < NetTer[0].Count())
                            {
                                string parameter = NetTer[0][max_size];

                                while (true)
                                {
                                    int aux = max_size;
                                    if (max_size < NetTer[0].Count())
                                    {
                                        parameter = NetTer[0][max_size];
                                        for (int k = 1; k < NetTer.Count(); k++)
                                        {
                                            if (max_size >= NetTer[k].Count() || NetTer[k][max_size] != parameter)
                                            {
                                                max_size--;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                        break;

                                    max_size++;
                                    if (aux == max_size)
                                        break;
                                }
                            }

                            //modified = NetTer[0];
                            modified.RemoveRange(max_size, modified.Count() - max_size);
                            modified.Add(Net1);
                            _Grammar.Add(modified);

                            for (int k = 0; k < NetTer.Count(); k++)
                            {
                                modified = NetTer[k];
                                modified.RemoveRange(2, max_size - 2);
                                modified[0] = Net1;
                                if (modified.Count().Equals(2))
                                    modified.Add("Epsilon");
                                _Grammar.Add(modified);
                                
                            }
                        }

                    }
                }
                else
                    break;
            }
        }

        private bool NotPassed(List<string> _Passed, string parameter)
        {
            foreach (var x in _Passed)
                if (x.Equals(parameter))
                    return false;
            return true;
        }

        private void Begin()
        {
            string codePath = PathGrammar.Substring(0, PathGrammar.LastIndexOf('\\')) + @"\ResultCode.cs";
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(codePath))
            {
                sw.Write(ExecCode);
            }

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            string Output = PathGrammar.Substring(0, PathGrammar.LastIndexOf('\\')) + @"\ExecFile.exe";

            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = true,
                OutputAssembly = Output
            };
            CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, codePath);
            if (results.Errors.Count > 0)
                return;

            Process.Start(Output);
        }

        private void AddGrammar_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog objects = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Fisiere TEXT|*.txt"
            };


            if (objects.ShowDialog() == true)
            {
                PathGrammar = objects.FileName;
                ModifyGrammarB.Visibility = Visibility.Visible;
                Create();
                // Afisare _Alphabet + _Grammar
                Print(_Alphabet, "_Alphabet\n");
                Print(_Grammar, "\n_Grammar\n");
                ModifyGrammarB.Visibility = Visibility.Visible;
                AddGrammar.Visibility = Visibility.Hidden;
            }
            else
                verify.Text = "Grammar not load!";
        }

        
    }
}
