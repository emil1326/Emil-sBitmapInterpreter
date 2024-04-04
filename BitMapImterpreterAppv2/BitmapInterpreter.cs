using BitMapMaker;

namespace BitMapImterpreterAppv2
{
    public class CodeBase
    {
        public string CBName;
        public List<string> Lines = new();

        public string GetAllText()
        {
            string s = "";
            foreach (string line in Lines)
                s += line + "\n";
            return s;
        }

        public void SaveToCB(string Val)
        {
            Lines = Val.Split("\n").ToList();
        }

        public void Run()
        {
            BitmapInterpreter.RunBitMapEnterpreter(Lines.ToArray());
        }

        public bool CheckSyntax()
        {
            return BitmapInterpreter.CheckSyntax(Lines.ToArray());
        }
    }

    static class BitmapInterpreter
    {
        public static bool CheckSyntax(string[] Instructions)
        {
            return false;
        }
        public static bool CheckSyntax(string Instructions)
        {
            return false;
        }

        // gotta make sure that for loops dont repeat themseves after the full exectution ==> should work
        // make if statments

        public static dynamic RunBitMapEnterpreter(string[] LineInput, Dictionary<string, dynamic> V = null, Dictionary<string, BitmapImage> B = null, List<Func> F = null, string ARG = "")
        {
            Dictionary<string, dynamic> Vars = new();
            Dictionary<string, BitmapImage> BM = new();
            List<Func> Functions = new();

            if (V != null)
            {
                Vars = V;
                BM = B;
                Functions = F;
            }

            string[] lines = LineInput;

            for (int i = 0; i < lines.Length; i++) // everylines to lower && trim
                lines[i] = lines[i].ToLower().Trim();

            if (!CheckSyntax(lines))
                throw new Exception("Error in syntax check");

            DiscoverFunctions();

            for (int i = 0; i < lines.Length; i++)    // main block going trough all the lines
            {
                string currline = lines[i];

                if (ARG == "forLoop")
                    if (currline == "endfor") // might not work? ==> terminates the loop
                        return true;

                if (ARG == "forXY")
                    if (currline == "endxy") // might not work?
                        return true;

                if (currline.StartsWith("//"))
                    break; // is comment
                else if (currline.Split(" ")[1] == "=")
                    DoNewValue(currline); // is new value or updated value
                else if (currline.StartsWith("forval"))
                    i = DoForLoops(i);
                else if (currline.StartsWith("forxy"))
                    i = DoForXYLoops(i);
                else if (currline.StartsWith("if"))
                    i = DoIfStatement(i);
                else if (currline.StartsWith("return"))
                    return ReturnFunction(currline);
                else if (currline.StartsWith("pxy ="))
                    return ReturnPixelForXY(currline);
                else
                    CheckFunction(i);
            }

            void DoNewValue(string currline)
            {
                string[] Words = currline.Split(" ");
                if (Vars.ContainsKey(Words[0]))
                    ModifyValue(currline);
                else
                    Vars.Add(Words[0], ReturnVar(Words[2]));
            }

            void ModifyValue(string currline)
            {
                string[] Words = currline.Split(" ");
                string key = Words[0];
                char OP = GetOperator(currline);

                if (ReturnVar(Vars[key]).GetType() == typeof(string))        //can add and substract string
                {
                    if (OP == '+')
                        Vars[key] = Words[2] + Words[4];
                    else if (OP == '-')
                        if (Words[2].EndsWith(Words[4]))
                            Vars[key] = Words[2].Remove(Words[2].Length - Words[4].Length);
                        else
                            throw new Exception($"cant substract different ending strings at {currline}");
                    else if (OP == ' ')
                        Vars[key] = Words[2];
                    else if (OP == '*' || OP == '/' || OP == '%')
                        throw new Exception($"cant do {OP} on strings at {currline}");
                }
                else if (ReturnVar(Vars[Words[0]]).GetType() == typeof(int))
                {
                    int val1 = int.Parse(Words[2]);
                    int val2 = 0;
                    if (OP != ' ')
                        val2 = int.Parse(Words[4]);

                    if (OP == '+')
                        Vars[key] = val1 + val2;
                    else if (OP == '-')
                        Vars[key] = val1 - val2;
                    else if (OP == '*')
                        Vars[key] = val1 * val2;
                    else if (OP == '/')
                        Vars[key] = val1 / val2;
                    else if (OP == '%')
                        Vars[key] = val1 % val2;
                    else if (OP == ' ')
                        Vars[key] = val1;
                    else
                        throw new Exception($"couldnt apply operand {OP} at line {currline}");
                }
                else if (ReturnVar(Vars[Words[0]]).GetType() == typeof(float))
                {
                    float val1 = float.Parse(Words[2]);
                    float val2 = 0;
                    if (OP != ' ')
                        val2 = float.Parse(Words[4]);

                    if (OP == '+')
                        Vars[key] = val1 + val2;
                    else if (OP == '-')
                        Vars[key] = val1 - val2;
                    else if (OP == '*')
                        Vars[key] = val1 * val2;
                    else if (OP == '/')
                        Vars[key] = val1 / val2;
                    else if (OP == '%')
                        Vars[key] = val1 % val2;
                    else if (OP == ' ')
                        Vars[key] = val1;
                    else
                        throw new Exception($"couldnt apply operand {OP} at line {currline}");
                }
                else if (ReturnVar(Vars[Words[0]]).GetType() == typeof(bool))
                {
                    bool val1 = (bool)Vars[Words[2]];
                    bool val2 = (bool)Vars[Words[4]];

                    OP = GetBoolOP(currline);

                    if (OP == '&') // AND
                        Vars[key] = val1 && val2;
                    else if (OP == '|') // OR
                        Vars[key] = val1 || val2;
                    else if (OP == '^') // XOR
                        Vars[key] = val1 ^ val2;
                    else if (OP == ' ')
                        Vars[key] = val1;
                    else
                        throw new Exception($"could not apply operand {OP} to boolean values at line {currline}");
                }

                char GetOperator(string Input)
                {
                    if (Input.Contains('+'))
                        return '+';
                    else if (Input.Contains('-'))
                        return '-';
                    else if (Input.Contains('*'))
                        return '*';
                    else if (Input.Contains('/'))
                        return '/';
                    else if (Input.Contains('%'))
                        return '%';
                    else if (Input.Contains('='))
                        return '=';
                    else
                        return ' ';
                }

                char GetBoolOP(string Input)
                {
                    if (Input.Contains('&'))
                        return '&';
                    else if (Input.Contains('|'))
                        return '|';
                    else if (Input.Contains('^'))
                        return '^';
                    else
                        return ' ';
                }
            }

            int DoForLoops(int Index)
            {
                Func ForFunc = new($"ForValLoop{Index}", false, false, null);

                int LineNB = 1;
                int LastLine = 0;
                for (int i = Index; i < lines.Length; i++)
                {
                    if (LineNB != 0)
                        if (lines[i].StartsWith("forval"))
                            LineNB++;
                        else if (lines[i].StartsWith("endforval"))
                            LineNB--;
                    LastLine = i;
                }

                ForFunc.Body = lines[Index..(LastLine + 1)];

                string start = lines[Index].Split(" ")[1][1..^1];
                string end = lines[Index].Split(" ")[2][..^1];

                int S = int.Parse(start);
                int E = int.Parse(end);

                string name = lines[Index].Split(" ")[5];

                for (int i = S; i < E; i++)
                {
                    DoNewValue($"{name} = {i}");                            //set value before executing it
                    ForFunc.Execute(Vars, BM, Functions, "forLoop");
                }

                Vars.Remove(name);
                return LastLine;
            }

            int DoForXYLoops(int Index)
            {
                Func ForFuncXY = new($"ForXYLoop{Index}", false, false, null);

                int LineNB = 1;
                int LastLine = 0;
                for (int i = Index; i < lines.Length; i++)
                {
                    if (LineNB != 0)
                        if (lines[i].StartsWith("forxy"))
                            LineNB++;
                        else if (lines[i].StartsWith("endforxy"))
                            LineNB--;
                    LastLine = i;
                }

                ForFuncXY.Body = lines[Index..(LastLine + 1)];

                string BMName = lines[Index].Split(" ")[1];
                string v1 = lines[Index].Split(" ")[3][..^1];
                string v2 = lines[Index].Split(" ")[4];

                for (int x = 0; x < BM[BMName].lengthX; x++)
                    for (int y = 0; y < BM[BMName].lengthY; y++)
                    {
                        DoNewValue($"{v1} = {x}");                            //set value before executing it
                        DoNewValue($"{v2} = {y}");
                        BM[BMName].Image[x, y] = ForFuncXY.Execute(Vars, BM, Functions, "forXY");
                    }

                Vars.Remove(v1);
                Vars.Remove(v2);

                return LastLine;
            }

            Pixel ReturnPixelForXY(string currline)
            {
                string[] words = currline.Replace(",", "").Split(" ");

                Pixel P = new();

                P.SetValues((byte)ReturnVar(words[2]), (byte)ReturnVar(words[2]), (byte)ReturnVar(words[2]));

                return P;
            }

            void DiscoverFunctions()
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string name = lines[i].Split(" ")[1];

                    if (lines[i].StartsWith("func"))
                        Functions.Add(new(name, false, false, GetFunctionBody()));
                    else if (lines[i].StartsWith("Vfunc"))
                        Functions.Add(new(name, true, false, GetFunctionBody()));
                    else if (lines[i].StartsWith("Rfunc"))
                        Functions.Add(new(name, false, true, GetFunctionBody()));
                    else if (lines[i].StartsWith("RVfunc"))
                        Functions.Add(new(name, true, true, GetFunctionBody()));

                    string[] GetFunctionBody()
                    {
                        List<string> body = new();
                        for (int NI = i; NI < lines.Length; NI++)
                            if (lines[NI] != "endFunc")
                                body.Add(lines[NI]);
                            else
                                return body.ToArray();

                        throw new Exception($"Couldnt get Function body at line {i}");
                    }
                }
            }

