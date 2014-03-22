using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data;
using System.Collections;
using System.Threading;
using fastBinaryJSON;
using System.Collections.Specialized;

namespace UnitTests
{
    public class Tests
    {
        #region [  helpers  ]
        static int count = 1000;
        static int tcount = 5;
        static DataSet ds = new DataSet();
        static bool exotic = false;
        static bool dsser = false;

        public enum Gender
        {
            Male,
            Female
        }

        public class colclass
        {
            public colclass()
            {
                items = new List<baseclass>();
                date = DateTime.Now;
                multilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ";
                isNew = true;
                booleanValue = true;
                ordinaryDouble = 0.001;
                gender = Gender.Female;
                intarray = new int[5] { 1, 2, 3, 4, 5 };
            }
            public bool booleanValue { get; set; }
            public DateTime date { get; set; }
            public string multilineString { get; set; }
            public List<baseclass> items { get; set; }
            public decimal ordinaryDecimal { get; set; }
            public double ordinaryDouble { get; set; }
            public bool isNew { get; set; }
            public string laststring { get; set; }
            public Gender gender { get; set; }

            public DataSet dataset { get; set; }
            public Dictionary<string, baseclass> stringDictionary { get; set; }
            public Dictionary<baseclass, baseclass> objectDictionary { get; set; }
            public Dictionary<int, baseclass> intDictionary { get; set; }
            public Guid? nullableGuid { get; set; }
            public decimal? nullableDecimal { get; set; }
            public double? nullableDouble { get; set; }
            public Hashtable hash { get; set; }
            public baseclass[] arrayType { get; set; }
            public byte[] bytes { get; set; }
            public int[] intarray { get; set; }

        }

        public static colclass CreateObject()
        {
            var c = new colclass();

            c.booleanValue = true;
            c.ordinaryDecimal = 3;

            if (exotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, baseclass>();
                c.objectDictionary = new Dictionary<baseclass, baseclass>();
                c.intDictionary = new Dictionary<int, baseclass>();
                c.nullableDouble = 100.003;

                if (dsser)
                    c.dataset = ds;
                c.nullableDecimal = 3.14M;

                c.hash.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
                c.hash.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

                c.stringDictionary.Add("name1", new class2("1", "code", "desc"));
                c.stringDictionary.Add("name2", new class1("1", "code", Guid.NewGuid()));

                c.intDictionary.Add(1, new class2("1", "code", "desc"));
                c.intDictionary.Add(2, new class1("1", "code", Guid.NewGuid()));

                c.objectDictionary.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
                c.objectDictionary.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

                c.arrayType = new baseclass[2];
                c.arrayType[0] = new class1();
                c.arrayType[1] = new class2();
            }


            c.items.Add(new class1("1", "1", Guid.NewGuid()));
            c.items.Add(new class2("2", "2", "desc1"));
            c.items.Add(new class1("3", "3", Guid.NewGuid()));
            c.items.Add(new class2("4", "4", "desc2"));

            c.laststring = "" + DateTime.Now;

            return c;
        }

        public class baseclass
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }

        public class class1 : baseclass
        {
            public class1() { }
            public class1(string name, string code, Guid g)
            {
                Name = name;
                Code = code;
                guid = g;
            }
            public Guid guid { get; set; }
        }

        public class class2 : baseclass
        {
            public class2() { }
            public class2(string name, string code, string desc)
            {
                Name = name;
                Code = code;
                description = desc;
            }
            public string description { get; set; }
        }

        public class NoExt
        {
            [System.Xml.Serialization.XmlIgnore()]
            public string Name { get; set; }
            public string Address { get; set; }
            public int Age { get; set; }
            public baseclass[] objs { get; set; }
            public Dictionary<string, class1> dic { get; set; }
            public NoExt intern { get; set; }
        }

        public class Retclass
        {
            public object ReturnEntity { get; set; }
            public string Name { get; set; }
            public string Field1;
            public int Field2;
            public string ppp { get { return "sdfas df "; } }
            public DateTime date { get; set; }
            public DataTable ds { get; set; }
        }

        public struct Retstruct
        {
            public object ReturnEntity { get; set; }
            public string Name { get; set; }
            public string Field1;
            public int Field2;
            public string ppp { get { return "sdfas df "; } }
            public DateTime date { get; set; }
            public DataTable ds { get; set; }
        }

        private static long CreateLong(string s)
        {
            long num = 0;
            bool neg = false;
            foreach (char cc in s)
            {
                if (cc == '-')
                    neg = true;
                else if (cc == '+')
                    neg = false;
                else
                {
                    num *= 10;
                    num += (int)(cc - '0');
                }
            }

            return neg ? -num : num;
        }

