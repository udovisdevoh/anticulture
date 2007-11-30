using System;
using System.Collections.Generic;
using System.Text;

namespace AntiCulture.Audio.Midi
{
    internal class MidiTrack
    {
        private List<MidiNoteEvent> mMidiNoteEvents = new List<MidiNoteEvent>();
        private string mName = "untitled";
        private string mInstument = "piano";

        public MidiTrack(string name, string instrument, IEnumerator<NoteEvent> noteEventEnumerator)
        {
            mName = name;
            mInstument = instrument;

            while (noteEventEnumerator.MoveNext())
            {
                NoteEvent noteEvent = noteEventEnumerator.Current;

                // Note down event
                MidiNoteEvent midiNoteEvent = new MidiNoteEvent();
                midiNoteEvent.Time = (uint)(noteEvent.Time*64);
                midiNoteEvent.Note = (byte)((noteEvent.Note + 60) & (0xFF >> 1));
                midiNoteEvent.Type = (byte)0x90;
                midiNoteEvent.Velocity = (byte)((uint)(noteEvent.Velocity * 127) & (0xFF >> 1));
                mMidiNoteEvents.Add(midiNoteEvent);

                // Note up event
                midiNoteEvent = new MidiNoteEvent();
                midiNoteEvent.Time = (uint)((noteEvent.Time + noteEvent.Duration) * 64);
                midiNoteEvent.Note = (byte)((noteEvent.Note + 60) & (0xFF >> 1));
                midiNoteEvent.Type = (byte)0x80;
                midiNoteEvent.Velocity = (byte)((uint)(noteEvent.Velocity * 127) & (0xFF >> 1));
                mMidiNoteEvents.Add(midiNoteEvent);
            }
        }

        internal uint GetMidiNoteEventCount()
        {
            return (uint)mMidiNoteEvents.Count;
        }

        internal IEnumerator<MidiNoteEvent> GetMidiNoteEventEnumerator()
        {
            mMidiNoteEvents.Sort();
            return mMidiNoteEvents.GetEnumerator();
        }
    }
}