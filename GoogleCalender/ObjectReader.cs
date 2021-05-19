using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace GoogleCalendar
{
    public class ObjectReader<T> :IDisposable
    {
        public enum result
        {
            Success,
            FileNotFoundException,
            ArgumentException,
            IOException,
            SecurityException, 
            JsonSerializationException,
            Exception,
        }
        private string _filePath;

        public ObjectReader(string filePath)
        {
            _filePath = filePath;
        }

        public result ReadSettings(ref T settings)
        {
            try
            {
                using (StreamReader stream = new StreamReader(_filePath))
                {
                    using (JsonTextReader JsonReader = new JsonTextReader(stream))
                    {
                        var serializer = new JsonSerializer();
                        settings = serializer.Deserialize<T>(JsonReader);
                    }

                }
                return result.Success;
            }
            catch (ArgumentException)
            {
                return result.ArgumentException;
            }

            catch (FileNotFoundException)
            {
                return result.FileNotFoundException;
            }
            catch (IOException)
            {
                return result.IOException;
            }
            catch (JsonSerializationException)
            {
                return result.JsonSerializationException;
            }
            catch(Exception)
            {
                return result.Exception;
            }
        }

        public void Dispose()
        {
        }
    }
}
