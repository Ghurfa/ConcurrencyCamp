// Threads.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <thread>
#include <atomic>
#include <mutex>
#include <condition_variable>


//std::atomic_bool wait{ true };
//std::mutex write{};
std::mutex lock{};

std::condition_variable condition{};
int inc = -2;
void threadMain(const char* str, int c)
{
	std::unique_lock<std::mutex> uniqueLock{lock};
	while (true)
	{
		if (inc == c - 1)
		{
			inc = c;
			break;
		}
	}
	//while (wait);
	//std::lock_guard<std::mutex> guard{ write };
	std::cout << str << std::endl;
}

int main()
{
	//spin up 5 threads, have them wait on an atomic variable before fully starting
	//print out a 10 char string in each thread

	std::thread thread1{ threadMain, "thread1" , 1};
	std::thread thread2{ threadMain, "thread2" , 2};
	std::thread thread3{ threadMain, "thread3" , 3};
	std::thread thread4{ threadMain, "thread4" , 4};
	std::thread thread5{ threadMain, "thread5" , 5};
	//wait = false;
	inc = -1;
	thread1.join();
	thread2.join();
	thread3.join();
	thread4.join();
	thread5.join();
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln fil

