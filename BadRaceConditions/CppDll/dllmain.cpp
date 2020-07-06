// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"

#include <iostream>
#include <thread>

DWORD WINAPI ThrFunc(LPVOID lpParam) {
	CONDITION_VARIABLE* condVar = (CONDITION_VARIABLE*)lpParam;
	WakeConditionVariable(condVar);
	return 0;
}

struct ThreadInit
{
	ThreadInit()
	{
		DWORD threadId;
		CONDITION_VARIABLE condVar;
		CRITICAL_SECTION crit;
		InitializeConditionVariable(&condVar);
		InitializeCriticalSection(&crit);
		CreateThread(nullptr, 0, ThrFunc, &condVar, 0, &threadId);
		EnterCriticalSection(&crit);
		SleepConditionVariableCS(&condVar, &crit, INFINITE);
		LeaveCriticalSection(&crit);
	}
};

static ThreadInit thread2;

static std::thread thread{ [] {

} };

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

