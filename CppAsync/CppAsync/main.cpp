#include <uv.h>
#include <stdint.h>
#include <functional>

#include <Windows.h>
#include <future>
#include <vector>
#include <queue>
#include <iostream>
#include <mutex>

//allocates buffer
static void Allocate(uv_handle_t* handle, size_t suggestedSize, uv_buf_t* buf)
{
	buf->base = (char*)malloc(suggestedSize);
	buf->len = suggestedSize;
}

static void CloseHandleUV(uv_handle_t* handle)
{
	free(handle);
}

static void LoopWalk(uv_handle_t* handle, void* param)
{
	uv_close(handle, CloseHandleUV);
}

static void TTYRead(uv_stream_t* stream, ssize_t length, const uv_buf_t* buf)
{
	char data[256];

	int copyLength = length < 256 ? length : 255;

	memcpy(data, buf->base, copyLength);

	data[copyLength] = '\0';

	if (strcmp(data, "quit\r\n") == 0)
	{
		printf("Quitting...\n");
		uv_walk(stream->loop, LoopWalk, NULL); //Runs loopWalk on each handle on loop
	}
	else
	{
		printf("%s", data);
	}

	free(buf->base);
}


static DWORD PASCAL threadMain(LPVOID lpThread) 
{
	printf("Hello!\n");
	uv_loop_t* loop = (uv_loop_t*)lpThread;

	loop->data = new std::thread::id(std::this_thread::get_id());

	uv_tty_t* tty = (uv_tty_t*)malloc(sizeof(uv_tty_t)); //tty - teletypewriter (stream)
	uv_tty_init(loop, tty, 0, TRUE); // 0 - stdin - console

	uv_read_start((uv_stream_t*)tty, Allocate, TTYRead);
	
	uv_run(loop, UV_RUN_DEFAULT); //exits when all handles are closed
	return 0;
}

template <typename T>
struct AsyncFunc{
	AsyncFunc(uv_loop_t* loop, std::function<void(std::promise<T>&)> callback)
		:loop(loop), callback(callback)
	{
		async = (uv_async_t*)malloc(sizeof(uv_async_t));
		uv_async_init(loop, async, [] (uv_async_t* handle){
			auto& self = *((AsyncFunc<T>*)handle->data);
			std::lock_guard<std::mutex> guard{ self.lock };
			while (!self.calledFuncs.empty())
			{
				auto& promise = self.calledFuncs.front();
				self.callback(promise);
				self.calledFuncs.pop();
			}
		});
		if (async)
		{
			async->data = this;
		}
	}

	std::future<T> CallFunc()
	{
		if (std::this_thread::get_id() == *(std::thread::id*)loop->data)
		{
			std::promise<T> promise;
			callback(promise);
			return promise.get_future();
		}

		std::future<T> future;
		{
			std::lock_guard<std::mutex> guard{ lock };
			calledFuncs.emplace();
			future = calledFuncs.back().get_future();
		}
		uv_async_send(async);
		return future;
	}

	std::function<void(std::promise<T>&)> callback;
	std::queue<std::promise<T>> calledFuncs;
	uv_async_t* async;
	uv_loop_t* loop;
	std::mutex lock;
};

int main()
{
	uv_loop_t* loop = (uv_loop_t*)malloc(sizeof(uv_loop_t));
	uv_loop_init(loop);

	AsyncFunc<int> asyncFunc = AsyncFunc<int>(loop, [](std::promise<int>& promise) {
		promise.set_value(2);
	});

	DWORD threadId = 0;
	HANDLE threadHandle = CreateThread(NULL, 0, threadMain, loop, 0, &threadId);

	auto future = asyncFunc.CallFunc();
	
	if (future.wait_for(std::chrono::milliseconds(0)) != std::future_status::ready)
	{
		//true if has result
	}

	future.wait();
	std::cout << future.get() << std::endl;

	if (threadHandle != 0)
	{
		WaitForSingleObject(threadHandle, INFINITE); //join
	}

	free(loop);
}