        private static DataSet CreateDataset()
        {
            DataSet ds = new DataSet();
            for (int j = 1; j < 3; j++)
            {
                DataTable dt = new DataTable();
                dt.TableName = "Table" + j;
                dt.Columns.Add("col1", typeof(int));
                dt.Columns.Add("col2", typeof(string));
                dt.Columns.Add("col3", typeof(Guid));
                dt.Columns.Add("col4", typeof(string));
                dt.Columns.Add("col5", typeof(bool));
                dt.Columns.Add("col6", typeof(string));
                dt.Columns.Add("col7", typeof(string));
                ds.Tables.Add(dt);
                Random rrr = new Random();
                for (int i = 0; i < 100; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = rrr.Next(int.MaxValue);
                    dr[1] = "" + rrr.Next(int.MaxValue);
                    dr[2] = Guid.NewGuid();
                    dr[3] = "" + rrr.Next(int.MaxValue);
                    dr[4] = true;
                    dr[5] = "" + rrr.Next(int.MaxValue);
                    dr[6] = "" + rrr.Next(int.MaxValue);

                    dt.Rows.Add(dr);
                }
            }
            return ds;
        }

        public class RetNestedclass
        {
            public Retclass Nested { get; set; }
        }
        #endregion

        [Test]
        public static void objectarray()
        {
            var o = new object[3] { 1, "sdfsdfs", DateTime.Now};
            var b = fastBinaryJSON.BJSON.Instance.ToBJSON(o);
            var s = fastBinaryJSON.BJSON.Instance.ToObject(b); 
        }

        [Test]
        public static void ClassTest()
        {
            Retclass r = new Retclass();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var b = fastBinaryJSON.BJSON.Instance.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.Instance.ToObject(b);

            Assert.AreEqual(2312, (o as Retclass).Field2);
        }


        [Test]
        public static void StructTest()
        {
            Retstruct r = new Retstruct();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var b = fastBinaryJSON.BJSON.Instance.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.Instance.ToObject(b);

            Assert.AreEqual(2312, ((Retstruct)o).Field2);
        }

        [Test]
        public static void ParseTest()
        {
            Retclass r = new Retclass();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.Instance.Parse(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void StringListTest()
        {
            List<string> ls = new List<string>();
            ls.AddRange(new string[] { "a", "b", "c", "d" });

            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(ls);

            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void IntListTest()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(ls);

            var p = fastBinaryJSON.BJSON.Instance.Parse(s);
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s); // long[] {1,2,3,4,5,10}

            Assert.IsNotNull(o);
        }

        [Test]
        public static void Variables()
        {
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(42);
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);
            Assert.AreEqual(o, 42);

            s = fastBinaryJSON.BJSON.Instance.ToBJSON("hello");
            o = fastBinaryJSON.BJSON.Instance.ToObject(s);
            Assert.AreEqual(o, "hello");
        }

        //[Test]
        //public static void SubClasses()
        //{

        //}

        //[Test]
        //public static void CasttoSomthing()
        //{

        //}

        //[Test]
        //public static void IgnoreCase()
        //{

        //}

        [Test]
        public static void Perftest()
        {
            string s = "123456";

            DateTime dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = CreateLong(s);
            }

            Console.WriteLine("convertlong (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

            dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = long.Parse(s);
            }

            Console.WriteLine("long.parse (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

            dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = Convert.ToInt64(s);
            }

            Console.WriteLine("convert.toint64 (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);
        }