            dynamic ReturnFunction(string currline)
            {
                return Vars[currline.Split(" ")[2]];
            }

            void CheckFunction(int Index)
            {
                string currline = lines[Index];

                int pos = FindUserFunctionByName(currline);

                if (pos == -1)
                    SysFunc(currline);
                else    // do user func
                    UserFunc(pos, currline);
            }

            void SysFunc(string currline) // need check that the function exist else its error in writing function name
            {
                string[] words = currline.Split(" ");

                if (currline.StartsWith("newimg"))
                    NewIMG();
                else if (currline.StartsWith("noise"))
                    Noise();
                else if (currline.StartsWith("noisealong"))
                    NoiseAlong();
                else if (currline.StartsWith("blur"))
                    Blur();
                else if (currline.StartsWith("luminosityinvert"))
                    LuminosityInvert();
                else if (currline.StartsWith("colorshift"))
                    ColorShift();
                else if (currline.StartsWith("sharpscale"))
                    SharpScale();
                else if (words[3] == "getrandomr")
                    GetRandomR();
                else if (currline.StartsWith("saveimage"))
                    SaveImage();
                else if (currline.StartsWith("savehistogram"))
                    SaveHistogram();
                else
                    throw new Exception($"Miss read function at line {currline}");


                void NewIMG()
                {
                    BitmapImage NewBM = new(GetArg<int>(2), GetArg<int>(3), GetArg<bool>(4), GetArg<string>(5));
                    BM.Add(words[1], NewBM);
                }
                void Noise()
                {
                    BM[words[1]].Noise(GetArg<int>(2), GetArg<int>(3), GetArg<int>(4), GetArg<int>(5), GetArg<byte>(6));
                }
                void NoiseAlong()
                {
                    BM[words[1]].NoiseAlong(GetArg<int>(2), GetArg<int>(3), GetArg<int>(4), GetArg<int>(5), GetArg<byte>(6));
                }
                void Blur()
                {
                    BM[words[1]].Blur(GetArg<int>(2), GetArg<int>(3), GetArg<int>(4), GetArg<int>(5), GetArg<byte>(6));
                }
                void LuminosityInvert()
                {
                    BM[words[1]].LuminosityInvertion();
                }
                void ColorShift()
                {
                    BM[words[1]].ColorShift(GetArg<short>(2));
                }
                void SharpScale()
                {
                    BM[words[1]].SharpScale(GetArg<float>(2));
                }
                void GetRandomR()
                {
                    if (words[6] != null)
                        Vars[words[0]] = RandomRange(GetArg<int>(4), GetArg<int>(5), GetArg<int>(6));
                    else
                        Vars[words[0]] = RandomRange(GetArg<int>(4), GetArg<int>(5));

                    static float RandomRange(float Min, float Max, int seed = -1)
                    {
                        Random rand;
                        if (seed != -1)
                            rand = new(seed);
                        else
                            rand = new();

                        return (rand.NextSingle() * Max + Min) - Min;
                    }
                }
                void SaveImage()
                {
                    if (words[2] != null)
                        BM[words[1]].SaveImage(GetArg<string>(2));
                    else
                        BM[words[1]].SaveImage();
                }
                void SaveHistogram()
                {
                    BM[words[1]].SaveHistogram();
                }

                T GetArg<T>(int WordIndex)
                {
                    string word = words[WordIndex];
                    word = word.Replace("(", "");
                    word = word.Replace(")", "");
                    word = word.Replace(",", "");
                    T TryiedConversion = (T)Convert.ChangeType(word, typeof(T));

                    if (TryiedConversion == null)
                        return Vars[word];
                    else
                        return TryiedConversion;
                }
            }

