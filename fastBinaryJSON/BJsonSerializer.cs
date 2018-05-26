﻿using System;
using System.Collections;
using System.Collections.Generic;
#if !SILVERLIGHT
using System.Data;
#endif
using System.IO;
using System.Collections.Specialized;

namespace fastBinaryJSON
{
    internal sealed class BJSONSerializer : IDisposable
    {
        private MemoryStream _output = new MemoryStream();
        //private MemoryStream _before = new MemoryStream();
        private int _typespointer = 0;
        private int _MAX_DEPTH = 20;
        int _current_depth = 0;
        private Dictionary<string, int> _globalTypes = new Dictionary<string, int>();
        private Dictionary<object, int> _cirobj = new Dictionary<object, int>();
        private BJSONParameters _params;

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                _output.Close();
                //_before.Close();
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal BJSONSerializer(BJSONParameters param)
        {
            _params = param;
            _MAX_DEPTH = param.SerializerMaxDepth;
        }

        internal byte[] ConvertToBJSON(object obj)
        {
            WriteValue(obj);

            // add $types
            if (_params.UsingGlobalTypes && _globalTypes != null && _globalTypes.Count > 0)
            {
                var pointer = (int)_output.Length;
                WriteName("$types");
                WriteColon();
                WriteTypes(_globalTypes);
                //var i = _output.Length;
                _output.Seek(_typespointer, SeekOrigin.Begin);
                _output.Write(Helper.GetBytes(pointer, false), 0, 4);

                return _output.ToArray();
            }

            return _output.ToArray();
        }

        private void WriteTypes(Dictionary<string, int> dic)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (var entry in dic)
            {
                if (pendingSeparator) WriteComma();

                WritePair(entry.Value.ToString(), entry.Key);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteValue(object obj)
        {
            if (obj == null || obj is DBNull)
                WriteNull();

            else if (obj is string)
                WriteString((string)obj);

            else if (obj is char)
                WriteChar((char)obj);

            else if (obj is Guid)
                WriteGuid((Guid)obj);

            else if (obj is bool)
                WriteBool((bool)obj);

            else if (obj is int)
                WriteInt((int)obj);

            else if (obj is uint)
                WriteUInt((uint)obj);

            else if (obj is long)
                WriteLong((long)obj);

            else if (obj is ulong)
                WriteULong((ulong)obj);

            else if (obj is decimal)
                WriteDecimal((decimal)obj);

            else if (obj is byte)
                WriteByte((byte)obj);

            else if (obj is double)
                WriteDouble((double)obj);

            else if (obj is float)
                WriteFloat((float)obj);

            else if (obj is short)
                WriteShort((short)obj);

            else if (obj is ushort)
                WriteUShort((ushort)obj);

            else if (obj is DateTime)
                WriteDateTime((DateTime)obj);

            else if (obj is TimeSpan)
                WriteTimeSpan((TimeSpan)obj);
#if NET4
            else if (obj is System.Dynamic.ExpandoObject)
                WriteStringDictionary((IDictionary<string, object>)obj);
#endif

            else if (obj is IDictionary && obj.GetType().IsGenericType && obj.GetType().GetGenericArguments()[0] == typeof(string))
                WriteStringDictionary((IDictionary)obj);

            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);
#if !SILVERLIGHT
            else if (obj is DataSet)
                WriteDataset((DataSet)obj);

            else if (obj is DataTable)
                this.WriteDataTable((DataTable)obj);
#endif
            else if (obj is byte[])
                WriteBytes((byte[])obj);

            else if (obj is StringDictionary)
                WriteSD((StringDictionary)obj);

            else if (obj is NameValueCollection)
                WriteNV((NameValueCollection)obj);

            else if (_params.UseTypedArrays && obj is Array)
                WriteTypedArray((ICollection)obj);

            else if (obj is IEnumerable)
                WriteArray((IEnumerable)obj);

            else if (obj is Enum)
                WriteEnum((Enum)obj);

            else if (Reflection.Instance.IsTypeRegistered(obj.GetType()))
                WriteCustom(obj);

            //else if (obj is Exception)
            //    WriteException((Exception)obj);

            else
                WriteObject(obj);
        }

