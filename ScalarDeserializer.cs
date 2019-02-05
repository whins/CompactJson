using System;
using System.Collections.Generic;
using System.Text;

namespace CompactJson
{
	public class ScalarDeserializer
	{
		public static bool IsScalar(Type type)
		{
			if (type != typeof(int) && type != typeof(int) && type != typeof(decimal) && type != typeof(long) && type != typeof(short) && type != typeof(float) && type != typeof(double))
			{
				return type == typeof(bool);
			}
			return true;
		}

		public static T Deserialize<T>(string data)
		{
			return IsScalar(typeof(T)) ? (T)Parse(data, typeof(T)) : default(T);
		}

		private static object Parse(string data, Type type)
		{
			try
			{
				if (type == typeof(int))
				{
					return int.Parse(data);
				}
				if (type == typeof(decimal))
				{
					return decimal.Parse(data);
				}
				if (type == typeof(double))
				{
					return double.Parse(data);
				}
				if (type == typeof(char))
				{
					return char.Parse(data);
				}
				if (type == typeof(long))
				{
					return long.Parse(data);
				}
				if (type == typeof(float))
				{
					return float.Parse(data);
				}
				if (type == typeof(short))
				{
					return short.Parse(data);
				}
				if (type == typeof(bool))
				{
					return bool.Parse(data);
				}				
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static object Deserialize(string data, Type type)
		{
			return IsScalar(type) ? Parse(data, type): null;
		}
	}
}
