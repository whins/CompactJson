namespace CompactJson
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	public class JsonDeserializer
	{
		private static readonly Type _IListType = typeof(IList);
		private readonly JsonReader _reader;
		private readonly string _fieldPrefix;

		public JsonDeserializer(JsonReader reader) : this(reader, string.Empty) { }
		public JsonDeserializer(JsonReader reader, string fieldPrefix)
		{
			_reader = reader;
			_fieldPrefix = fieldPrefix;
		}

		public static T Deserialize<T>(JsonReader reader)
		{
			return Deserialize<T>(reader, string.Empty);
		}
		public static T Deserialize<T>(JsonReader reader, string fieldPrefix)
		{
			reader.IsScalarValue = JsonDeserializer.IsScalarValue(typeof(T));
			return (T)new JsonDeserializer(reader, fieldPrefix).DeserializeValue(typeof(T));
		}
		public static object Deserialize(JsonReader reader, Type defaultType)
		{
			return new JsonDeserializer(reader, string.Empty).DeserializeValue(defaultType);
		}

		private static bool IsScalarValue(Type type)
		{
			if (type != typeof(int) && type != typeof(int) && (type != typeof(Decimal) && type != typeof(long)) && (type != typeof(short) && type != typeof(float) && type != typeof(double)))
			{
				return type == typeof(bool);
			}
			return true;
		}
		private object DeserializeValue(Type type)
		{
			_reader.SkipWhiteSpaces();
			if (type == typeof(int))
			{
				return _reader.ReadInt32();
			}
			if (type == typeof(string))
			{
				return _reader.ReadString();
			}
			if (type == typeof(decimal))
			{
				return _reader.ReadDecimal();
			}
			if (type == typeof(double))
			{
				return _reader.ReadDouble();
			}
			if (type == typeof(DateTime))
			{
				return _reader.ReadDateTime();
			}
			if (_IListType.IsAssignableFrom(type))
			{
				return DeserializeList(type);
			}
			if (type == typeof(char))
			{
				return _reader.ReadChar();
			}
			if (type.IsEnum)
			{
				return _reader.ReadEnum();
			}
			if (type == typeof(long))
			{
				return _reader.ReadInt64();
			}
			if (type == typeof(float))
			{
				return _reader.ReadFloat();
			}
			if (type == typeof(short))
			{
				return _reader.ReadInt16();
			}
			return ParseObject(type);
		}
		private object DeserializeList(Type listType)
		{
			_reader.SkipWhiteSpaces();
			bool isNull = _reader.AssertAndConsume(JsonTokens.StartArrayCharacter);
			if (isNull)
			{
				_reader.SkipWhiteSpaces();
				return null;
			}
			Type itemType = ListHelper.GetListItemType(listType);
			bool isReadonly;

			IList container = ListHelper.CreateContainer(listType, itemType, out isReadonly);

			do
			{
				_reader.SkipWhiteSpaces();
				container.Add(DeserializeValue(itemType));
				_reader.SkipWhiteSpaces();
			}
			while (!_reader.AssertNextIsDelimiterOrSeparator(JsonTokens.EndArrayCharacter));

			if (listType.IsArray)
			{
				return ListHelper.ToArray((List<object>)container, itemType);
			}
			if (!isReadonly)
			{
				return container;
			}

			return listType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[] { container.GetType() }, null).Invoke(new object[] { container });

		}
		private object ParseObject(Type type)
		{
			bool isNull = _reader.AssertAndConsume(JsonTokens.StartObjectLiteralCharacter);
			if (isNull)
			{
				_reader.SkipWhiteSpaces();
				return null;
			}
			//ConstructorInfo constructor = ReflectionHelper.GetDefaultConstructor(type);
			//object instance = constructor.Invoke(null);
			object instance = ReflectionHelper.GetDefaultConstructor(type).Invoke(null);
			do
			{
				_reader.SkipWhiteSpaces();
				string name = _reader.ReadString();
				if (!name.StartsWith(_fieldPrefix))
				{
					name = _fieldPrefix + name;
				}
				FieldInfo field = ReflectionHelper.FindField(type, name);
				_reader.SkipWhiteSpaces();
				_reader.AssertAndConsume(JsonTokens.PairSeparator);
				_reader.SkipWhiteSpaces();
				field.SetValue(instance, DeserializeValue(field.FieldType));
				_reader.SkipWhiteSpaces();
			}
			while (!_reader.AssertNextIsDelimiterOrSeparator(JsonTokens.EndObjectLiteralCharacter));

			return instance;
		}
	}
}