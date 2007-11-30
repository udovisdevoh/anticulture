using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Audio.Midi
{
    public struct MidiNoteEvent : IComparable
    {
        public uint Time;
        public byte Note;
        public byte Type;
        public byte Velocity;

        public int CompareTo(object obj)
        {
            return Time.CompareTo(((MidiNoteEvent)obj).Time);
        }
    }
}