        [Test]
        public static void List_int()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(ls);
            Console.WriteLine(s);
            var p = fastBinaryJSON.BJSON.Instance.Parse(s);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<List<int>>(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void Dictionary_String_RetClass()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_String_RetClass_noextensions()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass_noextensions()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass_noextentions()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.Instance.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass_noextensions()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.Instance.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void FillObject()
        {
            NoExt ne = new NoExt();
            ne.Name = "hello";
            ne.Address = "here";
            ne.Age = 10;
            ne.dic = new Dictionary<string, class1>();
            ne.dic.Add("hello", new class1("asda", "asdas", Guid.NewGuid()));
            ne.objs = new baseclass[] { new class1("a", "1", Guid.NewGuid()), new class2("b", "2", "desc") };

            byte[] str = fastBinaryJSON.BJSON.Instance.ToBJSON(ne, new fastBinaryJSON.BJSONParameters { UseExtensions = false, UsingGlobalTypes = false });
            object dic = fastBinaryJSON.BJSON.Instance.Parse(str);
            object oo = fastBinaryJSON.BJSON.Instance.ToObject<NoExt>(str);

            NoExt nee = new NoExt();
            nee.intern = new NoExt { Name = "aaa" };
            fastBinaryJSON.BJSON.Instance.FillObject(nee, str);
        }

        [Test]
        public static void AnonymousTypes()
        {
            Console.WriteLine(".net version = " + Environment.Version);
            var q = new { Name = "asassa", Address = "asadasd", Age = 12 };
            byte[] sq = fastBinaryJSON.BJSON.Instance.ToBJSON(q, new fastBinaryJSON.BJSONParameters { EnableAnonymousTypes = true });
        }

        [Test]
        public static void Speed_Test_Deserialize()
        {
            Console.Write("fastbinaryjson deserialize");
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                colclass deserializedStore;
                byte[] jsonText = fastBinaryJSON.BJSON.Instance.ToBJSON(c);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)fastBinaryJSON.BJSON.Instance.ToObject(jsonText);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void Speed_Test_Serialize()
        {
            Console.Write("fastbinaryjson serialize");
            //fastBinaryJSON.BJSON.Instance.Parameters.UsingGlobalTypes = false;
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                byte[] jsonText = null;
                for (int i = 0; i < count; i++)
                {
                    jsonText = fastBinaryJSON.BJSON.Instance.ToBJSON(c);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void List_NestedRetClass()
        {
            List<RetNestedclass> r = new List<RetNestedclass>();
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now } });
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now } });
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<List<RetNestedclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void NullTest()
        {
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(null);
            Assert.AreEqual(s[0], fastBinaryJSON.TOKENS.NULL);
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);
            Assert.AreEqual(null, o);
        }

        [Test]
        public static void ZeroArray()
        {
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(new object[] { });
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);
            var a = o as object[];
            Assert.AreEqual(0, a.Length);
        }

        [Test]
        public static void GermanNumbers()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de");
            decimal d = 3.141592654M;
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(d);
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);
            Assert.AreEqual(d, (decimal)o);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        }
		
	    public class arrayclass
        {
            public int[] ints { get; set; }
            public string[] strs;
        }
        [Test]
        public static void ArrayTest()
        {
            arrayclass a = new arrayclass();
            a.ints = new int[] { 3, 1, 4 };
            a.strs = new string[] {"a","b","c"};
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(a);
            var o = fastBinaryJSON.BJSON.Instance.ToObject(s);
        }
		
		[Test]
        public static void Datasets()
        {
            var ds = CreateDataset();

            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(ds);

            var o = fastBinaryJSON.BJSON.Instance.ToObject<DataSet>(s);

            Assert.AreEqual(typeof(DataSet), o.GetType());
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Tables.Count);


            s = fastBinaryJSON.BJSON.Instance.ToBJSON(ds.Tables[0]);
            var oo = fastBinaryJSON.BJSON.Instance.ToObject<DataTable>(s);
            Assert.IsNotNull(oo);
            Assert.AreEqual(typeof(DataTable), oo.GetType());
            Assert.AreEqual(100, oo.Rows.Count);
        }

        [Test]
        public static void DynamicTest()
        {
            var obj = new { Name = "aaaaaa", Age = 10, dob = DateTime.Parse("2000-01-01 00:00:00"), inner = new { prop = 30 } };

            byte[] b = fastBinaryJSON.BJSON.Instance.ToBJSON(
                obj,
                new fastBinaryJSON.BJSONParameters { UseExtensions = false, EnableAnonymousTypes= true });
            dynamic d = fastBinaryJSON.BJSON.Instance.ToDynamic(b);
            var ss = d.Name;
            var oo = d.Age;
            var dob = d.dob;
            var inp = d.inner.prop;

            Assert.AreEqual("aaaaaa", ss);
            Assert.AreEqual(10, oo);
            Assert.AreEqual(30, inp);
            Assert.AreEqual(DateTime.Parse("2000-01-01 00:00:00"), dob);
        }

        public class diclist
        {
            public Dictionary<string, List<string>> d;
        }

        [Test]
        public static void DictionaryWithListValue()
        {
            diclist dd = new diclist();
            dd.d = new Dictionary<string, List<string>>();
            dd.d.Add("a", new List<string> { "1", "2", "3" });
            dd.d.Add("b", new List<string> { "4", "5", "7" });
            byte[] s = BJSON.Instance.ToBJSON(dd, new BJSONParameters { UseExtensions = false });
            var o = BJSON.Instance.ToObject<diclist>(s);
            Assert.AreEqual(3, o.d["a"].Count);

            s = BJSON.Instance.ToBJSON(dd.d, new BJSONParameters { UseExtensions = false });
            var oo = BJSON.Instance.ToObject<Dictionary<string, List<string>>>(s);
            Assert.AreEqual(3, oo["a"].Count);
            var ooo = BJSON.Instance.ToObject<Dictionary<string, string[]>>(s);
            Assert.AreEqual(3, ooo["b"].Length);
        }

        [Test]
        public static void HashtableTest()
        {
            Hashtable h = new Hashtable();
            h.Add(1, "dsjfhksa");
            h.Add("dsds", new class1());

            var s = BJSON.Instance.ToBJSON(h);

            var o = BJSON.Instance.ToObject<Hashtable>(s);
            Assert.AreEqual(typeof(Hashtable), o.GetType());
            Assert.AreEqual(typeof(class1), o["dsds"].GetType());
        }

        public class coltest
        {
            public string name;
            public NameValueCollection nv;
            public StringDictionary sd;
        }

        [Test]
        public static void SpecialCollections()
        {
            var nv = new NameValueCollection();
            nv.Add("1", "a");
            nv.Add("2", "b");
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(nv);
            var oo = fastBinaryJSON.BJSON.Instance.ToObject<NameValueCollection>(s);
            Assert.AreEqual("a", oo["1"]);
            var sd = new StringDictionary();
            sd.Add("1", "a");
            sd.Add("2", "b");
            s = fastBinaryJSON.BJSON.Instance.ToBJSON(sd);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<StringDictionary>(s);
            Assert.AreEqual("b", o["2"]);

            coltest c = new coltest();
            c.name = "aaa";
            c.nv = nv;
            c.sd = sd;
            s = fastBinaryJSON.BJSON.Instance.ToBJSON(c);
            var ooo = fastBinaryJSON.BJSON.Instance.ToObject(s);
            Assert.AreEqual("a", (ooo as coltest).nv["1"]);
            Assert.AreEqual("b", (ooo as coltest).sd["2"]);
        }

        public enum enumt
        {
            A = 65,
            B = 90,
            C = 100
        }
        public class constch
        {
            public enumt e = enumt.B;
            public string Name = "aa";
            public const int age = 11;
        }

        [Test]
        public static void consttest()
        {
            var s = BJSON.Instance.ToBJSON(new constch());
            var o = BJSON.Instance.ToObject(s);
        }

        public class ignoreatt : Attribute
        {
        }

        public class ignore
        {
            public string Name { get; set; }
            [System.Xml.Serialization.XmlIgnore]
            public int Age1 { get; set; }
            [ignoreatt]
            public int Age2;
        }
        public class ignore1 : ignore
        {
        }

        [Test]
        public static void IgnoreAttributes()
        {
            var i = new ignore { Age1 = 10, Age2 = 20, Name = "aa" };
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(i);
            var o = fastBinaryJSON.BJSON.Instance.ToObject<ignore>(s);
            Assert.AreEqual(0,o.Age1);
            i = new ignore1 { Age1 = 10, Age2 = 20, Name = "bb" };
            var j = new BJSONParameters();
            j.IgnoreAttributes.Add(typeof(ignoreatt));
            s = fastBinaryJSON.BJSON.Instance.ToBJSON(i, j);
            var oo = fastBinaryJSON.BJSON.Instance.ToObject<ignore1>(s);
            Assert.AreEqual(0, oo.Age1);
            Assert.AreEqual(0, oo.Age2);
        }

        public class nondefaultctor
        {
            public nondefaultctor(int a)
            { age = a; }
            public int age;
        }

        [Test]
        public static void NonDefaultConstructor()
        {
            var o = new nondefaultctor(10);
            var s = fastBinaryJSON.BJSON.Instance.ToBJSON(o);
            Console.WriteLine(s);
            var obj = fastBinaryJSON.BJSON.Instance.ToObject<nondefaultctor>(s);
            Assert.AreEqual(10, obj.age);
            List<nondefaultctor> l = new List<nondefaultctor> { o, o, o };
            s = fastBinaryJSON.BJSON.Instance.ToBJSON(l);
            var obj2 = fastBinaryJSON.BJSON.Instance.ToObject<List<nondefaultctor>>(s);
            Assert.AreEqual(3, obj2.Count);
            Assert.AreEqual(10, obj2[1].age);
        }
    }
}