            void UserFunc(int pos, string line)
            {
                Func func = Functions[pos];

                if (func.HasArg && func.HasReturn)
                {
                    string[] Words = line.Split(" ");

                    List<string> Args = new();
                    for (int i = 2; i < Words.Length; i++)
                    {
                        Args.Add(Words[i]);
                        DoNewValue($"{Words[i]} = {Vars[Words[i]]}");
                    }

                    dynamic R = func.Execute(Vars, BM, Functions, "");

                    DoNewValue($"{Words[0]} = {R}");

                    foreach (string S in Args)
                        Vars.Remove(S);
                }
                else if (!func.HasArg && func.HasReturn)
                {
                    string[] Words = line.Split(" ");

                    dynamic R = func.Execute(Vars, BM, Functions, "");

                    DoNewValue($"{Words[0]} = {R}");
                }
                else if (func.HasArg && !func.HasReturn)
                {
                    string[] Words = line.Split(" ");

                    List<string> Args = new();
                    for (int i = 2; i < Words.Length; i++)
                    {
                        Args.Add(Words[i]);
                        DoNewValue($"{Words[i]} = {Vars[Words[i]]}");
                    }

                    func.Execute(Vars, BM, Functions, "");

                    foreach (string S in Args)
                        Vars.Remove(S);
                }
                else
                    func.Execute(Vars, BM, Functions, "");
            }

