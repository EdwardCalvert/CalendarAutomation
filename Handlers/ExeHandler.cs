using System.IO;
using System.Threading.Tasks;

namespace Handlers
{
    public class ExeHandler
    {
        private MemoryManager _memoryManager;

        public ExeHandler()
        {
            MemoryManager _memoryManager = new MemoryManager();
        }

        public void DelayExecution(int delay, string ProgramPath, string id)
        {
            Task.Delay(delay).ContinueWith(_ => RunExe(ProgramPath, id));

        }

        /// <summary>
        /// Uses Process.Start, to open Exe. This will throw errors if it doesn't exist.
        /// InvalidOperationException
        /// ObjectDisposedException
        /// Win32Exception
        /// </summary>
        /// <param name="ProgramPath"></param>
        /// <param name="id"></param>
        public void RunExe(string ProgramPath, string id)
        {
            if (!_memoryManager.TabOpened(ProgramPath, id))
            {
                System.Diagnostics.Process.Start(ProgramPath);
                _memoryManager.MarkTabAsOpen(ProgramPath, id);
            }
        }

        private static bool ExeExists(string ProgramPath)
        {
            return File.Exists(ProgramPath);
        }
    }
}
