#include "BlockingQueue.h"

template <typename T>
void BlockingQueue<T>::Enqueue(T value)
{
	std::lock_guard<std::mutex> guard{ lock };
	queue.push(value);
	condition.notify_one;
}

template <typename T>
T BlockingQueue<T>::Dequeue()
{
	std::unique_lock<std::mutex> uniqueLock{ lock };
	while (IsEmpty())
	{
		condition.wait(uniqueLock);
	}
	auto val = queue.front();
	queue.pop();
	return val;
}

template <typename T>
bool BlockingQueue<T>::IsEmpty()
{
	std::lock_guard<std::mutex> guard{ lock };
	return queue.size == 0;
}
