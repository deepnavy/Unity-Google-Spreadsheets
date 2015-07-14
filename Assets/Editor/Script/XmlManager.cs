using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XmlManager : MonoBehaviour {
	
}


public class TestEntity {

	public string name;
	public int number;
	
	public TestEntity(){
		name = "default";
		number = 0;
	}
}

[XmlRoot("TestCollection")]
public class TestContainer {
	[XmlArray("TestEntities")]
	[XmlArrayItem("testEntity")]
	public TestEntity[] testEntities;// = new skinEntity[];
	
	public TestContainer(){
	}
	
	public TestContainer(TestEntity[] arch){
		testEntities = arch;
	}
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(TestContainer));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static TestContainer Load(string path)
	{
		var serializer = new XmlSerializer(typeof(TestContainer));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as TestContainer;
		}
	}
}



