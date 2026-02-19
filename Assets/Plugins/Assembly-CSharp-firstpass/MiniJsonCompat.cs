using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// Self-contained MiniJSON implementation placed in the Plugins firstpass
// assembly so other firstpass scripts can reference it without an
// assembly-order dependency. This provides jsonDecode/jsonEncode and
// the string extension helpers used by the project. The implementation
// is intentionally minimal but sufficient for the usages in this repo.
public static class MiniJSON
{
    public static object jsonDecode(string json)
    {
        if (json == null) return null;
        var parser = new MiniJsonParser(json);
        return parser.ParseValue();
    }

    public static string jsonEncode(object obj)
    {
        var sb = new StringBuilder();
        var serializer = new MiniJsonSerializer(sb);
        serializer.SerializeValue(obj);
        return sb.ToString();
    }

    // Simple parser that returns Hashtable / ArrayList / string / long / double / bool / null
    private sealed class MiniJsonParser
    {
        private readonly string json;
        private int index;

        public MiniJsonParser(string json)
        {
            this.json = json;
            index = 0;
        }

        public object ParseValue()
        {
            EatWhitespace();
            if (index >= json.Length) return null;
            char c = json[index];
            if (c == '"') return ParseString();
            if (c == '{') return ParseObject();
            if (c == '[') return ParseArray();
            if (char.IsDigit(c) || c == '-') return ParseNumber();
            if (StartsWith("true")) { index += 4; return true; }
            if (StartsWith("false")) { index += 5; return false; }
            if (StartsWith("null")) { index += 4; return null; }
            return null;
        }

        private Hashtable ParseObject()
        {
            var table = new Hashtable();
            index++; // '{'
            while (true)
            {
                EatWhitespace();
                if (index >= json.Length) break;
                if (json[index] == '}') { index++; break; }
                var key = ParseString();
                EatWhitespace();
                if (index < json.Length && json[index] == ':') index++;
                var value = ParseValue();
                table[key] = value;
                EatWhitespace();
                if (index < json.Length && json[index] == ',') { index++; continue; }
            }
            return table;
        }

        private ArrayList ParseArray()
        {
            var array = new ArrayList();
            index++; // '['
            while (true)
            {
                EatWhitespace();
                if (index >= json.Length) break;
                if (json[index] == ']') { index++; break; }
                var value = ParseValue();
                array.Add(value);
                EatWhitespace();
                if (index < json.Length && json[index] == ',') { index++; continue; }
            }
            return array;
        }

        private string ParseString()
        {
            var sb = new StringBuilder();
            index++; // skip '"'
            while (index < json.Length)
            {
                char c = json[index++];
                if (c == '"') break;
                if (c == '\\' && index < json.Length)
                {
                    char esc = json[index++];
                    switch (esc)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (index + 4 <= json.Length)
                            {
                                string hex = json.Substring(index, 4);
                                sb.Append((char)Convert.ToInt32(hex, 16));
                                index += 4;
                            }
                            break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private object ParseNumber()
        {
            int start = index;
            if (json[index] == '-') index++;
            while (index < json.Length && char.IsDigit(json[index])) index++;
            if (index < json.Length && json[index] == '.')
            {
                index++;
                while (index < json.Length && char.IsDigit(json[index])) index++;
            }
            string number = json.Substring(start, index - start);
            if (number.IndexOf('.') != -1)
            {
                if (double.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d)) return d;
            }
            else
            {
                if (long.TryParse(number, out var l)) return l;
            }
            return null;
        }

        private void EatWhitespace()
        {
            while (index < json.Length && char.IsWhiteSpace(json[index])) index++;
        }

        private bool StartsWith(string s)
        {
            if (index + s.Length > json.Length) return false;
            for (int i = 0; i < s.Length; i++) if (json[index + i] != s[i]) return false;
            return true;
        }
    }

    private sealed class MiniJsonSerializer
    {
        private readonly StringBuilder builder;
        public MiniJsonSerializer(StringBuilder sb) { builder = sb; }

        public void SerializeValue(object value)
        {
            if (value == null) { builder.Append("null"); return; }
            if (value is string) { SerializeString((string)value); return; }
            if (value is bool) { builder.Append((bool)value ? "true" : "false"); return; }
            if (value is Hashtable) { SerializeObject((Hashtable)value); return; }
            if (value is IDictionary)
            {
                var table = new Hashtable();
                foreach (DictionaryEntry de in (IDictionary)value) table[de.Key] = de.Value;
                SerializeObject(table);
                return;
            }
            if (value is ArrayList) { SerializeArray((ArrayList)value); return; }
            if (value is IList)
            {
                var list = new ArrayList();
                foreach (var v in (IList)value) list.Add(v);
                SerializeArray(list);
                return;
            }
            if (value is double || value is float || value is int || value is long || value is decimal)
            {
                builder.Append(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                return;
            }
            SerializeString(value.ToString());
        }

        private void SerializeObject(Hashtable obj)
        {
            builder.Append('{');
            bool first = true;
            foreach (DictionaryEntry e in obj)
            {
                if (!first) builder.Append(',');
                SerializeString(e.Key.ToString());
                builder.Append(':');
                SerializeValue(e.Value);
                first = false;
            }
            builder.Append('}');
        }

        private void SerializeArray(ArrayList anArray)
        {
            builder.Append('[');
            bool first = true;
            foreach (object obj in anArray)
            {
                if (!first) builder.Append(',');
                SerializeValue(obj);
                first = false;
            }
            builder.Append(']');
        }

        private void SerializeString(string str)
        {
            builder.Append('"');
            foreach (char c in str)
            {
                switch (c)
                {
                    case '\\': builder.Append("\\\\"); break;
                    case '"': builder.Append("\\\""); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    default: builder.Append(c); break;
                }
            }
            builder.Append('"');
        }
    }
}

public static class MiniJsonExtensions
{
    public static ArrayList arrayListFromJson(this string json)
    {
        return MiniJSON.jsonDecode(json) as ArrayList;
    }

    public static Hashtable hashtableFromJson(this string json)
    {
        return MiniJSON.jsonDecode(json) as Hashtable;
    }
}