        private void WriteTimeSpan(TimeSpan obj)
        {
            _output.WriteByte(TOKENS.TIMESPAN);
            byte[] b = Helper.GetBytes(obj.Ticks, false);
            _output.Write(b, 0, b.Length);
        }

        private void WriteException(Exception obj)
        {

        }

        private void WriteTypedArray(ICollection array)
        {
            bool pendingSeperator = false;
            bool token = true;
            var t = array.GetType();
            if (t != null) // non generic array
            {
                if (t.GetElementType().IsClass)
                {
                    token = false;
                    // array type name
                    byte[] b = Reflection.Instance.utf8.GetBytes(Reflection.Instance.GetTypeAssemblyName(t.GetElementType()));
                    if (b.Length < 256)
                    {
                        _output.WriteByte(TOKENS.ARRAY_TYPED);
                        _output.WriteByte((byte)b.Length);
                        _output.Write(b, 0, b.Length);
                    }
                    else
                    {
                        _output.WriteByte(TOKENS.ARRAY_TYPED_LONG);
                        _output.Write(Helper.GetBytes(b.Length, false), 0, 2);
                        _output.Write(b, 0, b.Length);
                    }
                    // array count
                    _output.Write(Helper.GetBytes(array.Count, false), 0, 4); //count
                }
            }
            if (token)
                _output.WriteByte(TOKENS.ARRAY_START);

            foreach (object obj in array)
            {
                if (pendingSeperator) WriteComma();

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.WriteByte(TOKENS.ARRAY_END);
        }

        private void WriteNV(NameValueCollection nameValueCollection)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (string key in nameValueCollection)
            {
                if (pendingSeparator) _output.WriteByte(TOKENS.COMMA);

                WritePair(key, nameValueCollection[key]);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteSD(StringDictionary stringDictionary)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in stringDictionary)
            {
                if (pendingSeparator) _output.WriteByte(TOKENS.COMMA);

                WritePair((string)entry.Key, entry.Value);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteUShort(ushort p)
        {
            _output.WriteByte(TOKENS.USHORT);
            _output.Write(Helper.GetBytes(p, false), 0, 2);
        }

        private void WriteShort(short p)
        {
            _output.WriteByte(TOKENS.SHORT);
            _output.Write(Helper.GetBytes(p, false), 0, 2);
        }

        private void WriteFloat(float p)
        {
            _output.WriteByte(TOKENS.FLOAT);
            byte[] b = BitConverter.GetBytes(p);
            _output.Write(b, 0, b.Length);
        }

        private void WriteDouble(double p)
        {
            _output.WriteByte(TOKENS.DOUBLE);
            var b = BitConverter.GetBytes(p);
            _output.Write(b, 0, b.Length);
        }

        private void WriteByte(byte p)
        {
            _output.WriteByte(TOKENS.BYTE);
            _output.WriteByte(p);
        }

        private void WriteDecimal(decimal p)
        {
            _output.WriteByte(TOKENS.DECIMAL);
            var b = decimal.GetBits(p);
            foreach (var c in b)
                _output.Write(Helper.GetBytes(c, false), 0, 4);
        }

        private void WriteULong(ulong p)
        {
            _output.WriteByte(TOKENS.ULONG);
            _output.Write(Helper.GetBytes((long)p, false), 0, 8);
        }

        private void WriteUInt(uint p)
        {
            _output.WriteByte(TOKENS.UINT);
            _output.Write(Helper.GetBytes(p, false), 0, 4);
        }

        private void WriteLong(long p)
        {
            _output.WriteByte(TOKENS.LONG);
            _output.Write(Helper.GetBytes(p, false), 0, 8);
        }

        private void WriteChar(char p)
        {
            _output.WriteByte(TOKENS.CHAR);
            _output.Write(Helper.GetBytes((short)p, false), 0, 2);
        }

        private void WriteBytes(byte[] p)
        {
            _output.WriteByte(TOKENS.BYTEARRAY);
            _output.Write(Helper.GetBytes(p.Length, false), 0, 4);
            _output.Write(p, 0, p.Length);
        }

        private void WriteBool(bool p)
        {
            if (p)
                _output.WriteByte(TOKENS.TRUE);
            else
                _output.WriteByte(TOKENS.FALSE);
        }

        private void WriteNull()
        {
            _output.WriteByte(TOKENS.NULL);
        }


        private void WriteCustom(object obj)
        {
            Serialize s;
            Reflection.Instance._customSerializer.TryGetValue(obj.GetType(), out s);
            WriteString(s(obj));
        }

        private void WriteColon()
        {
            _output.WriteByte(TOKENS.COLON);
        }

        private void WriteComma()
        {
            _output.WriteByte(TOKENS.COMMA);
        }

        private void WriteEnum(Enum e)
        {
            WriteString(e.ToString());
        }

        private void WriteInt(int i)
        {
            _output.WriteByte(TOKENS.INT);
            _output.Write(Helper.GetBytes(i, false), 0, 4);
        }

        private void WriteGuid(Guid g)
        {
            _output.WriteByte(TOKENS.GUID);
            _output.Write(g.ToByteArray(), 0, 16);
        }

        private void WriteDateTime(DateTime dateTime)
        {
            DateTime dt = dateTime;
            if (_params.UseUTCDateTime)
                dt = dateTime.ToUniversalTime();

            _output.WriteByte(TOKENS.DATETIME);
            byte[] b = Helper.GetBytes(dt.Ticks, false);
            _output.Write(b, 0, b.Length);
        }

#if !SILVERLIGHT
        private DatasetSchema GetSchema(DataTable ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.TableName;

            foreach (DataColumn c in ds.Columns)
            {
                m.Info.Add(ds.TableName);
                m.Info.Add(c.ColumnName);
                m.Info.Add(c.DataType.ToString());
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private DatasetSchema GetSchema(DataSet ds)
        {
            if (ds == null) return null;

            DatasetSchema m = new DatasetSchema();
            m.Info = new List<string>();
            m.Name = ds.DataSetName;

            foreach (DataTable t in ds.Tables)
            {
                foreach (DataColumn c in t.Columns)
                {
                    m.Info.Add(t.TableName);
                    m.Info.Add(c.ColumnName);
                    m.Info.Add(c.DataType.ToString());
                }
            }
            // FEATURE : serialize relations and constraints here

            return m;
        }

        private string GetXmlSchema(DataTable dt)
        {
            using (var writer = new StringWriter())
            {
                dt.WriteXmlSchema(writer);
                return dt.ToString();
            }
        }

        private void WriteDataset(DataSet ds)
        {
            _output.WriteByte(TOKENS.DOC_START);
            {
                WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object)GetSchema(ds) : ds.GetXmlSchema());
                WriteComma();
            }
            bool tablesep = false;
            foreach (DataTable table in ds.Tables)
            {
                if (tablesep) WriteComma();
                tablesep = true;
                WriteDataTableData(table);
            }
            // end dataset
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteDataTableData(DataTable table)
        {
            WriteName(table.TableName);
            WriteColon();
            _output.WriteByte(TOKENS.ARRAY_START);
            DataColumnCollection cols = table.Columns;
            bool rowseparator = false;
            foreach (DataRow row in table.Rows)
            {
                if (rowseparator) WriteComma();
                rowseparator = true;
                _output.WriteByte(TOKENS.ARRAY_START);

                bool pendingSeperator = false;
                foreach (DataColumn column in cols)
                {
                    if (pendingSeperator) WriteComma();
                    WriteValue(row[column]);
                    pendingSeperator = true;
                }
                _output.WriteByte(TOKENS.ARRAY_END);
            }

            _output.WriteByte(TOKENS.ARRAY_END);
        }

        void WriteDataTable(DataTable dt)
        {
            _output.WriteByte(TOKENS.DOC_START);
            //if (this.useExtension)
            {
                this.WritePair("$schema", _params.UseOptimizedDatasetSchema ? (object)this.GetSchema(dt) : this.GetXmlSchema(dt));
                WriteComma();
            }

            WriteDataTableData(dt);

            // end datatable
            _output.WriteByte(TOKENS.DOC_END);
        }
#endif
        bool _TypesWritten = false;

        private void WriteObject(object obj)
        {
            int i = 0;
            if (_cirobj.TryGetValue(obj, out i) == false)
                _cirobj.Add(obj, _cirobj.Count + 1);
            else
            {
                if (_current_depth > 0)
                {
                    //_circular = true;
                    _output.WriteByte(TOKENS.DOC_START);
                    WriteName("$i");
                    WriteColon();
                    WriteValue(i);
                    _output.WriteByte(TOKENS.DOC_END);
                    return;
                }
            }
            if (_params.UsingGlobalTypes == false)
                _output.WriteByte(TOKENS.DOC_START);
            else
            {
                if (_TypesWritten == false)
                {
                    _output.WriteByte(TOKENS.DOC_START);
                    // write pointer to $types position
                    _output.WriteByte(TOKENS.TYPES_POINTER);
                    _typespointer = (int)_output.Length; // place holder
                    _output.Write(new byte[4], 0, 4); // zero pointer for now
                                                      //_output = new MemoryStream();
                    _TypesWritten = true;
                }
                else
                    _output.WriteByte(TOKENS.DOC_START);

            }
            _current_depth++;
            if (_current_depth > _MAX_DEPTH)
                throw new Exception("Serializer encountered maximum depth of " + _MAX_DEPTH);

            Type t = obj.GetType();
            bool append = false;
            if (_params.UseExtensions)
            {
                if (_params.UsingGlobalTypes == false)
                    WritePairFast("$type", Reflection.Instance.GetTypeAssemblyName(t));
                else
                {
                    int dt = 0;
                    string ct = Reflection.Instance.GetTypeAssemblyName(t);
                    if (_globalTypes.TryGetValue(ct, out dt) == false)
                    {
                        dt = _globalTypes.Count + 1;
                        _globalTypes.Add(ct, dt);
                    }
                    WritePairFast("$type", dt.ToString());
                }
                append = true;
            }

            Getters[] g = Reflection.Instance.GetGetters(t, _params.ShowReadOnlyProperties, _params.IgnoreAttributes);
            int c = g.Length;
            for (int ii = 0; ii < c; ii++)
            {
                var p = g[ii];
                var o = p.Getter(obj);
                if (_params.SerializeNulls == false && (o == null || o is DBNull))
                {

                }
                else
                {
                    if (append)
                        WriteComma();
                    WritePair(p.Name, o);
                    append = true;
                }
            }
            _output.WriteByte(TOKENS.DOC_END);
            _current_depth--;
        }

        private void WritePairFast(string name, string value)
        {
            if (_params.SerializeNulls == false && (value == null))
                return;
            WriteName(name);

            WriteColon();

            WriteString(value);
        }

        private void WritePair(string name, object value)
        {
            if (_params.SerializeNulls == false && (value == null || value is DBNull))
                return;
            WriteName(name);

            WriteColon();

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.WriteByte(TOKENS.ARRAY_START);

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator) WriteComma();

                WriteValue(obj);

                pendingSeperator = true;
            }
            _output.WriteByte(TOKENS.ARRAY_END);
        }

        private void WriteStringDictionary(IDictionary dic)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) WriteComma();

