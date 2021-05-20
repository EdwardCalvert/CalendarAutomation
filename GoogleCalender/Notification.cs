using System;
using System.IO;
using System.Media;

namespace GoogleCalender
{
    static class Notification
    {
        public const string DefaultFolder = @"\Sounds\";

        public static void PlaySound(string name)
        {
            if (name != null && name != "null" && name != "none")
            {
                SoundPlayer _openSound;
                if (!name.EndsWith(".wav"))
                    name += ".wav";
                if (File.Exists(Environment.CurrentDirectory + DefaultFolder + name))
                {
                    _openSound = new SoundPlayer(Environment.CurrentDirectory + DefaultFolder + name);

                }

                else
                {
                    _openSound = new SoundPlayer(Environment.CurrentDirectory + DefaultFolder + "Program Error.wav");
                }
                _openSound.Play();
                _openSound.Dispose();
            }

        }


        public static void OpenSound()
        {
            PlaySound("Open.wav");
        }

        public static void InputSound()
        {
            PlaySound("Input.wav");
        }

        public static void ExclamationSound()
        {
            PlaySound("Exclamation.wav");
        }

        public static void BootNoise()
        {
            PlaySound("Exit Windows.wav");
        }

        public static void ProgramErrorSound()
        {
            PlaySound("Program Error.wav");
        }
    }
}

