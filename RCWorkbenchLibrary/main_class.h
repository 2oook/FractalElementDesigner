#pragma once

#include <iostream>
#include <process.h>
#include <windows.h>
#include "../rcworkbenchlibrarycore/code/RCStructure.h"
#include "../rcworkbenchlibrarycore/code/CAnalyseParameters.h"
#include "../rcworkbenchlibrarycore/definitions.h"

class MainClass
{

public:
    CRCStructure* Structure1, * Structure5;
    CAnalyseParameters* pAnalyseParameters = NULL;

    static HANDLE hStopCompletionPort;
    HANDLE hCompletionPort;
    HANDLE* phThreads;
    int nThreads = 1;

    int ExitCode = 0;

    MainClass()
    {
        // ���������� ���������� �����������
        SYSTEM_INFO si;
        GetSystemInfo(&si);
        int nProcessors = si.dwNumberOfProcessors;
        if (nThreads > nProcessors)
            nThreads = nProcessors;

        // ������� ���� ����������
        hCompletionPort = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, nThreads);

        // ���������� ���� �����, ������ ������, ��� ���� �����������
        hStopCompletionPort = CreateEvent(NULL, true, false, NULL);

        // ������� ������-����������� �������
        phThreads = new HANDLE[nThreads];
        for (int i = 0; i < nThreads; ++i)
        {
            phThreads[i] = (HANDLE)_beginthreadex(NULL, 0, &MainClass::WorkerThread, hCompletionPort, 0, NULL);
            SetThreadPriority(phThreads[i], THREAD_PRIORITY_LOWEST);
            if (nThreads == nProcessors)
                SetThreadIdealProcessor(phThreads[i], i);
        }
    }

    int CalculateYParameters(CRCStructure* Structure, CRCStructureCalculateData* Data)
    {
        HANDLE hBadStructureEvent = CreateEvent(NULL, true, false, NULL);
        HANDLE* hDoneEvents = new HANDLE[Data->m_length];
        CJob job;
        job.Structure = Structure;
        job.Data = Data;
        job.isBadStructureEvent = hBadStructureEvent;
        job.isDoneEvents = hDoneEvents;

        Structure->ElementsToNodes(); // ����� �������� Y-���������� �������� ��������� �����
        // �������� �������
        for (int i = 0; i < Data->m_length; ++i)
        {
            hDoneEvents[i] = CreateEvent(NULL, true, false, NULL);
            PostQueuedCompletionStatus(hCompletionPort, 0, i, (OVERLAPPED*)&job);
        }

        // ������� ���������� �������
        for (int i = 0; i < Data->m_length; ++i)
        {
            if (WaitForSingleObject(hDoneEvents[i], INFINITE) != WAIT_OBJECT_0)
                cout << "Error while waiting event";
            CloseHandle(hDoneEvents[i]);
        }
        delete[] hDoneEvents;

        if (WaitForSingleObject(hBadStructureEvent, 0) == WAIT_OBJECT_0)
        {
            CloseHandle(hBadStructureEvent);
            return -1;
        }

        CloseHandle(hBadStructureEvent);
        return 0;
    }

    static unsigned int __stdcall WorkerThread(LPVOID CompletionPortID)
    {
        unsigned long numOfBytes;
        unsigned long key;
        OVERLAPPED* poverlapped;
        CJob* pJob;

        while (WaitForSingleObject(hStopCompletionPort, 0) == WAIT_TIMEOUT)
        {
            if (GetQueuedCompletionStatus((HANDLE)CompletionPortID, &numOfBytes, &key, &poverlapped, 1000))
            {
                pJob = (CJob*)poverlapped;
                // pJob->Data->m_w[key] - ������� // pJob->Data->m_MatY[key] - ����� ����������� ������� ����������� � ������ ������������ ��� �������  // Dest -> m_MatY
                if ((WaitForSingleObject(pJob->isBadStructureEvent, 0) == WAIT_TIMEOUT) && (pJob->Structure->YParameters(pJob->Data->m_w[key], pJob->Data->m_MatY[key], pJob->Data->y_result[key]) == -1))
                    SetEvent(pJob->isBadStructureEvent);
                SetEvent(pJob->isDoneEvents[key]);
            }
        }
        return 0;
    };

    struct CJob
    {
        CRCStructure* Structure;
        CRCStructureCalculateData* Data;
        HANDLE isBadStructureEvent;
        HANDLE* isDoneEvents;
    };
};

HANDLE MainClass::hStopCompletionPort = NULL;

