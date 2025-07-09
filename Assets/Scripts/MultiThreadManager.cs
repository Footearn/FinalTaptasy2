using System.Collections.Generic;

public class MultiThreadManager : Singleton<MultiThreadManager>
{
	private static Queue<Task> m_taskQueue = new Queue<Task>();

	private static object m_queueLock = new object();

	private void Update()
	{
		lock (m_queueLock)
		{
			if (m_taskQueue.Count > 0)
			{
				m_taskQueue.Dequeue()();
			}
		}
	}

	public static void RegistryTask(Task newTask)
	{
		lock (m_queueLock)
		{
			m_taskQueue.Enqueue(newTask);
		}
	}
}
