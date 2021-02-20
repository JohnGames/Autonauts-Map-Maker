using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

//This crazy class destroys empty collections and empty strings for us.

public class SkipEmptyCollectionsContractResolver : DefaultContractResolver
{
	protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
	{
		var property = base.CreateProperty(member, memberSerialization);

		if (property._required.HasValue)
		{
			return property;
		}

		


		var isDefaultValueIgnored = ((property.DefaultValueHandling ??
		DefaultValueHandling.Ignore) & DefaultValueHandling.Ignore) != 0;
		if (!isDefaultValueIgnored
		|| !typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
		{
			return property;
		}

		//Now, is this a string?

		if(property.PropertyType == typeof(string))
		{
			Predicate<object> newShouldSerialize = obj =>
			{
				var stringValue = property.ValueProvider.GetValue(obj) as string;
				return stringValue == null || stringValue != "";
			};

			var oldShouldSerialize = property.ShouldSerialize;
			property.ShouldSerialize = oldShouldSerialize != null ? // If this isn't null
				o => oldShouldSerialize(o) || newShouldSerialize(o) : //See if the old and the new both return that it should serialize
				newShouldSerialize; // Else only use the new serialize

		} else
		{
			//Ok, all that is left are collections
			Predicate<object> newShouldSerialize = obj =>
			{
				var collection = property.ValueProvider.GetValue(obj) as ICollection;
				return collection == null || collection.Count != 0;
			};

			var oldShouldSerialize = property.ShouldSerialize;
			property.ShouldSerialize = oldShouldSerialize != null
			? o => oldShouldSerialize(o) && newShouldSerialize(o)
			: newShouldSerialize;

		}

		

		return property;
	}
}