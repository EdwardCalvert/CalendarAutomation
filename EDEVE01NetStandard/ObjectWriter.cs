using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace EDDEV101.HelperMethods.V2
{
    public class ObjectWriter<T>
    {
        private string _filePath;

        public enum result
        {
            Success,
            FileNotFoundException,
            ArgumentException,
            IOException,
            SecurityException,
            UnauthorizedAccessException,
            DirectoryNotFoundException,
            PathTooLongException,
        }

        public ObjectWriter(string filePath)
        {
            _filePath = filePath;
        }

        public result SaveSettings( T settings)
        {
            try
            {
                using (StreamWriter stream = new StreamWriter(_filePath))
                {
                    using (JsonTextWriter JsonWriter = new JsonTextWriter(stream))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(JsonWriter, settings);
                    }

                }
                return result.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return result.UnauthorizedAccessException;
            }
            catch (ArgumentException)
            {
                return result.ArgumentException;
            }
            catch (DirectoryNotFoundException)
            {
                return result.DirectoryNotFoundException;
            }
            catch (PathTooLongException)
            {
                return result.PathTooLongException;
            }
            catch (IOException)
            {
                return result.PathTooLongException;
            }
            catch (System.Security.SecurityException)
            {
                return result.SecurityException;
            }

        }
       

          
        }
    }

