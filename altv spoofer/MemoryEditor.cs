using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class MemoryEditor
{
    // --- IMPORTY WINAPI ---
    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    const int PROCESS_WM_READ = 0x0010;
    const int PROCESS_WM_WRITE = 0x0020;
    const int PROCESS_VM_OPERATION = 0x0008;

    private IntPtr _processHandle;
    private Process _process;

    // --- PODŁĄCZANIE ---
    public bool Attach(string processName)
    {
        Process[] processes = Process.GetProcessesByName(processName);
        if (processes.Length > 0)
        {
            _process = processes[0];
            _processHandle = OpenProcess(PROCESS_WM_READ | PROCESS_WM_WRITE | PROCESS_VM_OPERATION, false, _process.Id);
            return _processHandle != IntPtr.Zero;
        }
        return false;
    }

    public IntPtr GetModuleBaseAddress(string moduleName)
    {
        try
        {
            foreach (ProcessModule module in _process.Modules)
            {
                if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    return module.BaseAddress;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to find: {ex.Message}");
        }
        return IntPtr.Zero;
    }

    public IntPtr ScanPattern(string moduleName, string signature)
    {
        ProcessModule targetModule = null;
        foreach (ProcessModule module in _process.Modules)
        {
            if (module.ModuleName.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            {
                targetModule = module;
                break;
            }
        }

        if (targetModule == null)
        {
            Console.WriteLine($"[ScanPattern] Not found module: {moduleName}, (did you start spoof before connecting to server?)");
            return IntPtr.Zero;
        }

        string[] tokens = signature.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        byte[] patternBytes = new byte[tokens.Length];
        bool[] mask = new bool[tokens.Length];

        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i] == "?" || tokens[i] == "??")
            {
                mask[i] = false; // Wildcard
                patternBytes[i] = 0x00;
            }
            else
            {
                mask[i] = true; 
                if (!byte.TryParse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out patternBytes[i]))
                {
                    Console.WriteLine($"[ScanPattern] failed to parse byte: {tokens[i]}");
                    return IntPtr.Zero;
                }
            }
        }

        long startAddress = targetModule.BaseAddress.ToInt64();
        long moduleSize = targetModule.ModuleMemorySize;

        byte[] buffer = new byte[4096];

        for (long i = 0; i < moduleSize; i += buffer.Length)
        {
            int bytesToRead = (int)Math.Min(buffer.Length, moduleSize - i);
            int bytesRead;

            ReadProcessMemory(_processHandle, (IntPtr)(startAddress + i), buffer, bytesToRead, out bytesRead);

            if (bytesRead == 0) continue; 

            for (int k = 0; k < bytesRead - patternBytes.Length; k++)
            {
                bool found = true;
                for (int j = 0; j < patternBytes.Length; j++)
                {
                    // Sprawdzamy maskę
                    if (mask[j] && buffer[k + j] != patternBytes[j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {

                    long resultAddress = startAddress + i + k;
                    Console.WriteLine($"[ScanPattern] Signature found: {resultAddress:X}");
                    return (IntPtr)resultAddress;
                }
            }


            i -= patternBytes.Length;
        }

        Console.WriteLine("[ScanPattern] The module was scanned, but no signature was found.");
        return IntPtr.Zero;
    }


    public string ReadString(long address, int length, bool isUnicode = false)
    {
        return ReadString((IntPtr)address, length, isUnicode);
    }

    public void WriteString(long address, string text, bool isUnicode = false)
    {
        WriteString((IntPtr)address, text, isUnicode);
    }

    private string ReadString(IntPtr address, int length, bool isUnicode)
    {
        byte[] buffer = new byte[length];
        int bytesRead;
        ReadProcessMemory(_processHandle, address, buffer, length, out bytesRead);
        System.Text.Encoding encoding = isUnicode ? System.Text.Encoding.Unicode : System.Text.Encoding.ASCII;
        string text = encoding.GetString(buffer);
        int nullIndex = text.IndexOf('\0');
        return nullIndex >= 0 ? text.Substring(0, nullIndex) : text;
    }

    private void WriteString(IntPtr address, string text, bool isUnicode)
    {
        System.Text.Encoding encoding = isUnicode ? System.Text.Encoding.Unicode : System.Text.Encoding.ASCII;
        byte[] buffer = encoding.GetBytes(text + '\0');
        int bytesWritten;
        WriteProcessMemory(_processHandle, address, buffer, buffer.Length, out bytesWritten);
    }

    public int ReadInt(long address)
    {
        byte[] buffer = new byte[4];
        int bytesRead;
        ReadProcessMemory(_processHandle, (IntPtr)address, buffer, buffer.Length, out bytesRead);
        return BitConverter.ToInt32(buffer, 0);
    }

    public void WriteInt(long address, int value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        int bytesWritten;
        WriteProcessMemory(_processHandle, (IntPtr)address, buffer, buffer.Length, out bytesWritten);
    }

    public float ReadFloat(long address)
    {
        byte[] buffer = new byte[sizeof(float)];
        int bytesRead;
        ReadProcessMemory(_processHandle, (IntPtr)address, buffer, buffer.Length, out bytesRead);
        return BitConverter.ToSingle(buffer, 0);
    }

    public void WriteFloat(long address, float value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        int bytesWritten;
        WriteProcessMemory(_processHandle, (IntPtr)address, buffer, buffer.Length, out bytesWritten);
    }

    public void Detach()
    {
        if (_processHandle != IntPtr.Zero)
        {
            CloseHandle(_processHandle);
        }
    }
}