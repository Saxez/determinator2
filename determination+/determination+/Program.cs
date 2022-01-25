using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinator
{
    class Program
    {
        struct Field
        {
            public string name;
            public Dictionary<string, string> TableTrans;
        }

        struct Init
        {
            public string type;
            public int length;
            public string pathToFile;
            public StreamReader sr;
            public string input;
            public Table table;
            public Table fields;
            public string[] param;
            public List<string> FirstZoom;
        }

        struct Table
        {
            public List<Field> table;
            public List<string> inTable;
        }
        static void Main(string[] args)
        {
            Init initData = new Init();
            Initialization(ref initData);
            FillTableNames(ref initData);
            FillTableValues(ref initData);
            Determinate(ref initData);
            WriteOut(initData.fields);
        }

        static void FillTableNames(ref Init initData)
        {
            for (int i = 0; i < initData.length; i++)
            {
                Field field = new Field();
                string name = GetNameToField(initData.param[i + 2]);
                field.name = name;
                field.TableTrans = new Dictionary<string, string>();
                initData.table.table.Add(field);
                initData.table.inTable.Add(name);
            }
        }

        static void Initialization(ref Init initData)
        {
            initData.pathToFile = Environment.CurrentDirectory + "\\Input.txt";
            initData.sr = new StreamReader(initData.pathToFile);
            initData.input = initData.sr.ReadToEnd();
            initData.param = initData.input.Replace("\r", "").Split('\n');
            initData.type = initData.param[0];
            initData.length = int.Parse(initData.param[1].ToString());
            initData.table.table = new List<Field>();
            initData.table.inTable = new List<string>();
            initData.fields.table = new List<Field>();
            initData.fields.inTable = new List<string>();
            initData.FirstZoom = initData.table.inTable;
        }

        static void FillTableValues(ref Init initData)
        {
            if (initData.type == "L")
            {
                Field fieldH = new Field();
                fieldH.name = "H";
                fieldH.TableTrans = new Dictionary<string, string>();
                initData.table.table.Add(fieldH);
                initData.fields.table.Add(fieldH);
                initData.fields.inTable.Add("H");
                for (int i = 0; i < initData.length; i++)
                {
                    GetInfoToFields(ref initData.table.table, initData.param[i + 2]);
                }
            }
            else
            {
                Field fieldStart = new Field();
                for (int i = 0; i < initData.table.table.Count; i++)
                {
                    if (initData.table.table[i].name == "S")
                    {
                        fieldStart = initData.table.table[i];
                    }
                }
                initData.fields.table.Add(fieldStart);
                initData.fields.inTable.Add("S");
                for (int i = 0; i < initData.length; i++)
                {
                    GetInfoToFieldsR(ref initData.table.table, initData.param[i + 2]);
                }
            }
        }

        static void Determinate(ref Init initData)
        {
            int iteration = 0;
            int x = initData.table.table.Count;
            for (int i = 0; i < initData.table.table.Count; i++)
            {
                DelZeroWay(ref initData, i);
            }
            while (iteration != initData.fields.inTable.Count)
            {
                iteration = initData.fields.inTable.Count;
                DeterminateIteration(ref initData.fields, initData.FirstZoom, initData.table);
            }
        }
        static void DelZeroWay(ref Init initData, int i)
        {
            List<string> against = new List<string>();
            Field test = initData.table.table[i];
            if (!test.TableTrans.ContainsKey("E"))
            {
                return;
            }

            while(test.TableTrans["E"].Length != 0)
            {
                against.Add(test.TableTrans["E"][0].ToString());
                test.TableTrans["E"] = test.TableTrans["E"].Remove(0, 1);
                for (int k = 0; k < initData.table.table.Count; k++)
                {
                    if (initData.table.table[k].name == against[against.Count-1])
                    {
                        foreach(string key in initData.table.table[k].TableTrans.Keys)
                        {
                            foreach (char sym in initData.table.table[k].TableTrans[key])
                            {
                                if(!initData.table.table[i].TableTrans.ContainsKey(key))
                                {
                                    initData.table.table[i].TableTrans.Add(key, sym.ToString());
                                }
                                if (!initData.table.table[i].TableTrans[key].Contains(sym))
                                {
                                    string added = initData.table.table[i].TableTrans[key];
                                    added += sym;
                                    initData.table.table[i].TableTrans.Remove(key);
                                    initData.table.table[i].TableTrans[key] = added;
                                }
                            }
                        }
                        break;
                    }
                }
            }

            initData.table.table[i].TableTrans.Remove("E");

        }
        static void GetInfoToFieldsR(ref List<Field> table, string param)
        {
            string mainName = param.Split(' ')[0];

            string[] transition = (param.Split(' '))[1].Split('|');

            for (int i = 0; i < transition.Length; i++)
            {

                if (transition[i].Length < 2)
                {
                    transition[i] = transition[i] + "H";
                }

                for (int j = 0; j < table.Count; j++)
                {
                    if (table[j].name == mainName)
                    {
                        if (table[j].TableTrans.ContainsKey(transition[i][0].ToString()))
                        {
                            string AddedTrans = transition[i][1].ToString() + table[j].TableTrans[transition[i][0].ToString()];
                            table[j].TableTrans.Remove(transition[i][0].ToString());
                            table[j].TableTrans.Add(transition[i][0].ToString(), AddedTrans);
                        }
                        else
                        {
                            table[j].TableTrans.Add(transition[i][0].ToString(), transition[i][1].ToString());
                        }
                        break;
                    }
                }
            }
        }

        static void DeterminateIteration(ref Table fields, List<string> FirstZoom, Table table)
        {
            List<string> GettedFields = new List<string>();
            for (int i = 0; i < fields.table.Count; i++)
            {
                List<string> Keys = new List<string>(fields.table[i].TableTrans.Keys);

                for (int j = 0; j < Keys.Count; j++)
                {
                    if ((!GettedFields.Contains(fields.table[i].TableTrans[Keys[j]])) && (!fields.inTable.Contains(fields.table[i].TableTrans[Keys[j]])) &&(fields.table[i].TableTrans[Keys[j]].Length != 0))
                    {
                        GettedFields.Add(fields.table[i].TableTrans[Keys[j]]);
                    }

                }
            }
            for (int i = 0; i < GettedFields.Count; i++)
            {
                if (!fields.inTable.Contains(GettedFields[i]))
                {
                    Field field = new Field();
                    field.TableTrans = new Dictionary<string, string>();
                    field.name = GettedFields[i];
                    if (!FirstZoom.Contains(GettedFields[i]))
                    {
                        GetNewField(ref field, table, GettedFields[i]);
                    }
                    else
                    {
                        GetOldField(ref field, table, GettedFields[i]);
                    }
                    fields.table.Add(field);
                    fields.inTable.Add(GettedFields[i]);
                }
            }

        }


        static void GetOldField(ref Field field, Table table, string newField)
        {
            for (int i = 0; i < table.table.Count; i++)
            {
                if (table.table[i].name == newField)
                {
                    field = table.table[i];
                    break;
                }
            }
        }

        static void GetNewField(ref Field field, Table table, string newField)
        {
            for (int j = 0; j < newField.Length; j++)
            {
                for (int k = 0; k < table.table.Count; k++)
                {
                    if (table.table[k].name == newField[j].ToString())
                    {
                        List<string> Keys = new List<string>(table.table[k].TableTrans.Keys);
                        foreach (string key in Keys)
                        {
                            if ((field.TableTrans.ContainsKey(key)) && (true))
                            {
                                string Added = field.TableTrans[key] + table.table[k].TableTrans[key];
                                Added = string.Concat(Added.Distinct());
                                field.TableTrans.Remove(key);
                                field.TableTrans.Add(key, Added);
                            }
                            else
                            {
                                field.TableTrans.Add(key, table.table[k].TableTrans[key]);
                            }
                        }
                    }
                }
            }
        }

        static string GetNameToField(string param)
        {
            return param.Split(' ')[0];
        }

        static void GetInfoToFields(ref List<Field> table, string param)
        {
            string AddPos = param.Split(' ')[0];
            string[] transition = (param.Split(' '))[1].Split('|');
            for (int i = 0; i < transition.Length; i++)
            {
                if (transition[i].Length < 2)
                {

                    transition[i] = "H" + transition[i];

                }

                for (int j = 0; j < table.Count; j++)
                {

                    if (table[j].name == transition[i][0].ToString())
                    {
                        if (table[j].TableTrans.ContainsKey(transition[i][1].ToString()))
                        {
                            string AddedTrans = AddPos + table[j].TableTrans[transition[i][1].ToString()];
                            table[j].TableTrans.Remove(transition[i][1].ToString());
                            table[j].TableTrans.Add(transition[i][1].ToString(), AddedTrans);
                        }
                        else
                        {
                            table[j].TableTrans.Add(transition[i][1].ToString(), AddPos);
                        }
                        break;
                    }
                }
            }
        }

        static void WriteOut(Table outTable)
        {
            StreamWriter f = new StreamWriter("output.txt");
            for (int i = 0; i < outTable.table.Count; i++)
            {
                string output = "";
                List<string> Keys = new List<string>(outTable.table[i].TableTrans.Keys);
                output = outTable.table[i].name + " -> ";
                foreach (string key in Keys)
                {
                    output = output + " " + key + outTable.table[i].TableTrans[key] + "|";
                }
                output = output.Remove(output.Length - 1, 1);
                f.WriteLine(output);
            }
            f.Close();
        }
    }
}