            int FindUserFunctionByName(string Input)
            {
                string Fname = Input.Split(" ")[0]; //if called alone or sys func
                int pos = Functions.FindIndex((F) => F.Name == Fname);

                if (pos == -1)
                {
                    Fname = Input.Split(" ")[2]; //if called as return to another variable
                    pos = Functions.FindIndex((F) => F.Name == Fname);
                }

                // if pos still -1 means its sys function

                return pos;
            }

            int DoIfStatement(int index)
            {
                string currline;
                string[] words;

                for (int i = index; i < lines.Length; i++)
                {
                    UpdateCurrline();
                    void UpdateCurrline()
                    {
                        currline = lines[i];
                        words = currline.Split(" ");
                    }

                    if (currline == "endif")
                        return i; //end loop

                    if (words.Length >= 3) // is operator
                    {
                        Func Ex = new($"IsFunc{i}", false, false, null);

                        if (currline.StartsWith("if "))
                            if (GetIsEquality(words[1], words[3], words[2]))
                                Ex.Body = GetBody(i);

                        if (currline.StartsWith("else if "))
                            if (GetIsEquality(words[2], words[4], words[3]))
                                Ex.Body = GetBody(i);

                        Ex.Execute(Vars, BM, Functions, "");
                    }
                    else if (words.Length == 2)//only one bool after
                    {
                        Func Ex = new($"IsFunc{i}", false, false, null);

                        if (currline.StartsWith("if "))
                            if (Vars[words[1]])
                                Ex.Body = GetBody(i);

                        if (currline.StartsWith("else if "))
                            if (Vars[words[2]])
                                Ex.Body = GetBody(i);
                    }
                    else //is an else statement
                    {

                    }

                        ;
                }

                string[] GetBody(int Currindex)
                {

                } 

                bool GetIsEquality(string w1, string w2, string op)
                {
                    return true;

                    bool EQ()
                    {
                        return true;
                    }
                }

                return 1;
            }

            return true; //returns true to say it worked
        }

        static object ReturnVar(string Input)
        {
            if (Input == "true" || Input == "false")
                return bool.Parse(Input);

            else if (float.TryParse(Input, out float floatValue))
                return floatValue;

            else if (int.TryParse(Input, out int intValue))
                return intValue;

            return Input; //if string   
        }
    }

    class Func
    {
        public string Name;
        public bool HasArg;
        public bool HasReturn;
        public string[] Body;

        public Func(string name, bool hasArg, bool hasReturn, string[] body)
        {
            Name = name;
            HasArg = hasArg;
            HasReturn = hasReturn;
            Body = body;
        }

        public dynamic Execute(Dictionary<string, dynamic> Vars, Dictionary<string, BitmapImage> BM, List<Func> Functions, string ARG)
        {
            return BitmapInterpreter.RunBitMapEnterpreter(Body, Vars, BM, Functions, ARG);
        }
    }
}