                WritePair((string)entry.Key, entry.Value);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteStringDictionary(IDictionary<string, object> dic)
        {
            _output.WriteByte(TOKENS.DOC_START);

            bool pendingSeparator = false;

            foreach (KeyValuePair<string, object> entry in dic)
            {
                if (pendingSeparator) WriteComma();

                WritePair((string)entry.Key, entry.Value);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.DOC_END);
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.WriteByte(TOKENS.ARRAY_START);

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator) WriteComma();
                _output.WriteByte(TOKENS.DOC_START);
                WritePair("k", entry.Key);
                WriteComma();
                WritePair("v", entry.Value);
                _output.WriteByte(TOKENS.DOC_END);

                pendingSeparator = true;
            }
            _output.WriteByte(TOKENS.ARRAY_END);
        }

        private void WriteName(string s)
        {
            _output.WriteByte(TOKENS.NAME);
            byte[] b = Reflection.Instance.utf8.GetBytes(s);
            _output.WriteByte((byte)b.Length);
            _output.Write(b, 0, b.Length % 256);
        }

        private void WriteString(string s)
        {
            byte[] b = null;
            if (_params.UseUnicodeStrings)
            {
                _output.WriteByte(TOKENS.UNICODE_STRING);
                b = Reflection.Instance.unicode.GetBytes(s);
            }
            else
            {
                _output.WriteByte(TOKENS.STRING);
                b = Reflection.Instance.utf8.GetBytes(s);
            }
            _output.Write(Helper.GetBytes(b.Length, false), 0, 4);
            _output.Write(b, 0, b.Length);
        }
    }
}
