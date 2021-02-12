using System;
using System.IO;

namespace Handlers
{
    public static class ExeHandler
    {
        public static void DelayExecution(string line)
        {
            
        }

        /// <summary>
        /// This method attemtps to open the program with the provided filepath. 
        /// </summary>
        /// <param name="ProgramPath"></param>
        /// <returns> True if successful, and false if not </returns>
        public static bool TryOpenProgram(string ProgramPath)
        {
            try
            {
                if (File.Exists(ProgramPath))
                {

                    System.Diagnostics.Process.Start(ProgramPath);

                    return true;
                }
                else
                {
                    return false;
                }
             
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
