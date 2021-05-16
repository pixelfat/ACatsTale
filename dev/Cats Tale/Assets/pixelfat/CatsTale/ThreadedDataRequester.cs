using Pixelfat.Unity;
using System;
using System.Collections.Generic;
using System.Threading;

// adapated from: https://github.com/SebLague/Procedural-Landmass-Generation
public class ThreadedDataRequester : Singleton<ThreadedDataRequester>
{

	struct ThreadInfo
	{
		public readonly Action<object> callback;
		public readonly object parameter;

		public ThreadInfo(Action<object> callback, object parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}

	}

	//static ThreadedDataRequester instance; // created on Awake()
	Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

	//void Awake()
	//{
	//	instance = this;
	//}

	public static void RequestData(Func<object> generateData, Action<object> callback)
	{

		if (Instance == null)
			Instance.gameObject.name += "!";

		ThreadStart threadStart = delegate {
			Instance.DataThread(generateData, callback);
		};

		new Thread(threadStart).Start();
	}

	void DataThread(Func<object> generateData, Action<object> callback)
	{
		object data = generateData();
		lock (dataQueue)
		{
			dataQueue.Enqueue(new ThreadInfo(callback, data));
		}
	}

	void Update()
	{
		if (dataQueue.Count > 0)
		{
			for (int i = 0; i < dataQueue.Count; i++)
			{
				ThreadInfo threadInfo = dataQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

}
