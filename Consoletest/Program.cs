﻿using fastBinaryJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace consoletest
{
    public
        struct
        //class
        Return 
    {
        public object ReturnEntity { get; set; }
        public string Name { get; set; }
        //public TimeSpan time { get; set;  }
        public string Field1;
        public int Field2;
        public string ppp { get { return "sdfas df "; } }
        public DateTime date { get; set; }
        public DataTable ds { get; set; }
        public bool isValid { get; set; }
    }

    public class returns : List<Return>
    {
        public string name { get; set; }
    }

    public class NoExt
    {
    	[System.Xml.Serialization.XmlIgnore()]
    	public string Name { get; set; }
    	public string Address { get; set;}
    	public int Age { get; set;}
        public DateTime Date { get; set; }
    	public baseclass[] objs { get; set;}
        public Dictionary<string, class1> dic { get; set; }
    }
    
    public class Program
    {
        static int count = 1000;
        static int tcount = 5;
        static DataSet ds = new DataSet();
        static bool exotic = false;
        static bool dsser = false;

        public class test { }
        public static void Main(string[] args)
        {

            // Return r = new Return();
            // r.Name = "hello";
            // r.Field1 = "dsasdF";
            // r.Field2 = 2312;
            // r.date = DateTime.Now;
            // r.isValid = true;
            // r.ds = CreateDataset().Tables[0];

            // r.ReturnEntity = 
            //     new List<int>( new int[] { 1,2,3,4,5 });
            // //    // Guid.NewGuid();
            // //    //	CreateDataset();
            // //    new object[] { new Return() };
            // //r.time = new TimeSpan(1, 2, 3);
            // //r.point = new System.Drawing.Point(10, 10);

            // //rr.Add(r);

            // //fastJSON.JSON.RegisterCustomType(typeof(TimeSpan), tsser, tsdes);
            // //fastJSON.JSON.RegisterCustomType(typeof(System.Drawing.Point), pser, pdes);
            // //List<object> list = new List<object>();
            // //list.Add("hello there");
            // //list.Add(Guid.NewGuid());
            // //list.Add(true);
            // //list.Add(null);
            // //list.Add(DateTime.Now);
            //// var ooo= fastBinaryJSON.BJSON.ToBJSON(list);
            //// var ppp = fastBinaryJSON.BJSON.ToObject(ooo);
            // byte[] ts = fastBinaryJSON.BJSON.ToBJSON(r);
            // object dic2 = fastBinaryJSON.BJSON.Parse(ts);
            // object tsd = fastBinaryJSON.BJSON.ToObject(ts);

            // NoExt ne = new NoExt();
            // ne.Name = "hello";
            // ne.Address = "here";
            // ne.Age= 10;
            // ne.dic = new Dictionary<string, class1>();
            // ne.dic.Add("hello", new class1("asda","asdas",Guid.NewGuid()));
            // ne.objs = new baseclass[] { new class1("a","1",Guid.NewGuid()), new class2("b","2","desc") };

            // //fastJSON.JSON.UseSerializerExtension = false;
            // //fastJSON.JSON.UseFastGuid = false;
            // byte[] str = fastBinaryJSON.BJSON.ToBJSON(ne);
            // object dic = fastBinaryJSON.BJSON.Parse(str);
            // object oo = fastBinaryJSON.BJSON.ToObject<NoExt>(str);//<NoExt>(str);

            Console.WriteLine(".net version = " + Environment.Version);
            Console.WriteLine("press key : (E)xotic ");
            //if (Console.ReadKey().Key == ConsoleKey.E)
            //    exotic = true;

            ds = CreateDataset();
            Console.WriteLine("-dataset");
            dsser = false;
            //bin_serialize();
            fastjson_serialize();
            //bin_deserialize();
            //fastjson_deserialize();

            //dsser = true;
            //Console.WriteLine();
            //Console.WriteLine("+dataset");
            ////bin_serialize();
            //fastjson_serialize();
            ////bin_deserialize();
            //fastjson_deserialize();

            #region [ other tests]

            //			litjson_serialize();
            //			jsonnet_serialize();
            //			jsonnet4_serialize();
            //stack_serialize();

            //systemweb_deserialize();
            //bin_deserialize();
            //fastjson_deserialize();

            //			litjson_deserialize();
            //			jsonnet_deserialize();
            //			jsonnet4_deserialize();
            //			stack_deserialize();
            #endregion
        }

        private static string pser(object data)
        {
            System.Drawing.Point p = (System.Drawing.Point) data;
            return p.X.ToString() + "," + p.Y.ToString();
        }

        private static object pdes(string data)
        {
            string[] ss = data.Split(',');

            return new System.Drawing.Point(
                int.Parse(ss[0]),
                int.Parse(ss[1])
                );
        }

        private static string tsser(object data)
        {
            return ((TimeSpan)data).Ticks.ToString();
        }

        private static object tsdes(string data)
        {
            return new TimeSpan(long.Parse(data));
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

        public static DataSet CreateDataset()
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

        private static void fastjson_deserialize()
        {
            Console.WriteLine();
            Console.Write("fastbinaryjson deserialize");
            colclass c = CreateObject();
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                colclass deserializedStore;
                byte[] jsonText = null;

                jsonText = fastBinaryJSON.BJSON.ToBJSON(c);
                //Console.WriteLine(" size = " + jsonText.Length);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)fastBinaryJSON.BJSON.ToObject(jsonText);
                }
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
        }

        private static void fastjson_serialize()
        {
            Console.WriteLine();
            Console.Write("fastbinaryjson serialize");
            colclass c = CreateObject();
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                byte[] jsonText = null;
                for (int i = 0; i < count; i++)
                {
                    jsonText = fastBinaryJSON.BJSON.ToBJSON(c);
                }
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
        }

        private static void bin_deserialize()
        {
            Console.WriteLine();
            Console.Write("bin deserialize");
            colclass c = CreateObject();
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, c);
                colclass deserializedStore = null;
                //Console.WriteLine(" size = " +ms.Length);
                for (int i = 0; i < count; i++)
                {
                    ms.Seek(0L, SeekOrigin.Begin);
                    deserializedStore = (colclass)bf.Deserialize(ms);
                }
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
        }

        private static void bin_serialize()
        {
            Console.Write("\r\nbin serialize");
            colclass c = CreateObject();
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                for (int i = 0; i < count; i++)
                {
                    ms = new MemoryStream();
                    bf.Serialize(ms, c);
                }
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
        }




        #region [   other tests  ]
        /*
		private static void systemweb_serialize()
		{
			Console.WriteLine();
			Console.Write("msjson serialize");
			colclass c = CreateObject();
			var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
			for (int pp = 0; pp < tcount; pp++)
			{
				DateTime st = DateTime.Now;
				colclass deserializedStore = null;
				string jsonText = null;

				//jsonText =sws.Serialize(c);
				//Console.WriteLine(" size = " + jsonText.Length);
				for (int i = 0; i < count; i++)
				{
					jsonText =sws.Serialize(c);
					//deserializedStore = (colclass)sws.DeserializeObject(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

//		private static void stack_serialize()
//		{
//			Console.WriteLine();
//			Console.Write("stack serialize");
//			colclass c = CreateObject();
//			for (int pp = 0; pp < 5; pp++)
//			{
//				DateTime st = DateTime.Now;
//				string jsonText = null;
//
//				for (int i = 0; i < count; i++)
//				{
//					jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
//				}
//				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
//			}
//		}		

		private static void systemweb_deserialize()
//		{
//			Console.WriteLine();
//			Console.Write("fastjson deserialize");
//			colclass c = CreateObject();
//			var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
//			for (int pp = 0; pp < tcount; pp++)
//			{
//				DateTime st = DateTime.Now;
//				colclass deserializedStore = null;
//				string jsonText = null;
//
//				jsonText =sws.Serialize(c);
//				//Console.WriteLine(" size = " + jsonText.Length);
//				for (int i = 0; i < count; i++)
//				{
//					deserializedStore = (colclass)sws.DeserializeObject(jsonText);
//				}
//				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
//			}
//		}

		private static void jsonnet4_deserialize()
		{
			Console.WriteLine();
			Console.Write("json.net4 deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				var s = new Newtonsoft.Json.JsonSerializerSettings();
				s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
				jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet4_serialize()
		{
			Console.WriteLine();
			Console.Write("json.net4 serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c = Tests.mytests.CreateObject();
				Newtonsoft.Json.JsonSerializerSettings s = null;
				string jsonText = null;
				s = new Newtonsoft.Json.JsonSerializerSettings();
				s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;

				for (int i = 0; i < count; i++)
				{
					jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void stack_deserialize()
		{
			Console.WriteLine();
			Console.Write("stack deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = ServiceStack.Text.JsonSerializer.DeserializeFromString<colclass>(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet_deserialize()
		{
			Console.WriteLine();
			Console.Write("json.net deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				var s = new json.net.JsonSerializerSettings();
				s.TypeNameHandling = json.net.TypeNameHandling.All;
				jsonText = json.net.JsonConvert.SerializeObject(c, json.net.Formatting.Indented, s);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)json.net.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void jsonnet_serialize()
		{
			Console.WriteLine();
			Console.Write("json.net serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c = Tests.mytests.CreateObject();
				json.net.JsonSerializerSettings s = null;
				string jsonText = null;
				s = new json.net.JsonSerializerSettings();
				s.TypeNameHandling = json.net.TypeNameHandling.All;

				for (int i = 0; i < count; i++)
				{
					jsonText = json.net.JsonConvert.SerializeObject(c, json.net.Formatting.Indented, s);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void litjson_deserialize()
		{
			Console.WriteLine();
			Console.Write("litjson deserialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				colclass deserializedStore = null;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
				for (int i = 0; i < count; i++)
				{
					deserializedStore = (colclass)BizFX.Common.JSON.JsonMapper.ToObject(jsonText);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		private static void litjson_serialize()
		{
			Console.WriteLine();
			Console.Write("litjson serialize");
			for (int pp = 0; pp < 5; pp++)
			{
				DateTime st = DateTime.Now;
				colclass c;
				string jsonText = null;
				c = Tests.mytests.CreateObject();
				for (int i = 0; i < count; i++)
				{
					jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
				}
				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
			}
		}

		
		 */
        #endregion
    }
}