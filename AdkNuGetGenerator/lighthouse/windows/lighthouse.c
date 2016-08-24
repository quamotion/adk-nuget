#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>

// Define the external variables here
TCHAR _moduleFileName[MAX_PATH];
int _moduleFileNameLength;
DWORD _lastError;

__declspec(dllexport) DWORD __cdecl get_moduleFileName(LPTSTR moduleFileName, int moduleFileNameLength)
{
	if (_lastError != 0)
	{
		return _lastError;
	}

	return wcscpy_s(moduleFileName, moduleFileNameLength, _moduleFileName);
}

BOOL APIENTRY DllMain(
	HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	if (ul_reason_for_call == DLL_PROCESS_ATTACH)
	{
		// When a process loads this dll, the DllMain method is called and we can fetch
		// the filename for this module.
		// Save it in a external (shared) variable, so we can retrieve it later.
		_moduleFileNameLength = GetModuleFileName(hModule, _moduleFileName, MAX_PATH);
		_lastError = GetLastError();
	}

	return TRUE;
}