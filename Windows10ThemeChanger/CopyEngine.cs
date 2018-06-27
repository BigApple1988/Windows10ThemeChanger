using System;
using System.Runtime.InteropServices;

namespace Windows10ThemeChanger
{

	public delegate void CopyEventHandler(CopyEngine sender, CopyEngine.CopyEventArgs  e);

//	
//	class Class1
//	{
//		
//		[STAThread]
//		static void Main(string[] args)
//		{
//			
//		
//			System.Threading.ThreadStart ev = new System.Threading.ThreadStart(StartCopy);
//			System.Threading.Thread Th = new System.Threading.Thread(ev);
//			Th.Priority = ThreadPriority.Lowest;
//			Th.Start();
//
//			//CopyTo("C:\\Documents and Settings\\rouxy\\Bureau\\posera\\Upload\\MD2003.exe","\\\\SRV_VIL\\MaitreD\\Test.old");
//			//CopyTo("C:\\Documents and Settings\\rouxy\\Bureau\\posera\\Upload\\MD2003.exe","C:\\Documents and Settings\\rouxy\\Bureau\\posera\\Upload\\MD2003.bak");
//
//		}
//		
//
//		static void CopyIsInProgress(CopyEngine Sender,CopyEngine.CopyEventArgs e)
//		{
//			Console.WriteLine("{0}%\t{1}Ko/s",e.CurrentPercent.ToString(".##"),e.CurrentTauxTransfert.ToString(".##"));
//		}
//
//
//		static void StartCopy()
//		{
//			Console.WriteLine("Launching Copy");
//			CopyEngine eng = new CopyEngine("C:\\Documents and Settings\\rouxy\\Bureau\\posera\\Upload\\MD2003.exe","\\\\SRV_LOU\\Maitred\\MD2003.bak");
//			
//			eng.CpEvHandler+= new CopyEventHandler(CopyIsInProgress);
//			
//			eng.CopyFiles();
//			
//		}
//
//	}//End Class Class1



	public class CopyEngine
	{
		

		// Delegate for CallBack Function
		private delegate Int32 CallBackDelegate(
			uint TotalFileSize_  
			,uint BytesTransfered_ 
			,uint StreamSize_ 
			,uint StreamBytesTransfered_ 
			,uint DwStreamNumber_ 
			,long dwCallbackReason_ 
			,long hSourceFile_ 
			,long hDestinationFile
			,long lpData);

		
		public event CopyEventHandler CpEvHandler;

		//Constantes API

		//Define possible return codes from the CopyFileEx callback routine
		private const Int32 PROGRESS_CONTINUE  = 0;
		private const Int32 PROGRESS_CANCEL    = 1;
		private const Int32 PROGRESS_STOP      = 2;
		private const Int32 PROGRESS_QUIET     = 3;

		// Define CopyFileEx callback routine state change values
		private const Int32 CALLBACK_CHUNK_FINISHED    = 0x00000000;
		private const Int32 CALLBACK_STREAM_SWITCH     = 0x00000001;

		// Define CopyFileEx option flags
		private const Int32 COPY_FILE_FAIL_IF_EXISTS               = 0x00000001;
		private const Int32 COPY_FILE_RESTARTABLE                  = 0x00000002;
		private const Int32 COPY_FILE_OPEN_SOURCE_FOR_WRITE        = 0x00000004;
		private const Int32 COPY_FILE_ALLOW_DECRYPTED_DESTINATION  = 0x00000008;


		//API Calls
		[DllImport("kernel32.dll", EntryPoint="CopyFileExA",SetLastError=true)]
		private static extern Int32 CopyFileEx ( 
			string lpExistingFileName,
			string lpNewFileName,
			[MarshalAs(UnmanagedType.FunctionPtr)] CallBackDelegate  CCBD,
			long lpData,
			bool pbCancel,
			Int32 dwCopyFlags);
		
		[DllImport("kernel32.dll")]
		private static extern Int32 FlushFileBuffers ( 
			long hFile);

		// Private values for properties
		private string SourceFile;
		private string DestinationFile;
		private bool IsCopy;
		private float nPercent;
		private double txTransf;
		private DateTime StartTime;
		private TimeSpan ElapsedTime;
	
	

		// public Properties
		public string Source
		{
			get{return SourceFile ;}
			set{if(!IsCopy ){SourceFile = value;}}
		}

		public string Destination
		{
			get{return DestinationFile;}
			set{if(!IsCopy ){DestinationFile = value;}}
		}

		public bool IsCopying
		{
			get{return IsCopy;}
		}

		public string Percentage
		{
			get{return nPercent.ToString(".##");}
		}

		public string TauxTransfert
		{
			get{return txTransf.ToString(".##");}
		}


		// Constructeur avec Arguments
		public CopyEngine(string Source,string Destination)
		{
			SourceFile = Source;
			DestinationFile = Destination;
		}

		// Constructeur sans Argument
		public CopyEngine()
		{}

		public void CopyFiles()
		{
			
			if((SourceFile !="")&&(DestinationFile !=""))
			{
				try
				{
				IsCopy =true;
				CallBackDelegate cb = new CallBackDelegate(CopyInProgress);
					StartTime = DateTime.Now;
					var flag = COPY_FILE_RESTARTABLE + COPY_FILE_ALLOW_DECRYPTED_DESTINATION;
					var res = CopyFileEx(SourceFile,DestinationFile,cb,0,false,flag);
					
					IsCopy =false;

				}

				catch(Exception e)
				{
					IsCopy =false;
					throw new Exception(e.Message);
				}
			}
		}


		// CallBack Routine
		private Int32 CopyInProgress(uint TotalFileSize,uint BytesTransfered,uint StreamSize,uint StreamBytesTransfered,uint DwStreamNumber,long dwCallbackReason,long hSourceFile,long hDestinationFile,long lpData)
		{
			
			float t = TotalFileSize;
			float bt = StreamSize ;
				
				nPercent = (bt / t )* 100;									//Etat de l'avancement en pourcentage
				ElapsedTime = (DateTime.Now - StartTime);					//Temps йcoulй depuis le lancement du transfert
				txTransf = (StreamSize /1024) / ElapsedTime.TotalSeconds;	//Calcul du taux de transfert moyen cumulй en Ko/s
	
			CpEvHandler(this,new CopyEventArgs(nPercent,txTransf));
			
			return PROGRESS_CONTINUE;
		}
		

		public class CopyEventArgs: EventArgs
		{
			private float Percentage;
			private double TauxTransfert;

			//Constructeur
			public CopyEventArgs(float nPercent,double TauxDeTransfert)
			{
				Percentage = nPercent;
				TauxTransfert = TauxDeTransfert;
			}	

			public float CurrentPercent
			{
				get{return Percentage;}
			}

			public double CurrentTauxTransfert
			{
				get{return TauxTransfert;}
			}
	
		}//End Class CopyEventHandler


	}//End Class CopyEx



	


}//NameSpace
