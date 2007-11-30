using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Audio.Midi
{
    public struct NoteEvent
    {
        public int Note;
        public float Time, Duration;
        public float Velocity;

        public NoteEvent(int note, float time, float duration, float velocity)
        {
            Note = note;
            Time = time;
            Duration = duration;
            Velocity = velocity;
        }
    }
}