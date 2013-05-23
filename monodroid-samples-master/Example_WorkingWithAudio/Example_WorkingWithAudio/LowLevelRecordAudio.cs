using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;

namespace Example_WorkingWithAudio
{
    class LowLevelRecordAudio : INotificationReceiver
    {
        static string filePath = "/data/data/Example_WorkingWithAudio.Example_WorkingWithAudio/files/testAudio.mp4";

        byte[] audioBuffer = null;
        AudioRecord audioRecord = null;
        bool endRecording = false;
        bool isRecording = false;

        public Boolean IsRecording {
            get { return (isRecording); }
        }

        void ReadAudio ()
        {
            using (var fileStream = new FileStream (filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
                while (true) {
                    if (endRecording) {
                        endRecording = false;
                        break;
                    }
                    try {
                        // Keep reading the buffer while there is audio input.
                        int numBytes = audioRecord.Read (audioBuffer, 0, audioBuffer.Length);
                        fileStream.Write (audioBuffer, 0, numBytes);
                        // Do something with the audio input.
                    } catch (Exception ex) {
                        Console.Out.WriteLine (ex.Message);
                        break;
                    }
                }
                fileStream.Close ();
            }
            audioRecord.Stop ();
            audioRecord.Release ();
            isRecording = false;
        }
        
        protected void StartRecorder ()
        {
            endRecording = false;
            isRecording = true;

            audioBuffer = new Byte[100000];
            audioRecord = new AudioRecord (
                // Hardware source of recording.
                AudioSource.Mic,
                // Frequency
                11025,
                // Mono or stereo
                ChannelIn.Mono,
                // Audio encoding
                Android.Media.Encoding.Pcm16bit,
                // Length of the audio clip.
                audioBuffer.Length
            );

            audioRecord.StartRecording ();

            // Off line this so that we do not block the UI thread.
            Thread thread = new Thread (new ThreadStart (ReadAudio));
            thread.Start ();
        }

        public void Start ()
        {
            StartRecorder ();
        }

        public void Stop ()
        {
            endRecording = true;
            Thread.Sleep (500); // Give it time to drop out.
        }

    }
}