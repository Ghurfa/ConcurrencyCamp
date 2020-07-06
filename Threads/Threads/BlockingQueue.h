#pragma once
#include <queue>
#include <mutex>
#include <condition_variable>

template <typename T>
class BlockingQueue
{
public:
	void Enqueue(T value);
	T Dequeue();
	bool IsEmpty();
private:
	std::queue<T> queue;
	std::mutex lock;
	std::condition_variable condition;
};