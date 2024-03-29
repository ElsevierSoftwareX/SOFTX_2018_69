using UnityEngine;
using System.Collections;
using System.Collections.Generic ;
using System.Xml ;
using System.Xml.Serialization ;
using System.IO ;
using UnityEngine.SceneManagement;


public class FPSInputTracker : MonoBehaviour 
{
	//prefix for saving file
	string xmlSaveFileName;
	//path for saving file
	public static string _path 
	{
		get
		{
			string s = Application.dataPath ;


			s = Path.Combine(s, "UserBehaviouralData") ;

			//if the fold doesn't exist, create it
			if(!Directory.Exists(s))
				Directory.CreateDirectory(s) ;
			print ("Folder Created");

			return s ;
		}
	}

	#region movement classes----------------------------------------------------------------------------
	public class Movement
	{

		public Vector3 startPosition ;
		public Vector3 endPosition ;
		public float startTime, endTime, totalTime, distanceMoved ;

		//default constructor
		public Movement()
		{
			this.startPosition = Vector3.zero ;
			this.startTime = 0f ;
			this.endTime = 0f ;
			this.totalTime = 0f ;
			this.endPosition = Vector3.zero ;
			this.distanceMoved = 0f ;
		}

		//alternative constructor
		public Movement(Vector3 startPosition)
		{
			this.startPosition = startPosition ;
			this.startTime = Time.timeSinceLevelLoad ;
		}

		//finishing movement
		public void EndMovement(Vector3 endPosition)
		{
			this.endTime = Time.timeSinceLevelLoad ;
			this.totalTime = endTime - startTime ;
			this.endPosition = endPosition ;
			this.distanceMoved = Vector3.Distance(this.startPosition, this.endPosition) ;
		}
	}


	public class MovementData
	{

		public float idleTime ;

		public Movement movement ;

		//default constructor
		public MovementData()
		{
			idleTime = 0f ;
			movement = new Movement() ;
		}
		//alternative constructor
		public MovementData(Movement movement, float previousEndTime)
		{
			this.movement = movement ;
			this.idleTime = movement.startTime - previousEndTime ;
		}
	}
	#endregion


	//movements collection
	public List<Movement> movements = new List<Movement>() ;


		
	//---this class is what gets saved-to/retrieved-from an xml file-->
	[XmlRoot("UserLog")]
	public class UserLog
	{
		[XmlElement("TotalTime")]
		//public float totalTime ;
		public float totalTime ;
		[XmlElement("TotalDistance")]
		public float totalDistance ;

		[XmlArray("MovementData"), XmlArrayItem("Information")]
		public MovementData[] movementData ;



		//default constructor
		public UserLog()
		{
			this.totalTime = 0f ;
			this.totalDistance = 0f ;
			movementData = new MovementData[0] ;

		}



		#region XML saving/loading -----------------------------------------------------------------------------------
		//script header must include [using]-> System.Xml, System.Xml.Serialization, System.IO

		public void Save(string path)
		{
			int index = 0 ;
			string p = path + "." + index.ToString() ;
			
			while(File.Exists(p + ".xml"))
			{
				index ++ ;
				p = path + "." + index.ToString() ;

			}

			var serializer = new XmlSerializer(typeof(UserLog)) ;
		
			var stream = new FileStream(p + ".xml", FileMode.Create) ;
	
			serializer.Serialize(stream, this) ;
			
			stream.Close() ;
		}
			
		public UserLog Load(string path)
		{

			if(!File.Exists(path))
				return new UserLog() ;

			var serializer = new XmlSerializer(typeof(UserLog)) ;
			var stream = new FileStream(path, FileMode.Open) ;
			
			var ul = serializer.Deserialize(stream) as UserLog ;
			
			stream.Close() ;
			return ul ;
		}
		#endregion
	}


	
	//cache reference to this objects transform
	Transform myTrans ;
	//cache reference to the camera transform
	Transform myCam ;



	void Start()
	{
		xmlSaveFileName = SceneManager.GetActiveScene ().name;
		myTrans = this.transform ;
		myCam = myTrans.Find("Camera (eye)");
		print ("Begin");

		//finding the camera object child
		if(!myCam || !myCam.GetComponent<Camera>())
		{
			Debug.LogError("Error : No child of name \"Main Camera\" was found on GameObject(\"" + myTrans.name + "\")") ;
			this.enabled = false ;
			return ;
		}

	}


	bool goalCompleted = false ;

	void Update()
	{	}




	int currentMovementIndex = 0 ;
	bool isMoving = false ;




	public void StartMovement()
	{
		print ("StartMovement()");
		movements.Add(new Movement(myTrans.position)) ;
		
	
		if(isMoving)
		{
			EndMovement() ;
		}
		
		isMoving = true ;
	}

	public void EndMovement()
	{
		print ("EndMovement()");
		print("Num. movements = " + movements.Count);
		print ("Movement index = " + currentMovementIndex);
		if(movements.Count <= 0 || currentMovementIndex < movements.Count)
			movements[currentMovementIndex].EndMovement(myTrans.position) ;

		currentMovementIndex ++ ;
		isMoving = false ;
	}



}
