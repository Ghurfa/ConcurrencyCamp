// CppAsync.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>

#include <uv.h>
#include <stdlib.h>

#include <future>
#include <thread>
int promisesFuture()
{
	//Futures like task, give you result in future
	//Promises provide futures, promise to give a result

	//Task.Run()
	std::future<int> futureResult = std::async([] {return 4; });

	futureResult.wait();
	int result = futureResult.get();
	std::cout << result << std::endl;


	std::promise<int> promise;
	std::future<int> future = promise.get_future();

	std::thread thr([](std::promise<int> p) {
		}, std::move(promise));

	//broken promise: promise is destructed so future.wait() advances and throws at get
	future.wait(); 
	
	//std::thread thr([&promise] {
	//	promise.set_value(7);
	//	});

	//future.wait();

	std::cout << future.get() << std::endl;
	thr.join(); 
	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
